using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Messagings.RunControlService;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;

namespace Bgt.Ocean.Service.Implementations.RunControl
{
    #region interface
    public interface ICrewService
    {
        ValidateCrewOnPortalResponse ValidateCrewOnPortal(ValidateCrewOnPortalRequest request);
        IEnumerable<RunResourceByGroupDetailAndWorkDayResult> GetRunResourceByGroupDetailAndWorkDay(RouteGroupDetailRequest request);

        bool CheckIfCrewOnDispatchedRun(Guid employeeGuid, DateTime workDate);
        int GetCrewRold(Guid dailyEmplyeeGuid);
        IEnumerable<string> GetDolphinConfiguration(Guid countryGuid, Guid siteGuid);

        IEnumerable<string> CheckEmployeeAssignInAnotherRun(IEnumerable<Guid> empGuid, Guid dailyRunGuid, DateTime? workDate);
    }
    #endregion

    public class CrewService : ICrewService
    {
        #region Objects & Variables
        private readonly IMasterDailyEmployeeRepository _masterDailyEmployeeRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        #endregion

        #region Constructor
        public CrewService(IMasterDailyEmployeeRepository masterDailyEmployeeRepository
                            , IMasterDailyRunResourceRepository dailyRunRepository
                            , ISystemServiceJobTypeRepository systemServiceJobTypeRepository
                            , ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository)
        {
            _masterDailyEmployeeRepository = masterDailyEmployeeRepository;
            _masterDailyRunRepository = dailyRunRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
        }
        #endregion

        #region Functions

        #region Select
        public ValidateCrewOnPortalResponse ValidateCrewOnPortal(ValidateCrewOnPortalRequest request)
        {
            var result = new ValidateCrewOnPortalResponse();
            var requestRepo = new ValidateCrewOnPortalView_Request
            {
                CrewID = request.crewId,
                RunDate = request.runDate,
                SiteCode = request.siteCode
            };

            var resultRepo = _masterDailyRunRepository.ValidateCrewOnPortal(requestRepo);
            var resultItem = MapperService.Map<IEnumerable<ValidateCrewOnPortalView>, IEnumerable<ValidateCrewOnPortalResponse_Main>>(resultRepo).ToList();

            result.result = resultItem;
            result.rows = resultItem.Count;

            return result;
        }

        public IEnumerable<RunResourceByGroupDetailAndWorkDayResult> GetRunResourceByGroupDetailAndWorkDay(RouteGroupDetailRequest request)
        {
            var WorkDate = request.strWorkDate.ChangeFromStringToDate(request.DateTimeFormat);

            var tblJobType = _systemServiceJobTypeRepository.FindById(request.JobTypeGuid.Value);

            var tblRun = _masterDailyRunRepository.Func_RunResourceByGroupDetailAndWorkDay(WorkDate, request.GroupDetailGuid, request.SiteGuid);

            switch (tblJobType.ServiceJobTypeID)
            {
                case IntTypeJob.P:
                case IntTypeJob.T:
                case IntTypeJob.BCP:
                case IntTypeJob.FLM:
                    break;
                case IntTypeJob.TV:
                case IntTypeJob.TV_MultiBr:
                    // ขา P และ D ต้องไม่ลงรถคันเดียวกัน      
                    if (request.LegName == JobActionAbb.StrDelivery)
                    {
                        if (request.RunGuid != null)
                        {
                            tblRun = tblRun.Where(o => o.Guid != request.RunGuid.GetValueOrDefault() && o.RunResourceDailyStatusID.Value == 1 && o.FlagRouteBalanceDone == false);
                        }
                        else
                        {
                            tblRun = tblRun.Where(o => o.RunResourceDailyStatusID.Value == 1 && o.FlagRouteBalanceDone == false);
                        }
                    }
                    else if (request.RunGuid != null)
                    {
                        tblRun = tblRun.Where(o => o.Guid != request.RunGuid.GetValueOrDefault());

                    }
                    break;
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:
                case IntTypeJob.BCD:
                case IntTypeJob.BCD_MultiBr:
                    // Ready
                    tblRun = tblRun.Where(o => o.RunResourceDailyStatusID.Value == 1 && o.FlagRouteBalanceDone == false);
                    break;

            }

            return tblRun;
        }

        public bool CheckIfCrewOnDispatchedRun(Guid employeeGuid, DateTime workDate)
        {
            return _masterDailyEmployeeRepository.CheckCrewOnDispatchRun(employeeGuid, workDate);
        }

        public int GetCrewRold(Guid dailyEmplyeeGuid)
        {
            return _masterDailyEmployeeRepository.GetCrewRole(dailyEmplyeeGuid);
        }

        public IEnumerable<string> GetDolphinConfiguration(Guid countryGuid, Guid siteGuid)
        {
            var allowLogin = _systemEnvironmentMasterCountryRepository.GetValueByAppKey(EnumAppKey.RoleAllowDolphinLogin, countryGuid, siteGuid);
            if (allowLogin != null)
            {
                var result = allowLogin.AppValue1.Split(',').AsEnumerable();
                return result;
            }
            return null;
        }


        public IEnumerable<string> CheckEmployeeAssignInAnotherRun(IEnumerable<Guid> empGuid, Guid dailyRunGuid, DateTime? workDate)
        {
            var lstEmployee = _masterDailyEmployeeRepository.CheckEmployeeAssignInAnotherRun(empGuid, dailyRunGuid,workDate);
            return lstEmployee;
        }
        #endregion

        #endregion
    }
}
