using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.Nemo.NemoSync;
using Bgt.Ocean.Models.Nemo.RouteOptimization;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Nemo;
using Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.RouteOptimization;
using Bgt.Ocean.Service.ModelViews.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Implementations.Nemo.RouteOptimization
{
    #region Interface
    public interface IRouteOptimizationService
    {
        #region Select
        IEnumerable<DailyRunOptimizationView> GetDailyRun_Optimizations(Guid siteGuid, string workDate);
        IEnumerable<Models.Nemo.RouteOptimization.MasterRouteOptimizationView> GetMasterRoute_Optimizations(Guid siteGuid);
        IEnumerable<NemoOptimizationTransactionResponse> GetOptimizationResult(Guid siteGuid, DateTime workDate, int optimizeMode);
        RouteOptimizedLocationDetails GetRouteOptimizedLocationDetails(Guid optimizeGuid);
        RouteDirectionAllResult GetRouteDirection(Guid optimizeGuid);
        IEnumerable<DdlViewModel> GetStatusList();
        IEnumerable<DdlViewModel> GetRouteTypeList();
        IEnumerable<DdlViewModel> GetRequestTypeListByRouteType(Guid routeTypeGuid);
        RequestManagementResponse GetRouteOptimizationRequestManagement(RequestManagementRequest request);       
        DailyRouteResponse GetDailyRouteList(DailyRouteRequest request);
        JobUnassignedResponse GetUnassignedList(DailyRouteRequest request);
        MasterRouteOptimizationResponse GetMasterRouteOptimizationRequests(MasterRouteOptimizationRequestListRequest request);
        MasterRouteRequestDetailResponse GetOptimizeRequestMangementRmDetail(Guid requestGuid);
        DailyRouteRequestDetailResponse GetOptimizeRequestMangementDRDetail(Guid requestGuid);
        MasterRouteOptimizationResponse GetRouteOptimizeByRequest(MasterRouteOptimizationListRequest req);
        MasterRouteSummaryResponse GetMasterRouteOptimizationsSummary(MasterRouteSummaryRequest request);
        ValidateResponse ValidateRouteBalancing(DailyRouteDetailRequest request);
        RouteOptimizeErrorDetailView GetErrorDetailInfo(Guid requestGuid);

        GetRouteOptimizePathResponse GetRouteOptimizePath(Guid countryGuid);
        #endregion

        #region Insert
        RouteOptimizeResponseView RouteOptimize_DailyRun(NemoOptimizationViewRequest request);
        RouteOptimizeResponseView RouteOptimize_MasterRoute(NemoOptimizationViewRequest request);

        MasterRouteRequestResponse InsertDailyRouteRequest(DailyRouteDetailRequest request);
        MasterRouteRequestResponse InsertMasterRouteRequest(MasterRouteInsertRequest req);
        GenerateRoadnetResponse GenerateFile();
        GenerateRoadnetResponse GenerateFileResponseToRoadnet();
        ReceiveRoadnetResponse ReceiveRoadnetFileUpdateInProgressAndCanceled();
        ReceiveRoadnetResponse ReceiveRoadnetFileUpdateCompleted();
        #endregion

        #region Update
        NemoOptimizationResponse OptimizationAction(NemoOptimizationActionRequest request);
        CancelRequestResponse UpdateCancelingRequest(CancelRequest request);
        #endregion
    }
    #endregion

    public class RouteOptimizationService : IRouteOptimizationService
    {
        #region Objects & Variables              
        private readonly IRouteOptimizationBroadcastService _routeOptBroadcastService;

        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly IMasterNemoQueueRouteOptimizationRepository _masterNemoQueueRouteOptimizationRepository;
        private readonly IMasterRouteJobHeaderRepository _masterRouteJobHeaderRepository;
        private readonly ISystemGlobalUnitRepository _systemGlobalUnitRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemRouteOptimizationStatusRepository _systemRouteOptimizationStatusRepository;
        private readonly ISystemRouteOptimizationRouteTypeRepository _systemRouteOptimizationRouteTypeRepository;
        private readonly ISystemRouteOptimizationRouteTypeRequestTypeRepository _systemRouteOptimizationRouteTypeRequestTypeRepository;
        private readonly ITransactionRouteOptimizationHeaderRepository _transactionRouteOptimizationHeaderRepository;
        private readonly IMasterRouteOptimizationStatusRepository _masterRouteOptimizationStatusRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly IBaseRequest _baseRequest;
        private readonly IUnitOfWork<OceanDbEntities> _uow;

        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemRunningValueCustomRepository _systemRunningValueCustomRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroupDetailRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly ITransactionRouteOptimizationHeaderQueueRepository _transactionRouteOptimizationHeaderQueueRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;

        private readonly IMasterRunResourceRepository _masterRunResourceRepository;
        #endregion

        #region Constructor
        public RouteOptimizationService(
            //INemoAuthenticationService nemoAuthenticationService
            // INemoConfigurationService nemoConfigurationService
             IRouteOptimizationBroadcastService routeOptBroadcastService
            , IMasterDailyRunResourceRepository masterDailyRunResourceRepository
            , IMasterNemoQueueRouteOptimizationRepository masterNemoQueueRouteOptimizationRepository
            , IMasterRouteJobHeaderRepository masterRouteJobHeaderRepository
            , ISystemGlobalUnitRepository systemGlobalUnitRepository
            , ISystemMessageRepository systemMessageRepository
            , ISystemService systemService
            , ISystemRouteOptimizationStatusRepository systemRouteOptimizationStatusRepository
            , ISystemRouteOptimizationRouteTypeRepository systemRouteOptimizationRouteTypeRepository
            , ISystemRouteOptimizationRouteTypeRequestTypeRepository systemRouteOptimizationRouteTypeRequestTypeRepository
            , ITransactionRouteOptimizationHeaderRepository transactionRouteOptimizationHeaderRepository
            , IMasterRouteOptimizationStatusRepository masterRouteOptimizationStatusRepository
            , ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository
            , IBaseRequest baseRequest
            , IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository
            , IMasterActualJobHeaderRepository masterActualJobHeaderRepository
            , IUnitOfWork<OceanDbEntities> uow
            , IMasterSiteRepository masterSiteRepository
            , ISystemRunningValueCustomRepository systemRunningValueCustomRepository
            , ISystemServiceJobTypeRepository systemServiceJobTypeRepository
            , IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository
            , ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository
            , ITransactionRouteOptimizationHeaderQueueRepository transactionRouteOptimizationHeaderQueueRepository
            , IMasterRunResourceRepository masterRunResourceRepository
            , IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository
            )
        {                        
            _routeOptBroadcastService = routeOptBroadcastService;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _masterNemoQueueRouteOptimizationRepository = masterNemoQueueRouteOptimizationRepository;
            _masterRouteJobHeaderRepository = masterRouteJobHeaderRepository;
            _systemGlobalUnitRepository = systemGlobalUnitRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemService = systemService;
            _systemRouteOptimizationStatusRepository = systemRouteOptimizationStatusRepository;
            _systemRouteOptimizationRouteTypeRepository = systemRouteOptimizationRouteTypeRepository;
            _systemRouteOptimizationRouteTypeRequestTypeRepository = systemRouteOptimizationRouteTypeRequestTypeRepository;
            _transactionRouteOptimizationHeaderRepository = transactionRouteOptimizationHeaderRepository;
            _masterRouteOptimizationStatusRepository = masterRouteOptimizationStatusRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _baseRequest = baseRequest;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _uow = uow;
            _masterSiteRepository = masterSiteRepository;
            _systemRunningValueCustomRepository = systemRunningValueCustomRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _masterRouteGroupDetailRepository = masterRouteGroupDetailRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _transactionRouteOptimizationHeaderQueueRepository = transactionRouteOptimizationHeaderQueueRepository;
            _masterRunResourceRepository = masterRunResourceRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
        }
        #endregion

        #region Public

        #region Select
        public IEnumerable<DailyRunOptimizationView> GetDailyRun_Optimizations(Guid siteGuid, string workDate)
        {
            return _masterDailyRunResourceRepository.GetDailyRun_Optimizations(siteGuid, workDate).OrderBy(o => o.MasterRouteGroupDetailName);
        }

        public IEnumerable<Models.Nemo.RouteOptimization.MasterRouteOptimizationView> GetMasterRoute_Optimizations(Guid siteGuid)
        {
            return _masterRouteJobHeaderRepository.GetMasterRoute_Optimizations(siteGuid).OrderBy(o => o.MasterDayOfWeek_Sequence).ThenBy(o => o.WeekTypeInt).ThenBy(o => o.MasterRouteGroupName).ThenBy(o => o.MasterRouteGroupDetailName);
        }

        public IEnumerable<NemoOptimizationTransactionResponse> GetOptimizationResult(Guid siteGuid, DateTime workDate, int optimizeMode)
        {
            var data = _masterNemoQueueRouteOptimizationRepository.GetOptimizationResult(siteGuid, workDate, optimizeMode);

            foreach (var item in data)
            {
                if (optimizeMode == 0)
                {
                    item.DailyRunResource_FullName = _masterDailyRunResourceRepository.GetDailyRunDetail(item.DailyRunResource_Guid)?.VehicleNumberFullName;
                }
                else if (optimizeMode == 1)
                {
                    item.DailyRunResource_FullName = _masterNemoQueueRouteOptimizationRepository.GetRunInMasterRouteFullname(item.MasterRoute_Guid.Value, item.DailyRunResource_Guid);
                }
            }
            return data;
        }

        public RouteOptimizeResponseView RouteOptimize_DailyRun(NemoOptimizationViewRequest request)
        {
            var result = new RouteOptimizeResponseView();
            result.NemoOptimizationResponse.Success = true;
            var typeOptimize = (int)EnumRouteOptimization.TypeOptimization.Create;
            try
            {
                #region Create Route Optimization Log
                var createPlan = false;
                var dailyRunOptmization = _masterNemoQueueRouteOptimizationRepository.GetOptimizationDailyRun(request.RunGuid);
                var optimizationLocations = _masterNemoQueueRouteOptimizationRepository.GetOptimizationDailyRunLocation(request.RunGuid);
                if (dailyRunOptmization.Any() && optimizationLocations.Any())
                {
                    var optimizationPlan = new List<TblMasterNemoQueueRouteOptimization>();
                    var optimizationDetail = new List<TblMasterNemoQueueRouteOptimization_Detail>();
                    var transactionID = _masterNemoQueueRouteOptimizationRepository.GetNextTransactionID();

                    var jobOrder = 1;
                    foreach (var plan in dailyRunOptmization)
                    {
                        optimizationPlan.Add(new TblMasterNemoQueueRouteOptimization()
                        {
                            Guid = plan.NemoQueueGuid,
                            MasterDailyRunResource_Guid = plan.DailyGuid,
                            MasterSite_Guid = plan.BranchGuid,
                            TransactionID = transactionID,
                            RouteOptimizeType = typeOptimize,
                            DateTimeShiftAsync = request.WorkDate,
                            StatusQueue = 1,
                            NemoAsyncErrorCount = 0,
                            OptimizationOrder = jobOrder++,
                            UserCreated = ApiSession.UserName,
                            DatetimeCreated = DateTime.Now,
                            UniversalDatetimeCreated = DateTime.UtcNow
                        });

                        optimizationDetail.AddRange(optimizationLocations.Where(o => o.DailyRunGuid == plan.DailyGuid).Select(o => new TblMasterNemoQueueRouteOptimization_Detail()
                        {
                            Guid = Guid.NewGuid(),
                            NemoQueueRouteOptimization_Guid = plan.NemoQueueGuid,
                            MasterJob_Guid = o.JobGuid,
                            MasterCustomerLocation_Guid = o.LocationGuid,
                            MasterCustomerLocation_Code = o.LocationCode,
                            JobOrder = o.JobOrder
                        }).ToList());
                    }
                    createPlan = _masterNemoQueueRouteOptimizationRepository.SaveOptimizationPlan(optimizationPlan, optimizationDetail);
                    result.TransactionID = transactionID;
                    result.NemoTaskGuid = optimizationPlan.FirstOrDefault(o => o.OptimizationOrder == (jobOrder - 1)).Guid;
                }
                #endregion
             
            }
            catch (Exception ex)
            {
                result.NemoOptimizationResponse = new NemoOptimizationResponse() { Success = false, Message = ex.Message };
            }
            return result;
        }

        public RouteOptimizeResponseView RouteOptimize_MasterRoute(NemoOptimizationViewRequest request)
        {
            var result = new RouteOptimizeResponseView();
            result.NemoOptimizationResponse.Success = true;
            try
            {
                var createPlan = false;
                var transactionID = _masterNemoQueueRouteOptimizationRepository.GetNextTransactionID();
                var jobOrder = 1;
                foreach (var route in request.MasterRoute)
                {
                    #region Create Route Optimization Log
                    var dailyRunOptmization = _masterNemoQueueRouteOptimizationRepository.GetOptimizationMasterRoute(route.RouteGuid, route.RouteDetailGuid);
                    var optimizationLocations = _masterNemoQueueRouteOptimizationRepository.GetOptimizationMasterRouteLocation(route.RouteGuid, route.RouteDetailGuid);
                    if (dailyRunOptmization.Any() && optimizationLocations.Any())
                    {
                        var optimizationPlan = new List<TblMasterNemoQueueRouteOptimization>();
                        var optimizationDetail = new List<TblMasterNemoQueueRouteOptimization_Detail>();

                        foreach (var plan in dailyRunOptmization)
                        {
                            optimizationPlan.Add(new TblMasterNemoQueueRouteOptimization()
                            {
                                Guid = plan.NemoQueueGuid,
                                MasterRoute_Guid = plan.ShiftGuid,
                                MasterDailyRunResource_Guid = plan.DailyGuid,
                                MasterSite_Guid = plan.BranchGuid,
                                TransactionID = transactionID,
                                RouteOptimizeType = request.OptimizationType,
                                WorkDate = request.WorkDate,
                                StatusQueue = 1,
                                NemoAsyncErrorCount = 0,
                                OptimizationOrder = jobOrder++,
                                UserCreated = ApiSession.UserName,
                                DatetimeCreated = DateTime.Now,
                                UniversalDatetimeCreated = DateTime.UtcNow
                            });

                            optimizationDetail.AddRange(optimizationLocations.Where(o => o.DailyRunGuid == plan.DailyGuid).Select(o => new TblMasterNemoQueueRouteOptimization_Detail()
                            {
                                Guid = Guid.NewGuid(),
                                NemoQueueRouteOptimization_Guid = plan.NemoQueueGuid,
                                MasterJob_Guid = o.JobGuid,
                                MasterCustomerLocation_Guid = o.LocationGuid,
                                MasterCustomerLocation_Code = o.LocationCode,
                                JobOrder = o.JobOrder
                            }).ToList());
                        }
                        createPlan = _masterNemoQueueRouteOptimizationRepository.SaveOptimizationPlan(optimizationPlan, optimizationDetail);
                        result.TransactionID = transactionID;
                        result.NemoTaskGuid = optimizationPlan.FirstOrDefault(o => o.OptimizationOrder == (jobOrder - 1)).Guid;
                    }
                    #endregion
                
                }
            }
            catch (Exception ex)
            {
                result.NemoOptimizationResponse = new NemoOptimizationResponse() { Success = false, Message = ex.Message };
            }
            return result;
        }

        public RouteOptimizedLocationDetails GetRouteOptimizedLocationDetails(Guid optimizeGuid)
        {
            return _masterNemoQueueRouteOptimizationRepository.GetRouteOptimizedLocationDetails(optimizeGuid);
        }

        public RouteDirectionAllResult GetRouteDirection(Guid optimizeGuid)
        {
            var routeResult = new RouteDirectionAllResult();

            var branchInfo = _masterNemoQueueRouteOptimizationRepository.GetBranchInformation(optimizeGuid);
            var locationDetail = _masterNemoQueueRouteOptimizationRepository.GetRouteOptimizedLocationDetails(optimizeGuid);
            var locationList = new RouteDirectionAll();

            if (locationDetail.Plan.Any())
            {
                locationList.RoutePlanned.Add(new RouteDirectionRequest()
                {
                    BranchCode = branchInfo.BranchCode,
                    CountryCode = branchInfo.CountryCode,
                    CustomerLocationCode = "DEPOT"
                });
                locationList.RoutePlanned.AddRange(locationDetail.Plan.Where(o => !string.IsNullOrEmpty(o.Code)).OrderBy(o => o.Order).Select(o => new RouteDirectionRequest()
                {
                    BranchCode = branchInfo.BranchCode,
                    CountryCode = branchInfo.CountryCode,
                    CustomerLocationCode = o.Code
                }));
                locationList.RoutePlanned.Add(new RouteDirectionRequest()
                {
                    BranchCode = branchInfo.BranchCode,
                    CountryCode = branchInfo.CountryCode,
                    CustomerLocationCode = "DEPOT"
                });
                //routeResult.RoutePlanned = SyncNemoRouteDirection(locationList.RoutePlanned).Result;
            }

            if (locationDetail.Optimized.Any())
            {
                locationList.RouteOptimized.AddRange(locationDetail.Optimized.Where(o => !string.IsNullOrEmpty(o.Code) && o.Order.HasValue).OrderBy(o => o.Order).Select(o => new RouteDirectionRequest()
                {
                    BranchCode = branchInfo.BranchCode,
                    CountryCode = branchInfo.CountryCode,
                    CustomerLocationCode = o.Code
                }));
                if (locationList.RouteOptimized.Any())
                {
                    //routeResult.RouteOptimized = SyncNemoRouteDirection(locationList.RouteOptimized).Result;
                }
            }
            return routeResult;
        }
        #endregion

        #region Update
        public NemoOptimizationResponse OptimizationAction(NemoOptimizationActionRequest request)
        {
            switch (request.OptimizeAction)
            {
                case 0:
                    var result = _masterNemoQueueRouteOptimizationRepository.UpdateOptimizedAction_Approve(request.OptimizeGuid, ApiSession.UserName);
                    if (!result.Success && result.Message == "-5001")
                    {
                        var message = _systemMessageRepository.GetMessage(-5001, ApiSession.UserLanguage_Guid.Value);
                        result.Title = message.MessageTextTitle;
                        result.Message = message.MessageTextContent;
                    }
                    return result;
                case 1:
                    return _masterNemoQueueRouteOptimizationRepository.UpdateOptimizedAction_Cancel(request.OptimizeGuid, ApiSession.UserName);
                default:
                    return new NemoOptimizationResponse() { Success = false, Message = "Please check optimization type" };
            }
        }
        #endregion

        #endregion

        #region RouteOptimiz
        public IEnumerable<DdlViewModel> GetStatusList()
        {
            var result = _systemRouteOptimizationStatusRepository.FindAll(f => f.FlagForRequest && f.RouteOptimizationStatusID != OptimizationStatusID.NONE).Select(s => new DdlViewModel
            {
                Text = s.RouteOptimizationStatusName,
                Value = s.Guid
            });
            return result;
        }

        public IEnumerable<DdlViewModel> GetRouteTypeList()
        {
            var result = _systemRouteOptimizationRouteTypeRepository.FindAll().Select(s => new DdlViewModel
            {
                Text = s.RouteOptimizationRouteTypeName,
                Value = s.Guid
            });

            return result;
        }
        public IEnumerable<DdlViewModel> GetRequestTypeListByRouteType(Guid routeTypeGuid)
        {
            var result = _systemRouteOptimizationRouteTypeRequestTypeRepository.FindAll(f => f.SystemRouteOptimizationRouteType_Guid == routeTypeGuid)
                .Select(s => new DdlViewModel
                {
                    Value = s.SystemRouteOptimizationRequestType_Guid,
                    Text = $"{s.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeCode}-{s.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeName}"
                }).OrderBy(o => o.Text);
            return result;
        }
        public MasterRouteRequestDetailResponse GetOptimizeRequestMangementRmDetail(Guid requestGuid)
        {
            var userData = _baseRequest.Data;
            return _transactionRouteOptimizationHeaderRepository.GetOptimizeRequestMangementRmDetail(requestGuid, userData.UserLanguageGuid, userData.UserFormatDate);
        }
        public DailyRouteRequestDetailResponse GetOptimizeRequestMangementDRDetail(Guid requestGuid)
        {
            var userData = _baseRequest.Data;
            return _transactionRouteOptimizationHeaderRepository.GetOptimizeRequestMangementRDDetail(requestGuid, userData.UserLanguageGuid, userData.UserFormatDate);
        }
        /// <summary>
        /// Display first grid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestManagementResponse GetRouteOptimizationRequestManagement(RequestManagementRequest request)
        {
            RequestManagementResponse response = new RequestManagementResponse();
            var userData = _baseRequest.Data;
            var data = _transactionRouteOptimizationHeaderRepository.GetOptimizeRequestMangement(request, userData.UserLanguageGuid);
            if (data != null)
            {
                var resp = PaginationHelper.ToPagination(data, request);
                response.RequestManagementList = resp.Data;
                response.Total = (int)resp.Total;
            }
            return response;
        }

        public MasterRouteSummaryResponse GetMasterRouteOptimizationsSummary(MasterRouteSummaryRequest request)
        {
            MasterRouteSummaryResponse result = new MasterRouteSummaryResponse();
            try
            {
                var data = _masterRouteOptimizationStatusRepository.GetRouteOptimizeSummary(request);

                if (data.Any())
                {
                    var res = PaginationHelper.ToPagination(data, request);
                    result.MasterRouteSummaryList = res.Data;
                    result.Total = (int)res.Total;
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        public RouteOptimizeErrorDetailView GetErrorDetailInfo(Guid requestGuid)
        {
            RouteOptimizeErrorDetailView response = new RouteOptimizeErrorDetailView();
            var data = _transactionRouteOptimizationHeaderRepository.FindById(requestGuid);
            if (data != null)
            {
                response.ErrorDetail = data.ErrorDetail;
                response.RequestGuid = requestGuid;
                response.RequestId = data.RequestID;
            }
            return response;
        }
        public MasterRouteOptimizationResponse GetRouteOptimizeByRequest(MasterRouteOptimizationListRequest req)
        {
            MasterRouteOptimizationResponse result = new MasterRouteOptimizationResponse();
            var userData = _baseRequest.Data;
            var data = _transactionRouteOptimizationHeaderRepository.GetRouteOptimizeByRequest(req.RequestGuid, req.MaxRow, userData.UserLanguageGuid);
            if (data != null)
            {
                var fillter = PaginationHelper.ToPagination(data, req);
                result.MasterRouteOptimizationList = fillter.Data.ConvertToView();
                result.Total = (int)fillter.Total;
            }
            return result;
        }
        public MasterRouteOptimizationResponse GetMasterRouteOptimizationRequests(MasterRouteOptimizationRequestListRequest request)
        {
            MasterRouteOptimizationResponse result = new MasterRouteOptimizationResponse();
            var userData = _baseRequest.Data;
            request.UserLangquage = userData.UserLanguageGuid;
            try
            {
                var pagingBase = (PagingBase)request;
                int[] allowShow = new int[] {
                    OptimizationStatusID.NONE,
                    OptimizationStatusID.COMPLETED,
                    OptimizationStatusID.BROKEN
                };
                var req = request.ConvertRequestToModelRequest();

                var datas = _masterRouteOptimizationStatusRepository.GetRouteOptimize(req);
                if (datas != null)
                {
                    datas = request.FlagShowAllStatus
                                            ? datas
                                            : datas.Where(w => allowShow.Contains(w.OptimizationStatusID));
                    var response = PaginationHelper.ToPagination(datas, pagingBase);
                    result.MasterRouteOptimizationList = response.Data.ConvertToView();
                    result.Total = (int)response.Total;


                }

                var msg = _systemMessageRepository.FindByMsgId(0, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true);
                result.SetMessageView(msg);
            }
            catch (Exception ex)
            {

                var msg = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                result.SetMessageView(msg);
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        private SystemMessageView ValidateDataBeforeSaveRequestMasterRouteOptimize(MasterRouteInsertRequest request)
        {
            SystemMessageView result = null;
            var rgdGuids = request.MasterRouteSelectedList.Select(s => s.MasterRouteGroupDetailGuid);
            var route = _masterRouteOptimizationStatusRepository.ValidateStatusBeforeRequestSave(rgdGuids, request.SiteGuid);
            if (route.Any())
            {
                result = _systemMessageRepository.FindByMsgId(-17334, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                result.MessageTextContent = string.Format(result.MessageTextContent, string.Join(",", route));
                result.IsWarning = true;
                return result;
            }
            var sameNameInprograss = _masterRouteOptimizationStatusRepository.ValidateSameRouteGroupDetailNameInprogress(rgdGuids, request.SiteGuid);
            if (sameNameInprograss != null)
            {
                result = _systemMessageRepository.FindByMsgId(-17337, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                result.MessageTextContent = string.Format(result.MessageTextContent, sameNameInprograss.RouteGroupDetailName, sameNameInprograss.RequestId);
                result.IsWarning = true;
                return result;
            }
            return result;

        }
        public CancelRequestResponse UpdateCancelingRequest(CancelRequest request)
        {
            var userData = _baseRequest.Data;
            var response = UpdateCancelRequest(new UpdateCancelRequest
            {
                RequestGuid = request.RequestGuid,
                UserModify = userData.UserName,
                UserLanguageGuid = userData.UserLanguageGuid,
                ClientDate = userData.LocalClientDateTime,
                UtcDate = DateTime.UtcNow,
                StatusFrom = OptimizationStatusID.INPROGRESS,
                StatusTo = OptimizationStatusID.CANCELING
            });

            return response;
        }
        private CancelRequestResponse UpdateCancelRequest(UpdateCancelRequest request)
        {
            var response = new CancelRequestResponse();
            try
            {
                var changeStatus = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(request.StatusTo);
                var tranHead = _transactionRouteOptimizationHeaderRepository.FindById(request.RequestGuid);
                if (tranHead.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID == request.StatusFrom)
                {
                    using (var tran = _uow.BeginTransaction())
                    {
                        // update tran header.
                        tranHead.SystemRouteOptimizationStatus_Guid = changeStatus.Guid;
                        tranHead.UserModified = request.UserModify;
                        tranHead.DatetimeModified = request.ClientDate;
                        tranHead.UniversalDatetimeModified = request.UtcDate;
                        if (tranHead.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RM)
                        {
                            var rgdList = tranHead.TblTransactionRouteOptimizationHeader_Detail
                                .GroupBy(g => g.MasterRouteGroupDetail_Guid)
                                .Select(s => s.Key);

                            var routeStatusTbl = _masterRouteOptimizationStatusRepository
                                                .FindAll(
                                                    f => f.MasterRoute_Guid == tranHead.MasterRoute_Guid
                                                    && rgdList.Any(a => a == f.MasterRouteGroupDetail_Guid)
                                                );
                            foreach (var rt in routeStatusTbl)
                            {
                                rt.SystemRouteOptimizationStatus_Guid = changeStatus.Guid;
                                rt.UserModified = request.UserModify;
                                rt.DatetimeModified = request.ClientDate;
                                rt.UniversalDatetimeModified = request.UtcDate;
                            }
                        }
                        else if (tranHead.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RD)
                        {
                            // update dailyrun 
                            var dailyRun = tranHead.TblTransactionRouteOptimizationHeader_Detail.Where(w => w.MasterDailyRunResource_Guid != null);
                            foreach (var d in dailyRun.Select(r => r.TblMasterDailyRunResource))
                            {
                                d.SystemRouteOptimizationStatus_Guid = changeStatus.Guid;
                                d.UserModifed = request.UserModify;
                                d.DatetimeModified = request.ClientDate;
                                d.UniversalDatetimeModified = request.UtcDate;
                            }
                            // update update job
                            var job = tranHead.TblTransactionRouteOptimizationHeader_Detail.Select(s => s.TblTransactionRouteOptimizationHeader_Detail_Item
                                        .Where(w => w.TblMasterActualJobServiceStopLegs != null && !w.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid.IsNullOrEmpty()))
                                        .SelectMany(m => m);
                            foreach (var j in job)
                            {
                                j.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid = changeStatus.Guid;
                            }

                        }
                        if (OptimizationStatusID.CANCELING == request.StatusTo)
                        {
                            tranHead.TblTransactionRouteOptimizationHeader_Queue.Add(SetRouteOptimizeQueue(request.UserModify, EnumHelper.GetDescription(RoadNetKeyFileName.OptCancelReq), request.ClientDate, request.UtcDate));
                        }
                        _uow.Commit();
                        tran.Complete();
                    }
                    response.SetMessageView(_systemMessageRepository.FindByMsgId(0, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true));
                }
                else
                {
                    response.SetMessageView(_systemMessageRepository.FindByMsgId(-17339, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
                    response.IsWarning = true;

                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-186, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false));


            }
            return response;
        }

        public GetRouteOptimizePathResponse GetRouteOptimizePath(Guid countryGuid)
        {
            var response = new GetRouteOptimizePathResponse() { IsSuccess = true };
            try
            {
                var config = EnumHelper.GetDescription(RoadNetFileConfig.RouteOptimizationDirectoryPath);
                var directioryPath = _systemEnvironment_GlobalRepository.Func_CountryOption_Get(config, null, countryGuid).AppValue1;
                if (directioryPath.IsEmpty())
                {
                    response.SetMessageView(_systemMessageRepository.FindByMsgId(-17332, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return response;
        }

        #region Send And Receive file Roadnet
        #region Send File
        public GenerateRoadnetResponse GenerateFile()
        {
            GenerateRoadnetResponse result = new GenerateRoadnetResponse();
            try
            {
                var keyFileName = EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvReq);
                if (_transactionRouteOptimizationHeaderQueueRepository.FindAll(f => !f.FlagComplete && f.FileFormat == keyFileName).Any())
                {
                    var data = _transactionRouteOptimizationHeaderRepository.GetOptimizeRequestGenerateFile();
                    if (data != null)
                    {
                        string subFolder = EnumHelper.GetDescription(RoadNetSubFolderSource.OCEAN_To_CountryRoadNet);
                        var country = data.RouteOptimizeRequestIdAndCountry.GroupBy(g => g.CountryGuid).Select(s => s.Key)?.ToList();
                        var config = EnumHelper.GetDescription(RoadNetFileConfig.RouteOptimizationDirectoryPath);
                        var strPath = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAndCountry(config, country);
                        List<Tuple<Guid, string, string>> queueList = new List<Tuple<Guid, string, string>>();
                        foreach (var d in data.RouteOptimizeRequestIdAndCountry)
                        {
                            var path = strPath.FirstOrDefault(f => f.MasterCountry_Guid == d.CountryGuid);
                            var root = CheckEndOfWordBackSpace(path.AppValue1);
                            string json = JsonConvert.SerializeObject(data.FileData.Where(w => w.Request_ID == d.RequestId).ToArray())
                                .Replace(@"""Route_Day""", @"""Route&Day""");
                            d.DirectoryPath = Path.Combine(root, subFolder);
                            d.FileName = $@"{ d.RequestId.Replace("-", "_")}_{keyFileName}_{ DateTime.Now.ToString("yyyyMMdd_HHmmssff")}.json";
                            if (!Directory.Exists(d.DirectoryPath))
                            {
                                Directory.CreateDirectory(d.DirectoryPath);
                            }
                            queueList.Add(Tuple.Create(d.RequestGuid, d.DirectoryPath, d.FileName));
                            string directory = Path.Combine(d.DirectoryPath, d.FileName);
                            Encoding utf8WithoutBom = new UTF8Encoding(false);
                            System.IO.File.WriteAllText(directory, json, utf8WithoutBom);
                        }
                        // update Queue

                        var queue = _transactionRouteOptimizationHeaderQueueRepository.GetTransactionRouteOptimizationByRequestID(data.RouteOptimizeRequestIdAndCountry.Select(s => s.RequestGuid));
                        foreach (var q in queue)
                        {
                            var d = queueList.FirstOrDefault(w => w.Item1 == q.TransactionRouteOptimizationHeader_Guid);
                            q.FlagComplete = true;
                            q.FileName = d.Item3;
                            q.DirectoryPath = d.Item2;
                            q.UserModified = "System";
                            q.DatetimeModified = DateTime.Now;
                            q.UniversalDatetimeModified = DateTime.UtcNow;
                        }
                        _uow.Commit();
                        //log here 
                        var reqJoin = string.Join(",", data.RouteOptimizeRequestIdAndCountry.Select(s => s.RequestId));
                        _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, $@"RoadNetService Created roadnet file from request {reqJoin} successfully.", "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);


                    }
                    result.TotalRequestGenerate = data?.RouteOptimizeRequestIdAndCountry.Count() ?? 0;
                }
                var msg = _systemMessageRepository.FindByMsgId(0, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                result.SetMessageView(msg);
            }
            catch (Exception ex)
            {
                var msg = _systemMessageRepository.FindByMsgId(-186, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                result.SetMessageView(msg);
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, $@"RoadNetService Create roadnet file error : {ex.Message}.", "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }


        public GenerateRoadnetResponse GenerateFileResponseToRoadnet()
        {
            GenerateRoadnetResponse result = new GenerateRoadnetResponse();
            var optAdvUpd = EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvUpd);
            var optUpdErr = EnumHelper.GetDescription(RoadNetKeyFileName.OptUpdErr);
            var optCancelReq = EnumHelper.GetDescription(RoadNetKeyFileName.OptCancelReq);
            var date = DateTime.Now;
            var utcdate = DateTime.UtcNow;
            string[] keyFileName = new string[] {
                optAdvUpd,
                optUpdErr,
                optCancelReq
            };
            var data = _transactionRouteOptimizationHeaderQueueRepository.FindAll(f => !f.FlagComplete && f.CountFail < 5 && keyFileName.Contains(f.FileFormat));
            if (data.Any())
            {
                string subFolder = EnumHelper.GetDescription(RoadNetSubFolderSource.OCEAN_To_CountryRoadNet);
                string requestDate = date.ToString("dd/MM/yyyy HH:mm:ss");
                string dateFileFormat = date.ToString("yyyyMMdd_HHmmssff");
                var listData = data.ToList();
                var listOfCountry = listData.GroupBy(g => g.TblTransactionRouteOptimizationHeader.TblMasterSite.MasterCountry_Guid).Select(s => s.Key);
                var config = EnumHelper.GetDescription(RoadNetFileConfig.RouteOptimizationDirectoryPath);
                var strPath = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAndCountry(config, listOfCountry);
                List<Guid> queueFail = new List<Guid>();
                foreach (var p in strPath)
                {
                    foreach (var d in data.Where(w => w.TblTransactionRouteOptimizationHeader.TblMasterSite.MasterCountry_Guid == p.MasterCountry_Guid))
                    {
                        try
                        {
                            var fileName = $@"{ d.TblTransactionRouteOptimizationHeader.RequestID.Replace("-", "_")}_{d.FileFormat}_{dateFileFormat}";
                            var root = CheckEndOfWordBackSpace(p.AppValue1);

                            var model = new OptAdvResponseModel()
                            {
                                Request_ID = d.TblTransactionRouteOptimizationHeader.RequestID,
                                Request_Type = d.TblTransactionRouteOptimizationHeader.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeCode,
                                Request_Date = requestDate,
                                Request_File = fileName,
                                Requester_Name = "Ocean_System"
                            };
                            if (d.FileFormat == optAdvUpd)
                            {
                                model.Request_Status = "RequestClosed";
                                model.Request_Comment = "";
                            }
                            else if (d.FileFormat == optUpdErr)
                            {
                                model.Request_Status = "Error";
                                model.Request_Comment = "Error detailed info";
                            }
                            else if (d.FileFormat == optCancelReq)
                            {
                                model.Request_Status = "CancellationRequested";
                                model.Request_Comment = "";

                            }


                            d.DirectoryPath = Path.Combine(root, subFolder);
                            d.FileName = $@"{fileName}.json";
                            d.FlagComplete = true;
                            d.UserModified = "System";
                            d.DatetimeModified = date;
                            d.UniversalDatetimeModified = utcdate;

                            string json = JsonConvert.SerializeObject(model);
                            if (!Directory.Exists(d.DirectoryPath))
                            {
                                Directory.CreateDirectory(d.DirectoryPath);
                            }
                            string directory = Path.Combine(d.DirectoryPath, d.FileName);
                            Encoding utf8WithoutBom = new UTF8Encoding(false);
                            System.IO.File.WriteAllText(directory, json, utf8WithoutBom);
                            _uow.Commit();
                        }
                        catch (Exception ex)
                        {
                            _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, $@"RoadNetService {nameof(GenerateFileResponseToRoadnet)} Create roadnet file error : {ex.Message}.", "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                            queueFail.Add(d.Guid);
                        }
                    }
                }

                if (queueFail.Any())
                {
                    var fail = _transactionRouteOptimizationHeaderQueueRepository.FindAll(f => queueFail.Contains(f.Guid));
                    foreach (var f in fail)
                    {
                        f.CountFail += 1;
                    }
                    _uow.Commit();
                }
            }
            return result;
        }
        #endregion Send File
        #region Receive file Roadnet
        #region step 1 Update to In-Progress
        //-------------------------------------
        // Step 1 Read data to update inprogress and canceled.
        public ReceiveRoadnetResponse ReceiveRoadnetFileUpdateInProgressAndCanceled()
        {
            ReceiveRoadnetResponse res = new ReceiveRoadnetResponse();
            try
            {
                var dateTime = DateTime.Now;
                var utcDate = DateTime.UtcNow;
                var config = EnumHelper.GetDescription(RoadNetFileConfig.RouteOptimizationDirectoryPath);
                var strPath = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAllCountry(config).Where(w => !string.IsNullOrEmpty(w.AppValue1));
                string subFolder = EnumHelper.GetDescription(RoadNetSubFolderSource.CountryRoadNet_To_OCEAN);
                string targetType = $@"*.JSON";
                foreach (var p in strPath)
                {
                    var root = CheckEndOfWordBackSpace(p.AppValue1);
                    string fullPath = Path.Combine(root, subFolder);
                    if (Directory.Exists(fullPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(fullPath);
                        var dataToUpdateInProgress = di.GetFiles(targetType).Where(w => w.Name.Contains(EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvUpd))).OrderBy(f => f.CreationTime)?.ToList();
                        if (dataToUpdateInProgress.Any())
                        {
                            var resultUpdateInProgress = ReceiveOptAdvUpd(dataToUpdateInProgress, root, dateTime, utcDate);
                            res.TotalSuccess = res.TotalSuccess + resultUpdateInProgress.TotalSuccess;
                            res.TotalFail = res.TotalFail + resultUpdateInProgress.TotalFail;
                        }

                        var canceled = di.GetFiles(targetType).Where(w => w.Name.Contains(EnumHelper.GetDescription(RoadNetKeyFileName.OptCancelRes))).OrderBy(f => f.CreationTime)?.ToList();
                        if (canceled.Any())
                        {
                            var resultUpdateInProgress = ReceiveOptCanceled(canceled, root, dateTime, utcDate);
                            res.TotalSuccess = res.TotalSuccess + resultUpdateInProgress.TotalSuccess;
                            res.TotalFail = res.TotalFail + resultUpdateInProgress.TotalFail;
                        }

                        //Error handle
                        var errorFile = di.GetFiles(targetType).Where(w => w.Name.Contains(EnumHelper.GetDescription(RoadNetKeyFileName.OptUpdErr))).OrderBy(f => f.CreationTime)?.ToList();
                        if (errorFile.Any())
                        {
                            var resultUpdateInProgress = ReceiveOptUpdErr(errorFile, root, dateTime, utcDate);
                            res.TotalSuccess = res.TotalSuccess + resultUpdateInProgress.TotalSuccess;
                            res.TotalFail = res.TotalFail + resultUpdateInProgress.TotalFail;
                        }
                    }
                }
                var msg = _systemMessageRepository.FindByMsgId(0, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                res.SetMessageView(msg);
            }
            catch (Exception ex)
            {
                var msg = _systemMessageRepository.FindByMsgId(-186, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                res.SetMessageView(msg);
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, $@"RoadNetService Create roadnet file error : {ex.Message}.", "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return res;
        }
        private ReceiveRoadnetResponse ReceiveOptUpdErr(IEnumerable<FileInfo> fileDir, string rootPath, DateTime clientDate, DateTimeOffset utcDate)
        {
            var response = new ReceiveRoadnetResponse();
            string user = "RoadNetSystem";
            var destPath = $@"{rootPath}{EnumHelper.GetDescription(RoadNetSubFolderArchives.CountryRoadNet_To_OCEAN)}";
            var statusGuid = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.FAILED);
            int[] statusTarget = new int[] { OptimizationStatusID.REQUESTING, OptimizationStatusID.INPROGRESS };
            foreach (var f in fileDir)
            {
                string reqId = "";

                using (var sr = new StreamReader(new FileStream(f.FullName, FileMode.Open), Encoding.UTF8, true))
                {
                    var json = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<OptAdvMsgModel>(json);
                        if (data.Request_Status.Equals("Error"))
                        {
                            try
                            {
                                reqId = data.Request_ID.Substring(0, 2);
                                var requestUpdate = new UpdateRouteOptimizeRequest
                                {
                                    RequestId = data.Request_ID,
                                    User = user,
                                    StatusUpdateGuid = statusGuid.Guid,
                                    ClientDate = clientDate,
                                    UtcDate = utcDate,
                                    ErrorDetail = data.Request_Comment,
                                    ConditionOptimizeStatusId = statusTarget
                                };
                                switch (reqId)
                                {
                                    case OptimizationRouteTypeID.RD:
                                        {
                                            UpdateOptimizeStatusDaily(requestUpdate);
                                            break;
                                        }
                                    case OptimizationRouteTypeID.RM:
                                        {
                                            UpdateOptimizeStatusTypeMasterRoute(requestUpdate);
                                            break;
                                        }
                                    default:
                                        break;
                                }

                                response.TotalSuccess = response.TotalSuccess + 1;
                            }
                            catch (Exception ex)
                            {
                                string content = $@"RoadNetService {nameof(ReceiveOptCanceled)} : {json} Update Fail.";
                                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                                _systemService.CreateHistoryError(ex);
                                response.TotalFail = response.TotalFail + 1;
                            }
                        }
                    }
                }

                MoveFileToArchives(f, destPath);
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler,
                    $@"RoadNetService { nameof(ReceiveOptCanceled)} request { reqId} was Updated.",
                    user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
            }
            return response;
        }

        private ReceiveRoadnetResponse ReceiveOptAdvUpd(IEnumerable<FileInfo> fileDir, string rootPath, DateTime clientDate, DateTimeOffset utcDate)
        {
            var response = new ReceiveRoadnetResponse();

            string user = "RoadNetSystem";
            var destPath = $@"{rootPath}{EnumHelper.GetDescription(RoadNetSubFolderArchives.CountryRoadNet_To_OCEAN)}";
            var statusGuid = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.INPROGRESS);
            int[] statusTarget = new int[] { OptimizationStatusID.REQUESTING };
            foreach (var f in fileDir)
            {
                string reqId = "";

                using (var sr = new StreamReader(new FileStream(f.FullName, FileMode.Open), Encoding.UTF8, true))
                {
                    var json = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<OptAdvMsgModel>(json);
                        try
                        {
                            reqId = data.Request_ID.Substring(0, 2);
                            var requestUpdate = new UpdateRouteOptimizeRequest
                            {
                                RequestId = data.Request_ID,
                                User = user,
                                StatusUpdateGuid = statusGuid.Guid,
                                ClientDate = clientDate,
                                UtcDate = utcDate,
                                ConditionOptimizeStatusId = statusTarget
                            };
                            switch (reqId)
                            {
                                case OptimizationRouteTypeID.RD:
                                    {
                                        UpdateOptimizeStatusDaily(requestUpdate);

                                        break;
                                    }
                                case OptimizationRouteTypeID.RM:
                                    {
                                        UpdateOptimizeStatusTypeMasterRoute(requestUpdate);
                                        break;
                                    }
                                default:
                                    break;
                            }

                            response.TotalSuccess = response.TotalSuccess + 1;
                        }
                        catch (Exception ex)
                        {
                            string content = $@"RoadNetService {nameof(ReceiveOptAdvUpd)} : {json} Update Fail.";
                            _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                            _systemService.CreateHistoryError(ex);
                            response.TotalFail = response.TotalFail + 1;
                        }
                    }
                }

                MoveFileToArchives(f, destPath);
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler,
                    $@"RoadNetService { nameof(ReceiveOptAdvUpd)} request { reqId} was Updated.",
                    user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
            }
            return response;
        }

        //private void UpdateRequestingToInProgressTypeDaily(string reqId, string user, Guid statusGuid, DateTime clientDate, DateTimeOffset utcDate)
        private void UpdateOptimizeStatusDaily(UpdateRouteOptimizeRequest request)
        {
            if (!string.IsNullOrEmpty(request.RequestId) && !string.IsNullOrWhiteSpace(request.RequestId))
            {
                using (var db = new OceanDbEntities())
                {
                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => request.RequestId == f.RequestID && request.ConditionOptimizeStatusId.Contains(f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        foreach (var h in tblTranHead)
                        {
                            h.SystemRouteOptimizationStatus_Guid = request.StatusUpdateGuid;
                            if (request.ConditionOptimizeStatusId.Contains(OptimizationStatusID.CANCELING))
                            {
                                h.CancelUser = h.UserModified;
                                h.CancelDatetime = request.ClientDate;
                            }
                            h.UserModified = request.User;
                            h.DatetimeModified = request.ClientDate;
                            h.UniversalDatetimeModified = request.UtcDate;
                            h.ErrorDetail = request.ErrorDetail;

                            var run = h.TblTransactionRouteOptimizationHeader_Detail.Where(w => w.MasterDailyRunResource_Guid != null)
                                    .Select(rd => rd.TblMasterDailyRunResource);
                            foreach (var rd in run)
                            {
                                rd.SystemRouteOptimizationStatus_Guid = request.StatusUpdateGuid;
                                rd.UserModifed = request.User;
                                rd.DatetimeModified = request.ClientDate;
                                rd.UniversalDatetimeModified = request.UtcDate;
                            }
                            var job = h.TblTransactionRouteOptimizationHeader_Detail
                                    .Select(s => s.TblTransactionRouteOptimizationHeader_Detail_Item
                                            .Where(w => w.TblMasterActualJobServiceStopLegs != null && !w.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid.IsNullOrEmpty())
                                                .Select(ss => ss.TblMasterActualJobServiceStopLegs))
                                    .SelectMany(sm => sm);
                            foreach (var j in job)
                            {
                                j.SystemRouteOptimizationStatus_Guid = request.StatusUpdateGuid;
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
        }
        private void UpdateOptimizeStatusTypeMasterRoute(UpdateRouteOptimizeRequest request)

        {
            if (!string.IsNullOrEmpty(request.RequestId) && !string.IsNullOrWhiteSpace(request.RequestId))
            {
                using (var db = new OceanDbEntities())
                {
                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => request.RequestId == f.RequestID && request.ConditionOptimizeStatusId.Contains(f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        foreach (var h in tblTranHead.ToList())
                        {
                            h.SystemRouteOptimizationStatus_Guid = request.StatusUpdateGuid;
                            if (request.ConditionOptimizeStatusId.Contains(OptimizationStatusID.CANCELING))
                            {
                                h.CancelUser = h.UserModified;
                                h.CancelDatetime = request.ClientDate;
                            }
                            h.UserModified = request.User;
                            h.DatetimeModified = request.ClientDate;
                            h.UniversalDatetimeModified = request.UtcDate;
                            h.ErrorDetail = request.ErrorDetail;

                            var routestatus = h.TblMasterRoute.TblMasterRoute_OptimizationStatus
                                .Select(s => s.TblMasterRoute.TblMasterRoute_OptimizationStatus)
                                .SelectMany(srr => srr);
                            foreach (var rm in routestatus)
                            {
                                rm.SystemRouteOptimizationStatus_Guid = request.StatusUpdateGuid;
                                rm.UserModified = request.User;
                                rm.DatetimeModified = request.ClientDate;
                                rm.UniversalDatetimeModified = request.UtcDate;
                            }
                        }
                        db.SaveChanges();
                    }

                }
            }
        }
        #endregion step 1 Update to In-Progress    

        #region step 2 Update to Completed
        public ReceiveRoadnetResponse ReceiveRoadnetFileUpdateCompleted()
        {
            ReceiveRoadnetResponse response = new ReceiveRoadnetResponse();
            try
            {
                var dateTime = DateTime.Now;
                var utcDate = DateTime.UtcNow;
                var config = EnumHelper.GetDescription(RoadNetFileConfig.RouteOptimizationDirectoryPath);
                var strPath = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAllCountry(config).Where(w => !string.IsNullOrEmpty(w.AppValue1));
                string subFolder = EnumHelper.GetDescription(RoadNetSubFolderSource.CountryRoadNet_To_OCEAN);
                string targetType = $@"*.JSON";
                if (strPath.Any())
                {
                    var strPartList = strPath.ToList();
                    foreach (var p in strPartList)
                    {
                        var root = CheckEndOfWordBackSpace(p.AppValue1);
                        string fullPath = $@"{root}{subFolder}";
                        if (Directory.Exists(fullPath))
                        {
                            DirectoryInfo di = new DirectoryInfo(fullPath);
                            var fileData = di.GetFiles(targetType).Where(w => w.Name.Contains(EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvRes))).OrderBy(f => f.CreationTime)?.ToList();
                            var res = ReceiveOptAdvRes(fileData, root, dateTime, utcDate);
                            response.TotalFail += res.TotalFail;
                            response.TotalSuccess += res.TotalSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string content = $@"RoadNetService {nameof(ReceiveRoadnetFileUpdateCompleted)} : read data error {ex.Message}.";
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                _systemService.CreateHistoryError(ex);
            }

            return response;

        }
        private ReceiveRoadnetResponse ReceiveOptAdvRes(IEnumerable<FileInfo> fileDir, string rootPath, DateTime clientDate, DateTimeOffset utcDate)
        {
            ReceiveRoadnetResponse response = new ReceiveRoadnetResponse();
            if (fileDir.Any())
            {
                string user = "RoadNetSystem";
                var statusGuid = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.COMPLETED);
                var jsonFile = ReadAndMoveResponseRoadNetFile(fileDir, rootPath, clientDate, utcDate, user);
                foreach (var j in jsonFile)
                {

                    if (j.FileName.Substring(0, 2) == OptimizationRouteTypeID.RD && j.Transaction.Any())
                    {
                        var rd = UpdateInProgressCompletedTypeDailRun(j.Transaction, user, statusGuid.Guid, clientDate, utcDate);
                        response.TotalFail += rd.TotalFail;
                        response.TotalSuccess += rd.TotalSuccess;
                    }
                    if (j.FileName.Substring(0, 2) == OptimizationRouteTypeID.RM && j.Transaction.Any())
                    {
                        var rm = UpdateInProgressCompletedTypeMasterRoute(j.Transaction, user, statusGuid.Guid, clientDate, utcDate);
                        response.TotalFail += rm.TotalFail;
                        response.TotalSuccess += rm.TotalSuccess;
                    }
                }
            }
            return response;
        }

        // read and move file 
        public List<RoadNetResModel> ReadAndMoveResponseRoadNetFile(IEnumerable<FileInfo> fileDir, string rootPath, DateTime clientDate, DateTimeOffset utcDate, string userName)
        {
            var destPath = $@"{rootPath}{EnumHelper.GetDescription(RoadNetSubFolderArchives.CountryRoadNet_To_OCEAN)}";
            List<RoadNetResModel> jsonFile = new List<RoadNetResModel>(fileDir.Count());
            foreach (var f in fileDir)
            {


                using (var sr = new StreamReader(new FileStream(f.FullName, FileMode.Open), Encoding.UTF8, true))
                {
                    var json = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(json))
                    {

                        var data = JsonConvert.DeserializeObject<List<RoadnetFileDataModel>>(json);
                        try
                        {
                            jsonFile.Add(new RoadNetResModel { FileName = f.Name, Transaction = data });
                        }
                        catch (Exception ex)
                        {
                            string content = $@"RoadNetService {nameof(ReceiveOptAdvRes)} : {json} Deserialize Fail.";
                            _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, userName, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                            _systemService.CreateHistoryError(ex);
                        }
                    }
                }
                MoveFileToArchives(f, destPath);
            }
            return jsonFile;
        }
        #region DailyRun
        private ReceiveRoadnetResponse UpdateInProgressCompletedTypeDailRun(List<RoadnetFileDataModel> fileReader, string user, Guid statusGuid, DateTime clientDate, DateTimeOffset utcDate)
        {
            ReceiveRoadnetResponse response = new ReceiveRoadnetResponse();
            var reqIds = fileReader.GroupBy(g => g.Request_ID).Select(s => s.Key).ToList();
            int[] optimizeStatusAllowUpdate = new int[] {
                            OptimizationStatusID.REQUESTING,
                            OptimizationStatusID.INPROGRESS
                        };
            try
            {
                using (var db = new OceanDbEntities())
                {
                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => reqIds.Any(a => a == f.RequestID) && optimizeStatusAllowUpdate.Any(a => a == f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        var tranH = tblTranHead.ToList();
                        List<RoadNetDailyRunLevelView> auditMasterRoute = new List<RoadNetDailyRunLevelView>();
                        List<TblMasterHistory_ActualJob> jobHistory = new List<TblMasterHistory_ActualJob>(fileReader.Count);
                        List<JobDetailModelView> changeStatus = new List<JobDetailModelView>(fileReader.Count);
                        foreach (var h in tranH)
                        {
                            // Update JobOrder

                            var reqIn = fileReader.Where(w => w.Request_ID == h.RequestID);
                            var tranDetail = h.TblTransactionRouteOptimizationHeader_Detail;
                            var rgdGuid = tranDetail.Where(w => w.MasterRouteGroupDetail_Guid != null).Select(s => s.MasterRouteGroupDetail_Guid);
                            var allRgd = db.TblMasterRouteGroup_Detail.Where(w => rgdGuid.Contains(w.Guid)).ToList();
                            var tranItem = h.TblTransactionRouteOptimizationHeader_Detail
                                        .Select(s => s.TblTransactionRouteOptimizationHeader_Detail_Item)
                                        .SelectMany(m => m);





                            if (tranItem.Any() && reqIn.Count() == tranItem.Count())
                            {
                                var jobHead = tranItem.Select(s => s.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid).Distinct();
                                var brinksLegs = db.TblMasterActualJobServiceStopLegs.Where(w => w.SequenceStop != 1 || !w.FlagDestination && jobHead.Contains(w.MasterActualJobHeader_Guid));
                                string[] requestType = new string[] { "R2", "R3" };
                                var legUpdate = tranItem
                                                .Join(reqIn, i => i.RequestItemID, r => r.Request_Item_ID, (i, r) =>

                                                {
                                                    List<TblMasterHistory_ActualJob> logJob = new List<TblMasterHistory_ActualJob>();
                                                    if (r.MasterRouteGroupDetailName.IndexOf("Unassigned") < 0)
                                                    {
                                                        var rgdOnRun = allRgd.FirstOrDefault(f => f.MasterRouteGroupDetailName == r.MasterRouteGroupDetailName);
                                                        int roadnetOrder = 0;
                                                        int.TryParse(r.Location_Sequence_RoadNet, out roadnetOrder);
                                                        string legAction = i.TblMasterActualJobServiceStopLegs.SequenceStop.GetValueOrDefault() % 2 == 0 ? "Delivery" : "Pickup";
                                                        i.SequenceAfter = roadnetOrder;
                                                        if (!string.IsNullOrEmpty(r.Location_Sequence_RoadNet))
                                                        {
                                                            var seqBf = !i.SequenceBefore.HasValue ? "-" : i.SequenceBefore.ToString();
                                                            logJob.Add(new TblMasterHistory_ActualJob
                                                            {
                                                                Guid = Guid.NewGuid(),
                                                                MasterActualJobHeader_Guid = i.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                                                MsgID = 6121,
                                                                DatetimeCreated = clientDate,
                                                                UniversalDatetimeCreated = utcDate,
                                                                UserCreated = h.RequestUser,
                                                                //[This job (Action {0}) was changed sequence from {1} to {2} by the route optimization in request ID {3}]
                                                                MsgParameter = JsonConvert.SerializeObject(new string[] { legAction, seqBf, roadnetOrder.ToString(), h.RequestID }),
                                                            });
                                                            i.TblMasterActualJobServiceStopLegs.JobOrder = roadnetOrder;
                                                        }

                                                        var sequenceStopBinrks = (i.TblMasterActualJobServiceStopLegs.SequenceStop % 2) == 0 ? i.TblMasterActualJobServiceStopLegs.SequenceStop - 1 : i.TblMasterActualJobServiceStopLegs.SequenceStop + 1;
                                                        // R2,R3 AND (DetailNotNull OR Detail != File.RgdName) AND (File.RgdName != null OR File.rgdName != ' ')
                                                        if (requestType.Contains(r.Request_Type) && (i.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid.IsNullOrEmpty()
                                                            || i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName != r.MasterRouteGroupDetailName)
                                                            && !(string.IsNullOrWhiteSpace(r.MasterRouteGroupDetailName) || string.IsNullOrEmpty(r.MasterRouteGroupDetailName)))

                                                        {
                                                            var dailyRunGuid = tranDetail.FirstOrDefault(f => f.MasterRouteGroupDetail_Guid == rgdOnRun.Guid);
                                                            i.TblMasterActualJobServiceStopLegs.MasterRunResourceDaily_Guid = dailyRunGuid.MasterDailyRunResource_Guid;
                                                            i.TblMasterActualJobServiceStopLegs.MasterRouteGroupDetail_Guid = dailyRunGuid.MasterRouteGroupDetail_Guid;
                                                            var brinksLeg = brinksLegs.FirstOrDefault(f => f.SequenceStop == sequenceStopBinrks && f.MasterActualJobHeader_Guid == i.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid);
                                                            UpdateBrinksLeg(brinksLeg, dailyRunGuid.MasterDailyRunResource_Guid, dailyRunGuid.MasterRouteGroupDetail_Guid);
                                                            string oldRun = i.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid.IsNullOrEmpty() ? "Unassigned" : i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName;
                                                            var history = new TblMasterHistory_ActualJob
                                                            {
                                                                Guid = Guid.NewGuid(),
                                                                MasterActualJobHeader_Guid = i.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                                                MsgID = 6122,
                                                                DatetimeCreated = clientDate,
                                                                UniversalDatetimeCreated = utcDate,
                                                                UserCreated = h.RequestUser,
                                                                MsgParameter = JsonConvert.SerializeObject(new string[] { legAction, oldRun, r.MasterRouteGroupDetailName, h.RequestID })
                                                            };
                                                            logJob.Add(history);
                                                            changeStatus.Add(new JobDetailModelView
                                                            {
                                                                JobHeaderGuid = history.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                                                ActionLeg = legAction,
                                                                SequenceStop = i.TblMasterActualJobServiceStopLegs.SequenceStop.GetValueOrDefault(),
                                                                ChangeDetail = ChangeDetail.ChangeRun
                                                            });
                                                        }
                                                        // move to unassign 
                                                        else if (requestType.Contains(r.Request_Type) && (string.IsNullOrWhiteSpace(r.MasterRouteGroupDetailName) || string.IsNullOrEmpty(r.MasterRouteGroupDetailName)))
                                                        {
                                                            var history = new TblMasterHistory_ActualJob
                                                            {
                                                                Guid = Guid.NewGuid(),
                                                                MasterActualJobHeader_Guid = i.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                                                MsgID = 6122,
                                                                DatetimeCreated = clientDate,
                                                                UniversalDatetimeCreated = utcDate,
                                                                UserCreated = h.RequestUser,
                                                                MsgParameter = JsonConvert.SerializeObject(new string[] { legAction, i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName, "Unassigned", h.RequestID })
                                                            };
                                                            logJob.Add(history);

                                                            changeStatus.Add(new JobDetailModelView
                                                            {
                                                                JobHeaderGuid = history.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                                                ActionLeg = legAction,
                                                                SequenceStop = i.TblMasterActualJobServiceStopLegs.SequenceStop.GetValueOrDefault(),
                                                                ChangeDetail = ChangeDetail.Unassign
                                                            });

                                                            i.TblMasterActualJobServiceStopLegs.MasterRouteGroupDetail_Guid = null;
                                                            i.TblMasterActualJobServiceStopLegs.MasterRunResourceDaily_Guid = null;
                                                            i.TblMasterActualJobServiceStopLegs.UnassignedBy = user;
                                                            i.TblMasterActualJobServiceStopLegs.UnassignedDatetime = clientDate;
                                                            var brinksLeg = brinksLegs.FirstOrDefault(f => f.SequenceStop == sequenceStopBinrks && f.MasterActualJobHeader_Guid == i.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid);
                                                            UpdateBrinksLeg(brinksLeg, null, null);
                                                        }
                                                    }
                                                    if (!i.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid.IsNullOrEmpty())
                                                    {
                                                        i.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid = statusGuid;
                                                    }
                                                    return logJob;
                                                })
                                                .SelectMany(s => s)
                                                ?.ToList();

                                jobHistory.AddRange(legUpdate);
                                h.TblTransactionRouteOptimizationHeader_Queue.Add(SetRouteOptimizeQueue(user, EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvUpd), clientDate, utcDate));

                            }
                            else
                            {
#pragma warning disable S112 // General exceptions should never be thrown
                                throw new NullReferenceException($@"Request item not equeal.");
#pragma warning restore S112 // General exceptions should never be thrown
                            }


                            //Update tranHeader
                            h.SystemRouteOptimizationStatus_Guid = statusGuid;
                            h.UserModified = user;
                            h.DatetimeModified = clientDate;
                            h.UniversalDatetimeModified = utcDate;
                            h.CompleteDatetime = clientDate;

                            //Update DailyRun status.
                            var run = h.TblTransactionRouteOptimizationHeader_Detail.Where(w => w.MasterDailyRunResource_Guid != null)
                                .Select(rd => rd.TblMasterDailyRunResource);
                            foreach (var rd in run)
                            {
                                auditMasterRoute.Add(new RoadNetDailyRunLevelView
                                {
                                    RequestId = h.RequestID,
                                    UserRequest = h.RequestUser,
                                    DailyRunGuid = rd.Guid
                                });
                                rd.SystemRouteOptimizationStatus_Guid = statusGuid;
                                rd.UserModifed = user;
                                rd.DatetimeModified = clientDate;
                                rd.UniversalDatetimeModified = utcDate;
                            }
                        }

                        //UpdateJobHeader
                        var jobHeadGuid = jobHistory.Select(s => s.MasterActualJobHeader_Guid).Distinct()?.ToList();
                        var tblJobhead = db.TblMasterActualJobHeader.Where(w => jobHeadGuid.Any(a => a == w.Guid))
                                        .ToList();
                        var jobType = db.TblSystemServiceJobType.Where(f => !(f.FlagDisable ?? false)).ToList();
                        foreach (var jh in tblJobhead)
                        {

                            var change = changeStatus.OrderBy(o => o.SequenceStop).FirstOrDefault(f => f.JobHeaderGuid == jh.Guid);
                            if (change != null)
                            {
                                var jobTypeID = jobType.First(f => f.Guid == jh.SystemServiceJobType_Guid).ServiceJobTypeID.GetValueOrDefault();
                                switch (change.ChangeDetail)
                                {
                                    case ChangeDetail.ChangeRun:
                                        jh.SystemStatusJobID = GetJobStatusChengedDailyRun(jobTypeID, jh.SystemStatusJobID.GetValueOrDefault(), change.SequenceStop);
                                        break;
                                    case ChangeDetail.Unassign:
                                        jh.SystemStatusJobID = GetJobStatusUnassigned(jobTypeID, jh.SystemStatusJobID.GetValueOrDefault());
                                        break;
                                    default:
                                        break;
                                }

                            }

                            if (jh.FlagSyncToMobile > 0)
                            {
                                jh.FlagJobChage = true;
                                jh.FlagJobReOrder = true;
                            }


                            jh.UserModifed = user;
                            jh.DatetimeModified = clientDate;
                            jh.UniversalDatetimeModified = utcDate;
                        }

                        // Job level.                       
                        db.TblMasterHistory_ActualJob.AddRange(jobHistory);
                        // MasterRoute Level.
                        List<TblMasterHistory_DailyRunResource> runHistory = new List<TblMasterHistory_DailyRunResource>();
                        foreach (var a in auditMasterRoute.GroupBy(g => new { g.DailyRunGuid, g.RequestId, g.UserRequest }))
                        {
                            runHistory.Add(new TblMasterHistory_DailyRunResource
                            {
                                Guid = Guid.NewGuid(),
                                MasterDailyRunResource_Guid = a.Key.DailyRunGuid,
                                MsgID = 6120,
                                DatetimeCreated = clientDate,
                                UniversalDatetimeCreated = utcDate,
                                UserCreated = a.Key.UserRequest,
                                //[This run was completed the route optimization in request ID {0}]
                                MsgParameter = JsonConvert.SerializeObject(new string[] { a.Key.RequestId }),
                            });
                        }
                        db.TblMasterHistory_DailyRunResource.AddRange(runHistory);
                        db.SaveChanges();
                        response.TotalSuccess = response.TotalSuccess + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                var requistId = string.Join(",", reqIds);
                UpdateInProgressCompletedTypeDailRunFail(reqIds, user, clientDate, utcDate, optimizeStatusAllowUpdate);
                string content = $@"RoadNetService {nameof(UpdateInProgressCompletedTypeDailRun)} : {requistId} Update Fail {ex.Message}.";
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                _systemService.CreateHistoryError(ex);
                response.TotalFail = response.TotalFail + 1;
            }
            return response;
        }

        private void UpdateBrinksLeg(TblMasterActualJobServiceStopLegs jobLeg, Guid? runGuid, Guid? rgdGuid)
        {
            if (jobLeg != null)
            {
                jobLeg.MasterRunResourceDaily_Guid = runGuid;
                jobLeg.MasterRouteGroupDetail_Guid = rgdGuid;
            }
        }
        private int GetJobStatusChengedDailyRun(int jobTypeId, int jobStatus, int sequenceStop)
        {
            #region Set StatusID
            var intStatusJob = sequenceStop;
            if (sequenceStop <= 2)
            {
                #region NOT IN T(D)

                if (jobStatus == IntStatusJob.Open)
                {
                    switch (jobTypeId)
                    {
                        case IntTypeJob.P:
                        case IntTypeJob.BCP:
                        case IntTypeJob.FLM:
                        case IntTypeJob.PM:
                        case IntTypeJob.KP:
                        case IntTypeJob.ECash:
                        case IntTypeJob.FSLM:
                        case IntTypeJob.TM:
                        case IntTypeJob.MCS:   //added: 2019/01/14
                        case IntTypeJob.T:
                        case IntTypeJob.P_MultiBr:      //added: 2019/03/27
                            intStatusJob = IntStatusJob.OnTruck;
                            break;
                        case IntTypeJob.TV:
                        case IntTypeJob.TV_MultiBr:     //added: 2019/03/27
                            intStatusJob = IntStatusJob.OnTruckPickUp;
                            break;
                        case IntTypeJob.D:
                        case IntTypeJob.AC:
                        case IntTypeJob.AE:
                        case IntTypeJob.BCD:
                        case IntTypeJob.BCD_MultiBr:    //added: 2019/03/27
                                                        //flagIntermediate = true; //updated: 2017/10/30
                            if (intStatusJob == IntStatusJob.ReadyToPreVault) break;
                            intStatusJob = IntStatusJob.Process;
                            break;
                        default:
                            intStatusJob = jobStatus;
                            break;
                    }

                    #endregion

                }
                #endregion
            }
            else  //(updateJobLeg.SequenceStop > 2)
            {
                #region TV (D)
                switch (jobStatus)
                {
                    case IntStatusJob.Open:
                        intStatusJob = IntStatusJob.OnTruckPickUp;
                        break;
                    case IntStatusJob.PartialInPrevaultDelivered:
                    case IntStatusJob.InPreVaultDelivery:
                        intStatusJob = IntStatusJob.InPreVaultDelivery;
                        break;
                    case IntStatusJob.OnTruckDelivery:
                        intStatusJob = IntStatusJob.OnTruckDelivery;
                        break;
                    default:
                        intStatusJob = jobStatus;
                        break;
                }
                #endregion                
            }
            return intStatusJob;
        }

        private int GetJobStatusUnassigned(int jobTypeId, int jobStatus)
        {
            List<int> statusJobChkTVJob = new List<int> { IntStatusJob.PickedUp, IntStatusJob.OnTruckPickUp, IntStatusJob.OnTheWayPickUp, IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery };

            int intStatusJob = jobStatus;
            switch (jobTypeId)
            {
                case IntTypeJob.P:
                case IntTypeJob.BCP:
                case IntTypeJob.FLM:
                case IntTypeJob.PM:
                case IntTypeJob.KP:
                case IntTypeJob.ECash:
                case IntTypeJob.FSLM:
                case IntTypeJob.TM:
                case IntTypeJob.MCS:
                case IntTypeJob.T:
                case IntTypeJob.P_MultiBr:
                    intStatusJob = IntStatusJob.Open;
                    break;
                case IntTypeJob.TV:
                case IntTypeJob.TV_MultiBr:
                    if (!statusJobChkTVJob.Contains(jobStatus))
                    {
                        intStatusJob = IntStatusJob.Open;
                    }
                    break;
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:
                case IntTypeJob.BCD:
                case IntTypeJob.BCD_MultiBr:
                    if (jobStatus == IntStatusJob.Process)
                    {
                        intStatusJob = IntStatusJob.Open;
                    }

                    break;
                default:
                    intStatusJob = jobStatus;
                    break;
            }

            return intStatusJob;
        }
        // update fail
        private void UpdateInProgressCompletedTypeDailRunFail(IEnumerable<string> reqId, string user, DateTime clientDate, DateTimeOffset utcDate, int[] status)
        {
            if (reqId.Any())
            {
                using (var db = new OceanDbEntities())
                {
                    var fail = db.TblSystemRouteOptimizationStatus.First(w => w.RouteOptimizationStatusID == OptimizationStatusID.FAILED);
                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => reqId.Any(a => a == f.RequestID) && status.Any(a => a == f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        foreach (var h in tblTranHead.ToList())
                        {
                            h.SystemRouteOptimizationStatus_Guid = fail.Guid;
                            h.UserModified = user;
                            h.DatetimeModified = clientDate;
                            h.UniversalDatetimeModified = utcDate;
                            var run = h.TblTransactionRouteOptimizationHeader_Detail.Where(w => w.MasterDailyRunResource_Guid != null)
                                     .Select(rd => rd.TblMasterDailyRunResource);
                            foreach (var rd in run)
                            {
                                rd.SystemRouteOptimizationStatus_Guid = fail.Guid;
                                rd.UserModifed = user;
                                rd.DatetimeModified = clientDate;
                                rd.UniversalDatetimeModified = utcDate;
                            }
                            h.TblTransactionRouteOptimizationHeader_Queue.Add(SetRouteOptimizeQueue(user, EnumHelper.GetDescription(RoadNetKeyFileName.OptUpdErr), clientDate, utcDate));

                            var leg = h.TblTransactionRouteOptimizationHeader_Detail.Select(s => s.TblTransactionRouteOptimizationHeader_Detail_Item.Where(w => w.TblMasterActualJobServiceStopLegs != null && !w.TblMasterActualJobServiceStopLegs.SystemRouteOptimizationStatus_Guid.IsNullOrEmpty()).Select(ss => ss.TblMasterActualJobServiceStopLegs)).SelectMany(m => m);
                            foreach (var l in leg)
                            {
                                l.SystemRouteOptimizationStatus_Guid = fail.Guid;
                            }
                        }
                        db.SaveChanges();
                    }

                }
            }
        }
        #endregion DailyRun
        #region MasterRoute
        private ReceiveRoadnetResponse UpdateInProgressCompletedTypeMasterRoute(List<RoadnetFileDataModel> fileReader, string user, Guid statusGuid, DateTime clientDate, DateTimeOffset utcDate)
        {
            ReceiveRoadnetResponse response = new ReceiveRoadnetResponse();
            var reqIds = fileReader.GroupBy(g => g.Request_ID).Select(s => s.Key).ToList();

            int[] optimizeStatusAllowUpdate = new int[] {
                            OptimizationStatusID.REQUESTING,
                            OptimizationStatusID.INPROGRESS
                        };
            try
            {
                using (var db = new OceanDbEntities())
                {

                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => reqIds.Any(a => a == f.RequestID) && optimizeStatusAllowUpdate.Any(a => a == f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        List<TblMasterRouteTransactionLog> log = new List<TblMasterRouteTransactionLog>();
                        var jobCategory = db.SFOTblSystemLogCategory.FirstOrDefault(f => f.CategoryCode == "MRJ_ROADNET" && f.SFOTblSystemLogProcess.ProcessCode == "MASTER_ROUTE_JOB");
                        var templateCategory = db.SFOTblSystemLogCategory.FirstOrDefault(f => f.CategoryCode == "MRT_ROADNET" && f.SFOTblSystemLogProcess.ProcessCode == "MASTER_ROUTE_TEMPLATE");

                        var tranH = tblTranHead.ToList();
                        List<RoadNetAuditLogJobMasterRouteLevelView> auditMasterRoute = new List<RoadNetAuditLogJobMasterRouteLevelView>();
                        foreach (var h in tranH)
                        {
                            // Update JobOrder

                            var reqIn = fileReader.Where(w => w.Request_ID == h.RequestID);
                            var tranDetail = h.TblTransactionRouteOptimizationHeader_Detail;
                            var tranItem = h.TblTransactionRouteOptimizationHeader_Detail
                            .Select(s => s.TblTransactionRouteOptimizationHeader_Detail_Item).SelectMany(m => m);

                            if (tranItem.Any() && reqIn.Count() == tranItem.Count())
                            {
                                // select and update 
                                var legUpdate = tranItem
                                                .Join(reqIn,
                                                i => i.RequestItemID,
                                                r => r.Request_Item_ID,
                                                (i, r) =>
                                                {
                                                    List<TblMasterRouteTransactionLog> logJob = new List<TblMasterRouteTransactionLog>();
                                                    if (!string.IsNullOrEmpty(r.Location_Sequence_RoadNet)
                                                            || i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName != r.MasterRouteGroupDetailName)
                                                    {
                                                        int roadnetOrder = 0;
                                                        int.TryParse(r.Location_Sequence_RoadNet, out roadnetOrder);

                                                        var legAction = i.TblMasterRouteJobServiceStopLegs.SequenceStop.GetValueOrDefault() % 2 == 0 ? "Delivery" : "Pickup";
                                                        if (!string.IsNullOrEmpty(r.Location_Sequence_RoadNet))
                                                        {
                                                            i.SequenceAfter = roadnetOrder;
                                                            i.TblMasterRouteJobServiceStopLegs.JobOrder = roadnetOrder;

                                                            logJob.Add(new TblMasterRouteTransactionLog()
                                                            {
                                                                Guid = Guid.NewGuid(),
                                                                SystemLogCategory_Guid = jobCategory.Guid,
                                                                SystemLogProcess_Guid = jobCategory.SystemLogProcess_Guid,
                                                                ReferenceValue_Guid = i.TblMasterRouteJobServiceStopLegs.MasterRouteJobHeader_Guid.GetValueOrDefault(),
                                                                SystemMsgID = (6119).ToString(),
                                                                JSONValue = JsonConvert.SerializeObject(new string[] { legAction, i.SequenceBefore.GetValueOrDefault().ToString(), roadnetOrder.ToString(), h.RequestID }),
                                                                Remark = "RoadNet Optimizetion Sequence Changed Job Level.",
                                                                UserCreated = h.RequestUser,
                                                                DatetimeCreated = clientDate,
                                                                UniversalDatetimeCreated = utcDate
                                                            });
                                                        }

                                                        if (r.Request_Type == "R3" &&
                                                                i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName != r.MasterRouteGroupDetailName)
                                                        {
                                                            var rgd = tranDetail.FirstOrDefault(f => f.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName == r.MasterRouteGroupDetailName);
                                                            if (rgd != null)
                                                            {
                                                                logJob.Add(new TblMasterRouteTransactionLog()
                                                                {
                                                                    Guid = Guid.NewGuid(),
                                                                    SystemLogCategory_Guid = jobCategory.Guid,
                                                                    SystemLogProcess_Guid = jobCategory.SystemLogProcess_Guid,
                                                                    ReferenceValue_Guid = i.TblMasterRouteJobServiceStopLegs.MasterRouteJobHeader_Guid.GetValueOrDefault(),
                                                                    SystemMsgID = (6122).ToString(),
                                                                    JSONValue = JsonConvert.SerializeObject(
                                                                                    new string[]
                                                                                    {   legAction,
                                                                                        i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName,
                                                                                        rgd.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName,
                                                                                        h.RequestID
                                                                                    }),
                                                                    Remark = "RoadNet Optimizetion Route Group Detail Changed Job Level.",
                                                                    UserCreated = h.RequestUser,
                                                                    DatetimeCreated = clientDate,
                                                                    UniversalDatetimeCreated = utcDate
                                                                });
                                                                i.TblMasterRouteJobServiceStopLegs.MasterRouteGroupDetail_Guid = rgd.MasterRouteGroupDetail_Guid;
                                                            }
                                                            else if (r.Request_Type == "R3" && (string.IsNullOrEmpty(r.MasterRouteGroupDetailName) || string.IsNullOrWhiteSpace(r.MasterRouteGroupDetailName)))
                                                            {
                                                                logJob.Add(new TblMasterRouteTransactionLog()
                                                                {
                                                                    Guid = Guid.NewGuid(),
                                                                    SystemLogCategory_Guid = jobCategory.Guid,
                                                                    SystemLogProcess_Guid = jobCategory.SystemLogProcess_Guid,
                                                                    ReferenceValue_Guid = i.TblMasterRouteJobServiceStopLegs.MasterRouteJobHeader_Guid.GetValueOrDefault(),
                                                                    SystemMsgID = (6122).ToString(),
                                                                    JSONValue = JsonConvert.SerializeObject(
                                                                                   new string[]
                                                                                   {   legAction,
                                                                                        i.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName,
                                                                                        "Empty",
                                                                                        h.RequestID
                                                                                   }),
                                                                    Remark = "RoadNet Optimizetion Route Group Detail Changed Job Level.",
                                                                    UserCreated = h.RequestUser,
                                                                    DatetimeCreated = clientDate,
                                                                    UniversalDatetimeCreated = utcDate
                                                                });
                                                                i.TblMasterRouteJobServiceStopLegs.MasterRouteGroupDetail_Guid = null;
                                                            }
                                                            else
                                                            {
#pragma warning disable S112 // General exceptions should never be thrown
                                                                throw new NullReferenceException($@"Not found RouteGroup {r.MasterRouteGroupDetailName}");
#pragma warning restore S112 // General exceptions should never be thrown
                                                            }
                                                        }
                                                    }
                                                    return logJob;
                                                })
                                                .SelectMany(s => s)
                                                ?.ToList();
                                log.AddRange(legUpdate);
                                h.TblTransactionRouteOptimizationHeader_Queue.Add(SetRouteOptimizeQueue("System", EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvUpd), clientDate, utcDate));
                            }
                            else
                            {
#pragma warning disable S112 // General exceptions should never be thrown
                                throw new NullReferenceException($@"Request item not equeal.");
#pragma warning restore S112 // General exceptions should never be thrown
                            }


                            //Update tranHeader
                            h.SystemRouteOptimizationStatus_Guid = statusGuid;
                            h.UserModified = user;
                            h.DatetimeModified = clientDate;
                            h.UniversalDatetimeModified = utcDate;
                            h.CompleteDatetime = clientDate;


                            //Update Optimize status.


                            var rgdInDetail = tranDetail.Select(s => new { s.MasterRouteGroupDetail_Guid });
                            var routestatus = h.TblMasterRoute.TblMasterRoute_OptimizationStatus.Where(w => rgdInDetail.Any(a => a.MasterRouteGroupDetail_Guid == w.MasterRouteGroupDetail_Guid))
                                .Select(s => s);
                            foreach (var rm in routestatus)
                            {
                                auditMasterRoute.Add(new RoadNetAuditLogJobMasterRouteLevelView
                                {
                                    RequestId = h.RequestID,
                                    UserRequest = h.RequestUser,
                                    MasterRouteGuid = rm.MasterRoute_Guid,
                                    RouteGroupName = rm.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName
                                });
                                rm.SystemRouteOptimizationStatus_Guid = statusGuid;
                                rm.UserModified = user;
                                rm.DatetimeModified = clientDate;
                                rm.UniversalDatetimeModified = utcDate;
                            }
                        }

                        //UpdateJobHeader
                        var jobHeadGuid = log.GroupBy(g => g.ReferenceValue_Guid).Select(s => s.Key);
                        var tblJobhead = db.TblMasterRouteJobHeader.Where(w => jobHeadGuid.Contains(w.Guid));

                        foreach (var jh in tblJobhead)
                        {
                            jh.UserModifed = user;
                            jh.DatetimeModified = clientDate;
                            jh.UniversalDatetimeModified = utcDate;
                        }


                        // MasterRoute Level.
                        foreach (var a in auditMasterRoute.GroupBy(g => new { g.MasterRouteGuid, g.RequestId, g.UserRequest }))
                        {
                            var routeGroupDetail = string.Join(",", auditMasterRoute.Where(w => w.MasterRouteGuid == a.Key.MasterRouteGuid).GroupBy(g => g.RouteGroupName).OrderBy(o => o.Key).Select(s => s.Key));
                            log.Add(new TblMasterRouteTransactionLog()
                            {
                                Guid = Guid.NewGuid(),
                                SystemLogCategory_Guid = templateCategory.Guid,
                                SystemLogProcess_Guid = templateCategory.SystemLogProcess_Guid,
                                ReferenceValue_Guid = a.Key.MasterRouteGuid,
                                SystemMsgID = (6118).ToString(),
                                JSONValue = JsonConvert.SerializeObject(new string[] { routeGroupDetail, a.Key.RequestId.ToString() }),
                                Remark = "RoadNet Optimizetion MasterRoute Level.",
                                UserCreated = a.Key.UserRequest,
                                DatetimeCreated = clientDate,
                                UniversalDatetimeCreated = utcDate
                            });
                        }

                        db.TblMasterRouteTransactionLog.AddRange(log);
                        db.SaveChanges();

                        response.TotalSuccess = response.TotalSuccess + 1;
                    }

                }
            }
            catch (Exception ex)
            {
                var requistId = string.Join(",", reqIds);
                UpdateInProgressCompletedTypeMasterRouteFail(reqIds, user, clientDate, utcDate, optimizeStatusAllowUpdate);
                string content = $@"RoadNetService {nameof(UpdateInProgressCompletedTypeMasterRoute)} : {requistId} Update Fail {ex.Message}.";
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                _systemService.CreateHistoryError(ex);
                response.TotalFail = response.TotalFail + 1;
            }
            return response;
        }
        // Master Route Update Fail

        private void UpdateInProgressCompletedTypeMasterRouteFail(IEnumerable<string> reqId, string user, DateTime clientDate, DateTimeOffset utcDate, int[] status)
        {
            if (reqId.Any())
            {
                using (var db = new OceanDbEntities())
                {
                    var fail = db.TblSystemRouteOptimizationStatus.First(w => w.RouteOptimizationStatusID == OptimizationStatusID.FAILED);
                    var tblTranHead = db.TblTransactionRouteOptimizationHeader.Where(f => reqId.Any(a => a == f.RequestID) && status.Any(a => a == f.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID));
                    if (tblTranHead.Any())
                    {
                        foreach (var h in tblTranHead.ToList())
                        {
                            h.SystemRouteOptimizationStatus_Guid = fail.Guid;
                            h.UserModified = user;
                            h.DatetimeModified = clientDate;
                            h.UniversalDatetimeModified = utcDate;
                            var routestatus = h.TblMasterRoute.TblMasterRoute_OptimizationStatus
                                .Select(s => s.TblMasterRoute.TblMasterRoute_OptimizationStatus)
                                .SelectMany(srr => srr);
                            foreach (var rm in routestatus)
                            {
                                rm.SystemRouteOptimizationStatus_Guid = fail.Guid;
                                rm.UserModified = user;
                                rm.DatetimeModified = clientDate;
                                rm.UniversalDatetimeModified = utcDate;
                            }
                            h.TblTransactionRouteOptimizationHeader_Queue.Add(SetRouteOptimizeQueue("System", EnumHelper.GetDescription(RoadNetKeyFileName.OptUpdErr), clientDate, utcDate));
                        }
                        db.SaveChanges();
                    }
                }
            }
        }
        #endregion MasterRoute
        #endregion step 2 Update to Completed
        private void MoveFileToArchives(FileInfo file, string destPath)
        {
            try
            {
                if (!Directory.Exists(destPath))
                {

                    Directory.CreateDirectory(destPath);

                }
                destPath = Path.Combine(destPath, file.Name);
                file.CopyTo(destPath, true);
                file.Delete();
            }
            catch (UnauthorizedAccessException)
            {
                FileAttributes attr = file.Attributes;
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler
                                , $@"RoadNetService Move file to {destPath},Directory is unable to access file."
                                , "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                if ((attr & FileAttributes.ReadOnly) > 0)
                {
                    _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler
                                , $@"RoadNetService Move file to {destPath},The file is read-only."
                                , "System", SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                }
            }
        }

        //step 3 cancel
        private ReceiveRoadnetResponse ReceiveOptCanceled(IEnumerable<FileInfo> fileDir, string rootPath, DateTime clientDate, DateTimeOffset utcDate)
        {
            var response = new ReceiveRoadnetResponse();

            string user = "RoadNetSystem";
            var destPath = Path.Combine(rootPath, EnumHelper.GetDescription(RoadNetSubFolderArchives.CountryRoadNet_To_OCEAN));
            var canceledGuid = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.CANCELED);
            var in_progressGuid = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.INPROGRESS);
            int[] statusTarget = new int[] { OptimizationStatusID.CANCELING };
            foreach (var f in fileDir)
            {
                string reqId = "";

                using (var sr = new StreamReader(new FileStream(f.FullName, FileMode.Open), Encoding.UTF8, true))
                {
                    var json = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<OptAdvMsgModel>(json);
                        try
                        {
                            reqId = data.Request_ID.Substring(0, 2);
                            var requestUpdate = new UpdateRouteOptimizeRequest
                            {
                                RequestId = data.Request_ID,
                                User = user,
                                StatusUpdateGuid = data.Request_Status.Equals("CancellationConfirmed") ? canceledGuid.Guid : in_progressGuid.Guid,
                                ClientDate = clientDate,
                                UtcDate = utcDate,
                                ConditionOptimizeStatusId = statusTarget
                            };
                            switch (reqId)
                            {
                                case OptimizationRouteTypeID.RD:
                                    {
                                        UpdateOptimizeStatusDaily(requestUpdate);
                                        break;
                                    }
                                case OptimizationRouteTypeID.RM:
                                    {
                                        UpdateOptimizeStatusTypeMasterRoute(requestUpdate);
                                        break;
                                    }
                                default:
                                    break;
                            }

                            response.TotalSuccess = response.TotalSuccess + 1;
                        }
                        catch (Exception ex)
                        {
                            string content = $@"RoadNetService {nameof(ReceiveOptCanceled)} : {json} Update Fail.";
                            _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler, content, user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
                            _systemService.CreateHistoryError(ex);
                            response.TotalFail = response.TotalFail + 1;
                        }
                    }
                }

                MoveFileToArchives(f, destPath);
                _systemService.CreateLogActivity(SystemActivityLog.FluentScheduler,
                    $@"RoadNetService { nameof(ReceiveOptCanceled)} request { reqId} was Updated.",
                    user, SystemHelper.CurrentIpAddress, ApplicationKey.OceanOnline);
            }
            return response;
        }

        #endregion Receive file Roadnet
        private string CheckEndOfWordBackSpace(string str)
        {
            return str.EndsWith("\\") ? str : $@"{str}\\";

        }
        #endregion  Send And Receive file Roadnet
        #region DailyRoute
        public DailyRouteResponse GetDailyRouteList(DailyRouteRequest request)
        {
            DailyRouteResponse response = new DailyRouteResponse();
            var userData = _baseRequest.Data;
            var flagValidateRunLiabilityLimit = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(request.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit);
            IEnumerable<DailyRouteView> dataDaily;
            if (request.RequestGuid.HasValue)
                dataDaily = _transactionRouteOptimizationHeaderRepository.GetDailyRunByOptimizationGuid(request, userData.UserLanguageGuid.Value, flagValidateRunLiabilityLimit);
            else
                dataDaily = _masterDailyRunResourceRepository.GetDailyRoute_For_Optimization(request, userData.UserLanguageGuid.Value, flagValidateRunLiabilityLimit);

            response.DailyRouteList = dataDaily;
            response.Total = dataDaily.Count();

            return response;
        }

        public JobUnassignedResponse GetUnassignedList(DailyRouteRequest request)
        {
            JobUnassignedResponse response = new JobUnassignedResponse();
            var userData = _baseRequest.Data;
            IEnumerable<JobUnassignedView> UnassignJob;
            var flagValidateRunLiabilityLimit = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(request.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit);
            if (request.RequestGuid.HasValue)
                UnassignJob = _transactionRouteOptimizationHeaderRepository.GetUnassignByOptimizationGuid(request, userData.UserLanguageGuid.Value, flagValidateRunLiabilityLimit);
            else
                UnassignJob = _masterDailyRunResourceRepository.GetUnassigned_For_Optimization(request, userData.UserLanguageGuid.Value, flagValidateRunLiabilityLimit);

            response.JobUnassignedList = UnassignJob;
            return response;
        }

        public MasterRouteRequestResponse InsertDailyRouteRequest(DailyRouteDetailRequest request)
        {
            MasterRouteRequestResponse response = new MasterRouteRequestResponse();
            response.IsSuccess = false;
            response.IsWarning = false;
            var userData = _baseRequest.Data;
            int[] allowCreate = new int[] {
                    OptimizationStatusID.NONE,
                    OptimizationStatusID.COMPLETED,
                    OptimizationStatusID.BROKEN,
                    OptimizationStatusID.CANCELED,
                    OptimizationStatusID.FAILED
                };
            List<Guid> ListallowGuid = _systemRouteOptimizationStatusRepository.FindAllAsQueryable(e => allowCreate.Contains(e.RouteOptimizationStatusID)).Select(e => e.Guid).ToList();
            var TranferVaultGuid = _systemServiceJobTypeRepository.FindByTypeId(IntTypeJob.TV).Guid;
            var InProgressGuid = _systemRouteOptimizationStatusRepository.FindAllAsQueryable(e => e.RouteOptimizationStatusID == OptimizationStatusID.REQUESTING).FirstOrDefault().Guid;
            var routeType = _systemRouteOptimizationRouteTypeRepository.GetRouteOptimizationRouteTypeByCode(OptimizationRouteTypeID.RD);
            var siteCode = _masterSiteRepository.FindById(request.SiteGuid);
            int RunningNumber = 1;
            bool FlagOneCar = request.DailyRouteGuidList.Count() == 1;

            //check Group detail ที่ทำไปแล้ว
            var dataDaily = _masterDailyRunResourceRepository.FindAllAsQueryable(e => request.DailyRouteGuidList.Contains(e.Guid)).ToList();
            var rgd = dataDaily.Where(w => !w.MasterRouteGroup_Detail_Guid.IsNullOrEmpty()).Select(s => s.MasterRouteGroup_Detail_Guid.Value);
            var route = _masterRouteOptimizationStatusRepository.ValidateStatusBeforeRequestSave(rgd, request.SiteGuid);
            if (route.Any())
            {
                var Message = _systemMessageRepository.GetMessage(-17334, userData.UserLanguageGuid.GetValueOrDefault());
                Message.MessageTextContent = string.Format(Message.MessageTextContent, string.Join(",", route));
                response.SetMessageView(Message.ConvertToMessageView(false));
                response.IsWarning = true;
                return response;
            }
            var runNo = SetRequestID(routeType.RouteOptimizationRouteTypeCode, siteCode.SiteCode, siteCode.Guid, userData.LocalClientDateTime);

            var runChange = dataDaily.Where(e => !(e.SystemRouteOptimizationStatus_Guid == null || ListallowGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value)) || e.RunResourceDailyStatusID != DailyRunStatus.Ready).Select(e => new { e.Guid, e.MasterRouteGroup_Detail_Guid });
            if (runChange.Any())
            {
                var Message = _systemMessageRepository.GetMessage(-17335, userData.UserLanguageGuid.GetValueOrDefault());
                var routeGroupListGuid = runChange.Select(e => e.MasterRouteGroup_Detail_Guid);
                Message.MessageTextContent = string.Format(Message.MessageTextContent, string.Join(",", _masterRouteGroupDetailRepository.FindAllAsQueryable(e => routeGroupListGuid.Contains(e.Guid)).Select(f => f.MasterRouteGroupDetailName)));
                response.SetMessageView(Message.ConvertToMessageView(false));
                response.InValid_DailyRoute_Guid = runChange.Select(e => e.Guid);
                response.IsWarning = true;
                return response;
            }

            TblTransactionRouteOptimizationHeader insertHeader = new TblTransactionRouteOptimizationHeader()
            {
                Guid = Guid.NewGuid(),
                MasterSite_Guid = request.SiteGuid,
                SystemRouteOptimizationRouteType_Guid = routeType.Guid,
                SystemRouteOptimizationRequestType_Guid = request.RequestTypeGuid,
                SystemRouteOptimizationStatus_Guid = InProgressGuid,
                RequestID = runNo,
                RequestUser = userData.UserName,
                RequestDatetime = userData.LocalClientDateTime,
                WorkDate = request.WorkDate,
                FlagDisable = false,
                UserCreated = userData.UserName,
                DatetimeCreated = userData.LocalClientDateTime,
                UniversalDatetimeCreated = DateTime.UtcNow,
                TblTransactionRouteOptimizationHeader_Queue = new List<TblTransactionRouteOptimizationHeader_Queue>()
                                                                { SetRouteOptimizeQueue(userData.UserName,EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvReq), userData.LocalClientDateTime, DateTime.UtcNow) }
            };

            foreach (var itemRun in dataDaily)
            {
                TblTransactionRouteOptimizationHeader_Detail itemDetail = new TblTransactionRouteOptimizationHeader_Detail()
                {
                    Guid = Guid.NewGuid(),
                    TransactionRouteOptimizationHeader_Guid = insertHeader.Guid,
                    MasterRouteGroupDetail_Guid = itemRun.MasterRouteGroup_Detail_Guid.Value,
                    MasterDailyRunResource_Guid = itemRun.Guid,
                    RouteStartTime = itemRun.StartTime,
                    FlagDisable = false,
                    UserCreated = userData.UserName,
                    DatetimeCreated = userData.LocalClientDateTime,
                    UniversalDatetimeCreated = DateTime.UtcNow
                };

                var jobleg = _masterActualJobServiceStopLegsRepository.GetLegsForOptimizationByDailyRun(itemRun.Guid);
                var jobheaderGuidList = jobleg.Select(e => e.MasterActualJobHeader_Guid).ToList();
                var headerList = _masterActualJobHeaderRepository.FindAllAsQueryable(e => jobheaderGuidList.Contains(e.Guid)).Select(e => new { e.Guid, e.SystemServiceJobType_Guid }).ToList();
                foreach (var itemLeg in jobleg)
                {
                    TblTransactionRouteOptimizationHeader_Detail_Item itemDetailitem = new TblTransactionRouteOptimizationHeader_Detail_Item()
                    {
                        Guid = Guid.NewGuid(),
                        TransactionRouteOptimizationHeader_Detail_Guid = itemDetail.Guid,
                        RequestItemID = insertHeader.RequestID + "-" + RunningNumber++.ToString("00000"),
                        MasterRouteJobServiceStopLegs_Guid = null,
                        MasterActualJobServiceStopLegs_Guid = itemLeg.Guid,
                        MasterCustomerLocation_Guid = itemLeg.MasterCustomerLocation_Guid,
                        SystemServiceJobType_Guid = headerList.FirstOrDefault(e => e.Guid == itemLeg.MasterActualJobHeader_Guid).SystemServiceJobType_Guid,
                        SequenceBefore = itemLeg.JobOrder,
                        SequenceAfter = null,
                        FlagDisable = false,
                        UserCreated = userData.UserName,
                        DatetimeCreated = userData.LocalClientDateTime,
                        UniversalDatetimeCreated = DateTime.UtcNow

                    };
                    itemDetail.TblTransactionRouteOptimizationHeader_Detail_Item.Add(itemDetailitem);
                    itemLeg.SystemRouteOptimizationStatus_Guid = InProgressGuid;
                }
                insertHeader.TblTransactionRouteOptimizationHeader_Detail.Add(itemDetail);
                //brake route balance
                if (itemRun.FlagRouteBalanceDone)
                {
                    itemRun.FlagRouteBalanceDone = false;
                    var jobseal = _masterActualJobItemsSealRepository.FindAllAsQueryable(e => jobheaderGuidList.Contains(e.MasterActualJobHeader_Guid) && e.FlagRouteBalance);
                    foreach (var itemseal in jobseal)
                        itemseal.FlagRouteBalance = false;
                }
                itemRun.SystemRouteOptimizationStatus_Guid = InProgressGuid;
            }

            if (request.UnassignedJobGuidList != null && request.UnassignedJobGuidList.Any())
            {
                TblTransactionRouteOptimizationHeader_Detail itemDetail = new TblTransactionRouteOptimizationHeader_Detail()
                {
                    Guid = Guid.NewGuid(),
                    TransactionRouteOptimizationHeader_Guid = insertHeader.Guid,
                    MasterRouteGroupDetail_Guid = null,
                    MasterDailyRunResource_Guid = null,
                    RouteStartTime = null,
                    FlagDisable = false,
                    UserCreated = userData.UserName,
                    DatetimeCreated = userData.LocalClientDateTime,
                    UniversalDatetimeCreated = DateTime.UtcNow
                };

                var jobleg = _masterActualJobServiceStopLegsRepository.GetALLLegsByLegGuid(request.UnassignedJobGuidList.ToList()).ToList();
                if (FlagOneCar)
                {
                    var GuidList = jobleg.GroupBy(e => e.MasterActualJobHeader_Guid).Where(e => e.Count() > 1).Select(e => e.Key).ToList();
                    if (GuidList.Any() &&
                        _masterActualJobHeaderRepository.FindAllAsQueryable(e => GuidList.Contains(e.Guid) && e.SystemServiceJobType_Guid == TranferVaultGuid).Any()
                        )
                    {
                        //ERROR
                        var Message = _systemMessageRepository.GetMessage(-17338, userData.UserLanguageGuid.GetValueOrDefault());
                        response.SetMessageView(Message.ConvertToMessageView(false));
                        response.IsWarning = true;
                        return response;
                    }
                }

                var jobCheck = jobleg.Where(e => (e.SystemRouteOptimizationStatus_Guid != null)).Select(e => new { e.Guid, e.MasterActualJobHeader_Guid });
                if (jobCheck.Any())
                {
                    var Message = _systemMessageRepository.GetMessage(-17336, userData.UserLanguageGuid.GetValueOrDefault());
                    var GuidList = jobCheck.Select(e => e.MasterActualJobHeader_Guid);
                    Message.MessageTextContent = string.Format(Message.MessageTextContent, string.Join(",", _masterActualJobHeaderRepository.FindAllAsQueryable(e => GuidList.Contains(e.Guid)).Select(e => e.JobNo)));
                    response.SetMessageView(Message.ConvertToMessageView(false));
                    response.IsWarning = true;
                    response.InValid_UnassignLeg_Guid = jobCheck.Select(e => e.Guid);
                    return response;
                }

                jobCheck = jobleg.Where(e => (e.MasterRunResourceDaily_Guid.HasValue)).Select(e => new { e.Guid, e.MasterActualJobHeader_Guid });
                if (jobCheck.Any())
                {
                    var Message = _systemMessageRepository.GetMessage(-17336, userData.UserLanguageGuid.GetValueOrDefault());
                    var GuidList = jobCheck.Select(e => e.MasterActualJobHeader_Guid);
                    Message.MessageTextContent = string.Format(Message.MessageTextContent, string.Join(",", _masterActualJobHeaderRepository.FindAllAsQueryable(e => GuidList.Contains(e.Guid)).Select(e => e.JobNo)));
                    response.SetMessageView(Message.ConvertToMessageView(false));
                    response.IsWarning = true;
                    response.InValid_UnassignLeg_Guid = jobCheck.Select(e => e.Guid);
                    return response;
                }

                var jobheaderGuidList = jobleg.Select(e => e.MasterActualJobHeader_Guid).ToList();
                var headerList = _masterActualJobHeaderRepository.FindAllAsQueryable(e => jobheaderGuidList.Contains(e.Guid)).Select(e => new { e.Guid, e.SystemServiceJobType_Guid }).ToList();
                foreach (var itemleg in jobleg)
                {

                    TblTransactionRouteOptimizationHeader_Detail_Item itemDetailitem = new TblTransactionRouteOptimizationHeader_Detail_Item()
                    {
                        Guid = Guid.NewGuid(),
                        TransactionRouteOptimizationHeader_Detail_Guid = itemDetail.Guid,
                        RequestItemID = insertHeader.RequestID + "-" + RunningNumber++.ToString("00000"),
                        MasterRouteJobServiceStopLegs_Guid = null,
                        MasterActualJobServiceStopLegs_Guid = itemleg.Guid,
                        MasterCustomerLocation_Guid = itemleg.MasterCustomerLocation_Guid,
                        SystemServiceJobType_Guid = headerList.FirstOrDefault(e => e.Guid == itemleg.MasterActualJobHeader_Guid).SystemServiceJobType_Guid,
                        SequenceBefore = null,
                        SequenceAfter = null,
                        FlagDisable = false,
                        UserCreated = userData.UserName,
                        DatetimeCreated = userData.LocalClientDateTime,
                        UniversalDatetimeCreated = DateTime.UtcNow

                    };
                    itemDetail.TblTransactionRouteOptimizationHeader_Detail_Item.Add(itemDetailitem);
                    itemleg.SystemRouteOptimizationStatus_Guid = InProgressGuid;
                }
                insertHeader.TblTransactionRouteOptimizationHeader_Detail.Add(itemDetail);
            }

            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    _transactionRouteOptimizationHeaderRepository.Create(insertHeader);
                    _uow.Commit();
                    transection.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.SetMessageView(_systemMessageRepository.FindByMsgId(-186, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false));
                    return response;
                }
            }

            response.IsSuccess = true;
            response.SetMessageView(_systemMessageRepository.FindByMsgId(0, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false));
            return response;
        }
        #endregion

        /// <summary>
        /// add new route optimize type Master-Route(RM).
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public MasterRouteRequestResponse InsertMasterRouteRequest(MasterRouteInsertRequest req)
        {
            MasterRouteRequestResponse response = new MasterRouteRequestResponse();
            try
            {
                var validate = ValidateDataBeforeSaveRequestMasterRouteOptimize(req);
                if (validate != null)
                {
                    response.SetMessageView(validate);
                    return response;
                }
                var dateTime = req.LocalClientDateTime;
                var utcDate = DateTime.UtcNow;
                var routeType = _systemRouteOptimizationRouteTypeRepository.GetRouteOptimizationRouteTypeByCode(OptimizationRouteTypeID.RM);
                var siteCode = _masterSiteRepository.FindById(req.SiteGuid);// wait font-end 
                                                                            //1) insert transaction header
                Guid headerGuid = Guid.NewGuid();
                var requestRunningId = SetRequestID(routeType.RouteOptimizationRouteTypeCode, siteCode.SiteCode, siteCode.Guid, dateTime);
                var requestStatus = _systemRouteOptimizationStatusRepository.GetRouteOptimizationStatusByID(OptimizationStatusID.REQUESTING);
                TblTransactionRouteOptimizationHeader headData = new TblTransactionRouteOptimizationHeader()
                {
                    Guid = headerGuid,
                    MasterSite_Guid = req.SiteGuid,
                    SystemRouteOptimizationRequestType_Guid = req.RequestTypeGuid,
                    SystemRouteOptimizationRouteType_Guid = routeType.Guid,
                    MasterRoute_Guid = req.MasterRouteGuid,
                    SystemRouteOptimizationStatus_Guid = requestStatus.Guid,
                    RequestID = requestRunningId,
                    RequestUser = req.UserName,
                    RequestDatetime = dateTime,
                    UserCreated = req.UserName,
                    DatetimeCreated = dateTime,
                    UniversalDatetimeCreated = utcDate,
                    TblTransactionRouteOptimizationHeader_Detail = SetTransactionRouteOptimizationDetail(req, requestRunningId, dateTime, utcDate),
                    TblTransactionRouteOptimizationHeader_Queue = new List<TblTransactionRouteOptimizationHeader_Queue>() { SetRouteOptimizeQueue(req.UserName, EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvReq), dateTime, utcDate) }
                };
                _masterRouteOptimizationStatusRepository.CreateRange(MergeRouteOptimizeStatus(req, requestStatus.Guid, dateTime, utcDate));
                _transactionRouteOptimizationHeaderRepository.Create(headData);
                _uow.Commit();
                response.SetMessageView(_systemMessageRepository.FindByMsgId(0, req.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true));
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-186, req.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false));
            }
            return response;
        }



        private TblTransactionRouteOptimizationHeader_Queue SetRouteOptimizeQueue(string userName, string fileFormat, DateTime clientDate, DateTimeOffset utcDate)
        {
            return new TblTransactionRouteOptimizationHeader_Queue
            {
                Guid = Guid.NewGuid(),
                UserCreated = userName,
                DatetimeCreated = clientDate,
                UniversalDatetimeCreated = utcDate,
                FileFormat = fileFormat

            };
        }
        private List<TblMasterRoute_OptimizationStatus> MergeRouteOptimizeStatus(MasterRouteInsertRequest req, Guid systemStatusGuid, DateTime clientDate, DateTimeOffset utcDate)
        {
            // update
            List<Guid> hasData = new List<Guid>();
            var tblRgd = _masterRouteOptimizationStatusRepository.FindAll(f => f.MasterRoute_Guid == req.MasterRouteGuid
                                                                            && req.MasterRouteSelectedList.Any(a => a.MasterRouteGroupDetailGuid == f.MasterRouteGroupDetail_Guid));
            if (tblRgd.Any())
            {
                foreach (var rgd in tblRgd)
                {
                    rgd.SystemRouteOptimizationStatus_Guid = systemStatusGuid;
                    rgd.UserModified = req.UserName;
                    rgd.DatetimeModified = clientDate;
                    rgd.UniversalDatetimeModified = utcDate;
                    hasData.Add(rgd.MasterRouteGroupDetail_Guid);
                }
            }
            //Insert
            var hasNotData = req.MasterRouteSelectedList.Where(w => !hasData.Any(a => a == w.MasterRouteGroupDetailGuid));
            List<TblMasterRoute_OptimizationStatus> data = new List<TblMasterRoute_OptimizationStatus>(req.MasterRouteSelectedList.Count);
            if (hasNotData.Any())
            {
                foreach (var rgd in hasNotData)
                {
                    var item = new TblMasterRoute_OptimizationStatus
                    {
                        Guid = Guid.NewGuid(),
                        MasterRoute_Guid = req.MasterRouteGuid,
                        MasterRouteGroupDetail_Guid = rgd.MasterRouteGroupDetailGuid,
                        SystemRouteOptimizationStatus_Guid = systemStatusGuid,
                        UserCreated = req.UserName,
                        DatetimeCreated = clientDate,
                        UniversalDatetimeCreated = utcDate
                    };
                    data.Add(item);
                }
            }
            return data;

        }
        private List<TblTransactionRouteOptimizationHeader_Detail> SetTransactionRouteOptimizationDetail(MasterRouteInsertRequest req, string requestRunning, DateTime clientDate, DateTimeOffset utcDate)
        {
            List<TblTransactionRouteOptimizationHeader_Detail> tblOzHeaderDetail = new List<TblTransactionRouteOptimizationHeader_Detail>(req.MasterRouteSelectedList.Count);

            var data = _masterRouteOptimizationStatusRepository.GetMasterRouteJobLegRequestOptimize(req.MasterRouteGuid, req.MasterRouteSelectedList.Select(s => s.MasterRouteGroupDetailGuid), req.SiteGuid);
            var routeGroupDetails = data.GroupBy(g => g.RouteGroupDetailGuid).Select(s => s.Key);
            int run = 1;
            foreach (var rgd in routeGroupDetails)
            {
                var joblegs = data.Where(w => w.RouteGroupDetailGuid == rgd);
                List<TblTransactionRouteOptimizationHeader_Detail_Item> tmpDetailItem = new List<TblTransactionRouteOptimizationHeader_Detail_Item>(joblegs.Count());

                foreach (var leg in joblegs)
                {
                    var item = new TblTransactionRouteOptimizationHeader_Detail_Item
                    {
                        Guid = Guid.NewGuid(),
                        RequestItemID = string.Format("{0}-{1}", requestRunning, run.ToString("0000")),
                        MasterRouteJobServiceStopLegs_Guid = leg.JobLegGuid,
                        MasterCustomerLocation_Guid = leg.LocationGuid,
                        SystemServiceJobType_Guid = leg.SystemServiceJobTypeGuid,
                        SequenceBefore = leg.JobOrder,
                        UserCreated = req.UserName,
                        DatetimeCreated = clientDate,
                        UniversalDatetimeCreated = utcDate
                    };
                    tmpDetailItem.Add(item);
                    run++;
                }
                var detail = new TblTransactionRouteOptimizationHeader_Detail
                {
                    Guid = Guid.NewGuid(),
                    MasterRouteGroupDetail_Guid = rgd,
                    UserCreated = req.UserName,
                    DatetimeCreated = clientDate,
                    UniversalDatetimeCreated = utcDate,

                    TblTransactionRouteOptimizationHeader_Detail_Item = tmpDetailItem //SetTransactionRouteOptimizationDetailItem(joblegs, req.UserName, requestRunning, clientDate, utcDate)
                };
                tblOzHeaderDetail.Add(detail);
            }
            return tblOzHeaderDetail;
        }

        private List<TblTransactionRouteOptimizationHeader_Detail_Item> SetTransactionRouteOptimizationDetailItem(IEnumerable<MasterJobRequestOptimizeViewModel> joblegs, string userName, string requestRunning, DateTime clientDate, DateTimeOffset utcDate)
        {
            List<TblTransactionRouteOptimizationHeader_Detail_Item> tmpDetailItem = new List<TblTransactionRouteOptimizationHeader_Detail_Item>(joblegs.Count());
            var run = 1;
            foreach (var leg in joblegs)
            {
                var item = new TblTransactionRouteOptimizationHeader_Detail_Item
                {
                    Guid = Guid.NewGuid(),
                    RequestItemID = string.Format("{0}-{1}", requestRunning, run.ToString("0000")),
                    MasterRouteJobServiceStopLegs_Guid = leg.JobLegGuid,
                    MasterCustomerLocation_Guid = leg.LocationGuid,
                    SystemServiceJobType_Guid = leg.SystemServiceJobTypeGuid,
                    SequenceBefore = leg.JobOrder,
                    UserCreated = userName,
                    DatetimeCreated = clientDate,
                    UniversalDatetimeCreated = utcDate
                };
                tmpDetailItem.Add(item);
                run++;
            }
            return tmpDetailItem;
        }

        private string SetRequestID(string routeTypeCode, string siteCode, Guid siteGuid, DateTime dateTime)
        {
            var SearchRef = new RunNumberRefModel();
            SearchRef.referenceValue1 = siteGuid.ToString();
            SearchRef.referenceValue2 = dateTime.ToString("yyyy-MM-dd");
            var requestRunning = _systemRunningValueCustomRepository.SetRunningIndependent(EnumRunningKey.RouteOptimizeRunning, SearchRef, 1);
            var run = requestRunning.ToString("0000");
            return $"{routeTypeCode}-B{siteCode}-{dateTime.ToString("yyyyMMdd")}-{run}";
        }

        public ValidateResponse ValidateRouteBalancing(DailyRouteDetailRequest request)
        {
            var response = new ValidateResponse();
            var userData = _baseRequest.Data;
            List<Guid> runlist = request.DailyRouteGuidList.Select(e => e).ToList();

            var listRunDoneBalance = _masterDailyRunResourceRepository.FindAllAsQueryable(e => runlist.Contains(e.Guid)).Where(e => e.FlagRouteBalanceDone).Select(e => e.MasterRunResource_Guid);
            if (listRunDoneBalance.Any())
            {
                var Message = _systemMessageRepository.GetMessage(-17369, userData.UserLanguageGuid.GetValueOrDefault());
                Message.MessageTextContent = string.Format(Message.MessageTextContent, string.Join(",", _masterRunResourceRepository.FindAllAsQueryable(e => listRunDoneBalance.Contains(e.Guid)).Select(f => f.VehicleNumber)));
                response.SetMessageView(Message.ConvertToMessageView(false));
                response.IsSuccess = false;
                response.IsWarning = true;
                return response;
            }

            response.IsSuccess = true;
            return response;
        }

        #endregion      

        #region Private

        #region Select
        private NemoOptimizationRequest GetNemoOptimizationRequest(int typeOptimization, NemoRouteOptimizationObject route, IEnumerable<NemoOptimizationLocations> locations)
        {
            string nullValue = null;
            var nemoOptimizationRequest = new NemoOptimizationRequest()
            {
                Depot = "DEPOT",
                BranchCode = route.BranchCode,
                BranchGuid = route.BranchGuid,
                CountryCode = route.CountryCode,
                CountryGuid = route.CountryGuid,
                ShiftGuid = route.NemoQueueGuid,
                Shift = string.Concat(route.CountryCode, "_", route.BranchCode, "_", route.DateStart.GetValueOrDefault()),
                ShiftServiceStart = this.GetNewStartHour(route.ShiftServiceStart),
                ShiftServiceEnd = this.GetNewEndHour(route.ShiftServiceStart, route.ShiftServiceEnd.GetValueOrDefault()),
                DateStart = route.DateStart.GetValueOrDefault(),
                TimeZone = route.TimeZone,
                RouteOptimizeType = typeOptimization,
                Nodes = locations.Select(o => o.LocationCode).ToArray(),
                Routes = new List<string[]>() { locations.Select(o => o.LocationCode).ToArray() },
                ServiceTypes = locations.Select(o => o.ServiceType).ToArray(),
                ParentNodes = locations.Select(o => nullValue).ToArray(),
                ParentJobs = locations.Select(o => nullValue).ToArray(),
                Unassigned = new List<string>().ToArray(),
                Volumes = locations.Select(o => (decimal)0).ToArray(),
                Capacities = locations.Select(o => (decimal)0).ToArray(),
                Liabilities = locations.Select(o => (decimal)0).ToArray(),
                Orders = locations.Select(o => 0).ToArray(),
                Jobs = locations.Select(o => o.JobGuid.ToString()).ToArray(),
                DateSchedules = new List<string>().ToArray(),
                TimeSchedules = new List<string>().ToArray(),
                ReferenceURL = string.Empty
            };
            return nemoOptimizationRequest;
        }
     
        private string GetNewStartHour(DateTime shiftServiceStart)
        {
            string newHour;
            TimeSpan defaultTime = new TimeSpan(0, 0, 0);
            TimeSpan currentTime = shiftServiceStart.TimeOfDay;
            DateTime newTempDate;
            if (defaultTime == currentTime)
            {
                shiftServiceStart = new DateTime(shiftServiceStart.Year, shiftServiceStart.Month, shiftServiceStart.Day, 1, 0, 0, 0);
                newTempDate = new DateTime(shiftServiceStart.Year, shiftServiceStart.Month, shiftServiceStart.Day, 1, 0, 0, 0);
                newHour = newTempDate.ToString("HH:mm:ss");
            }
            else
            {
                newHour = shiftServiceStart.ToString("HH:mm:ss");
            }

            return newHour;
        }

        private string GetNewEndHour(DateTime shiftServiceStart, DateTime shiftServiceEnd)
        {
            string newHour;
            DateTime newTempDate;
            TimeSpan defaultTime = new TimeSpan(0, 0, 0);
            TimeSpan currentStartTime = shiftServiceStart.TimeOfDay;
            TimeSpan currentEndTime = shiftServiceEnd.TimeOfDay;
            TimeSpan diference = currentEndTime - currentStartTime;

            if (defaultTime == currentEndTime || (diference.Days == 0 && diference.Hours == 0 && diference.Minutes == 0))
            {
                newTempDate = new DateTime(shiftServiceEnd.Year, shiftServiceEnd.Month, shiftServiceEnd.Day, 23, 59, 0, 0);
                newHour = newTempDate.ToString("HH:mm:ss");
            }
            else
            {
                newHour = shiftServiceEnd.ToString("HH:mm:ss");
            }

            return newHour;
        }

   
        #endregion

        #region Update
        private bool UpdateOptimizedDetails(SolutionRouteOptimizationResponse optimizedData)
        {
            var optmized = new NemoOptimizedDetails()
            {
                NemoTaskGuid = optimizedData.ShiftGuid,
                Distance = optimizedData.Distance,
                DurationTime = optimizedData.DurationTime,
                WaitTime = optimizedData.WaitTime,
                Jobs = optimizedData.Routes.FirstOrDefault()?.Jobs.Where(o => !string.IsNullOrWhiteSpace(o)).Select((o, i) => new RouteOptimizedJobDetails()
                {
                    JobGuid = Guid.Parse(o),
                    JobOrder = i + 1,
                    ScheduleTime = Convert.ToDateTime("1900-01-01 " + optimizedData.Routes.FirstOrDefault().Arrivals.ElementAt(i))
                }).ToList()
            };
            return _masterNemoQueueRouteOptimizationRepository.UpdateOptimizedDetails(optmized);
        }


        #endregion

        #endregion
    }
    public class JobDetailModelView
    {
        public Guid JobHeaderGuid { get; set; }
        public string ActionLeg { get; set; }
        public int SequenceStop { get; set; }
        public ChangeDetail ChangeDetail { get; set; }

    }
    public enum ChangeDetail
    {
        None,
        ChangeRun,
        Unassign
    }
}
