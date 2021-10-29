using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Service.Messagings.AlarmHub;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Service.Implementations.Hubs
{
    #region Interface

    public interface IAlarmHubService
    {
        void TriggerAlarm(AlarmHubCreateRequest request);
        IEnumerable<AlarmHubTriggeredResponse> GetCurrentAlarmTriggeredList(Guid userGuid);
        IEnumerable<Guid> IsHasAlarm(IEnumerable<Guid> masterDailyRunResourceGuidList);
        bool AcknowledgeAlarm(Guid masterDailyRunResourceAlarmGuid, string userAcknowledge, DateTime datetimeAcknowledge);
    }

    #endregion
    public class AlarmHubService : IAlarmHubService
    {
        private readonly IAlarmHubBroadcastService _alarmHubBroadcastService;
        private readonly IMasterDailyRunResourceAlarmRepository _masterDailyRunResourceAlarmRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLog;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IMasterGroupRepository _masterGroupRepository;
        private readonly IMasterDailyRunResourceHistoryRepository _masterDailyRunResourceHistoryRepository;

        private readonly static object _lockAcknowledge = new object();

        public AlarmHubService(
                IMasterDailyRunResourceAlarmRepository masterDailyRunResourceAlarmRepository,
                IUnitOfWork<OceanDbEntities> uow,
                ISystemLog_HistoryErrorRepository systemLog,
                IAlarmHubBroadcastService alarmHubBroadcastService,
                IMasterGroupRepository masterGroupRepository,
                IMasterDailyRunResourceHistoryRepository masterDailyRunResourceHistoryRepository
            )
        {
            _masterDailyRunResourceAlarmRepository = masterDailyRunResourceAlarmRepository;
            _uow = uow;
            _systemLog = systemLog;
            _alarmHubBroadcastService = alarmHubBroadcastService;
            _masterGroupRepository = masterGroupRepository;
            _masterDailyRunResourceHistoryRepository = masterDailyRunResourceHistoryRepository;
        }

        #region Trigger Alarm

        public void TriggerAlarm(AlarmHubCreateRequest request)
        {
            // 1. insert dailyrun_alarm table
            CreateDailyRunAlarm(request);
            _uow.Commit();

            // 2. broadcast to conected client      
            _alarmHubBroadcastService.BroadcastAlarm(request.MasterDailyRunResouceGuid);
        }

        private void CreateDailyRunAlarm(AlarmHubCreateRequest request)
        {
            var newAlarm = new TblMasterDailyRunResource_Alarm
            {
                Guid = Guid.NewGuid(),
                DatetimeCreated = request.DateTimeAlert,
                EmployeeName = request.EmployeeName,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                MasterDailyRunResource_Guid = request.MasterDailyRunResouceGuid,
                MasterSite_Guid = request.MasterSiteGuid,
                Phone = request.Phone,
                RouteName = request.RouteName,
                RunNo = request.RunNo,
                SiteName = request.SiteName,
                UniversalDatetimeCreated = DateTime.UtcNow,
                UserCreated = request.UserCreated,
                MasterEmployee_Guid = request.EmployeeGuid,
                MasterCustomerLocation_Guid = request.LocationGuid
            };

            _masterDailyRunResourceAlarmRepository.Create(newAlarm);

        }

        #endregion

        #region Get Alarm List

        public IEnumerable<AlarmHubTriggeredResponse> GetCurrentAlarmTriggeredList(Guid userGuid)
        {
            var emptyList = Enumerable.Empty<AlarmHubTriggeredResponse>();

            var masterGroupsList = _masterGroupRepository.GetPermittedAlarmGroupByUser(userGuid);

            if (masterGroupsList.IsEmpty() || !masterGroupsList.Any(e => !e.MasterSiteHandleList.IsEmpty()))
                return emptyList;

            var siteHandleList = masterGroupsList
                .SelectMany(e => e.MasterSiteHandleList)
                .Distinct();

            // find daily run alarm that user has permission by site handle in group.
            var dailyRunAlarmList = _masterDailyRunResourceAlarmRepository
                    .FindAllAsQueryable()
                    .Where(e =>
                        !e.FlagAcknowledged && !e.FlagDeactivated
                        && siteHandleList
                            .Any(d => d == e.MasterSite_Guid.Value)
                        );

            if (dailyRunAlarmList.IsEmpty())
                return Enumerable.Empty<AlarmHubTriggeredResponse>();

            return dailyRunAlarmList
                .OrderBy(e => e.DatetimeCreated)
                .AsEnumerable()
                .Select(drAlarm =>
                {
                    var model = drAlarm.ConvertToAlarmHubTriggeredResponse();
                    var group = masterGroupsList.FirstOrDefault(g => g.FlagAllowAcknowledge && g.MasterSiteHandleList.Any(site => site == drAlarm.MasterSite_Guid));
                    model.FlagAllowAcknowledged = (group != null) && group.FlagAllowAcknowledge;

                    return model;
                });
        }

        #endregion


        /// <summary>
        /// Find which run of the list has alarm
        /// </summary>
        /// <param name="masterDailyRunResourceGuidList"></param>
        /// <returns>masterDailyRunGuid who has alarm</returns>
        public IEnumerable<Guid> IsHasAlarm(IEnumerable<Guid> masterDailyRunResourceGuidList)
        {
            var result = _masterDailyRunResourceAlarmRepository
                    .FindAllAsQueryable(e => masterDailyRunResourceGuidList.Any(r => r == e.MasterDailyRunResource_Guid) && (!e.FlagAcknowledged || !e.FlagDeactivated))
                    .Select(e => e.MasterDailyRunResource_Guid.Value)
                    .AsEnumerable();

            return result;
        }


        public bool AcknowledgeAlarm(Guid masterDailyRunResourceAlarmGuid, string userAcknowledge, DateTime datetimeAcknowledge)
        {
            lock (_lockAcknowledge)
            {
                try
                {
                    var data = _masterDailyRunResourceAlarmRepository.FindOne(e => e.Guid == masterDailyRunResourceAlarmGuid && !e.FlagAcknowledged);

                    if (data == null) return false;

                    data.FlagAcknowledged = true;
                    data.UserAcknowledged = userAcknowledge;
                    data.DatetimeAcknowledged = datetimeAcknowledge;
                    data.UniversalDatetimeAcknowledged = DateTime.UtcNow;

                    _masterDailyRunResourceAlarmRepository.Modify(data);
                    _masterDailyRunResourceHistoryRepository.CreateDailyRunLog(new Models.RunControl.MasterDailyRunLogView
                    {
                        JSONParameter = null,
                        MasterDailyRunResourceGuid = data.MasterDailyRunResource_Guid.Value,
                        MsgID = 869,
                        UserCreated = userAcknowledge,
                        ClientDate = datetimeAcknowledge
                    });
                    _uow.Commit();

                    _alarmHubBroadcastService.BroadcastAlarmAcknowledged(data.Guid);

                    return true;

                }
                catch (Exception err)
                {
                    _systemLog.CreateHistoryError(err);
                    return false;
                }
            }
        }
    }
}
