using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Implementations.TruckLiabilityLimit;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.Messagings.TruckLiabilityLimit;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_RouteController : ApiControllerBase
    {
        private readonly ITruckLiabilityLimitService _truckLiabilityLimitService;
        private readonly IRunControlService _runControlService;
        private readonly IRouteGroupService _routeGroupService;
        private readonly ICrewService _crewService;
        private readonly IOnHandRouteService _onHandRouteService;
        public v1_RouteController(
            ITruckLiabilityLimitService truckLiabilityLimitService,
            IRunControlService runControlService,
            IRouteGroupService routeGroupService,
            ICrewService crewService,
            IOnHandRouteService onHandRouteService)
        {
            _truckLiabilityLimitService = truckLiabilityLimitService;
            _runControlService = runControlService;
            _routeGroupService = routeGroupService;
            _crewService = crewService;
            _onHandRouteService = onHandRouteService;
        }


        #region Product Backlog Item 25284:Run Control: Allow user to override and Close the Run Manually
        /// <summary>
        ///  /* Required! Format 'MM/dd/yyyy' */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView CloseRunManuallyUpdate(ManuallyCloseRunRequest request)
        {

            return _runControlService.UpdateCloseRunManually(request);
        }

        #endregion

        #region Other
        [HttpPost]
        public IEnumerable<Ocean.Models.TblMasterRouteGroup> GetRouteGroupBySite(Guid siteGuid, bool isKendoDroupdown = false)
        {
            var data = _routeGroupService.GetRouteGroupBySite(siteGuid).OrderBy(e => e.MasterRouteGroupName);
            return data;
        }
        [HttpGet]
        public RouteGroupAndGroupDetailView GetRouteGroupAndRouteDetail(Guid? siteGuid)
        {
            return _routeGroupService.GetRouteGroupAndRouteDetailBySite(siteGuid.Value);
        }
        [HttpPost]
        public IEnumerable<RunResourceByGroupDetailAndWorkDayResult> GetDailyRun(RouteGroupDetailRequest request)
        {
            return _crewService.GetRunResourceByGroupDetailAndWorkDay(request);
        }
        #endregion

        #region Product Backlog Item 26642:Modify Job Property in Run Control to support Cash Add Workflow

        [HttpPost]
        public JobPropertiesResponse GetCashAddJobProperites(JobPropertiesRequest request)
        {

            return _runControlService.GetCashAddJobProperites(request);
        }

        #endregion

        #region Job(s) not allow to close run
        /// <summary>
        /// Alert job(s) not allow to close run when there is P or TV-P job that has no seal attached.
        /// </summary>
        /// <param name="dailyRunGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public SystemMessageView JobItemIsEmpty(Guid dailyRunGuid)
        {
            return _runControlService.IsJobEmptyByDailyRun(dailyRunGuid);
        }

        /// <summary>
        /// Job(s) not allow to close run when there is P or TV-P job that has no seal attached.
        /// </summary>
        /// <param name="runDailyGuid"></param>
        /// <param name="isDolphin"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> GetJobCannotClose(Guid runDailyGuid, bool isDolphin = false)
        {
            return _runControlService.GetJobCannotClose(runDailyGuid, isDolphin);
        }

        [HttpGet]
        public SystemMessageView ValidateJobCannotCloseRun(Guid dailyRunguid, string vehicleNo)
        {
            return _runControlService.GetValidateJobCannotClose(dailyRunguid, ApiSession.UserLanguage_Guid.GetValueOrDefault(), vehicleNo);
        }

        #endregion


        /// <summary>
        /// Validate on move jobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public LiabilityLimitResponse ValidateTruckLiabilityLimitMoveJobs(LiabilityLimitExistsJobsRequest request)
        {
            return _truckLiabilityLimitService.IsOverLiabilityLimitWhenExistJobs(request);
        }

        /// <summary>
        ///  Validate on update jobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public LiabilityLimitResponse ValidateTruckLiabilityLimit(LiabilityLimitNoExistsItemsRequest request)
        {
            return _truckLiabilityLimitService.IsOverLiabilityLimitWhenNoExistsItems(request);
        }


        [HttpPost]
        public LiabilityLimitResponse ValidateTruckLiabilityLimitBCO(LiabilityLimitNoExistsJobsRequest request)
        {
            return _truckLiabilityLimitService.IsOverLialibityLimitWhenNoExistsJobs(request);
        }

        /// <summary>
        ///  Validate at run control display icon
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public PercentageLiabilityLimitAlertResponse GetTruckLiabilityLimitPercentageAlert(LiabilityLimitExistsRunRequest request)
        {
            return _truckLiabilityLimitService.GetTruckLiabilityLimitPercentageAlert(request);
        }

       /// <summary>
       /// Get STC by job list
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        [HttpPost]
        public JobWithSTCResponse GetSTCOnHandByJob(JobWithSTCRequest request)
        {
            return _onHandRouteService.GetSTCOnHandSummaryTips(request);
        }
      
        [HttpPost]
        public ConvertBankCleanOutTotalLiabilityResponse GetConvertBankCleanOutTotalLiability(ConvertBankCleanOutTotalLiabilityRequest request)
        {
            return _truckLiabilityLimitService.GetConvertBankCleanOutTotalLiability(request);
        }
       
    }
}