using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static Bgt.Ocean.Infrastructure.Util.EnumMasterRoute;
using static Bgt.Ocean.Models.MasterRoute.MassUpdateView;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_MasterRouteController : ApiControllerBase
    {

        private readonly IMasterRouteService _masterRouteService;
        private readonly IMachineService _machineService;
        private readonly ISystemService _systemService;

        public v1_MasterRouteController(
            IMasterRouteService masterRouteService,
            IMachineService machineService,
            ISystemService systemService)
        {
            _masterRouteService = masterRouteService;
            _machineService = machineService;
            _systemService = systemService;
        }


        #region ### Copy Master Route Jobs
        /// <summary>
        ///  
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public MasterRouteCopyJobsResponse MasterRouteCopyJobsToSameDay(MasterRouteCopyJobsRequest request)
        {
            return _masterRouteService.MasterRouteCopyJobs(request);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public MasterRouteCopyJobsResponse MasterRouteCopyJobsToAnotherday(MasterRouteCopyJobsRequest request)
        {
            return _masterRouteService.MasterRouteCopyJobs(request);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public MasterRouteCopyJobsResponse MasterRouteCopyJobsToHoliday(MasterRouteCopyJobsRequest request)
        {

            return _masterRouteService.MasterRouteCopyJobs(request);
        }
        #endregion

        #region ### Get MultiBr Detail

        [HttpPost]
        public MultiBrDetailResponse GetDestinationByOriginLocation(GetMultiBrDestinationDetailRequest request)
        {
            return _masterRouteService.GetMutiBrDeliveryDetailByOriginLocation(request);
        }


        [HttpPost]
        public MultiBrDetailResponse GetBCDMutiBrPickupDetailByDestinationLocation(GetMultiBrOriginDetailRequest request)
        {
            return _masterRouteService.GetBCDMutiBrPickupDetailByDestinationLocation(request);
        }

        [HttpPost]
        public GetMasterRouteAllCustomerAndLocationResponse GetAllCustomerBySite(GetMasterRouteAllCustomerAndLocationRequest request)
        {
            return _masterRouteService.GetAllCustomerBySite(request);
        }

        [HttpPost]
        public GetMasterRouteAllCustomerAndLocationResponse GetAllLocationByCustomer(GetMasterRouteAllCustomerAndLocationRequest request)
        {
            return _masterRouteService.GetAllLocationByCustomer(request);
        }

        [HttpPost]
        public GetAllSitePathResponse GetAllSitePath(GetAllSitePathRequest request)
        {
            return _masterRouteService.GetAllSitePath(request);
        }
        #endregion

        #region ### Create or Update Master Route Job Muti Branch
        //TV MutiBr
        [HttpPost]
        public MasterRouteCreateJobResponse CreateMasterJobTVMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.CreateMasterJobTVMultiBranch(request));
        }
        [HttpPost]
        public MasterRouteCreateJobResponse UpdateMasterJobTVMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.UpdateMasterJobTVMultiBranch(request));            
        }

        //P MutiBr
        [HttpPost]
        public MasterRouteCreateJobResponse CreateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.CreateMasterJobPickupMultiBranch(request));
        }
        [HttpPost]
        public MasterRouteCreateJobResponse UpdateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.UpdateMasterJobPickupMultiBranch(request));            
        }

        //BCD MutiBr
        [HttpPost]
        public MasterRouteCreateJobResponse CreateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.CreateMasterJobBCDMultiBranch(request));
        }
        [HttpPost]
        public MasterRouteCreateJobResponse UpdateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest request)
        {
            return ValidateCryptoAndProcess(request, () => _masterRouteService.UpdateMasterJobBCDMultiBranch(request));            
        }

        private MasterRouteCreateJobResponse ValidateCryptoAndProcess(MasterRouteCreateJobRequest request, Func<MasterRouteCreateJobResponse> fnProcess)
        {
            try
            {
                var locationGuids = new Guid?[]
               {
                    request.Pickupleg?.MasterCustomerLocation_Guid.GetValueOrDefault(),
                    request.Deliveryleg?.MasterCustomerLocation_Guid.GetValueOrDefault(),                    
               }
               .Select(e => e.GetValueOrDefault()).Distinct().ToArray();

                if (_machineService.IsMachineHasCryptoLock(locationGuids))
                {
                    var response = new MasterRouteCreateJobResponse();
                    var msg = _systemService.GetMessageByMsgId(-17373); // This location has crypto lock
                    response.SetMessageView(msg);
                    return response;
                }

                return fnProcess();
            }
            catch 
            {
                return fnProcess();
            }
        }

        #endregion

        #region ### Update Job Order
        [HttpPost]
        public bool MasterRouteUpdateJobOrder(UpdateJobOrderRequest request)
        {
            return _masterRouteService.UpdateJobOrderByJobs(request);
        }

        [HttpPost]
        public UpdateSequenceIndexResponse ModifyMasterrouteSequenceIndex(UpdateSequenceIndexRequest request)
        {
            return _masterRouteService.UpdateSequenceIndex(request);
        }
        /// <summary>
        /// Update manual seq-index
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ManualSortSeqIndexResponse ModifyManualSortSeqIndex(ManualSortSeqIndexRequest request)
        {
            return _masterRouteService.UpdateManualSortSeqIndex(request);

        }
        #endregion

        #region Mass Update     
        [HttpPost]
        public MassUpdateJobResponse SubmitMassUpdate(MassUpdateJobCriteriaRequest request)
        {
            return _masterRouteService.SetDataMassUpdate(request);
        }

        [HttpPost]
        public SystemMessageView UpdateMasterRouteJobOrder(MassUpdateDataJobOrderViewRequest model)
        {
            model.SystemCategoryID = EnumMasterRouteLogCategory.Job_MassUpdateJob;
            return _masterRouteService.UpdateJobOrderByMasterRoute(model);
        }
        /// <summary>
        /// Get dropdown day of week.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropDownDayOfWeekView> GetDayOfWeek()
        {
            IEnumerable<DropDownDayOfWeekView> data = null;
            data = _masterRouteService.GetDayOfWeek();
            return data;
        }

        /// <summary>
        /// Get dropdown LOB.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropDownLobView> GetLOBDropdownList()
        {
            IEnumerable<DropDownLobView> data = null;
            data = _masterRouteService.GetLOBDropdownList();
            return data;
        }

        [HttpGet]
        public IEnumerable<DropDownCustomerView> GetCustomerBySite(Guid siteGuid)
        {
            IEnumerable<DropDownCustomerView> data = _masterRouteService.GetCustomerDDLBySite(siteGuid);
            return data;
        }

        [HttpGet]
        public IEnumerable<DropDownLocationView> GetLocationByCustomer(Guid siteGuid, Guid customerGuid)
        {
            IEnumerable<DropDownLocationView> data = _masterRouteService.GetCustomerLocationDDLBySite(siteGuid, customerGuid);
            return data;
        }

        [HttpPost]
        public IEnumerable<DropDownServiceTypeView> GetServiceTypeByLOBs(ServiceTypeByLOBsRequest request)
        {
            IEnumerable<DropDownServiceTypeView> data = _masterRouteService.GetServiceTypeByLOB(request.LobGuids, request.FlagPcustomer, request.FlagDcustomer, request.FlagSameSite);
            return data;
        }
        #endregion
    }
}