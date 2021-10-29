using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Consolidation;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SiteNetwork;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.Consolidation;
using Bgt.Ocean.Service.ModelViews.Consolidation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumPreVault;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Service.Implementations.Consolidation
{
    public interface IConsolidationService
    {
        Task<ConsolidateInfoModel> GetMainConsolidateLocation(DateTime workDate, Guid siteGuid);
        Task<ConsolidateInfoModel> GetMainConsolidateRoute(DateTime workDate, Guid siteGuid);
        Task<ConsolidateInfoModel> GetMainConsolidateInterBranch(DateTime workDate, Guid siteGuid);
        Task<ConsolidateInfoModel> GetMainConsolidateMultiBranch(DateTime workDate, Guid siteGuid);

        ConMultiBranchEditResponse GetEditDetailMultiBranchConsolidation(ConMultiBranchEditModelRequest request);
        ConMultiBranchCreateUpdateResponse CreateConsolidate_MultiBranch(ConMultiBranchCreateUpdateRequest request);
        ConMultiBranchCreateUpdateResponse UpdateConsolidate_MultiBranch(ConMultiBranchCreateUpdateRequest request);

        ConGetItemResponse GetItemConMultiBranch_New(ItemAvailableConRequest request);
        Dictionary<string, ConsolidateAllItemView> GetItemConMultiBranch_Edit(ItemAvailableConRequest request);
        IEnumerable<DropdownSitePathView> GetSitePathHaveItemDDL(SitePathDDLRequest request);
    }

    public class ConsolidationService : IConsolidationService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemService _systemService;
        private readonly IMasterConAndDeconsolidate_HeaderRepository _masterConAndDeconsolidate_HeaderRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
        private readonly IMasterHistory_SealRepository _masterHistory_SealRepository;
        private readonly IMasterHistory_ActualJobRepository _masterHistory_ActualJobRepository;
        private readonly IMasterActualJobItemsCommodityRepository _masterActualJobItemsCommodityRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly IMasterSitePathHeaderRepository _masterSitePathHeaderRepository;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroup_DetailRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerLocationInternalDepartmentRepository _masterCustomerLocationInternalDepartmentRepository;

        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;

        public ConsolidationService(IUnitOfWork<OceanDbEntities> uow, ISystemService systemService, IMasterConAndDeconsolidate_HeaderRepository masterConAndDeconsolidate_HeaderRepository,
            IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository,
            IMasterHistory_SealRepository masterHistory_SealRepository,
            IMasterHistory_ActualJobRepository masterHistory_ActualJobRepository,
            IMasterActualJobItemsCommodityRepository masterActualJobItemsCommodityRepository,
            IMasterSiteRepository masterSiteRepository,
            IMasterSitePathHeaderRepository masterSitePathHeaderRepository,
            IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository,
            IMasterCustomerRepository masterCustomerRepository,
            IMasterCustomerLocationRepository masterCustomerLocationRepository,
            IMasterCustomerLocationInternalDepartmentRepository masterCustomerLocationInternalDepartmentRepository,
            ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository
            )
        {
            _uow = uow;
            _systemService = systemService;
            _masterConAndDeconsolidate_HeaderRepository = masterConAndDeconsolidate_HeaderRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
            _masterHistory_SealRepository = masterHistory_SealRepository;
            _masterHistory_ActualJobRepository = masterHistory_ActualJobRepository;
            _masterActualJobItemsCommodityRepository = masterActualJobItemsCommodityRepository;
            _masterSiteRepository = masterSiteRepository;
            _masterSitePathHeaderRepository = masterSitePathHeaderRepository;
            _masterRouteGroup_DetailRepository = masterRouteGroupDetailRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerLocationInternalDepartmentRepository = masterCustomerLocationInternalDepartmentRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
        }

        #region Main Screen
        public async Task<ConsolidateInfoModel> GetMainConsolidateLocation(DateTime workDate, Guid siteGuid)
        {
            ConsolidateInfoResult info = new ConsolidateInfoResult();
            ConsolidateInfoModel result = new ConsolidateInfoModel();
            try
            {
                StringBuilder query = null;

                //Query String :: Select Data
                query = new StringBuilder();
                query.Append($@" BEGIN
                    SELECT  DISTINCT
		                                CON.Guid AS MasterID_Guid,
		                                CON.MasterID AS MasterID,
                                        TblSystemConAndDeconsolidateStatus.StatusID AS StatusID,
		                                TblSystemConAndDeconsolidateStatus.StatusName AS StatusName,   
		                                TblMasterCustomer.CustomerFullName + ' - ' + TblMasterCustomerLocation.BranchName AS LocationName, 
		                                CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
				                                ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END 
				                                AS RouteName
                                        FROM    TblMasterConAndDeconsolidate_Header CON
		                                        INNER JOIN TblMasterCustomerLocation ON CON.MasterCustomerLocation_Guid = TblMasterCustomerLocation.Guid
		                                        INNER JOIN TblSystemConAndDeconsolidateStatus ON CON.SystemCoAndDeSolidateStatus_Guid = TblSystemConAndDeconsolidateStatus.Guid
		                                        INNER JOIN TblMasterCustomer ON TblMasterCustomerLocation.MasterCustomer_Guid = TblMasterCustomer.Guid
		                                        INNER JOIN TblMasterDailyRunResource ON CON.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
		                                        INNER JOIN TblMasterRunResource ON TblMasterRunResource.Guid = TblMasterDailyRunResource.MasterRunResource_Guid
		                                        INNER JOIN TblMasterRouteGroup_Detail ON TblMasterRouteGroup_Detail.Guid = TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid
                                        WHERE   CON.MasterSite_Guid = @SiteGuid
	                                            AND CON.WorkDate = @WorkDate
                                                AND CON.ConsolidationRoute_Guid IS NULL 
		                                        AND CON.MasterRouteGroup_Detail_Guid IS NULL
		                                        AND CON.MasterCustomerLocation_Guid IS NOT NULL
		                                        AND CON.SystemConsolidateSourceID = 1
		                                        AND TblSystemConAndDeconsolidateStatus.StatusID IN (1,2)
                                    END
                ");

                using (var dbContext = new OceanDbEntities())
                {
                    info.LocationConsolidation = await dbContext.Database.SqlQuery<PreVaultConsolidateInfoResult>(
                                query.ToString()
                                , new SqlParameter("@WorkDate", workDate)
                                , new SqlParameter("@SiteGuid", siteGuid)
                                ).ToListAsync();        
                }
                result.LocationConsolidation = info.LocationConsolidation.ConvertToLocationInfoView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public async Task<ConsolidateInfoModel> GetMainConsolidateRoute(DateTime workDate, Guid siteGuid)
        {
            ConsolidateInfoResult info = new ConsolidateInfoResult();
            ConsolidateInfoModel result = new ConsolidateInfoModel();
            try
            {
                StringBuilder query = null;

                //Query String :: Select Data
                query = new StringBuilder();
                query.Append($@" BEGIN
			        SELECT	CON.Guid AS MasterID_Guid, 
			        		CON.MasterID AS MasterID, 
                            TblSystemConAndDeconsolidateStatus.StatusID AS StatusID,
			        		TblSystemConAndDeconsolidateStatus.StatusName AS StatusName, 
			        		CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
			        			ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END 
			        			AS RouteName
			        FROM    TblMasterConAndDeconsolidate_Header CON
			        		INNER JOIN TblSystemConAndDeconsolidateStatus ON CON.SystemCoAndDeSolidateStatus_Guid = TblSystemConAndDeconsolidateStatus.Guid
			        		INNER JOIN TblMasterDailyRunResource ON CON.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
			        		INNER JOIN TblMasterRunResource ON TblMasterRunResource.Guid = TblMasterDailyRunResource.MasterRunResource_Guid
			        		INNER JOIN TblMasterRouteGroup_Detail ON TblMasterRouteGroup_Detail.Guid = TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid
			        WHERE	CON.MasterSite_Guid = @SiteGuid
			        		AND CON.WorkDate = @WorkDate
			        		AND CON.MasterRouteGroup_Detail_Guid IS NOT NULL
			        		AND CON.MasterCustomerLocation_Guid IS NULL
			        		AND CON.SystemConsolidateSourceID = 1
			        		AND TblSystemConAndDeconsolidateStatus.StatusID IN (1,2)
			        		AND TblMasterDailyRunResource.FlagDisable = 0
			        		AND TblMasterDailyRunResource.RunResourceDailyStatusID = 1
                END
                ");

                using (var dbContext = new OceanDbEntities())
                {
                    info.RouteConsolidation = await dbContext.Database.SqlQuery<PreVaultConsolidateInfoResult>(
                                query.ToString()
                                , new SqlParameter("@WorkDate", workDate)
                                , new SqlParameter("@SiteGuid", siteGuid)
                                ).ToListAsync();
                }
                result.RouteConsolidation = info.RouteConsolidation.ConvertToRouteInfoView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public async Task<ConsolidateInfoModel> GetMainConsolidateInterBranch(DateTime workDate, Guid siteGuid)
        {
            ConsolidateInfoResult info = new ConsolidateInfoResult();
            ConsolidateInfoModel result = new ConsolidateInfoModel();
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append($@" BEGIN
	                SELECT      CON.Guid AS MasterID_Guid, 
	                            CON.MasterID AS MasterID, 
                                TblSystemConAndDeconsolidateStatus.StatusID AS StatusID,
	                            TblSystemConAndDeconsolidateStatus.StatusName AS StatusName,  
	                            TblMasterCustomer.CustomerFullName + ' - ' + TblMasterCustomerLocation.BranchName AS LocationName,
	                            DestSite.SiteCode + ' - ' + DestSite.SiteName AS Destination_MasterSite_Name,			 
                                IIF(CON.MasterCustomerLocation_Guid IS NOT NULL, '',
	                                (CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
	                                    ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END)
	                            )AS RouteName,
	                            ISNULL(IntDep.InterDepartmentName, Owd.OnwardDestinationName) AS InternalDepartmentName,
			                    '' AS SitePathName
	                    FROM    TblMasterConAndDeconsolidate_Header CON
	                            INNER JOIN TblSystemConAndDeconsolidateStatus ON CON.SystemCoAndDeSolidateStatus_Guid = TblSystemConAndDeconsolidateStatus.Guid
	                            INNER JOIN TblMasterSite DestSite ON CON.Destination_MasterSite_Guid = DestSite.Guid	                			
	                            LEFT JOIN TblMasterCustomerLocation ON CON.MasterCustomerLocation_Guid = TblMasterCustomerLocation.Guid
	                            LEFT JOIN TblMasterCustomer ON TblMasterCustomerLocation.MasterCustomer_Guid = TblMasterCustomer.Guid	                			
	                            LEFT JOIN TblMasterDailyRunResource ON CON.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
	                            LEFT JOIN TblMasterRouteGroup_Detail ON TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid = TblMasterRouteGroup_Detail.Guid
	                            LEFT JOIN TblMasterRunResource ON TblMasterDailyRunResource.MasterRunResource_Guid = TblMasterRunResource.Guid
	                            LEFT JOIN TblMasterCustomerLocation_InternalDepartment IntDep ON CON.OnwardDestination_Guid = IntDep.Guid
	                            LEFT JOIN TblSystemOnwardDestinationType Owd ON CON.OnwardDestination_Guid = Owd.Guid
	                	WHERE   CON.MasterSite_Guid = @SiteGuid
	                			AND CON.WorkDate = @WorkDate
	                			AND CON.ConsolidationRoute_Guid IS NULL 
	                			AND CON.SystemConsolidateSourceID = 3
	                			AND TblSystemConAndDeconsolidateStatus.StatusID IN (1,2)
                    END
                ");

                using (var dbContext = new OceanDbEntities())
                {
                    info.InterBranchConsolidation = await dbContext.Database.SqlQuery<PreVaultConsolidateInfoResult>(
                                query.ToString()
                                , new SqlParameter("@WorkDate", workDate)
                                , new SqlParameter("@SiteGuid", siteGuid)
                                ).ToListAsync();
                }
                result.InterBranchConsolidation = info.InterBranchConsolidation.ConvertToInterBranchInfoView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public async Task<ConsolidateInfoModel> GetMainConsolidateMultiBranch(DateTime workDate, Guid siteGuid)
        {
            ConsolidateInfoResult info = new ConsolidateInfoResult();
            ConsolidateInfoModel result = new ConsolidateInfoModel();
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append($@" BEGIN
	                SELECT      CON.Guid AS MasterID_Guid, 
	                			CON.MasterID AS MasterID, 
                                TblSystemConAndDeconsolidateStatus.StatusID AS StatusID,
	                			TblSystemConAndDeconsolidateStatus.StatusName AS StatusName,  
	                			TblMasterCustomer.CustomerFullName + ' - ' + TblMasterCustomerLocation.BranchName AS LocationName,
	                			DestSite.SiteCode + ' - ' + DestSite.SiteName AS Destination_MasterSite_Name,
                                IIF(CON.MasterCustomerLocation_Guid IS NOT NULL, '',
	                			    (CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
	                			    	ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END)
	                			)AS RouteName,
	                			ISNULL(IntDep.InterDepartmentName, Owd.OnwardDestinationName) AS InternalDepartmentName,
								SP.SitePathName AS SitePathName
	                	FROM    TblMasterConAndDeconsolidate_Header CON
	                			INNER JOIN TblSystemConAndDeconsolidateStatus ON CON.SystemCoAndDeSolidateStatus_Guid = TblSystemConAndDeconsolidateStatus.Guid
	                			INNER JOIN TblMasterSite DestSite ON CON.Destination_MasterSite_Guid = DestSite.Guid	                			
	                			LEFT JOIN TblMasterCustomerLocation ON CON.MasterCustomerLocation_Guid = TblMasterCustomerLocation.Guid
	                			LEFT JOIN TblMasterCustomer ON TblMasterCustomerLocation.MasterCustomer_Guid = TblMasterCustomer.Guid	                			
	                			LEFT JOIN TblMasterDailyRunResource ON CON.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
	                			LEFT JOIN TblMasterRouteGroup_Detail ON TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid = TblMasterRouteGroup_Detail.Guid
	                			LEFT JOIN TblMasterRunResource ON TblMasterDailyRunResource.MasterRunResource_Guid = TblMasterRunResource.Guid
	                			LEFT JOIN TblMasterCustomerLocation_InternalDepartment IntDep ON CON.OnwardDestination_Guid = IntDep.Guid
	                			LEFT JOIN TblSystemOnwardDestinationType Owd ON CON.OnwardDestination_Guid = Owd.Guid
								LEFT JOIN TblMasterSitePathHeader SP ON CON.MasterSitePathHeader_Guid = SP.Guid
	                	WHERE   CON.MasterSite_Guid = @SiteGuid
	                			AND CON.WorkDate = @WorkDate
	                			AND CON.ConsolidationRoute_Guid IS NULL 
	                			AND CON.SystemConsolidateSourceID = 4      
	                			AND TblSystemConAndDeconsolidateStatus.StatusID IN (1,2)
                    END
                ");

                using (var dbContext = new OceanDbEntities())
                {
                    info.MultiranchConsolidation = await dbContext.Database.SqlQuery<PreVaultConsolidateInfoResult>(
                                query.ToString()
                                , new SqlParameter("@WorkDate", workDate)
                                , new SqlParameter("@SiteGuid", siteGuid)
                                ).ToListAsync();
                }
                result.MultiBranchConsolidation = info.MultiranchConsolidation.ConvertToMultiBranchInfoView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        #endregion

        #region Multi-Branch
        public ConMultiBranchEditResponse GetEditDetailMultiBranchConsolidation(ConMultiBranchEditModelRequest request)
        {
            ConMultiBranchEditResponse response = new ConMultiBranchEditResponse();
            Guid userLanguageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();

            try
            {
                var masterID = _masterConAndDeconsolidate_HeaderRepository.FindById(request.MasterID_Guid);
                var status = _masterConAndDeconsolidate_HeaderRepository.GetStatusConByStatusGuid(masterID.SystemCoAndDeSolidateStatus_Guid);
                string branchName = null;
                if (masterID.MasterCustomerLocation_Guid.HasValue)
                {
                    var cuslo = _masterCustomerLocationRepository.FindById(masterID.MasterCustomerLocation_Guid.Value);
                    var customerName = _masterCustomerRepository.FindById(cuslo.MasterCustomer_Guid);
                    branchName = customerName.CustomerFullName + " - " + cuslo.BranchName;
                }

                TblMasterSite destination;
                string routeName = null;
                if (masterID.MasterRouteGroup_Detail_Guid.HasValue)
                {
                    routeName = _masterRouteGroup_DetailRepository.GetRouteName(masterID.MasterRouteGroup_Detail_Guid.GetValueOrDefault(), masterID.MasterDailyRunResource_Guid.GetValueOrDefault());
                }
                destination = _masterSiteRepository.FindById(masterID.Destination_MasterSite_Guid);

                string internalName = null;
                if (masterID.OnwardDestination_Guid.HasValue)
                {
                    internalName = _masterCustomerLocationInternalDepartmentRepository.GetInternalName(masterID.OnwardDestination_Guid.GetValueOrDefault());
                }

                string sitePathName = _masterSitePathHeaderRepository.FindById(masterID.MasterSitePathHeader_Guid).SitePathName;

                response.ConMultiBranchEditModel = new ConsolidateMultiBranchView()
                {
                    MasterID_Guid = masterID.Guid,
                    MasterID = masterID.MasterID,
                    MasterSitePathHeader_Guid = masterID.MasterSitePathHeader_Guid,
                    MasterCustomerLocation_Guid = masterID.MasterCustomerLocation_Guid,
                    LocationName = branchName,
                    MasterRouteGroup_Detail_Guid = masterID.MasterRouteGroup_Detail_Guid,
                    GroupDetailName = routeName,
                    MasterDailyRunResource_Guid = masterID.MasterDailyRunResource_Guid,
                    MasterSite_Guid = masterID.MasterSite_Guid,
                    SystemCoAndDeSolidateStatus_Guid = status.Guid,
                    StatusID = status.StatusID,
                    StatusName = status.StatusName,
                    SitePathName = sitePathName,
                    Destination_MasterSite_Guid = masterID.Destination_MasterSite_Guid,
                    DestinationSiteName = destination.SiteCode + " - " + destination.SiteName,
                    InterDepartmentName = internalName,
                    MinimumSealDigi = Int32.Parse(_systemEnvironment_GlobalRepository.Func_CountryOption_Get(EnumAppKey.MinmumSealDigi.ToString(), request.MasterSite_Guid, null).AppValue1)
                    //DestinationRouteName = routeName
                };

                response.Message = _systemService.GetMessage(0, userLanguageGuid);
                return response;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.Message = _systemService.GetMessage(-186, userLanguageGuid);
                response.Message.IsSuccess = false;
                return response;
            }
        }

        public ConMultiBranchCreateUpdateResponse CreateConsolidate_MultiBranch(ConMultiBranchCreateUpdateRequest request)
        {
            ConMultiBranchCreateUpdateResponse response = new ConMultiBranchCreateUpdateResponse();
            Guid userLanguageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();

            try
            {
                #region Check cannot seal MasterID empty.
                if (!request.ItemSeals.Any() && !request.ItemCommodity.Any())
                {
                    //Cannot seal an empty consolidation, please add items into the consolidation.
                    response.Message = _systemService.GetMessage(-750, userLanguageGuid);
                    response.Message.IsSuccess = false;
                    response.Message.IsWarning = true;
                    return response;
                }
                #endregion

                request.MasterID_Guid = Guid.NewGuid();

                #region Check will cannot have Duplicate MasterID in Same Destination Daily Run
                bool isDupMasterID = _masterConAndDeconsolidate_HeaderRepository.CheckDupMasterID_MultiBranch(request.MasterID, request.MasterID_Guid, request.MasterSite_Guid.GetValueOrDefault(), request.Destination_MasterSite_Guid.GetValueOrDefault(), request.MasterSitePathHeader_Guid.GetValueOrDefault());
                if (isDupMasterID)
                {
                    response.Message = _systemService.GetMessage(-258, userLanguageGuid, new string[] { request.MasterID });
                    response.Message.IsSuccess = false;
                    return response;
                }
                #endregion

                request.WorkDate = request.StrWorkDate.ChangeFromStringToDate(request.FormatDate).GetValueOrDefault();

                Guid? masterIDGuid = request.MasterID_Guid;
                Guid statusGuid = _masterConAndDeconsolidate_HeaderRepository.GetGuidStatusByStatusID(request.FlagSealed == true ? StatusConsolidate.Sealed : StatusConsolidate.Inprocessive);
                bool isConRoute = (request.MasterRouteGroup_Detail_Guid != null);
                bool isConSite = (request.MasterRouteGroup_Detail_Guid == null && request.MasterCustomerLocation_Guid == null);
                string siteName = _masterSiteRepository.FindById(request.MasterSite_Guid).SiteName;
                string destSiteName = _masterSiteRepository.FindById(request.Destination_MasterSite_Guid).SiteName;
                string sitepath = _masterSitePathHeaderRepository.FindById(request.MasterSitePathHeader_Guid).SitePathName;

                #region Consolidate Seals
                if (request.ItemSeals != null && request.ItemSeals.Any())
                {
                    
                    if (isConRoute)
                    {
                        IEnumerable<Guid> sealGuids = request.ItemSeals.Select(s => s.SealGuid);
                        _masterActualJobItemsSealRepository.UpdateSealWithMasterID_Route(sealGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);

                        #region Insert history seal (TFS#36894)
                        IEnumerable<TblMasterHistory_Seal> historyseals = request.ItemSeals.Select(s =>
                        {
                            var departmentName = _masterCustomerLocationInternalDepartmentRepository.FindById(s.CustomerInternalDepartmentGuid)?.InterDepartmentName;
                            departmentName = string.IsNullOrEmpty(departmentName) ? " - " : departmentName;

                            return new TblMasterHistory_Seal()
                            {
                                Guid = Guid.NewGuid(),
                                SealNo = s.SealNo,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = isConRoute ? masterIDGuid : null,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                                MasterID_Route = isConRoute ? request.MasterID : null,
                                MasterID = !isConRoute ? request.MasterID : null,
                                MsgID = 834, //Seal No. {0} was multi-branch consolidated as Master ID: {1} with site path {2} from origin site {3} to destination site {4} at vaulted {5} by Ocean Online.
                                MsgParameter = new string[] { s.SealNo, request.MasterID, sitepath, siteName, destSiteName, departmentName }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_SealRepository.VirtualCreateRange(historyseals);
                        #endregion
                    }
                    else
                    {
                        if (isConSite)
                        {
                            #region ### Insert CON INNER Multi-Branch Route
                            var innerConGuids = request.ItemSeals.Where(e => e.MasterID_Guid != null).Select(e => e.MasterID_Guid.GetValueOrDefault()).ToList(); //it's a master seal items
                            var innerCons = _masterConAndDeconsolidate_HeaderRepository.GetConByGuidList(innerConGuids);

                            foreach (var item in innerCons)
                            {
                                #region MAP SEALS IN INTER-BRANCH ROUTE MASTER_ID
                                var sealsInRouteMaster = _masterActualJobItemsSealRepository.GetItemInCon(item.Guid, true).ToList();

                                IEnumerable<Guid> sealsInnnerConGuids = sealsInRouteMaster.Select(s => s.Guid);
                                _masterActualJobItemsSealRepository.UpdateSealWithMasterID(sealsInnnerConGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);

                                #region ## History Seals
                                IEnumerable<TblMasterHistory_Seal> historySealsInnerCon = sealsInRouteMaster.Select(s =>
                                {
                                    var departmentName = _masterCustomerLocationInternalDepartmentRepository.FindById(s.MasterCustomerLocation_InternalDepartment_Guid)?.InterDepartmentName;
                                    departmentName = string.IsNullOrEmpty(departmentName) ? " - " : departmentName;

                                    return new TblMasterHistory_Seal()
                                    {
                                        Guid = Guid.NewGuid(),
                                        SealNo = s.SealNo,
                                        MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid,
                                        MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                                        MasterID_Route = s.MasterID_Route,
                                        MasterID = !isConRoute ? request.MasterID : null,
                                        MsgID = 834, //Seal No. {0} was multi-branch consolidated as Master ID: {1} with site path {2} from origin site {3} to destination site {4} at vaulted {5} by Ocean Online.
                                        MsgParameter = new string[] { s.SealNo, request.MasterID, sitepath, siteName, destSiteName, departmentName }.ToJSONString(),
                                        UserCreated = request.UserName,
                                        DatetimeCreated = request.ClientDateTime.DateTime,
                                        UniversalDatetimeCreated = request.UniversalDatetime
                                    };
                                });
                                _masterHistory_SealRepository.VirtualCreateRange(historySealsInnerCon);

                                IEnumerable<TblMasterHistory_ActualJob> historyJobsInnerCon = sealsInRouteMaster.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                                {
                                    return new TblMasterHistory_ActualJob()
                                    {
                                        Guid = Guid.NewGuid(),
                                        MasterActualJobHeader_Guid = e,
                                        MsgID = 835, //Seals was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                                        MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                                        UserCreated = request.UserName,
                                        DatetimeCreated = request.ClientDateTime.DateTime,
                                        UniversalDatetimeCreated = request.UniversalDatetime
                                    };
                                });
                                _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobsInnerCon);
                                #endregion ## End History

                                #endregion

                                #region MAP NON-BARCODE IN INTER-BRANCH ROUTE MASTER_ID
                                var commodityInRouteMaster = _masterActualJobItemsCommodityRepository.GetItemInCon(item.Guid, true).ToList();

                                IEnumerable<Guid> commodityGuids = commodityInRouteMaster.Select(s => s.Guid);
                                _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID(commodityGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);

                                IEnumerable<TblMasterHistory_ActualJob> historyJobsInnerConCommodity = commodityInRouteMaster.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                                {
                                    return new TblMasterHistory_ActualJob()
                                    {
                                        Guid = Guid.NewGuid(),
                                        MasterActualJobHeader_Guid = e,
                                        MsgID = 836, //Commodities was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                                        MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                                        UserCreated = request.UserName,
                                        DatetimeCreated = request.ClientDateTime.DateTime,
                                        UniversalDatetimeCreated = request.UniversalDatetime
                                    };
                                });
                                _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobsInnerConCommodity);
                                #endregion

                                //Update Inner Con
                                _masterConAndDeconsolidate_HeaderRepository.UpdateConsolidateHeader(item.Guid, item.SystemCoAndDeSolidateStatus_Guid, request.MasterID_Guid, item.MasterID
                                                                                , request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                            }

                            #region ## History Inner Con Layer
                            IEnumerable<TblMasterHistory_Seal> historyInnerCon = innerCons.Select(s =>
                            {
                                return new TblMasterHistory_Seal()
                                {
                                    Guid = Guid.NewGuid(),
                                    SealNo = s.MasterID,
                                    MsgID = 845, //MasterID_Route {0} has been multi-branch consolidated as Master_ID {1} By Ocean Online.
                                    MsgParameter = new string[] { s.MasterID, request.MasterID }.ToJSONString(),
                                    UserCreated = request.UserName,
                                    DatetimeCreated = request.ClientDateTime.DateTime,
                                    UniversalDatetimeCreated = request.UniversalDatetime
                                };
                            });
                            _masterHistory_SealRepository.VirtualCreateRange(historyInnerCon);

                            #endregion

                            #endregion
                        }

                        #region ### Insert Seal items
                        List<ConSealView> sealsUpdate = request.ItemSeals.Where(w => w.MasterID_Guid == null).ToList();
                        IEnumerable<Guid> sealGuids = sealsUpdate.Select(s => s.SealGuid);
                        _masterActualJobItemsSealRepository.UpdateSealWithMasterID(sealGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                        #endregion ###

                        #region ## History Seal items
                        IEnumerable<TblMasterHistory_Seal> historyseals = sealsUpdate.Select(s =>
                        {
                            var departmentName = _masterCustomerLocationInternalDepartmentRepository.FindById(s.CustomerInternalDepartmentGuid)?.InterDepartmentName;
                            departmentName = string.IsNullOrEmpty(departmentName) ? " - " : departmentName;

                            return new TblMasterHistory_Seal()
                            {
                                Guid = Guid.NewGuid(),
                                SealNo = s.SealNo,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = isConRoute ? masterIDGuid : null,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                                MasterID_Route = isConRoute ? request.MasterID : null,
                                MasterID = !isConRoute ? request.MasterID : null,
                                MsgID = 834, //Seal No. {0} was multi-branch consolidated as Master ID: {1} with site path {2} from origin site {3} to destination site {4} at vaulted {5} by Ocean Online.
                                MsgParameter = new string[] { s.SealNo, request.MasterID, sitepath, siteName, destSiteName, departmentName }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_SealRepository.VirtualCreateRange(historyseals);
                        #endregion
                    }

                    var updateSeal = _masterActualJobItemsSealRepository.FindSeals(request.ItemSeals.Select(e => e.SealGuid)).ToList();
                    IEnumerable<TblMasterHistory_ActualJob> historyJobs = updateSeal.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                    {
                        return new TblMasterHistory_ActualJob()
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = e,
                            MsgID = 835, //Seals was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                            MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                }
                #endregion

                #region Consolidate NonBarcode
                if (request.ItemCommodity != null && request.ItemCommodity.Any())
                {
                    if (isConRoute)
                    {
                        IEnumerable<Guid> commodityGuids = request.ItemCommodity.Select(s => s.CommodityGuid);
                        _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID_Route(commodityGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }
                    else
                    {
                        IEnumerable<Guid> commodityGuids = request.ItemCommodity.Select(s => s.CommodityGuid);
                        _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID(commodityGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }

                    var tblCommodity = _masterActualJobItemsCommodityRepository.FindCommodities(request.ItemCommodity.Select(e => e.CommodityGuid)).ToList();
                    IEnumerable<TblMasterHistory_ActualJob> historyJobs = tblCommodity.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                    {
                        return new TblMasterHistory_ActualJob()
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = e,
                            MsgID = 836, //Commodities was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                            MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                }
                #endregion

                TblMasterConAndDeconsolidate_Header newConHead = CreateConsolidateHeader(request, statusGuid, request.UniversalDatetime);

                response.MasterID_Guid = masterIDGuid.GetValueOrDefault();
                response.StatusName = request.FlagSealed == true ? ConsolidateStatusName.Sealed : ConsolidateStatusName.Inprocess;

                //Msg = 429 : Master_ID {0} was created.
                response.Message = _systemService.GetMessage(429, userLanguageGuid);
                response.Message.IsSuccess = true;
                response.Message.MessageTextContent = string.Format(response.Message.MessageTextContent, request.MasterID);

                using (var transection = _uow.BeginTransaction())
                {
                    _masterHistory_SealRepository.CreateVirtualRangeToDbContext();
                    _masterHistory_ActualJobRepository.CreateVirtualRangeToDbContext();
                    _masterConAndDeconsolidate_HeaderRepository.Create(newConHead);

                    _uow.Commit();
                    transection.Complete();
                }
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.Message = _systemService.GetMessage(-186, userLanguageGuid);
                response.Message.IsSuccess = false;
                return response;
            }

            return response;
        }

        private TblMasterConAndDeconsolidate_Header CreateConsolidateHeader(ConMultiBranchCreateUpdateRequest request, Guid statusGuid, DateTimeOffset universalDateTime)
        {
            #region CREATE MASTER ID
            var newConsolidation = new TblMasterConAndDeconsolidate_Header()
            {
                Guid = request.MasterID_Guid,
                MasterID = request.MasterID,
                SystemCoAndDeSolidateStatus_Guid = statusGuid,
                MasterCustomerLocation_Guid = request.MasterCustomerLocation_Guid,
                MasterRouteGroup_Detail_Guid = request.MasterRouteGroup_Detail_Guid,
                Workdate = request.WorkDate,
                UserCreated = request.UserName,
                DatetimeCreated = request.ClientDateTime.DateTime,
                UniversalDatetimeCreated = universalDateTime,
                SystemConsolidateSourceID = ConsolidateSource.MultiBranch,
                MasterSite_Guid = request.MasterSite_Guid,
                Destination_MasterSite_Guid = request.Destination_MasterSite_Guid,
                MasterDailyRunResource_Guid = request.MasterDailyRunResource_Guid,
                OnwardDestination_Guid = request.OnwardDestination_Guid,
                FlagInPreVault = true,
                FlagMultiBranch = true,
                MasterSitePathHeader_Guid = request.MasterSitePathHeader_Guid
            };

            //Save Log for Sealed Master ID
            var historyMasterSeal = new TblMasterHistory_Seal()
            {
                Guid = Guid.NewGuid(),
                SealNo = request.MasterID,
                MsgID = request.FlagSealed ? 453 : 837, //453: MasterID {0} was sealed by Ocean Online. //837: MasterID {0} was saved by Ocean Online.
                MsgParameter = new string[] { request.MasterID }.ToJSONString(),
                UserCreated = request.UserName,
                DatetimeCreated = request.ClientDateTime.DateTime,
                UniversalDatetimeCreated = request.UniversalDatetime
            };
            _masterHistory_SealRepository.Create(historyMasterSeal);

            return newConsolidation;
            #endregion
        }

        public ConMultiBranchCreateUpdateResponse UpdateConsolidate_MultiBranch(ConMultiBranchCreateUpdateRequest request)
        {
            ConMultiBranchCreateUpdateResponse response = new ConMultiBranchCreateUpdateResponse();
            Guid userLanguageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();

            try
            {
                #region Check cannot seal MasterID empty.
                if (!request.ItemSeals.Any() && !request.ItemCommodity.Any())
                {
                    //Cannot seal an empty consolidation, please add items into the consolidation.
                    response.Message = _systemService.GetMessage(-750, userLanguageGuid);
                    response.Message.IsSuccess = false;
                    return response;
                }
                #endregion

                #region Check will cannot have Duplicate MasterID in Same Destination Daily Run
                //**** ห้ามซีลด้วยชื่อเดิม ถ้ากด unseal มา (request.FlagUnsealed == true)
                bool isDupMasterID = _masterConAndDeconsolidate_HeaderRepository.CheckDupMasterID_MultiBranch(request.MasterID, request.MasterID_Guid, request.MasterSite_Guid.GetValueOrDefault(), request.Destination_MasterSite_Guid.GetValueOrDefault(), request.MasterSitePathHeader_Guid.GetValueOrDefault());
                if (isDupMasterID)
                {
                    response.Message = _systemService.GetMessage(-258, userLanguageGuid, new string[] { request.MasterID });
                    response.Message.IsSuccess = false;
                    return response;
                }
                #endregion

                Guid? masterIDGuid = request.MasterID_Guid;
                bool isConRoute = (request.MasterRouteGroup_Detail_Guid != null);
                bool isConSite = (request.MasterRouteGroup_Detail_Guid == null && request.MasterCustomerLocation_Guid == null);
                string siteName = _masterSiteRepository.FindById(request.MasterSite_Guid).SiteName;
                string destSiteName = _masterSiteRepository.FindById(request.Destination_MasterSite_Guid).SiteName;
                string sitepath = _masterSitePathHeaderRepository.FindById(request.MasterSitePathHeader_Guid)?.SitePathName;

                var updateConsolidate = _masterConAndDeconsolidate_HeaderRepository.FindById(request.MasterID_Guid);
                var old_MasterID = updateConsolidate.MasterID;
                var statusConGuid = _masterConAndDeconsolidate_HeaderRepository.GetGuidStatusByStatusID((request.FlagSealed) ? (int)StatusConsolidate.Sealed : (int)StatusConsolidate.Inprocessive);

                #region Update Route MasterID (Con inner layer)
                if (isConSite)
                {
                    #region Remove Route MasterID
                    var oldMaster = _masterConAndDeconsolidate_HeaderRepository.GetInnerConLayer(request.MasterID_Guid).ToList();
                    var newConGuidList = request.ItemSeals.Where(x => x.MasterID_Guid != null).Select(x => x.MasterID_Guid.GetValueOrDefault()).ToList();
                    var newOrUpdateMaster = _masterConAndDeconsolidate_HeaderRepository.GetConByGuidList(newConGuidList).ToList();
                    if (oldMaster.Any())
                    {
                        var removedMaster = oldMaster.FindAll(e => !newOrUpdateMaster.Contains(e));
                        newOrUpdateMaster = newOrUpdateMaster.FindAll(e => !removedMaster.Contains(e));
                        if (removedMaster != null && removedMaster.Any())
                        {
                            foreach (var item in removedMaster)
                            {
                                #region DELETE SEALS IN MASTER_ID เลขนำ
                                var sealsInRouteMaster = _masterActualJobItemsSealRepository.GetItemInCon(item.Guid, true).ToList();

                                IEnumerable<Guid> sealsInnnerConGuids = sealsInRouteMaster.Select(s => s.Guid);
                                _masterActualJobItemsSealRepository.RemoveSealWithMasterID(sealsInnnerConGuids, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime, 1);

                                IEnumerable<TblMasterHistory_Seal> historyseals = sealsInRouteMaster.Select(s =>
                                {
                                    return new TblMasterHistory_Seal()
                                    {
                                        Guid = Guid.NewGuid(),
                                        SealNo = s.SealNo,
                                        MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid,
                                        MasterConAndDeconsolidateHeaderMasterID_Guid = null,
                                        MasterID_Route = s.MasterID_Route,
                                        MasterID = null,
                                        MsgID = 442, //Seal No. {0} in Master_ID {1} was removed by Ocean Online.
                                        MsgParameter = new string[] { s.SealNo, old_MasterID }.ToJSONString(),
                                        UserCreated = request.UserName,
                                        DatetimeCreated = request.ClientDateTime.DateTime,
                                        UniversalDatetimeCreated = request.UniversalDatetime
                                    };
                                });
                                _masterHistory_SealRepository.VirtualCreateRange(historyseals);
                                #endregion

                                #region DELETE NON-BARCODE IN MASTER_ID เลขนำ
                                var commodityInLocationMaster = _masterActualJobItemsCommodityRepository.GetItemInCon(item.Guid, true);

                                IEnumerable<Guid> comInnnerConGuids = commodityInLocationMaster.Select(s => s.Guid);
                                _masterActualJobItemsCommodityRepository.RemoveCommodityWithMasterID(comInnnerConGuids, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime, 1);

                                IEnumerable<TblMasterHistory_ActualJob> historyJobs = commodityInLocationMaster.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                                {
                                    return new TblMasterHistory_ActualJob()
                                    {
                                        Guid = Guid.NewGuid(),
                                        MasterActualJobHeader_Guid = e,
                                        MsgID = 445, //Commodities in Master ID: {0} was removed by Ocean Online.
                                        MsgParameter = new string[] { old_MasterID }.ToJSONString(),
                                        UserCreated = request.UserName,
                                        DatetimeCreated = request.ClientDateTime.DateTime,
                                        UniversalDatetimeCreated = request.UniversalDatetime
                                    };
                                });
                                _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                                #endregion

                                //Update inner con layer.
                                _masterConAndDeconsolidate_HeaderRepository.UpdateConsolidateHeader(item.Guid, item.SystemCoAndDeSolidateStatus_Guid, null, item.MasterID, request.UserName
                                                                                , request.ClientDateTime.DateTime, request.UniversalDatetime);
                            }

                            #region ## History Inner Con Layer
                            IEnumerable<TblMasterHistory_Seal> historyInnerCon = removedMaster.Select(s =>
                            {
                                return new TblMasterHistory_Seal()
                                {
                                    Guid = Guid.NewGuid(),
                                    SealNo = s.MasterID,
                                    MsgID = 846, //MasterID_Route {0} in MasterID {1} was removed by Ocean Online.
                                    MsgParameter = new string[] { s.MasterID, request.MasterID }.ToJSONString(),
                                    UserCreated = request.UserName,
                                    DatetimeCreated = request.ClientDateTime.DateTime,
                                    UniversalDatetimeCreated = request.UniversalDatetime
                                };
                            });
                            _masterHistory_SealRepository.VirtualCreateRange(historyInnerCon);
                            #endregion
                        }
                    }
                    #endregion

                    //Insert Master Route Multi-Branch
                    #region ### Insert CON INNER Multi-Branch Route
                    foreach (var item in newOrUpdateMaster)
                    {
                        #region MAP SEALS IN INTER-BRANCH ROUTE MASTER_ID
                        var sealsInRouteMaster = _masterActualJobItemsSealRepository.GetItemInCon(item.Guid, true).ToList();

                        IEnumerable<Guid> sealsInnnerConGuids = sealsInRouteMaster.Select(s => s.Guid);
                        _masterActualJobItemsSealRepository.UpdateSealWithMasterID(sealsInnnerConGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);

                        #region ## History Seals
                        IEnumerable<TblMasterHistory_Seal> historySealsInnerCon = sealsInRouteMaster.Select(s =>
                        {
                            var departmentName = _masterCustomerLocationInternalDepartmentRepository.FindById(s.MasterCustomerLocation_InternalDepartment_Guid)?.InterDepartmentName;
                            departmentName = string.IsNullOrEmpty(departmentName) ? " - " : departmentName;

                            return new TblMasterHistory_Seal()
                            {
                                Guid = Guid.NewGuid(),
                                SealNo = s.SealNo,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                                MasterID_Route = s.MasterID_Route,
                                MasterID = !isConRoute ? request.MasterID : null,
                                MsgID = 834, //Seal No. {0} was multi-branch consolidated as Master ID: {1} with site path {2} from origin site {3} to destination site {4} at vaulted {5} by Ocean Online.
                                MsgParameter = new string[] { s.SealNo, request.MasterID, sitepath, siteName, destSiteName, departmentName }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_SealRepository.VirtualCreateRange(historySealsInnerCon);

                        IEnumerable<TblMasterHistory_ActualJob> historyJobsInnerCon = sealsInRouteMaster.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                        {
                            return new TblMasterHistory_ActualJob()
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = e,
                                MsgID = 835, //Seals was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                                MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobsInnerCon);
                        #endregion ## End History

                        #endregion

                        #region MAP NON-BARCODE IN INTER-BRANCH ROUTE MASTER_ID
                        var commodityInRouteMaster = _masterActualJobItemsCommodityRepository.GetItemInCon(item.Guid, true).ToList();

                        IEnumerable<Guid> commodityGuids = commodityInRouteMaster.Select(s => s.Guid);
                        _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID(commodityGuids, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);

                        IEnumerable<TblMasterHistory_ActualJob> historyJobsInnerConCommodity = commodityInRouteMaster.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                        {
                            return new TblMasterHistory_ActualJob()
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = e,
                                MsgID = 836, //Commodities was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                                MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobsInnerConCommodity);
                        #endregion

                        //Update Inner Con
                        _masterConAndDeconsolidate_HeaderRepository.UpdateConsolidateHeader(item.Guid, item.SystemCoAndDeSolidateStatus_Guid, request.MasterID_Guid, item.MasterID
                                                                        , request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }

                    #region ## History Inner Con Layer
                    IEnumerable<TblMasterHistory_Seal> historyInsertCon = newOrUpdateMaster.Select(s =>
                    {
                        return new TblMasterHistory_Seal()
                        {
                            Guid = Guid.NewGuid(),
                            SealNo = s.MasterID,
                            MsgID = 845, //MasterID_Route {0} has been multi-branch consolidated as Master_ID {1} By Ocean Online.
                            MsgParameter = new string[] { s.MasterID, request.MasterID }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_SealRepository.VirtualCreateRange(historyInsertCon);
                    #endregion

                    #endregion ### END Insert CON INNER Multi-Branch Route
                }
                #endregion

                #region Remove Seal and save log
                List<Guid> seals = request.ItemSeals.Where(w => w.MasterID_Guid == null).Select(s => s.SealGuid).ToList();
                var oldSeal = (isConRoute) ? _masterActualJobItemsSealRepository.GetItemInConR(request.MasterID_Guid).ToList() : _masterActualJobItemsSealRepository.GetItemInConL(request.MasterID_Guid).ToList();
                var newOrUpdateSeal = _masterActualJobItemsSealRepository.FindSeals(seals).ToList();
                if (oldSeal != null && oldSeal.Any())
                {
                    var removedSeal = oldSeal.FindAll(e => !newOrUpdateSeal.Contains(e)); //old seal that did not insert
                    newOrUpdateSeal = newOrUpdateSeal.FindAll(e => !removedSeal.Contains(e));

                    if (removedSeal != null && removedSeal.Any())
                    {
                        if (isConRoute)
                        {
                            _masterActualJobItemsSealRepository.RemoveSealWithMasterID_Route(removedSeal.Select(s => s.Guid), request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                        }
                        else
                        {
                            _masterActualJobItemsSealRepository.RemoveSealWithMasterID(removedSeal.Select(s => s.Guid), request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime, 0);
                        }

                        IEnumerable<TblMasterHistory_Seal> historyseals = removedSeal.Select(s =>
                        {
                            return new TblMasterHistory_Seal()
                            {
                                Guid = Guid.NewGuid(),
                                SealNo = s.SealNo,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = isConRoute ? masterIDGuid : null,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                                MasterID_Route = isConRoute ? request.MasterID : null,
                                MasterID = !isConRoute ? request.MasterID : null,
                                MsgID = 442, //Seal No. {0} in Master_ID {1} was removed by Ocean Online.
                                MsgParameter = new string[] { s.SealNo, old_MasterID }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_SealRepository.VirtualCreateRange(historyseals);

                        IEnumerable<TblMasterHistory_ActualJob> historyJobs = removedSeal.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                        {
                            return new TblMasterHistory_ActualJob()
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = e,
                                MsgID = 441, //Seals in Master_ID: {0} was unsealed by Ocean Online.
                                MsgParameter = new string[] { old_MasterID }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                    }
                }
                #endregion

                #region Remove Commodity and save log
                List<Guid> commodities = request.ItemCommodity.Select(s => s.CommodityGuid).ToList();
                var oldCommodity = (isConRoute) ? _masterActualJobItemsCommodityRepository.GetItemInConR(request.MasterID_Guid).ToList() : _masterActualJobItemsCommodityRepository.GetItemInConL(request.MasterID_Guid).ToList();
                var newOrUpdateCommodity = _masterActualJobItemsCommodityRepository.FindCommodities(commodities).ToList();

                if (oldCommodity != null && !oldCommodity.IsEmpty())
                {
                    var removedCommodity = oldCommodity.FindAll(e => !newOrUpdateCommodity.Contains(e)); //old commodity that did not insert
                    newOrUpdateCommodity = newOrUpdateCommodity.FindAll(e => !removedCommodity.Contains(e));
                    if (removedCommodity != null && removedCommodity.Any())
                    {
                        if (isConRoute)
                        {
                            _masterActualJobItemsCommodityRepository.RemoveCommodityWithMasterID_Route(removedCommodity.Select(s => s.Guid), request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                        }
                        else
                        {
                            _masterActualJobItemsCommodityRepository.RemoveCommodityWithMasterID(removedCommodity.Select(s => s.Guid), request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                        }

                        IEnumerable<TblMasterHistory_ActualJob> historyJobs = removedCommodity.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                        {
                            return new TblMasterHistory_ActualJob()
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = e,
                                MsgID = 445, //Commodities in Master ID: {0} was removed by Ocean Online.
                                MsgParameter = new string[] { old_MasterID }.ToJSONString(),
                                UserCreated = request.UserName,
                                DatetimeCreated = request.ClientDateTime.DateTime,
                                UniversalDatetimeCreated = request.UniversalDatetime
                            };
                        });
                        _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                    }
                }
                #endregion

                //Insert Seals
                #region Consolidate Seal
                if (newOrUpdateSeal != null && newOrUpdateSeal.Any())
                {
                    var updateSeal = newOrUpdateSeal.Select(s => s.Guid);

                    if (isConRoute)
                    {
                        _masterActualJobItemsSealRepository.UpdateSealWithMasterID_Route(updateSeal, request.MasterID_Guid, request.MasterID,
                                                            request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }
                    else
                    {
                        _masterActualJobItemsSealRepository.UpdateSealWithMasterID(updateSeal, request.MasterID_Guid, request.MasterID,
                                                            request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }

                    IEnumerable<TblMasterHistory_Seal> historyseals = newOrUpdateSeal.Select(s =>
                    {
                        var departmentName = _masterCustomerLocationInternalDepartmentRepository.FindById(s.MasterCustomerLocation_InternalDepartment_Guid)?.InterDepartmentName;
                        departmentName = string.IsNullOrEmpty(departmentName) ? " - " : departmentName;

                        return new TblMasterHistory_Seal()
                        {
                            Guid = Guid.NewGuid(),
                            SealNo = s.SealNo,
                            MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = isConRoute ? masterIDGuid : null,
                            MasterConAndDeconsolidateHeaderMasterID_Guid = !isConRoute ? masterIDGuid : null,
                            MasterID_Route = isConRoute ? request.MasterID : null,
                            MasterID = !isConRoute ? request.MasterID : null,
                            MsgID = 834, //Seal No. {0} was multi-branch consolidated as Master ID: {1} with site path {2} from origin site {3} to destination site {4} at vaulted {5} by Ocean Online.
                            MsgParameter = new string[] { s.SealNo, request.MasterID, sitepath, siteName, destSiteName, departmentName }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_SealRepository.VirtualCreateRange(historyseals);

                    IEnumerable<TblMasterHistory_ActualJob> historyJobs = newOrUpdateSeal.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                    {
                        return new TblMasterHistory_ActualJob()
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = e,
                            MsgID = 835, //Seals was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                            MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                }
                #endregion

                //Insert NonBarcode
                #region Consolidate Commodities
                if (newOrUpdateCommodity != null && newOrUpdateCommodity.Any())
                {
                    var updateCommodity = newOrUpdateCommodity.Select(s => s.Guid);

                    if (isConRoute)
                    {
                        _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID_Route(updateCommodity, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }
                    else
                    {
                        _masterActualJobItemsCommodityRepository.UpdateCommodityWithMasterID(updateCommodity, request.MasterID_Guid, request.MasterID, request.UserName, request.ClientDateTime.DateTime, request.UniversalDatetime);
                    }

                    IEnumerable<TblMasterHistory_ActualJob> historyJobs = newOrUpdateCommodity.Select(s => s.MasterActualJobHeader_Guid).Distinct().Select(e =>
                    {
                        return new TblMasterHistory_ActualJob()
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = e,
                            MsgID = 836, //Commodities was multi-branch consolidated as Master_ID: {0} in Site Path {1} by Ocean Online.
                            MsgParameter = new string[] { request.MasterID, sitepath }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                    });
                    _masterHistory_ActualJobRepository.VirtualCreateRange(historyJobs);
                }
                #endregion

                #region Update Master_ID
                _masterConAndDeconsolidate_HeaderRepository.UpdateConsolidateHeader(request.MasterID_Guid, statusConGuid, null, request.MasterID, request.UserName
                                                                                            , request.ClientDateTime.DateTime, request.UniversalDatetime);

                if (request.FlagUnsealed)
                {
                    var historySeal = new TblMasterHistory_Seal()
                    {
                        Guid = Guid.NewGuid(),
                        SealNo = old_MasterID,
                        MsgID = 438, //Master ID: {0} was unsealed because {1}, remark: {2} by Ocean Online.
                        MsgParameter = new string[] { old_MasterID, request.reasonName, request.remark }.ToJSONString(),
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    };
                    _masterHistory_SealRepository.VirtualCreate(historySeal);

                    //Save Log for Renaming Master ID
                    var historyMasterSeal = new TblMasterHistory_Seal()
                    {
                        Guid = Guid.NewGuid(),
                        SealNo = request.MasterID,
                        MsgID = 443, //Master ID {0} was updated as {1} by Ocean Online.
                        MsgParameter = new string[] { old_MasterID, request.MasterID }.ToJSONString(),
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    };
                    _masterHistory_SealRepository.VirtualCreate(historyMasterSeal);
                }
                else
                {
                    if (request.FlagSealed)
                    {
                        //Save Log for Sealed Master ID
                        var historyMasterSeal = new TblMasterHistory_Seal()
                        {
                            Guid = Guid.NewGuid(),
                            SealNo = request.MasterID,
                            MsgID = 453, //MasterID {0} was sealed by Ocean Online.
                            MsgParameter = new string[] { request.MasterID }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                        _masterHistory_SealRepository.VirtualCreate(historyMasterSeal);
                    }
                    else
                    {
                        //Save Log for Renaming Master ID
                        var historyMasterSeal = new TblMasterHistory_Seal()
                        {
                            Guid = Guid.NewGuid(),
                            SealNo = request.MasterID,
                            MsgID = 443, //Master ID {0} was updated as {1} by Ocean Online.
                            MsgParameter = new string[] { old_MasterID, request.MasterID }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                        _masterHistory_SealRepository.VirtualCreate(historyMasterSeal);
                    }
                }
                #endregion

                response.MasterID_Guid = request.MasterID_Guid;
                response.StatusName = request.FlagSealed ? ConsolidateStatusName.Sealed : ConsolidateStatusName.Inprocess;

                response.Message = _systemService.GetMessage(434, userLanguageGuid);
                response.Message.MessageTextContent = string.Format(response.Message.MessageTextContent, request.MasterID);
                response.Message.IsSuccess = true;

                using (var transection = _uow.BeginTransaction())
                {
                    _masterHistory_SealRepository.CreateVirtualRangeToDbContext();
                    _masterHistory_ActualJobRepository.CreateVirtualRangeToDbContext();

                    _uow.Commit();
                    transection.Complete();
                }
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.Message = _systemService.GetMessage(-186, userLanguageGuid);
                response.Message.IsSuccess = false;
            }
            return response;
        }

        private SummayItemConsolidateView GetSummaryDetail(List<ConsolidateItemView> itemAvailableSeal, bool flagGetEditData, bool flagSealList)
        {
            SummayItemConsolidateView dataSummary = new SummayItemConsolidateView();
            if (!flagGetEditData)
            {
                #region Get Summary for NEW Available Items
                IEnumerable<SummaryCommodityView> commoditySum = itemAvailableSeal.Where(o => !String.IsNullOrEmpty(o.Commodity) && o.GroupScanName.Equals(FixStringPrevault.Non_Barcode))
                                        .GroupBy(o => o.Commodity).Select(o => new SummaryCommodityView
                                        {
                                            //FlagDisable = false,
                                            ItemsSummary = o.First().Commodity,
                                            GroupScanName = o.First().GroupScanName,
                                            Total = o.Select(x => x.Qty ?? 0).Sum()
                                        });

                IEnumerable<SummarySealView> LiabilitySum = itemAvailableSeal.Where(o => o.Liability != null && !string.IsNullOrEmpty(o.CurrencyNameAbb))
                                        .GroupBy(o => o.CurrencyNameAbb)
                                        .Select(o => new SummarySealView()
                                        {
                                            //FlagDisable = false,
                                            ItemsSummary = o.First().CurrencyNameAbb,
                                            GroupScanName = o.First().GroupScanName,
                                            Total = o.Select(x => x.Liability ?? 0.00).Sum()
                                        });

                dataSummary.AvailableSummarySeal = LiabilitySum;
                dataSummary.AvailableSummaryCommodity = commoditySum;
                #endregion
            }
            else
            {
                #region Get Summary Detail for EDIT Page
                if (flagSealList == true)
                {
                    List<ConsolidateItemView> LiabilitySum = itemAvailableSeal.Where(o => o.Liability != null && !string.IsNullOrEmpty(o.CurrencyNameAbb)).ToList();
                    dataSummary.ConsolidatedSummarySeal = LiabilitySum.Where(e => e.FlagCheckEdit == true).GroupBy(o => o.CurrencyNameAbb)
                        .Select(o => new ConsolidatedSummarySealView
                        {
                            ItemsSummary = o.First().CurrencyNameAbb,
                            GroupScanName = o.First().GroupScanName,
                            FlagCheckEdit = true,
                            Total = o.Select(x => x.Liability ?? 0.00).Sum()
                        });

                    dataSummary.AvailableSummarySeal = LiabilitySum.Where(e => e.FlagCheckEdit == false).GroupBy(o => o.CurrencyNameAbb)
                        .Select(o => new SummarySealView
                        {
                            ItemsSummary = o.First().CurrencyNameAbb,
                            GroupScanName = o.First().GroupScanName,
                            FlagCheckEdit = false,
                            Total = o.Select(x => x.Liability ?? 0.00).Sum()
                        });
                }
                else
                {
                    List<ConsolidateItemView> commoditySum = itemAvailableSeal.Where(o => !String.IsNullOrEmpty(o.Commodity) && o.GroupScanName.Equals(FixStringPrevault.Non_Barcode)).ToList();

                    dataSummary.ConsolidatedSummaryCommodity = commoditySum.Where(e => e.FlagCheckEdit == true).GroupBy(o => o.Commodity).Select(o => new ConsolidatedSummaryCommodityView
                    {
                        ItemsSummary = o.First().Commodity,
                        GroupScanName = o.First().GroupScanName,
                        FlagCheckEdit = true,
                        Total = o.Select(x => x.Qty ?? 0).Sum()
                    });

                    dataSummary.AvailableSummaryCommodity = commoditySum.Where(e => e.FlagCheckEdit == false).GroupBy(o => o.Commodity).Select(o => new SummaryCommodityView
                    {
                        ItemsSummary = o.First().Commodity,
                        GroupScanName = o.First().GroupScanName,
                        FlagCheckEdit = false,
                        Total = o.Select(x => x.Qty ?? 0).Sum()
                    });
                }
                #endregion
            }
            return dataSummary;
        }

        public ConGetItemResponse GetItemConMultiBranch_New(ItemAvailableConRequest request)
        {
            ConGetItemResponse response = new ConGetItemResponse();
            Dictionary<string, ConsolidateAllItemView> data = new Dictionary<string, ConsolidateAllItemView>();

            try
            {
                var items = _masterConAndDeconsolidate_HeaderRepository.GetItemCanCon(request.WorkDate, request.SiteGuid, request.DestinationSiteGuid.GetValueOrDefault(), request.SitePathGuid.GetValueOrDefault()).ConvertToItemView();

                ConsolidateAllItemView mainSeal = new ConsolidateAllItemView();
                ConsolidateAllItemView mainCommodity = new ConsolidateAllItemView();

                mainSeal.SealItems = items.Where(e => e.GroupScanName == FixStringPrevault.Seal);
                mainCommodity.CommodityItems = items.Where(e => e.GroupScanName == FixStringPrevault.Non_Barcode);

                // CALCULATE Summary Data
                if (items.Any() && items != null)
                {
                    var summary = GetSummaryDetail(items.ToList(), false, false);
                    mainSeal.AvailableSummarySeal = summary.AvailableSummarySeal.Where(e => e.GroupScanName == FixStringPrevault.Seal);
                    mainCommodity.AvailableSummaryCommodity = summary.AvailableSummaryCommodity.Where(e => e.GroupScanName == FixStringPrevault.Non_Barcode);
                }

                data.Add(FixStringPrevault.Seal, mainSeal);
                data.Add(FixStringPrevault.NonBarcode, mainCommodity);

                var destRouteDDL = items.Where(e => e.RouteName != null)
                    .GroupBy(e => new { e.MasterRouteGroupDetail_Guid, e.RouteName, e.DailyRunGuid })
                    .Select(x => new DestRouteDropdownResponse() { MasterRouteGroupDetail_Guid = x.Key.MasterRouteGroupDetail_Guid, DestinationRoute = x.Key.RouteName, MasterDailyRunResource_Guid = x.Key.DailyRunGuid });

                response.Items = data;
                response.DestinationRoute = destRouteDDL;
            }

            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return response;
        }

        public Dictionary<string, ConsolidateAllItemView> GetItemConMultiBranch_Edit(ItemAvailableConRequest request)
        {
            Dictionary<string, ConsolidateAllItemView> response = new Dictionary<string, ConsolidateAllItemView>();

            try
            {
                var items = _masterConAndDeconsolidate_HeaderRepository.GetItemEditCon(request.WorkDate, request.SiteGuid, request.DestinationSiteGuid.GetValueOrDefault(), request.SitePathGuid.GetValueOrDefault(), request.MasterID_Guid).ConvertToItemView();

                ConsolidateAllItemView mainSeal = new ConsolidateAllItemView();
                ConsolidateAllItemView mainCommodity = new ConsolidateAllItemView();

                mainSeal.SealItems = items.Where(e => e.GroupScanName == FixStringPrevault.Seal);
                mainCommodity.CommodityItems = items.Where(e => e.GroupScanName == FixStringPrevault.Non_Barcode);

                //Add summary
                var summarySealData = GetSummaryDetail(mainSeal.SealItems.ToList(), true, true);
                var summaryCommodityData = GetSummaryDetail(mainCommodity.CommodityItems.ToList(), true, false);
                mainSeal.AvailableSummarySeal = summarySealData.AvailableSummarySeal;
                mainSeal.ConsolidatedSummarySeal = summarySealData.ConsolidatedSummarySeal;
                mainCommodity.AvailableSummaryCommodity = summaryCommodityData.AvailableSummaryCommodity;
                mainCommodity.ConsolidatedSummaryCommodity = summaryCommodityData.ConsolidatedSummaryCommodity;

                response.Add(FixStringPrevault.Seal, mainSeal);
                response.Add(FixStringPrevault.NonBarcode, mainCommodity);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return response;
        }

        public IEnumerable<DropdownSitePathView> GetSitePathHaveItemDDL(SitePathDDLRequest request)
        {
            return _masterSitePathHeaderRepository.GetSitePathHaveItem(request.SiteGuid, request.DestinationSiteGuid, request.WorkDate);
        }

        #endregion
    }

}

