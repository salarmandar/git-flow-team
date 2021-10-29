using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Group;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.Hubs
{
    #region Interface

    public interface IAlarmHubBroadcastService
    {
        void BroadcastAlarm(Guid masterDailyRunGuid);
        void BroadcastAlarmAcknowledged(Guid masterDailyRunAlarmGuid);
    }

    #endregion

    public class AlarmHubBroadcastService : IAlarmHubBroadcastService
    {
        private readonly IHubConnectionContext<dynamic> _clients;
        private readonly IMasterDailyRunResourceAlarmRepository _masterDailyRunAlarmRepository;
        private readonly IMasterGroupRepository _masterGroupRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLogHistoryErrorRepository;

        public AlarmHubBroadcastService(
                IHubConnectionContext<dynamic> clients,
                IMasterGroupRepository masterGroupRepository,
                IMasterDailyRunResourceAlarmRepository masterDailyRunAlarmRepository,
                ISystemLog_HistoryErrorRepository systemLogHistoryErrorRepository
            )
        {
            _clients = clients;
            _masterGroupRepository = masterGroupRepository;
            _masterDailyRunAlarmRepository = masterDailyRunAlarmRepository;
            _systemLogHistoryErrorRepository = systemLogHistoryErrorRepository;
        }

        public void BroadcastAlarm(Guid masterDailyRunGuid)
        {
            var dailyRunAlarm = _masterDailyRunAlarmRepository.GetNewDailyRunAlarmByRunGuid(masterDailyRunGuid);

            BroadcastClients(
                dailyRunAlarm,
                (item, proxy) =>
                {
                    var model = dailyRunAlarm.ConvertToAlarmHubTriggeredResponse();
                    model.FlagAllowAcknowledged = item.FlagAllowAcknowledge;

                    proxy.onAlarmTriggered(model);
                }
            );
        }

        public void BroadcastAlarmAcknowledged(Guid masterDailyRunAlarmGuid)
        {
            var dailyRunAlarm = _masterDailyRunAlarmRepository.FindById(masterDailyRunAlarmGuid);

            BroadcastClients(
                dailyRunAlarm,
                (item, proxy) =>
                {
                    proxy.onAlarmAcknowledged(masterDailyRunAlarmGuid);
                }
            );

        }

        private void BroadcastClients(TblMasterDailyRunResource_Alarm dailyRunAlarm, Action<MasterGroupAlarmModel, dynamic> fnBroadcast)
        {
            try
            {
                if (dailyRunAlarm == null) return;

                var masterGroup = _masterGroupRepository.GetPermittedAlarmGroupBySite(dailyRunAlarm.MasterSite_Guid.Value)
                    .OrderByDescending(e => e.FlagAllowAcknowledge);

                if (masterGroup == null || !masterGroup.Any()) return;

                foreach (var item in masterGroup)
                {
                    dynamic proxy = _clients.Group(item.Guid.ToString());

                    fnBroadcast(item, proxy);
                }
            }
            catch (Exception err)
            {
                _systemLogHistoryErrorRepository.CreateHistoryError(err);
            }   
        }
    }
}
