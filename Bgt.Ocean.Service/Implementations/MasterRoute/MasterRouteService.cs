using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Configuration;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SiteNetwork;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.Masterroute;
using static Bgt.Ocean.Models.MasterRoute.MassUpdateView;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using static Bgt.Ocean.Infrastructure.Util.EnumMasterRoute;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Service.Messagings.RouteOptimization;
using Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization;

namespace Bgt.Ocean.Service.Implementations.MasterRoute
{
    public interface IMasterRouteService
    {
        //Product Backlog Item 27142:[Master Route - Copy Function] Copy template shows an error when user selects the route group that has over 50 jobs.
        MasterRouteCopyJobsResponse MasterRouteCopyJobs(MasterRouteCopyJobsRequest req);
        MultiBrDetailResponse GetMutiBrDeliveryDetailByOriginLocation(GetMultiBrDestinationDetailRequest req);
        MultiBrDetailResponse GetBCDMutiBrPickupDetailByDestinationLocation(GetMultiBrOriginDetailRequest req);
        GetMasterRouteAllCustomerAndLocationResponse GetAllCustomerBySite(GetMasterRouteAllCustomerAndLocationRequest req);
        GetMasterRouteAllCustomerAndLocationResponse GetAllLocationByCustomer(GetMasterRouteAllCustomerAndLocationRequest req);
        GetAllSitePathResponse GetAllSitePath(GetAllSitePathRequest req);
        IEnumerable<DdlViewModel> GetMasterRouteDDL(MasterRouteNameRequest request);
        bool UpdateJobOrderByJobs(UpdateJobOrderRequest request);
        SystemMessageView UpdateJobOrderByMasterRoute(MassUpdateDataJobOrderViewRequest model);
        //TV MutiBr
        MasterRouteCreateJobResponse CreateMasterJobTVMultiBranch(MasterRouteCreateJobRequest req);
        MasterRouteCreateJobResponse UpdateMasterJobTVMultiBranch(MasterRouteCreateJobRequest req);
        //P MutiBr
        MasterRouteCreateJobResponse CreateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest req);
        MasterRouteCreateJobResponse UpdateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest req);
        //BCD MutiBr
        MasterRouteCreateJobResponse CreateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest req);
        MasterRouteCreateJobResponse UpdateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest req);

        UpdateSequenceIndexResponse UpdateSequenceIndex(UpdateSequenceIndexRequest model);
        ManualSortSeqIndexResponse UpdateManualSortSeqIndex(ManualSortSeqIndexRequest model);

        #region Mass Update
        IEnumerable<DropDownDayOfWeekView> GetDayOfWeek();
        IEnumerable<DropDownLobView> GetLOBDropdownList();
        IEnumerable<DropDownCustomerView> GetCustomerDDLBySite(Guid siteGuid);
        IEnumerable<DropDownLocationView> GetCustomerLocationDDLBySite(Guid siteGuid, Guid customerGuid);
        IEnumerable<DropDownServiceTypeView> GetServiceTypeByLOB(List<Guid> LobGuids, bool flagPcustomer, bool flagDcustomer, bool flagSameSite);
        MassUpdateJobResponse SetDataMassUpdate(MassUpdateJobCriteriaRequest model);
        #endregion
    }

    public class MasterRouteService : IMasterRouteService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemJobActionsRepository _systemJobActionsRepository;
        private readonly IMasterSitePathHeaderRepository _masterSitePathHeaderRepository;
        private readonly IMasterRouteJobHeaderRepository _masterRouteJobHeaderRepository;
        private readonly ISystemServiceStopTypesRepository _systemServiceStopTypesRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterRouteJobServiceStopLegsRepository _masterRouteJobServiceStopLegsRepository;
        private readonly IMasterCustomerLocationLocationDestinationRepository _masterCustomerLocation_LocationDestinationRepository;
        private readonly ISystemServiceJobTypeLOBRepository _systemServiceJobTypeLOBRepository;
        private readonly ISystemService _systemService;
        private readonly ISFOTblSystemLogCategoryRepository _sFOTblSystemLogCategoryRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;

        private readonly IObjectComparerService _objectComparerService;
        private readonly IMasterRouteTransactionLogService _masterRouteTransactionLogService;
        private readonly IMasterRouteRepository _masterRouteRepository;
        private readonly ITransactionRouteOptimizationHeaderRepository _transactionRouteOptimizationHeaderRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemEnvironmentMasterCountryValueRepository _systemEnvironmentMasterCountryValueRepository;

        public MasterRouteService(IUnitOfWork<OceanDbEntities> uow
            , ISystemMessageRepository systemMessageRepository
            , ISystemJobActionsRepository systemJobActionsRepository
            , IMasterSitePathHeaderRepository masterSitePathHeaderRepository
            , IMasterRouteJobHeaderRepository masterRouteJobHeaderRepository
            , ISystemServiceStopTypesRepository systemServiceStopTypesRepository
            , IMasterCustomerLocationRepository masterCustomerLocationRepository
            , IMasterRouteJobServiceStopLegsRepository masterRouteJobServiceStopLegsRepository
            , IMasterCustomerLocationLocationDestinationRepository masterCustomerLocation_LocationDestinationRepository
            , ISystemServiceJobTypeLOBRepository systemServiceJobTypeLOBRepository
            , IMasterSiteRepository masterSiteRepository
            , ISystemService systemService
            , ISFOTblSystemLogCategoryRepository sFOTblSystemLogCategoryRepository
            , ISystemServiceJobTypeRepository systemServiceJobTypeRepository
            , IObjectComparerService objectComparerService
            , IMasterRouteTransactionLogService masterRouteTransactionLogService
            , IMasterRouteRepository masterRouteRepository
            , ITransactionRouteOptimizationHeaderRepository transactionRouteOptimizationHeaderRepository  
            , ISystemEnvironmentMasterCountryValueRepository systemEnvironmentMasterCountryValueRepository
            )
        {
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _systemJobActionsRepository = systemJobActionsRepository;
            _masterSitePathHeaderRepository = masterSitePathHeaderRepository;
            _masterRouteJobHeaderRepository = masterRouteJobHeaderRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _systemServiceStopTypesRepository = systemServiceStopTypesRepository;
            _masterRouteJobServiceStopLegsRepository = masterRouteJobServiceStopLegsRepository;
            _masterCustomerLocation_LocationDestinationRepository = masterCustomerLocation_LocationDestinationRepository;
            _systemServiceJobTypeLOBRepository = systemServiceJobTypeLOBRepository;
            _systemService = systemService;
            _sFOTblSystemLogCategoryRepository = sFOTblSystemLogCategoryRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _objectComparerService = objectComparerService;
            _masterRouteTransactionLogService = masterRouteTransactionLogService;
            _masterRouteRepository = masterRouteRepository;
            _transactionRouteOptimizationHeaderRepository = transactionRouteOptimizationHeaderRepository;
            _masterSiteRepository = masterSiteRepository;
            _systemEnvironmentMasterCountryValueRepository = systemEnvironmentMasterCountryValueRepository;
        }

        #region ### Copy Master Route Jobs [not in use]
        public MasterRouteCopyJobsResponse MasterRouteCopyJobs(MasterRouteCopyJobsRequest req)
        {
            MasterRouteCopyJobsResponse result = new MasterRouteCopyJobsResponse();

            try
            {
                //input parameters
                MasterRouteCopyJobsProcedure proc = new MasterRouteCopyJobsProcedure();
                proc.Src_RouteGuid = req.Src_RouteGuid;
                proc.OverNightJobs = req.OverNightJobs;
                proc.SeletedJobs = req.SeletedJobs;

                proc.Dst_RouteGuid = req.Dst_RouteGuid;
                proc.Dst_DayOfWeekGuid = req.Dst_DayOfWeekGuid;
                proc.Dst_RouteGroupDetailGuid = req.Dst_RouteGroupDetailGuid;
                proc.FlagAllTemplate = req.FlagAllTemplate;

                proc.LanguageGuid = req.LanguageGuid;
                proc.UserGuid = req.UserGuid;
                proc.ClientDate = req.ClientDate;


                //output result
                var res = _masterRouteJobHeaderRepository.MasterRouteCopyJobs(proc);
                result.CantCopyJobs = res.CantCopyJobs;
                result.Message = _systemMessageRepository.FindByMsgId(res.MsgID, req.LanguageGuid).ConvertToMessageView();
            }
            catch (Exception)
            {
                result.Message = _systemMessageRepository.FindByMsgId(-184, req.LanguageGuid).ConvertToMessageView();
            }

            return result;
        }

        #endregion

        #region ### Get MultiBr Detail [P,TV,BCD]

        #region -Has predefined from stardard table master data
        //Get Destination Detail [P MultiBr,TV MultiBr]
        public MultiBrDetailResponse GetMutiBrDeliveryDetailByOriginLocation(GetMultiBrDestinationDetailRequest req)
        {
            MultiBrDetailResponse result = new MultiBrDetailResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetMultiBrDeliveryDetailByOriginLocation(req.OriginCustomerLocation_Guid, req.OriginSite_Guid, req.SystemSubServiceType_Guid, req.SystemServiceJobType_Guid, req.MasterLineOfBusiness_Guid, req.SystemDayOfWeek_Guid, req.ClientDateTime.DateTime, req.FlagShowAll, req.MasterRoute_Guid);
            return result;
        }

        //Get Origin Detail [BCD MultiBr]
        public MultiBrDetailResponse GetBCDMutiBrPickupDetailByDestinationLocation(GetMultiBrOriginDetailRequest req)
        {
            MultiBrDetailResponse result = new MultiBrDetailResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetBCDMutiBrPickupDetailByDestinationLocation(req.DestLocation_Guid, req.DestSite_Guid);
            return result;
        }
        #endregion

        #region -No predefined
        public GetMasterRouteAllCustomerAndLocationResponse GetAllCustomerBySite(GetMasterRouteAllCustomerAndLocationRequest req)
        {
            GetMasterRouteAllCustomerAndLocationResponse result = new GetMasterRouteAllCustomerAndLocationResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetMasterRouteAllCustomerBySite(req.Site_Guid, req.SystemSubServiceType_Guid, req.SystemServiceJobType_Guid, req.MasterLineOfBusiness_Guid, req.SystemDayOfWeek_Guid, req.ClientDateTime.DateTime, req.FlagShowAll, req.MasterRoute_Guid, req.OriginSite_Guid, req.strJobActionAbb);
            return result;
        }

        public GetMasterRouteAllCustomerAndLocationResponse GetAllLocationByCustomer(GetMasterRouteAllCustomerAndLocationRequest req)
        {
            GetMasterRouteAllCustomerAndLocationResponse result = new GetMasterRouteAllCustomerAndLocationResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetMasterRouteAllLocationByCustomer(req.Site_Guid, req.SystemSubServiceType_Guid, req.SystemServiceJobType_Guid, req.MasterLineOfBusiness_Guid, req.SystemDayOfWeek_Guid, req.ClientDateTime.DateTime, req.FlagShowAll, req.MasterRoute_Guid, req.MasterCustomer_Guid, req.OriginSite_Guid, req.strJobActionAbb);
            return result;
        }

        public GetAllSitePathResponse GetAllSitePath(GetAllSitePathRequest req)
        {
            GetAllSitePathResponse result = new GetAllSitePathResponse();
            result.sitepath = _masterSitePathHeaderRepository.GetAllSitePath(req.OriginSite_Guid, req.DestinationSite_Guid);
            return result;
        }
        #endregion

        #endregion

        #region ### Manage Master Route Job (Same Site)
        //P:Create
        //P:Update
        //D:Create
        //D:Update
        //T:Create
        //T:Update
        //TV:Create
        //TV:Update
        //BCD:Create
        //BCD:Update
        #endregion

        #region ### Manage Master Route Job (Inter Branch)
        //P:Create
        //P:Update
        //TV:Create
        //TV:Update
        #endregion

        #region ### Manange Master Route Job (Muti Branch)

        private Guid? GetCustomerContract(Guid? MasterSite_Guid, Guid? Customer_Guid, Guid? Location_Guid, DateTime? ClientDateTime)
        {
            //Get PickUp Contract
            bool FlagJobWithoutContract = _masterCustomerLocation_LocationDestinationRepository.GetMasterRouteFlagJobWithoutContract(MasterSite_Guid);
            var Contract_Guid = FlagJobWithoutContract ? null : _masterCustomerLocation_LocationDestinationRepository.GetCustomerContractGuid(Customer_Guid,
                                                                                              Location_Guid,
                                                                                              ClientDateTime);
            return Contract_Guid;
        }
        //TV:Create
        public MasterRouteCreateJobResponse CreateMasterJobTVMultiBranch(MasterRouteCreateJobRequest req)
        {
            req.IsUpdate = false;
            req.Pickupleg.SequenceStop = 1;
            req.Deliveryleg.SequenceStop = 2;

            return CreateOrUpdateMasterRouteJob(req);
        }
        //TV:Update
        public MasterRouteCreateJobResponse UpdateMasterJobTVMultiBranch(MasterRouteCreateJobRequest req)
        {
            req.IsUpdate = true;
            req.Pickupleg.SequenceStop = 1;
            req.Deliveryleg.SequenceStop = 2;

            return CreateOrUpdateMasterRouteJob(req);
        }

        //P:Create
        public MasterRouteCreateJobResponse CreateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest req)
        {
            #region Check Roadnet
            List<MasterRouteGuidModel> MasterRouteCheckGuid = new List<MasterRouteGuidModel>();
            //leg P new
            if (req.Pickupleg.MasterRouteGroupDetail_Guid.HasValue)
                MasterRouteCheckGuid.Add(new MasterRouteGuidModel { MasterRouteGuid = req.MasterRoute_Guid.Value, RouteGroupDetailGuid = new List<Guid> { req.Pickupleg.MasterRouteGroupDetail_Guid.Value } });
          
            if (MasterRouteCheckGuid.Any())
            {
                
                // get country option : RouteOptimizationDirectoryPath for check freezing     
                var CountryGuid = _masterSiteRepository.FindById(req.Deliveryleg.MasterSite_Guid).MasterCountry_Guid;
                var roadNetPath = _systemEnvironmentMasterCountryValueRepository.GetSpecificKeyByCountryAndKey(CountryGuid, EnumAppKey.RouteOptimizationDirectoryPath.ToString())?.AppValue1;                
                if (!string.IsNullOrEmpty(roadNetPath))
                {
                    var checkRoadnet = _transactionRouteOptimizationHeaderRepository.GetRouteGroupHasOptimize(new Models.RouteOptimization.RouteOptimizeSearchModel() { MasterRouteModel = MasterRouteCheckGuid });
                    if (checkRoadnet.FlaglockProcess)
                    {
                        MasterRouteCreateJobResponse result = new MasterRouteCreateJobResponse();
                        result.SetMessageView(_systemMessageRepository.FindByMsgId(-17354, req.UserLanguageGuid.Value).ConvertToMessageView());
                        result.Message = string.Format(result.Message, checkRoadnet.returnMessage);
                        result.IsSuccess = false;
                        result.IsWarning = true;
                        result.DateTimeModify = req.ClientDateTime.DateTime;
                        return result;
                    }
                }
            }
            #endregion

            req.IsUpdate = false;
            req.Pickupleg.SequenceStop = 1;
            req.Deliveryleg.SequenceStop = 2;
            req.Deliveryleg.FlagDeliveryLegForTV = false;
            req.Deliveryleg.MasterCustomerLocation_Guid = _masterCustomerLocationRepository.GetCompanyGuidBySite(req.Deliveryleg.MasterSite_Guid.GetValueOrDefault());

            return CreateOrUpdateMasterRouteJob(req);
        }
        //P:Update
        public MasterRouteCreateJobResponse UpdateMasterJobPickupMultiBranch(MasterRouteCreateJobRequest req)
        {
            #region Check Roadnet
            List<MasterRouteGuidModel> MasterRouteCheckGuid = new List<MasterRouteGuidModel>();
            //leg P new
            if (req.Pickupleg.MasterRouteGroupDetail_Guid.HasValue)
                MasterRouteCheckGuid.Add(new MasterRouteGuidModel { MasterRouteGuid = req.MasterRoute_Guid.Value, RouteGroupDetailGuid = new List<Guid> { req.Pickupleg.MasterRouteGroupDetail_Guid.Value } });

            if (MasterRouteCheckGuid.Any())
            {

                // get country option : RouteOptimizationDirectoryPath for check freezing     
                var CountryGuid = _masterSiteRepository.FindById(req.Deliveryleg.MasterSite_Guid).MasterCountry_Guid;
                var roadNetPath = _systemEnvironmentMasterCountryValueRepository.GetSpecificKeyByCountryAndKey(CountryGuid, EnumAppKey.RouteOptimizationDirectoryPath.ToString())?.AppValue1;
                if (!string.IsNullOrEmpty(roadNetPath))
                {
                    var checkRoadnet = _transactionRouteOptimizationHeaderRepository.GetRouteGroupHasOptimize(new Models.RouteOptimization.RouteOptimizeSearchModel() { MasterRouteModel = MasterRouteCheckGuid });
                    if (checkRoadnet.FlaglockProcess)
                    {
                        MasterRouteCreateJobResponse result = new MasterRouteCreateJobResponse();
                        result.SetMessageView(_systemMessageRepository.FindByMsgId(-17354, req.UserLanguageGuid.Value).ConvertToMessageView());
                        result.Message = string.Format(result.Message, checkRoadnet.returnMessage);
                        result.IsSuccess = false;
                        result.IsWarning = true;
                        result.DateTimeModify = req.ClientDateTime.DateTime;
                        return result;
                    }
                }
            }
            #endregion

            req.IsUpdate = true;
            req.Pickupleg.SequenceStop = 1;
            req.Deliveryleg.SequenceStop = 2;
            req.Deliveryleg.FlagDeliveryLegForTV = false;
            req.Deliveryleg.MasterCustomerLocation_Guid = _masterCustomerLocationRepository.GetCompanyGuidBySite(req.Deliveryleg.MasterSite_Guid.GetValueOrDefault());

            return CreateOrUpdateMasterRouteJob(req);
        }

        //BCD:Create
        public MasterRouteCreateJobResponse CreateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest req)
        {

            req.IsUpdate = false;
            req.Pickupleg = null; //<-- not has leg P
            req.Deliveryleg.SequenceStop = 2; //<-- has only one stop

            return CreateOrUpdateMasterRouteJob(req);
        }
        //BCD:Update
        public MasterRouteCreateJobResponse UpdateMasterJobBCDMultiBranch(MasterRouteCreateJobRequest req)
        {
            req.IsUpdate = true;
            req.Pickupleg = null; //<-- not has leg P
            req.Deliveryleg.SequenceStop = 2; //<-- has only one stop

            //Get Delivery Contract
            var Contract_Guid = GetCustomerContract(req.Deliveryleg.MasterSite_Guid, req.Deliveryleg.MasterCustomer_Guid, req.Deliveryleg.MasterCustomerLocation_Guid, req.ClientDateTime.DateTime);
            req.MasterCustomerContract_Guid = Contract_Guid;

            return CreateOrUpdateMasterRouteJob(req);
        }

        #endregion

        #region ### Update Job Order
        public bool UpdateJobOrderByJobs(UpdateJobOrderRequest request)
        {
            var logCat = _sFOTblSystemLogCategoryRepository.FindCategoryByCode(EnumHelper.GetDescription(request.IsUpdate ? EnumMasterRouteLogCategory.Job_Edit : EnumMasterRouteLogCategory.Job_Create));
            foreach (var routeGuid in request.JobDetails.Select(x => x.routeGuid).Distinct())
            {
                var routeGroupDetailGuids = request.JobDetails.Where(e => e.routeGuid == routeGuid).Select(x => x.routeGroupDetailGuid.GetValueOrDefault()).Distinct();
                string groupGuidComma = routeGroupDetailGuids == null ? string.Empty : string.Join(",", routeGroupDetailGuids);
                groupGuidComma = groupGuidComma.Replace(Guid.Empty.ToString(), FixStringRoute.NA);

                //must change
                var date = request.DateTimeModify.HasValue ? request.DateTimeModify.Value : request.LocalClientDateTime;
                _masterRouteJobServiceStopLegsRepository.MasterRouteUpdateJobOrder(routeGuid, groupGuidComma, false, logCat.SystemLogProcess_Guid, logCat.Guid, request.UserName, date, null);
            }
            return true;
        }


        public UpdateSequenceIndexResponse UpdateSequenceIndex(UpdateSequenceIndexRequest model)
        {
            UpdateSequenceIndexResponse response = new UpdateSequenceIndexResponse();

            try
            {
                if (model.RouteGroupDetailGuid.Any())
                {
                    var result = _masterRouteJobServiceStopLegsRepository.UpdateMasterRouteJobSeqIndex(model.MasterRouteGudGuid, model.RouteGroupDetailGuid, model.UserName, model.LocalClientDateTime);
                    if (result != null)
                    {
                        var msg = _systemMessageRepository.FindByMsgId(result.MsgId, model.Langquage.GetValueOrDefault()).ConvertToMessageView(result.MsgId == 0);
                        response.SetMessageView(msg);
                    }
                }

            }

            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-184, model.Langquage.GetValueOrDefault()).ConvertToMessageView());
            }

            return response;
        }


        public ManualSortSeqIndexResponse UpdateManualSortSeqIndex(ManualSortSeqIndexRequest model)
        {
            ManualSortSeqIndexResponse response = new ManualSortSeqIndexResponse();
            try
            {
                if (model.SourceJobLegs.Any(w => w.JobOrder == model.TargetJobOrder))
                {
                    var queryGetMasterRouteJob = new UpdateSeqIndexQuery().QueryGetMasterRouteJob;
                    var mixSeqSource = model.SourceJobLegs.Min(m => m.SeqIndex);
                    var optionChange = mixSeqSource >= model.TargetSeqindex ? 1 : 2;
                    var tmp = SetQueryManual(model, mixSeqSource >= model.TargetSeqindex);
                    string queryStr = string.Format(queryGetMasterRouteJob, tmp, mixSeqSource >= model.TargetSeqindex ? @">=" : @"<=");
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("@MasterRoute_Guid", model.MasterRouteGudGuid);
                    param.Add("@MasterRouteGroupDetail_Guid", model.RouteGroupDetailGuid.IsNullOrEmpty() ? "N/A" : model.RouteGroupDetailGuid.ToString());
                    param.Add("@TargetSeqIndex", model.TargetSeqindex);
                    param.Add("@JobOrder", model.TargetJobOrder);
                    param.Add("@UserModifed", model.UserName);
                    param.Add("@ClientDate", model.LocalClientDateTime);
                    param.Add("@OptionChange", optionChange);
                    var result = _masterRouteJobServiceStopLegsRepository
                        .ExectueQuery<MasterRouteUpdateSeqIndexResult>(queryStr, param).FirstOrDefault();
                    if (result != null)
                    {
                        var msg = _systemMessageRepository.FindByMsgId(result.MsgId, model.Langquage.GetValueOrDefault()).ConvertToMessageView(result.MsgId == 0);
                        response.SetMessageView(msg);
                    }

                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-184, model.Langquage.GetValueOrDefault()).ConvertToMessageView());
            }


            return response;
        }
        private string SetQueryManual(ManualSortSeqIndexRequest model, bool optionChange)
        {
            List<string> query = new List<string>();
            string strModel = "('{0}',{1})";
            int i = model.TargetSeqindex;
            if (optionChange)
            {
                foreach (var job in model.SourceJobLegs.OrderBy(o => o.SeqIndex))
                {
                    query.Add(string.Format(strModel, job.Guid, i));

                    i++;
                }
            }
            else
            {
                foreach (var job in model.SourceJobLegs.OrderByDescending(o => o.SeqIndex))
                {
                    query.Add(string.Format(strModel, job.Guid, i));

                    i--;
                }
            }

            return string.Format(@"INSERT INTO @TmpSourceJob (Guid,SeqIndex) VALUES {0}", string.Join(",", query));
        }
        #endregion

        #region ### Mass Update
        public IEnumerable<DropDownDayOfWeekView> GetDayOfWeek()
        {
            IEnumerable<SystemDayOfWeekView> daysOfWeek = _systemService.GetSystemDayOfWeekList();
            IEnumerable<DropDownDayOfWeekView> DayOfWeekList = daysOfWeek.Select(s => new DropDownDayOfWeekView { DayOfWeekGuid = s.Guid, DayOfWeekName = s.MasterDayOfWeek_Name, DayOfWeekSequence = s.MasterDayOfWeek_Sequence.GetValueOrDefault() });
            return DayOfWeekList;
        }

        public IEnumerable<DropDownLobView> GetLOBDropdownList()
        {
            IEnumerable<LobView> tblLOB = _systemService.GetLOBList().ToList();
            IEnumerable<DropDownLobView> LOBList = tblLOB.Select(s => new DropDownLobView { LobName = s.LOBFullName, LobGuid = s.Guid });
            return LOBList;
        }

        public IEnumerable<DropDownCustomerView> GetCustomerDDLBySite(Guid siteGuid)
        {
            IEnumerable<DropDownCustomerView> result;
            var data = _masterCustomerLocation_LocationDestinationRepository.GetAdhocAllCustomerBySite(siteGuid, null);
            result = data.CustomerList.Select(s => new DropDownCustomerView { CustomerGuid = s.CustomerGuid.GetValueOrDefault(), CustomerName = s.CustomerName });
            return result;
        }

        public IEnumerable<DropDownLocationView> GetCustomerLocationDDLBySite(Guid siteGuid, Guid customerGuid)
        {
            IEnumerable<DropDownLocationView> result;
            var data = _masterCustomerLocation_LocationDestinationRepository.GetAdhocAllLocationByCustomer(siteGuid, customerGuid, null);
            result = data.LocationList.Select(s => new DropDownLocationView { LocationGuid = s.LocationGuid.GetValueOrDefault(), LocationName = s.LocationName });
            return result;
        }

        public IEnumerable<DropDownServiceTypeView> GetServiceTypeByLOB(List<Guid> LobGuids, bool flagPcustomer, bool flagDcustomer, bool flagSameSite)
        {
            var data = _systemServiceJobTypeLOBRepository.GetServiceTypeByLOBs_MassUpdate(LobGuids, flagPcustomer, flagDcustomer, flagSameSite);
            return data;
        }

        public MassUpdateJobResponse SetDataMassUpdate(MassUpdateJobCriteriaRequest model)
        {
            MassUpdateJobResponse response = new MassUpdateJobResponse();
            try
            {
                var objQuer = new UpdateSeqIndexQuery();
                var queryGetMasterRouteJob = objQuer.QueryUpdateMassUpdate;
                var validate = objQuer.QueryCheckMassUpdateJobUnderOptimize;
                var strParam = SetMassUpdateParameter(model);
                if (!strParam.ToString().IsEmpty())
                {
                    queryGetMasterRouteJob = string.Format(queryGetMasterRouteJob, strParam.ToString());
                    validate = string.Format(validate, strParam.ToString());
                }

                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@PickupLocGuid", model.PickUpLeg.CustomerLocationGuid);
                param.Add("@PickUpSiteGuid", model.PickUpLeg.MasterSiteGuid);
                param.Add("@IsPickUpLocation", model.PickUpLeg.FlagIsCustomerLocation);
                param.Add("@DeliveryLocaGuid", model.DeliveryLeg.CustomerLocationGuid);
                param.Add("@DeliverySiteGuid", model.DeliveryLeg.MasterSiteGuid);
                param.Add("@IsDeliveryLocation", model.DeliveryLeg.FlagIsCustomerLocation);
                param.Add("@ReplaceType", model.ActionReplace);
                param.Add("@FlagHolliday", model.FlagIncludeHoliday);
                param.Add("@UserModify", model.UserName);
                param.Add("@ClientDate", model.LocalClientDateTime);

                /*replace*/
                param.Add("@LocReplaceGuid", model.DataForReplace.CustomerLocationGuid);
                param.Add("@SiteReplaceGuid", model.DataForReplace.MasterSiteGuid);
               
                var q = _masterRouteJobServiceStopLegsRepository.ExectueQuery< ValidateRouteUnderOptimizeModel>(validate, param);
                if (!q.Any())
                {
                    //Audit log
                    param.Add("@msg_3p", "[\"%s\",\"%s\",\"%s\"]");
                    param.Add("@msg_1p", "[\"%s\"]");
                    var result = _masterRouteJobServiceStopLegsRepository.ExectueMassUpdateData(queryGetMasterRouteJob, param);
                    if (result.Msg != null)
                    {
                        response.SetMessageView(_systemMessageRepository.FindByMsgId(result.Msg.MsgId, Guid.Empty).ConvertToMessageView(result.Msg.MsgId == 0));
                        response.MasterRouteDetail = result.Masterroute;
                    }
                }
                else
                {
                    var msg = _systemMessageRepository.FindByMsgId(-17354, Guid.Empty).ConvertToMessageView();
                    var rgdOptimize = string.Join(",", q.Select(s => s.MasterRouteGroupDetailName));
                    msg.MessageTextContent = string.Format(msg.MessageTextContent, rgdOptimize);
                    response.SetMessageView(msg);
                }


            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-184, model.Langquage.GetValueOrDefault()).ConvertToMessageView());

            }
            return response;
        }

        public SystemMessageView UpdateJobOrderByMasterRoute(MassUpdateDataJobOrderViewRequest model)
        {
            SystemMessageView response;
            try
            {
                if (model.Masterroute.Any())
                {
                    var logCat = _sFOTblSystemLogCategoryRepository.FindCategoryByCode(EnumHelper.GetDescription(model.SystemCategoryID));
                    var m = model.Masterroute.ToList();
                    foreach (var data in model.Masterroute.Select(s => s.MasterRouteGuid).Distinct())
                    {
                        var rd = m.Where(w => w.MasterRouteGuid == data).Select(s => s.RouteGroupDetailGuid.IsNullOrEmpty() ? "N/A" : s.RouteGroupDetailGuid.ToString());
                        string routeDetail = string.Join(",", rd);
                        //must change
                        _masterRouteJobServiceStopLegsRepository.MasterRouteUpdateJobOrder(data, routeDetail, false, logCat.SystemLogProcess_Guid, logCat.Guid, model.UserName, model.LocalClientDateTime, null);
                        //string
                    }
                }
                response = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView(true);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response = _systemMessageRepository.FindByMsgId(-184, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView(true);
            }
            return response;

        }
        #endregion

        #region *** Shared Function ***
        private StringBuilder SetMassUpdateParameter(MassUpdateJobCriteriaRequest model)
        {
            StringBuilder strBuil = new StringBuilder();
            if (model.LobGuids.Any())
            {
                string queryModle = @"INSERT INTO @TmpLob 
                                    SELECT Guid,LOBAbbrevaitionName 
                                    FROM TblSystemLineOfBusiness where Guid in ('{0}'); ";
                var param = string.Join(@"','", model.LobGuids);
                strBuil.AppendLine(string.Format(queryModle, param));
            }

            if (model.ServiceJobTypeGuids.Any())
            {
                string queryModle = @"INSERT INTO @TmpServiceType
                                    SELECT GUID, ServiceJobTypeID, ServiceJobTypeNameAbb FROM TblSystemServiceJobType where Guid in ('{0}'); ";
                var param = string.Join(@"','", model.ServiceJobTypeGuids);
                strBuil.AppendLine(string.Format(queryModle, param));

            }

            if (model.DayOfWeekGuids.Any())
            {
                string queryModle = @"INSERT INTO @TmpDayOfWeek
                                    SELECT Guid,MasterDayOfWeek_Sequence,MasterDayOfWeek_Name FROM TblSystemDayOfWeek where Guid in ('{0}') ";
                var param = string.Join(@"','", model.DayOfWeekGuids);
                strBuil.AppendLine(string.Format(queryModle, param));
            }
            List<int> t = new List<int>();
            if (model.FlagEveryWeek)
                t.Add(1);
            if (model.FlagA_Week)
                t.Add(2);
            if (model.FlagB_Week)
                t.Add(3);
            if (t.Any())
            {
                string queryModle = @"INSERT INTO @TmpWeekType
                                    SELECT Guid,WeekTypeName FROM TblSystemMaterRouteTypeOfWeek WHERE WeekTypeInt IN ({0}) ";
                var param = string.Join(@",", t);
                strBuil.AppendLine(string.Format(queryModle, param));
            }
            return strBuil;
        }
        private MasterRouteCreateJobResponse CreateOrUpdateMasterRouteJob(MasterRouteCreateJobRequest req)
        {
            MasterRouteCreateJobResponse result = new MasterRouteCreateJobResponse();
            try
            {
                using (var transcope = _uow.BeginTransaction())
                {
                    /*Internal ID -> 1 : Service Stop, 2 : Crew break, 3 : Inter Branch */
                    Guid SystemStopType_Guid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid;
                    bool HasPickupDetail = req.Pickupleg != null && result.data != null;
                    bool HasDeliveryDetail = req.Deliveryleg != null && result.data != null;

                    #region ### SET Shared request
                    req.FlagJobMultiBranch = true;
                    req.FlagJobInterBranch = false;
                    req.UserName = ApiSession.UserName;
                    req.SystemStopType_Guid = SystemStopType_Guid;
                    req.ClientDateTime_Local = req.ClientDateTime.DateTime;
                    req.UtcDateTimeModifyDefined = req.ClientDateTime;
                    #endregion

                    //[1]#Create/Update Master Route Job Header
                    ManageMasterRouteJobHeaderToDB(req);

                    if (HasPickupDetail)
                    {
                        //[2]#Create/Update Master Route Pickup Leg
                        var pleg = ManageMasterRouteJobServiceStopLegToDB(req, JobActionAbb.StrPickUp);
                        result.data.Add(pleg);
                    }

                    if (HasDeliveryDetail)
                    {
                        //[3]#Create/Update Master Route Delivery Leg
                        var dleg = ManageMasterRouteJobServiceStopLegToDB(req, JobActionAbb.StrDelivery);
                        result.data.Add(dleg);
                    }
                    if (!req.IsUpdate)
                    {
                        InsertLogCreateMasterRouteJob(req); //only create Master Job. Add Audit log here
                    }


                    //[4]#output message
                    result.SetMessageView(_systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView(true));
                    result.DateTimeModify = req.ClientDateTime_Local;
                    _uow.Commit();
                    transcope.Complete();
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView());

            }
            return result;
        }
        private void ManageMasterRouteJobHeaderToDB(MasterRouteCreateJobRequest req)
        {
            TblMasterRouteJobHeader head = null;
            List<ChangeInfo> compareList = new List<ChangeInfo>();

            #region INSERT OR UPDATE HEADER
            if (req.IsUpdate)
            {
                head = _masterRouteJobHeaderRepository.FindById(req.MasterJobHead_Guid);

                #region Insert audit log (Edit master route job - Multi-branch)
                compareList.AddRange(CompareResultFieldOfJob(head, req));
                #endregion

                req.MasterJobHead_Guid = head.Guid; // For Update Leg
                head.UserModifed = req.UserName;
                head.DatetimeModified = req.ClientDateTime_Local;
                head.UniversalDatetimeModified = req.UtcDateTimeModifyDefined;
                head.SystemMasterRouteJobNotAllowGenerateReason_Guid = null;
            }
            else
            {
                head = new TblMasterRouteJobHeader();
                head.Guid = Guid.NewGuid();
                req.MasterJobHead_Guid = head.Guid; // For Create Leg
                head.UserCreated = req.UserName;
                head.DatetimeCreated = req.ClientDateTime_Local;
                head.UniversalDatetimeCreated = req.UtcDateTimeModifyDefined;
            }
            #endregion

            #region SET HEADER DETAIL
            head.FlagDisable = false;
            head.MasterRoute_Guid = req.MasterRoute_Guid;
            head.DayInVault = req.DayInVault;
            head.FlagJobInterBranch = req.FlagJobInterBranch;
            head.FlagJobMultiBranch = req.FlagJobMultiBranch;
            head.MasterCustomerContract_Guid = req.MasterCustomerContract_Guid;
            head.MasterSitePathHeader_Guid = req.MasterSitePathHeader_Guid;
            head.MasterSubServiceType_Guid = req.MasterSubServiceType_Guid;
            head.OnwardDestinationType = req.OnwardDestinationType;
            head.OnwardDestination_Guid = req.OnwardDestination_Guid;
            head.SystemLineOfBusiness_Guid = req.SystemLineOfBusiness_Guid;
            head.SystemServiceJobType_Guid = req.SystemServiceJobType_Guid;
            head.SystemStopType_Guid = req.SystemStopType_Guid;
            head.SystemTripIndicator_Guid = req.SystemTripIndicator_Guid;

            #endregion

            #region SET DB STATE
            if (req.IsUpdate)
                _masterRouteJobHeaderRepository.Modify(head);
            else
                _masterRouteJobHeaderRepository.Create(head);

            #endregion

            if (compareList.Any())
            {
                InsertLogFieldChange(compareList, req);
            }
        }
        private UpdatedLegsView ManageMasterRouteJobServiceStopLegToDB(MasterRouteCreateJobRequest req, string JobAction)
        {
            MasterRouteJobServiceStopLegsView l = null;
            TblMasterRouteJobServiceStopLegs baseLeg = null;
            UpdatedLegsView response = new UpdatedLegsView();
            List<ChangeInfo> compareList = new List<ChangeInfo>();
            string seq_MsgID = string.Empty;

            #region SET PickUp Leg/Delivery Leg
            //coding like this for avoid sonar issue :(
            if (JobAction == JobActionAbb.StrPickUp)
            {
                l = req.Pickupleg;
                l.CustomerLocationAction_Guid = _systemJobActionsRepository.FindByAbbrevaition(JobActionAbb.StrPickUp).Guid;
                seq_MsgID = "5004";
            }
            else if (JobAction == JobActionAbb.StrDelivery)
            {
                l = req.Deliveryleg;
                l.CustomerLocationAction_Guid = _systemJobActionsRepository.FindByAbbrevaition(JobActionAbb.StrDelivery).Guid;
                seq_MsgID = "5010";
            }
            else
            {
                l = new MasterRouteJobServiceStopLegsView();
            }
            #endregion

            #region SET INSERT OR UPDATE LEG
            if (req.IsUpdate)
            {
                baseLeg = _masterRouteJobServiceStopLegsRepository.FindById(l.MasterRouteLeg_Guid);

                #region Insert audit log (Edit master route job - Multi-branch)
                compareList.AddRange(CompareResultFieldOfLeg(baseLeg, l, JobAction, req));
                #endregion

                #region Insert log sequence
                if (!(isRouteNotChange(baseLeg, l)))
                {
                    InsertLogSequence(baseLeg.JobOrder.GetValueOrDefault(), seq_MsgID, req.UserName, req.ClientDateTime_Local, req.UtcDateTimeModifyDefined, req.MasterJobHead_Guid, EnumMasterRouteLogCategory.Seq_EditJob);
                }
                #endregion
            }
            else
            {
                baseLeg = new TblMasterRouteJobServiceStopLegs();
                l.MasterRouteLeg_Guid = Guid.NewGuid();
            }

            #endregion

            #region SET Leg Detail
            baseLeg.Guid = l.MasterRouteLeg_Guid;
            baseLeg.MasterRouteJobHeader_Guid = req.MasterJobHead_Guid;
            baseLeg.CustomerLocationAction_Guid = l.CustomerLocationAction_Guid;
            baseLeg.DayOfWeekCompletion_Sequence = l.DayOfWeekCompletion_Sequence;
            baseLeg.DayOfWeek_Sequence = l.DayOfWeek_Sequence;
            baseLeg.FlagDeliveryLegForTV = l.FlagDeliveryLegForTV;
            baseLeg.FlagDestination = l.FlagDestination;
            baseLeg.FlagDisableRoute = l.FlagDisableRoute;
            baseLeg.FlagMultiplesDaysJob = l.FlagMultiplesDaysJob;
            baseLeg.FlagNonBillable = l.FlagNonBillable;
            baseLeg.JobOrder = l.JobOrder;
            baseLeg.MasterCustomerLocation_Guid = l.MasterCustomerLocation_Guid;
            baseLeg.MasterRouteDeliveryLeg_Guid = l.MasterRouteDeliveryLeg_Guid;
            baseLeg.MasterRouteGroupDetail_Guid = l.MasterRouteGroupDetail_Guid;
            baseLeg.MasterSite_Guid = l.MasterSite_Guid;
            baseLeg.NumberOfDaysCompletionJob = l.NumberOfDaysCompletionJob;
            baseLeg.SchduleTime = l.StrSchduleTime.ChangeFromTimeToDateTime();
            baseLeg.SeqIndex = l.SeqIndex;
            baseLeg.SequenceStop = l.SequenceStop;

            #endregion

            #region SET DB STATE
            if (req.IsUpdate)
            {
                _masterRouteJobServiceStopLegsRepository.Modify(baseLeg);
                InsertLogFieldChange(compareList, req);
            }
            else
            {
                _masterRouteJobServiceStopLegsRepository.Create(baseLeg);
            }
            #endregion

            #region SET OUTPUT

            //for update job order
            response.LegGuid = baseLeg.Guid;
            response.MasterRouteGuid = JobAction == JobActionAbb.StrPickUp ? req.MasterRoute_Guid : baseLeg.MasterRouteDeliveryLeg_Guid;
            response.RouteGroupDetailGuid = baseLeg.MasterRouteGroupDetail_Guid ?? Guid.Empty;
            response.SiteGuid = baseLeg.MasterSite_Guid;

            #endregion

            return response;
        }

        private bool isRouteNotChange(TblMasterRouteJobServiceStopLegs source, MasterRouteJobServiceStopLegsView dest)
        {
            return (source.MasterRouteGroupDetail_Guid == dest.MasterRouteGroupDetail_Guid) && (source.MasterCustomerLocation_Guid == dest.MasterCustomerLocation_Guid) && (source.SchduleTime == dest.StrSchduleTime.ToTimeDateTime());
        }

        #region [TFS#47783] Insert log field change
        private void InsertLogFieldChange(List<ChangeInfo> compareResult, MasterRouteCreateJobRequest request)
        {
            var compareResultGrp = compareResult.GroupBy(x => x.LabelKey).Select(o => o.FirstOrDefault());
            var fieldChange = compareResultGrp
                .Select(o => new MasterRouteLogRequest()
                {
                    ReferenceValue_Guid = request.MasterJobHead_Guid,
                    Remark = "MasterRouteJobHeaderGuid",
                    UserName = request.UserName,
                    DatetimeCreated = request.ClientDateTime_Local,
                    UniversalDatetimeCreated = request.UtcDateTimeModifyDefined,
                    SystemMsgID = "5066", //Change {0} from {1} to {2}.
                    JSONValue = new string[] { o.LabelKey, o.OldValue == null ? "-" : o.OldValue, o.NewValue == null ? "-" : o.NewValue }.ToJSONString()
                });
            _masterRouteTransactionLogService.InsertMasterRouteTransactionLogAsync(EnumMasterRouteLogCategory.Job_Edit, EnumMasterRouteProcess.MASTER_ROUTE_JOB, fieldChange?.ToArray());
        }
        private List<ChangeInfo> CompareResultFieldOfJob(TblMasterRouteJobHeader routeJob, MasterRouteCreateJobRequest request)
        {
            List<ChangeInfo> compareList = new List<ChangeInfo>();
            var resultChangeJob = _objectComparerService.GetCompareResult(routeJob, request, EnumHelper.GetDescription(EnumMasterRouteConfigKey.MRJ_EDIT_JOB_API)).ChangeInfoList;

            int[] jobAction = new int[] { IntTypeJob.P, IntTypeJob.TV, IntTypeJob.T, IntTypeJob.BCP, IntTypeJob.P_MultiBr, IntTypeJob.TV_MultiBr };
            int jobTypeID = _systemServiceJobTypeRepository.FindById(request.SystemServiceJobType_Guid).ServiceJobTypeID.GetValueOrDefault();
            string jobActionPrefix = jobAction.Any(x => x == jobTypeID) ? MasterRouteJobLegDisplay.StrDelivery : MasterRouteJobLegDisplay.StrPickUp;
            foreach (var item in resultChangeJob)
            {
                bool isOnwardValue = (item.LabelKey == "Onward Destination" || item.LabelKey == "Onward Type");
                bool isSitePath = item.LabelKey == "Site Path";
                if (isOnwardValue || isSitePath)
                {
                    item.LabelKey = jobActionPrefix + item.LabelKey;
                    item.OldValue = item.OldValue == "0" ? null : item.OldValue;
                    item.NewValue = item.NewValue == "0" ? null : item.NewValue;
                }
            }
            compareList.AddRange(resultChangeJob);
            return compareList;
        }
        private List<ChangeInfo> CompareResultFieldOfLeg(TblMasterRouteJobServiceStopLegs routeLeg, MasterRouteJobServiceStopLegsView routeLegModel
            , string strAction, MasterRouteCreateJobRequest req)
        {

            int? jobTypeID = _systemServiceJobTypeRepository.FindById(req.SystemServiceJobType_Guid)?.ServiceJobTypeID;

            List<ChangeInfo> compareList = new List<ChangeInfo>();
            var tblCustomerLoc_Old = _masterCustomerLocationRepository.FindById(routeLeg.MasterCustomerLocation_Guid);
            var tblCustomerLoc_New = _masterCustomerLocationRepository.FindById(routeLegModel.MasterCustomerLocation_Guid);

            //Prepare data for compare (some value not have model)
            //OldData
            routeLeg.CustomerGuid = tblCustomerLoc_Old.MasterCustomer_Guid;
            routeLeg.Time = routeLeg.SchduleTime.GetValueOrDefault().ToString("HH:mm");
            //NewData
            routeLegModel.MasterCustomer_Guid = tblCustomerLoc_New.MasterCustomer_Guid;
            routeLegModel.StrSchduleTime = routeLegModel.StrSchduleTime ?? "00:00";

            var resultChangeLeg = _objectComparerService.GetCompareResult(routeLeg, routeLegModel, EnumHelper.GetDescription(EnumMasterRouteConfigKey.MRJ_EDIT_LEG_API)).ChangeInfoList;

            //Label no action leg
            string[] labelList = new string[] { "Multiples Days Job", "Master Route (Delivery)", "Days" };
            string strLeg = strAction == JobActionAbb.StrPickUp ? MasterRouteJobLegDisplay.StrPickUp : MasterRouteJobLegDisplay.StrDelivery;

            foreach (var s in resultChangeLeg)
            {
                bool isLabelDay = s.LabelKey == "Days";
                string strSuffixDay = (strAction == "P" ? " Pick Up" : " Delivery");
                string strLabelDays = (labelList.Any(x => x == s.LabelKey) ? string.Empty : strLeg) + s.LabelKey + (isLabelDay ? strSuffixDay : string.Empty);
                s.LabelKey = (jobTypeID != IntTypeJob.TV_MultiBr) && isLabelDay ? "Number of days" : strLabelDays;
                s.OldValue = CheckboxValue(s.OldValue);
                s.NewValue = CheckboxValue(s.NewValue);
                compareList.Add(s);
            }

            return compareList;
        }
        private static string CheckboxValue(string txtValue)
        {
            if (txtValue == "True")
            {
                txtValue = MasterRouteJobCheckbox.StrSelected;
            }
            else if (txtValue == "False")
            {
                txtValue = MasterRouteJobCheckbox.StrUnSelected;
            }
            return txtValue;
        }

        private void InsertLogSequence(int jobOrder, string msgID, string userName, DateTime clientDateTime, DateTimeOffset utcDateTime, Guid refGuid, EnumMasterRouteLogCategory enumLogCate)
        {
            var logChangeOrder = new MasterRouteLogRequest()
            {
                ReferenceValue_Guid = refGuid,
                Remark = "MasterRouteJobHeaderGuid",
                UserName = userName,
                DatetimeCreated = clientDateTime,
                UniversalDatetimeCreated = utcDateTime,
                SystemMsgID = msgID,
                JSONValue = new string[] { jobOrder.ToString() }.ToJSONString()
            };
            _masterRouteTransactionLogService.InsertMasterRouteTransactionLogAsync(enumLogCate, EnumMasterRouteProcess.MASTER_ROUTE_JOB, logChangeOrder);
        }
        #endregion

        #region [TFS#25348(47784)] [Job] - Create Maser Route Job
        private MasterRouteLogRequest CreateMasterRouteJobLogRequest(MasterRouteCreateJobRequest request, string systemMsgID, string jsonValue = "")
        {
            string title = string.Empty;
            switch (systemMsgID)
            {
                case "5089": title = "Create Job : (5089) The Master Route Job is created."; break;
                default: title = string.Empty; break;
            }
            string remark = string.Format("{0} : ReferenceValue_Guid is MasterRouteJobHeaderGuid", title);
            return new MasterRouteLogRequest()
            {
                ReferenceValue_Guid = request.MasterJobHead_Guid,
                Remark = remark,
                UserName = request.UserName,
                DatetimeCreated = request.ClientDateTime_Local,
                UniversalDatetimeCreated = request.UtcDateTimeModifyDefined,
                SystemMsgID = systemMsgID,
                JSONValue = jsonValue
            };
        }
        private void InsertLogCreateMasterRouteJob(MasterRouteCreateJobRequest request)
        {
            IEnumerable<MasterRouteLogRequest> listMasterLogRequest = new List<MasterRouteLogRequest>()
            {
                CreateMasterRouteJobLogRequest(request, "5089")           //The Master Route Job is created.
            };
            _masterRouteTransactionLogService.InsertMasterRouteTransactionLogAsync(EnumMasterRouteLogCategory.Job_Create, EnumMasterRouteProcess.MASTER_ROUTE_JOB, listMasterLogRequest?.ToArray());
        }
        #endregion //[TFS#25348(47784)] [Job] - Create Maser Route Job
        #endregion
        #region ### Route Optimization 

        public IEnumerable<DdlViewModel> GetMasterRouteDDL(MasterRouteNameRequest request)
        {
            //request
            var masterRoute = _masterRouteRepository.GetMasterRoute(request.ConvertToModelRequest())
                .Select(s => new DdlViewModel
                {
                    Text = s.MasterRouteName,
                    Value = s.Guid
                }).OrderBy(o => o.Text);
            return masterRoute;
        }
        #endregion
    }

  
}