using Bgt.Ocean.Models.Nemo.RouteOptimization;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Implementations.Nemo.RouteOptimization;
using Bgt.Ocean.Service.Messagings.RouteOptimization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_RouteOptimizeController : ApiControllerBase
    {
        #region Objects & Variables
        private readonly IRouteOptimizationService _routeOptimizationService;
        private readonly IMasterRouteService _masterRouteService;
        #endregion

        #region Constructor
        public v1_RouteOptimizeController(
            IRouteOptimizationService routeOptimizationService,
            IMasterRouteService masterRouteService)
        {
            _routeOptimizationService = routeOptimizationService;
            _masterRouteService = masterRouteService;
        }
        #endregion

        #region Route Optimization

        #region Select
        [HttpGet]
        public IEnumerable<DailyRunOptimizationView> GetDailyRun_Optimizations(Guid siteGuid, DateTime workDate)
        {
            return _routeOptimizationService.GetDailyRun_Optimizations(siteGuid, workDate.ToString("MM/dd/yyyy"));
        }

        [HttpGet]
        public IEnumerable<Ocean.Models.Nemo.RouteOptimization.MasterRouteOptimizationView> GetMasterRoute_Optimizations(Guid siteGuid)
        {
            return _routeOptimizationService.GetMasterRoute_Optimizations(siteGuid);
        }

        [HttpGet]
        public IEnumerable<NemoOptimizationTransactionResponse> GetOptimizationResult(Guid siteGuid, DateTime workDate, int optimizeMode)
        {
            return _routeOptimizationService.GetOptimizationResult(siteGuid, workDate, optimizeMode);
        }

        [HttpGet]
        public RouteOptimizedLocationDetails GetMapDetails(Guid optimizeGuid)
        {
            return _routeOptimizationService.GetRouteOptimizedLocationDetails(optimizeGuid);
        }

        [HttpGet]
        public RouteDirectionAllResult GetRouteDirection(Guid optimizeGuid)
        {
            return _routeOptimizationService.GetRouteDirection(optimizeGuid);
        }
        #endregion

        #region Save
        [HttpPost]
        public RouteOptimizeResponseView RequestOptimization(NemoOptimizationViewRequest request)
        {
            switch (request.OptimizationType)
            {
                case 0:
                    return _routeOptimizationService.RouteOptimize_DailyRun(request);
                case 1:
                    return _routeOptimizationService.RouteOptimize_MasterRoute(request);
                default:
                    return new RouteOptimizeResponseView();
            }
        }

        [HttpPost]
        public NemoOptimizationResponse ApproveAction(NemoOptimizationActionRequest request)
        {
            return _routeOptimizationService.OptimizationAction(request);
        }
        #endregion


        #endregion

        #region Master Route Optimaization Summary
        [HttpPost]
        public MasterRouteSummaryResponse PostMasterRoute_OptimizationsSummary(MasterRouteSummaryRequest request)
        {
            MasterRouteSummaryResponse response = new MasterRouteSummaryResponse();
            //List<MasterRouteSummaryView> list = new List<MasterRouteSummaryView>();
            //for (int i = 0; i < 5; i++)
            //{
            //    list.Add(new MasterRouteSummaryView
            //    {
            //        MasterRouteName = "MON-EW-01",
            //        Holiday = "No",
            //        MasterRouteGroupName = "RG001",
            //        MasterRouteGroupDetailName = "RG00" + i,
            //        Stops = 10 + i,
            //        StrStops = (10 + i).ToString(),
            //        Locations = 15 + i,
            //        StrLocations = (15 + i).ToString(),
            //        Jobs = 20 + i,
            //        StrJobs = (20 + i).ToString(),
            //        OptimizationStatus = (i == 1 || i == 5) ? "In-progress" : (i == 2 ? "Completed" : "None"),
            //        OptimizationStatusId = (i == 1 || i == 5) ? 1 : (i == 2 ? 3 : 0) // 0: NONE Please return 0 instead of NULL
            //    });
            //}


            response = _routeOptimizationService.GetMasterRouteOptimizationsSummary(request);
            return response;
        }

        [HttpPost]
        public DailyRouteSummaryResponse PostDailyRoute_OptimizationsSummary(DailyRouteSummaryRequest request)
        {
            DailyRouteSummaryResponse response = new DailyRouteSummaryResponse();
            DailyRouteRequest set = new DailyRouteRequest()
            {
                SiteGuid = request.SiteGuid,
                WorkDate = request.WorkDate,
                SortWith = request.SortWith,
                SortBy = request.SortBy,
                Skip = request.Skip,
                PageNumber = request.PageNumber,
                NumberPerPage = request.NumberPerPage,
                MaxRow = request.MaxRow,
                FlagShowAllRunStatus = true,
                FlagShowAllOptStatus = true,
                Filter = request.Filter
            };
            DailyRouteResponse get = _routeOptimizationService.GetDailyRouteList(set);
            response.DailyRouteSummaryList = get.DailyRouteList;
            response.Total = get.Total;
            return response;
        }
        #endregion

        #region Request Management      


        [HttpGet]
        public IEnumerable<DdlViewModel> GetStatusList()
        {
            var result = _routeOptimizationService.GetStatusList();
            return result;
        }

        [HttpGet]
        public IEnumerable<DdlViewModel> GetRequestTypeListByRouteType(Guid routeTypeGuid)
        {
            var result = _routeOptimizationService.GetRequestTypeListByRouteType(routeTypeGuid);
            return result;
        }

        [HttpGet]
        public IEnumerable<DdlViewModel> GetRouteTypeList()
        {
            var result = _routeOptimizationService.GetRouteTypeList();
            return result;
        }

        [HttpPost]
        public RequestManagementResponse PostRequestManagementList(RequestManagementRequest request)
        {
            var result = _routeOptimizationService.GetRouteOptimizationRequestManagement(request);
            return result;
        }

        [HttpGet]
        public MasterRouteRequestDetailResponse GetRequestManagementDetailRM(Guid requestGuid)
        {
            var result = _routeOptimizationService.GetOptimizeRequestMangementRmDetail(requestGuid);
            return result;
        }

        [HttpPost]
        public IEnumerable<DdlViewModel> PostMasterRouteNameList(MasterRouteNameRequest request)
        {
            var result = _masterRouteService.GetMasterRouteDDL(request);
            return result;
        }

        [HttpPost]
        public MasterRouteOptimizationResponse PostMasterRouteOptimizationList(MasterRouteOptimizationListRequest request)
        {
            var result = _routeOptimizationService.GetRouteOptimizeByRequest(request);
            result.IsSuccess = true;
            return result;
        }

        [HttpPost]
        public MasterRouteOptimizationResponse PostMasterRouteOptimizationRequestList(MasterRouteOptimizationRequestListRequest request)
        {

            var result = _routeOptimizationService.GetMasterRouteOptimizationRequests(request);
            return result;
        }

        [HttpPost]
        public MasterRouteRequestResponse InsertMasterRouteRequest(MasterRouteInsertRequest request)
        {
            var response = _routeOptimizationService.InsertMasterRouteRequest(request);
            return response;
        }

        [HttpPost]
        public CancelRequestResponse UpdateCancelRequest(CancelRequest request)
        {
         
            return _routeOptimizationService.UpdateCancelingRequest(request);
        }

        [HttpPost]
        public HttpResponseMessage GenerateRoadnetFile()
        {
            var data = _routeOptimizationService.GenerateFile();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage GenerateFileResponseToRoadnet()
        {
            var data = _routeOptimizationService.GenerateFileResponseToRoadnet();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }
        

        [HttpPost]
        public HttpResponseMessage ReceiveRoadnetFileUpdateInProgress()
        {
            var data = _routeOptimizationService.ReceiveRoadnetFileUpdateInProgressAndCanceled();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage ReceiveRoadnetFileUpdateCompleted()
        {
            var data = _routeOptimizationService.ReceiveRoadnetFileUpdateCompleted();
            data.IsSuccess = string.IsNullOrEmpty(data.Message);            
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        [HttpPost]
        public DailyRouteResponse PostDailyRouteList(DailyRouteRequest request)
        {
            DailyRouteResponse response = _routeOptimizationService.GetDailyRouteList(request);
            return response;
        }

        [HttpPost]
        public JobUnassignedResponse PostUnassignedList(DailyRouteRequest request)
        {
            var response = _routeOptimizationService.GetUnassignedList(request);
            return response;
        }

        [HttpGet]
        public RequestManagementDetailResponse GetRequestManagementDetailRD(Guid requestGuid)
        {
            var result = _routeOptimizationService.GetOptimizeRequestMangementDRDetail(requestGuid);
            return result;
        }
        [HttpGet]
        public RouteOptimizeErrorDetailView GetErrorDetailInfo(Guid requestGuid)
        {
            return _routeOptimizationService.GetErrorDetailInfo(requestGuid);
        }
        [HttpPost]
        public ValidateResponse ValidateRouteBalancing(DailyRouteDetailRequest request)
        {
            return _routeOptimizationService.ValidateRouteBalancing(request);
        }
        /// <summary>
        /// ยืม Model response มาจาก master route 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public MasterRouteRequestResponse InsertDailyRouteRequest(DailyRouteDetailRequest request)
        {
            var response = _routeOptimizationService.InsertDailyRouteRequest(request);
            return response;
        }

        [HttpGet]
        public GetRouteOptimizePathResponse GetRouteOptimizePath(Guid countryGuid)
        {
            return _routeOptimizationService.GetRouteOptimizePath(countryGuid);
        }

        #region GenerateFileRoadNet 

        #endregion
        #endregion
    }
}