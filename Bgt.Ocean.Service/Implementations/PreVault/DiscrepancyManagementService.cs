using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Domain;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.PreVault.DiscrepancyManagement;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.PreVault
{
    public interface IDiscrepancyManagementService
    {
        IEnumerable<DiscrepancyItemsResponse> GetDiscrepancyVaultBalanceItems(Guid prevaultGuid);
        IEnumerable<DiscrepancyItemsResponse> GetDiscrepancyCheckInProcessItems(Guid siteGuid, Guid prevaultGuid);
        SystemMessageView ClosecaseVaultBalance(DiscrepancyCloseCaseModel request);
    }

    public class DiscrepancyManagementService : IDiscrepancyManagementService
    {
        private readonly IBaseRequest _baseRequest;
        private readonly ISystemFormatDateRepository _systemFormatDateRepository;
        private readonly IVaultBalanceDiscrepancyRepository _vaultBalanceDiscrepancyRepository;
        private readonly IMasterActualJobItemUnknowRepository _masterActualJobItemUnknowRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
		private readonly IMasterConAndDeconsolidate_HeaderRepository _masterConAndDeconsolidate_HeaderRepository;
		private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
		private readonly IMasterHistoryActualJobRepository _masterHistory_ActualJob;
		private readonly IMasterHistory_SealRepository _masterHistory_SealRepository;
		private readonly IDnsWrapper _dnsWrapper;
		private readonly IUnitOfWork<OceanDbEntities> _uow;
		private readonly ISystemService _systemService;


		#region ### DEPENDENCY INJECTION ###
		public DiscrepancyManagementService(IBaseRequest baseRequest,
                                            ISystemFormatDateRepository systemFormatDateRepository,
                                            IVaultBalanceDiscrepancyRepository vaultBalanceDiscrepancyRepository,
                                            IMasterActualJobItemUnknowRepository masterActualJobItemUnknowRepository,
                                            IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository,
											IMasterConAndDeconsolidate_HeaderRepository masterConAndDeconsolidate_Header,
											IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                                            IMasterSiteRepository masterSiteRepository,
											IMasterHistoryActualJobRepository masterHistory_ActualJob,
											IMasterHistory_SealRepository masterHistory_SealRepository,
											IDnsWrapper dnsWrapper,
											IUnitOfWork<OceanDbEntities> uow,
											ISystemService systemService)
        {
            _baseRequest = baseRequest;
            _systemFormatDateRepository = systemFormatDateRepository;
            _vaultBalanceDiscrepancyRepository = vaultBalanceDiscrepancyRepository;
            _masterActualJobItemUnknowRepository = masterActualJobItemUnknowRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
			_masterConAndDeconsolidate_HeaderRepository = masterConAndDeconsolidate_Header;
			_masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterSiteRepository = masterSiteRepository;
			_masterHistory_ActualJob = masterHistory_ActualJob;
			_masterHistory_SealRepository = masterHistory_SealRepository;
			_dnsWrapper = dnsWrapper;
			_uow = uow;
			_systemService = systemService;
		}
        #endregion

        #region ### Discrepancy Vault Balance ###
        public IEnumerable<DiscrepancyItemsResponse> GetDiscrepancyVaultBalanceItems(Guid prevaultGuid)
        {
            using (var context = new OceanDbEntities())
            {
                int sqlFormatCode = _systemFormatDateRepository.FindByUserFormatDate(_baseRequest.Data.UserFormatDate);
                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@prevault_Guid", prevaultGuid));
                sqlParam.Add(new SqlParameter("@IntDateFormatSQL", sqlFormatCode));
                return context.Database.SqlQuery<DiscrepancyItemsResponse>(new SqlCommand(GetDiscrepancyItemsOfVaultBalanceQuery).CommandText, sqlParam.ToArray()).ToList();
            }
        }

        public string GetDiscrepancyItemsOfVaultBalanceQuery
        {
            get
            {
                return @"
                   SELECT 
					vdis.Guid
                    ,vdis.MasterActualJobHeader_Guid AS JobGuid
                    ,ISNULL(header.JobNo,'') as JobNo
                    ,cusP.CustomerFullName + ' - ' + locP.BranchName AS PU_Location
                    ,cusD.CustomerFullName + ' - ' + locD.BranchName AS DL_Location
                    ,(CASE WHEN (vdis.MasterConAndDeconsolidateHeader_Guid IS NULL) THEN 'Seal No. ' + vdis.SealNo ELSE vdis.SealNo END) AS SealNo
                    ,vdis.QtyShortage
                    ,vdis.QtyOverage
                    ,reason.ReasonTypeName
                    ,vdis.Remarks
                    ,vdis.UsernameSupervisorVerify
                    ,CONVERT(nvarchar(100),vdis.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),vdis.DatetimeSupervisorVerify,108)  AS DatetimeVerify
                    ,vdis.DatetimeSupervisorVerify
                    ,NULL AS MasterRunResourceDaily_Guid
                    ,NULL AS MasterCustomerLocation_InternalDepartment_Guid
                    ,vdis.ClientHostNameScan
                    ,NULL AS MasterCommodity_Guid
                    ,NULL AS FlagNonDelivery
                    ,vdis.MasterCustomerLocation_Pickup_Guid AS CusLocPGuid
                    ,NULL AS SitePickUpGuid
                    ,vdis.MasterCustomerLocation_Delivery_Guid AS CusLocDGuid
                    ,NULL AS SiteDeliveryGuid
                    ,NULL AS FlagItemPartial
                    ,NULL AS FlagNotAllowReturn
                    ,'' AS ItemName
                    FROM TblVaultBalance_Discrepancy vdis
                    LEFT JOIN TblMasterActualJobHeader header ON vdis.MasterActualJobHeader_Guid = header.Guid
                    INNER JOIN TblMasterReasonType reason ON reason.Guid = vdis.MasterReasonType_Guid
                    LEFT JOIN TblMasterCustomer cusP ON cusP.Guid= vdis.MasterCustomer_Pickup_Guid
                    LEFT JOIN tblMasterCustomer cusD ON cusD.Guid= vdis.MasterCustomer_Delivery_Guid
                    LEFT JOIN TblMasterCustomerLocation locP ON locP.Guid = vdis.MasterCustomerLocation_Pickup_Guid
                    LEFT JOIN TblMasterCustomerLocation locD ON locD.Guid = vdis.MasterCustomerLocation_Delivery_Guid
                    WHERE MasterCustomerLocation_InternalDepartment_Guid = @prevault_Guid
						  AND vdis.FlagCloseCase = 0
                          AND vdis.FlagTempDiscrepancy = 0
						  AND vdis.MasterActualJobItemsCommodity_Guid IS NULL		
						  AND vdis.MasterCommodity_Guid IS NULL
                    ORDER BY vdis.DatetimeSupervisorVerify DESC";
            }
        }

        public SystemMessageView ClosecaseVaultBalance(DiscrepancyCloseCaseModel request)
        {
            SystemMessageView msg = new SystemMessageView();
			var LangGuid = ApiSession.UserLanguage_Guid.HasValue ? ApiSession.UserLanguage_Guid.Value : new Guid("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
			msg = _systemService.GetMessage(0, LangGuid);
			msg.IsSuccess = true;
            msg.MsgID = 0;
            List<TblMasterActualJobItemUnknow> ListUnknowSeal = new List<TblMasterActualJobItemUnknow>();
            List<Guid> ListSealGuidforDelete = new List<Guid>();

            List<TblMasterHistory_ActualJob> logHistory_Job = new List<TblMasterHistory_ActualJob>();
            List<TblMasterHistory_Seal> logHistory_Seal = new List<TblMasterHistory_Seal>();

			
			var getHostName = _dnsWrapper.ClientHostName;
            try
            {

				using (var context = _uow.BeginTransaction())
				{
					//update flagClosecase in tblvaultbalance Dis             
					var getdataDis = _vaultBalanceDiscrepancyRepository.FindAllAsQueryable(e => request.DiscrepancyGuidList.Contains(e.Guid));
					
					foreach (var itemData in getdataDis)
                    {
						string masterID = null;
						Guid? masterGuid = new Guid?();
						string Route = null;
						Guid? RouteGuid = new Guid?();
						itemData.FlagCloseCase = true;
						itemData.ReasonClosedCase = request.Reason;
						var messageParameter = _vaultBalanceDiscrepancyRepository.getdateforLogClosecase(itemData.Guid);
						var messageLog = messageParameter.SealNo + "," + messageParameter.JobNo + "," + messageParameter.SiteName + "," + messageParameter.BranchName + "," + messageParameter.InterDepartmentName;

						int messageID = 0;
                        if (itemData.QtyOverage != 0) // Over
                        {
                            // Add to unknow seal                        
                            TblMasterActualJobItemUnknow UnknowSeal = new TblMasterActualJobItemUnknow()
                            {
                                Guid = Guid.NewGuid(),
                                MasterRunResourceDaily_Guid = null,
                                SealNo = itemData.SealNo,
                                MasterCommodity_Guid = itemData.MasterCommodity_Guid,
                                Quantity = itemData.QtyOverage,
                                FlagMatchDone = false,
                                ClientHostNameScan = getHostName,
                                UserCreated = ApiSession.UserName,
                                DatetimeCreated = ApiSession.ClientDatetime.DateTime,
                                UniversalDatetimeCreated = DateTime.UtcNow,
                                MasterCustomerLocation_InternalDepartment_Guid = itemData.MasterCustomerLocation_InternalDepartment_Guid,
                                Remarks = itemData.Remarks,
                            };
                            ListUnknowSeal.Add(UnknowSeal);

                            messageID = 6202;

                        }
                        else if (itemData.QtyShortage != 0) // Short
                        {
                            //check shot over
                            if (!request.IsFoundItem) // not found
                            {
								//delete seal from jobseal
								if (itemData.MasterActualJobItemsSeal_Guid.HasValue)
								{
									ListSealGuidforDelete.Add(itemData.MasterActualJobItemsSeal_Guid.Value);
									messageID = 6201;
								}
                            }
                            else // found
                            {
                                messageID = 6200;
                            }

							if (itemData.MasterConAndDeconsolidateHeader_Guid.HasValue)
                            {
								var getdatacon = _masterConAndDeconsolidate_HeaderRepository.FindById(itemData.MasterConAndDeconsolidateHeader_Guid);

								// check Con route
								if (!getdatacon.ConsolidationRoute_Guid.HasValue && !getdatacon.MasterCustomerLocation_Guid.HasValue && getdatacon.MasterRouteGroup_Detail_Guid.HasValue)
								{
									RouteGuid = itemData.MasterConAndDeconsolidateHeader_Guid;
									Route = getdatacon.MasterID;
								}

								// check loc master
								if (!getdatacon.ConsolidationRoute_Guid.HasValue && !getdatacon.MasterRouteGroup_Detail_Guid.HasValue)
								{
									masterGuid = itemData.MasterConAndDeconsolidateHeader_Guid;
									masterID = getdatacon.MasterID;
								}

								messageID = 6230;
								messageLog = messageParameter.SealNo + "," + messageParameter.SiteName + "," + messageParameter.BranchName + "," + messageParameter.InterDepartmentName;
							}

						}					

						// add History log
						if (itemData.MasterActualJobHeader_Guid.HasValue)
                        {
							TblMasterHistory_ActualJob joblog = new TblMasterHistory_ActualJob()
							{
								Guid = Guid.NewGuid(),
								MasterActualJobHeader_Guid = itemData.MasterActualJobHeader_Guid,
								MsgID = messageID,
								MsgParameter = messageLog,
								UserCreated = ApiSession.UserName,
								DatetimeCreated = ApiSession.ClientDatetime.DateTime,
								UniversalDatetimeCreated = DateTime.UtcNow,
								FlagIsStaging = false
							};
							logHistory_Job.Add(joblog);
						}

						if (messageID == 6230)
                        {							
							TblMasterHistory_Seal seallog = new TblMasterHistory_Seal()
							{
								Guid = Guid.NewGuid(),
								MasterActualJobItemsSeal_Guid = itemData.MasterActualJobItemsSeal_Guid,
								Site_Guid = itemData.MasterSite_Guid,
								SealNo = itemData.SealNo,
								MsgID = 6230,
								MsgParameter = messageLog,
								UserCreated = ApiSession.UserName,
								DatetimeCreated = ApiSession.ClientDatetime.DateTime,
								UniversalDatetimeCreated = DateTime.UtcNow,
								MasterConAndDeconsolidateHeaderMasterID_Guid = masterGuid,
								MasterID = masterID,
								MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = RouteGuid,
								MasterID_Route = Route,
							};
							logHistory_Seal.Add(seallog);
						} else if (itemData.MasterActualJobItemsSeal_Guid.HasValue)
						{
							TblMasterHistory_Seal seallog = new TblMasterHistory_Seal()
							{								
								Guid = Guid.NewGuid(),
								MasterActualJobItemsSeal_Guid = itemData.MasterActualJobItemsSeal_Guid,
								Site_Guid = itemData.MasterSite_Guid,
								SealNo = itemData.SealNo,
								MsgID = messageID,
								MsgParameter = messageLog,
								UserCreated = ApiSession.UserName,
								DatetimeCreated = ApiSession.ClientDatetime.DateTime,
								UniversalDatetimeCreated = DateTime.UtcNow ,
								MasterConAndDeconsolidateHeaderMasterID_Guid = masterGuid,
								MasterID = masterID,
								MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = RouteGuid,
								MasterID_Route = Route,
							};
							logHistory_Seal.Add(seallog);
						}

                        _vaultBalanceDiscrepancyRepository.Modify(itemData);
                    }


                    try
                    {
                        // add unknow seal
                        if (ListUnknowSeal.Any()) _masterActualJobItemUnknowRepository.CreateRange(ListUnknowSeal);

                        // delete seal from job seal
                        if (ListSealGuidforDelete.Any()) _masterActualJobItemsSealRepository.RemoveRange(_masterActualJobItemsSealRepository.FindAllAsQueryable(e => ListSealGuidforDelete.Contains(e.Guid)));

                        // add Job’s history log.
                        if (logHistory_Job.Any()) _masterHistory_ActualJob.CreateRange(logHistory_Job);

                        // add tracking seal history
                        if (logHistory_Seal.Any()) _masterHistory_SealRepository.CreateRange(logHistory_Seal);

						_uow.Commit();
						context.Complete();
					}
                    catch (Exception ex)
                    {						
						msg = _systemService.GetMessage(-184, LangGuid);
						msg.IsSuccess = false;
                        msg.MessageTextContent = ex.Message;
                        msg.MessageTextTitle = "Error";
                        msg.MsgID = -184;						
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
				msg = _systemService.GetMessage(-184, LangGuid);
				msg.IsSuccess = false;
                msg.MessageTextContent = ex.Message;
                msg.MessageTextTitle = "Error";
                msg.MsgID = -184;
                return msg;
            }

        }
        #endregion

        #region ### Check in Process ###
        public IEnumerable<DiscrepancyItemsResponse> GetDiscrepancyCheckInProcessItems(Guid siteGuid, Guid prevaultGuid)
        {
            using (var context = new OceanDbEntities())
            {
                int sqlFormatCode = _systemFormatDateRepository.FindByUserFormatDate(_baseRequest.Data.UserFormatDate);
                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("@InternalDepartmentGuid", prevaultGuid));
                sqlParam.Add(new SqlParameter("@SiteGuid", siteGuid));
                sqlParam.Add(new SqlParameter("@IntDateFormatSQL", sqlFormatCode));
                return context.Database.SqlQuery<DiscrepancyItemsResponse>(new SqlCommand(GetDiscrepancyItemsCheckInProcess).CommandText, sqlParam.ToArray()).ToList();
            }
        }
        public string GetDiscrepancyItemsCheckInProcess
        {
            get
            {
                return @"
                    IF OBJECT_ID('tempdb..#TmpJobDiscrepancies') IS NOT NULL
						BEGIN
							DROP TABLE #TmpJobDiscrepancies
						END

					CREATE TABLE #TmpJobDiscrepancies(  JobGuid					uniqueidentifier,
														JobNo					nvarchar(200)		collate database_default,
														JobTypeID				Int,
														FlagNonDelivery			bit,
														TransectionDate			datetime,
														CusLocPGuid				uniqueidentifier,
														SitePickUpGuid			uniqueidentifier,
														PU_Location				nvarchar(500)		collate database_default,
														CusLocDGuid				uniqueidentifier,
														SiteDeliveryGuid		uniqueidentifier,
														DL_Location				nvarchar(500)		collate database_default,
														FlagMultiBranch			bit
														)
					-- ###  ====  Temp Job Discrepancies  =====  ###
					INSERT INTO #TmpJobDiscrepancies
					SELECT      Job.Guid as JobGuid,
							    Job.JobNo,
								jt.ServiceJobTypeID as JobTypeID,
								Job.FlagNonDelivery,
								Job.TransectionDate,
								LegsP.MasterCustomerLocation_Guid as CusLocPGuid,
								LegsP.MasterSite_Guid as SitePickUpGuid,
								cusP.CustomerFullName +' - '+ locP.BranchName as PU_Location,
								LegsD.MasterCustomerLocation_Guid as CusLocDGuid,
								LegsD.MasterSite_Guid as SiteDeliveryGuid,
								cusD.CustomerFullName +' - '+ locD.BranchName as DL_Location,
								Job.FlagMultiBranch
					FROM        TblMasterActualJobHeader Job
								INNER JOIN TblSystemServiceJobType jt ON jt.Guid = Job.SystemServiceJobType_Guid
								INNER JOIN TblMasterActualJobServiceStopLegs LegsP ON LegsP.MasterActualJobHeader_Guid = (CASE WHEN Job.JobBCP_Ref_Guid IS NOT NULL 
																								  THEN Job.JobBCP_Ref_Guid
																								  ELSE Job.Guid END)
																					AND LegsP.SequenceStop = 1
								LEFT JOIN TblMasterCustomerLocation locP ON LegsP.MasterCustomerLocation_Guid = locP.Guid 
								INNER JOIN TblMasterCustomer cusP ON locP.MasterCustomer_Guid = cusP.Guid
					
								INNER JOIN TblMasterActualJobServiceStopLegs LegsD ON Job.Guid = LegsD.MasterActualJobHeader_Guid 
																			        AND LegsD.FlagDestination = 1
								LEFT JOIN TblMasterCustomerLocation locD ON LegsD.MasterCustomerLocation_Guid = locD.Guid 
								INNER JOIN TblMasterCustomer cusD ON locD.MasterCustomer_Guid = cusD.Guid
					WHERE       Job.FlagCancelAll = 0 
								AND Job.FlagJobDiscrepancies = 1
								AND (LegsP.MasterSite_Guid = @SiteGuid OR LegsD.MasterSite_Guid = @SiteGuid)
					
					-- ###  ====  item seal  =====  ###
					SELECT tmpDisc.JobGuid
						   ,ISNULL(tmpDisc.JobNo, '')  JobNo
						   ,tmpDisc.CusLocPGuid
						   ,ISNULL(tmpDisc.PU_Location, '')  PU_Location
						   ,tmpDisc.SitePickUpGuid
						   ,tmpDisc.CusLocDGuid
						   ,ISNULL(tmpDisc.DL_Location, '')  DL_Location
						   ,tmpDisc.SiteDeliveryGuid
						   ,'Seal No. ' + item.SealNo as ItemName 
						   ,ISNULL(CASE WHEN item.SealNo IS NOT NULL AND item.QtyMissing > 0 THEN item.QtyMissing END,0) AS QtyShortage
						   ,0 AS QtyOverage
						   ,item.SealNo
						   ,reason.ReasonTypeName
						   ,item.Remarks
						   ,item.UsernameSupervisorVerify
						   ,CONVERT(nvarchar(100),item.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),item.DatetimeSupervisorVerify,108)  AS DatetimeVerify
						   ,item.Guid
						   ,item.MasterRunResourceDaily_Guid
						   ,item.MasterCustomerLocation_InternalDepartment_Guid
						   ,item.ClientHostNameScan
						   ,item.MasterCommodity_Guid
						   ,item.DatetimeSupervisorVerify
						   ,tmpDisc.FlagNonDelivery
						   ,s.FlagPartial as FlagItemPartial
						   ,tmpDisc.FlagMultiBranch AS FlagNotAllowReturn
					FROM TblMasterActualJobItemDiscrapencies item
					INNER JOIN #TmpJobDiscrepancies tmpDisc ON tmpDisc.JobGuid = item.MasterActualJobHeader_Guid
					INNER JOIN TblMasterActualJobItemsSeal s ON  s.MasterActualJobHeader_Guid = item.MasterActualJobHeader_Guid
																AND (s.FlagSealDiscrepancies = 1 AND s.FlagIntDisc = 0)
																AND s.SealNo = item.SealNo
					
					LEFT JOIN TblMasterReasonType reason on reason.Guid = item.MasterReasonType_Guid
					WHERE item.MasterCommodity_Guid IS NULL and
					item.MasterCustomerLocation_InternalDepartment_Guid = @InternalDepartmentGuid and
					item.FlagCloseCase = 0
					
					UNION
					-- ###  ====  item commodity  =====  ###
					SELECT tmpDisc.JobGuid
						   ,ISNULL(tmpDisc.JobNo, '')  JobNo
						   ,tmpDisc.CusLocPGuid
						   ,ISNULL(tmpDisc.PU_Location, '')  PU_Location
						   ,tmpDisc.SitePickUpGuid
						   ,tmpDisc.CusLocDGuid
						   ,ISNULL(tmpDisc.DL_Location, '')  DL_Location
						   ,tmpDisc.SiteDeliveryGuid
						   ,comm.CommodityName as ItemName 
						   ,ISNULL(CASE WHEN item.SealNo IS NULL AND item.QtyMissing > 0 THEN item.QtyMissing END,0) AS QtyShortage
						   ,ISNULL(CASE WHEN item.SealNo IS NULL AND item.QtyOverage > 0 THEN item.QtyOverage END,0) AS QtyOverage
						   ,item.SealNo
						   ,reason.ReasonTypeName
						   ,item.Remarks
						   ,item.UsernameSupervisorVerify
						   ,CONVERT(nvarchar(100),item.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),item.DatetimeSupervisorVerify,108)  AS DatetimeVerify
						   ,item.Guid
						   ,item.MasterRunResourceDaily_Guid
						   ,item.MasterCustomerLocation_InternalDepartment_Guid
						   ,item.ClientHostNameScan
						   ,item.MasterCommodity_Guid
						   ,item.DatetimeSupervisorVerify
						   ,tmpDisc.FlagNonDelivery
						   ,c.FlagPartial as FlagItemPartial
						   ,tmpDisc.FlagMultiBranch AS FlagNotAllowReturn
					FROM TblMasterActualJobItemDiscrapencies item
					LEFT JOIN #TmpJobDiscrepancies tmpDisc ON tmpDisc.JobGuid = item.MasterActualJobHeader_Guid
					LEFT JOIN TblMasterActualJobItemsCommodity c ON c.MasterActualJobHeader_Guid = item.MasterActualJobHeader_Guid
																AND c.FlagCommodityDiscrepancies = 1
																AND c.MasterCommodity_Guid = item.MasterCommodity_Guid
					INNER JOIN TblMasterCommodity comm ON comm.Guid = c.MasterCommodity_Guid
					LEFT JOIN TblMasterReasonType reason on reason.Guid = item.MasterReasonType_Guid
					WHERE item.MasterCustomerLocation_InternalDepartment_Guid = @InternalDepartmentGuid and
					item.FlagCloseCase = 0
					
					UNION
					-- ###  ====  item unknow job but have run =====  ###
					SELECT  NULL as JobGuid
						   ,'' as JobNo
						   ,NULL as CusLocPGuid
						   ,'' as CustomerLocPickUpName
						   ,NULL as SitePickUpGuid
						   ,NULL as CusLocDGuid
						   ,'' as CustomerLocDeliveryName
						   ,NULL as SiteDeliveryGuid
						   ,(CASE WHEN item.MasterCommodity_Guid IS NULL AND item.SealNo IS NULL THEN 'Unknown' 
											   WHEN item.MasterCommodity_Guid IS NOT NULL THEN comm.CommodityName 
											   ELSE 'Seal No. ' + item.SealNo END) as ItemName 
						   ,ISNULL(item.QtyMissing, 0) AS QtyShortage
						   ,ISNULL(item.QtyOverage, 0) AS QtyOverage
						   ,item.SealNo
						   ,reason.ReasonTypeName
						   ,item.Remarks
						   ,item.UsernameSupervisorVerify
						   ,CONVERT(nvarchar(100),item.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),item.DatetimeSupervisorVerify,108)  AS DatetimeVerify
						   ,item.Guid
						   ,item.MasterRunResourceDaily_Guid
						   ,item.MasterCustomerLocation_InternalDepartment_Guid
						   ,item.ClientHostNameScan
						   ,item.MasterCommodity_Guid
						   ,item.DatetimeSupervisorVerify
						   ,CAST(0 as bit) as FlagNonDelivery
						   ,CAST(0 as bit) as FlagItemPartial
						   ,CAST(0 as bit) AS FlagNotAllowReturn
					FROM	TblMasterActualJobItemDiscrapencies item 
					INNER JOIN TblMasterReasonType reason ON reason.Guid = item.MasterReasonType_Guid
					LEFT JOIN TblMasterCommodity comm ON comm.Guid = item.MasterCommodity_Guid
					WHERE   item.MasterCustomerLocation_InternalDepartment_Guid = @InternalDepartmentGuid AND 
					item.FlagCloseCase = 0 AND 
					item.MasterActualJobHeader_Guid IS NULL AND 
					item.MasterRunResourceDaily_Guid IN (SELECT	daily.Guid
														 FROM	TblMasterDailyRunResource daily
														 INNER JOIN TblMasterRunResource run ON daily.MasterRunResource_Guid = run.Guid
														 WHERE	run.MasterSite_Guid = @SiteGuid)
					
					UNION
					-- ###  ====  item unknow job doesn't have run =====  ###
					SELECT  NULL as JobGuid
						   ,'' as JobNo
						   ,NULL as CusLocPGuid
						   ,'' as CustomerLocPickUpName
						   ,NULL as SitePickUpGuid
						   ,NULL as CusLocDGuid
						   ,'' as CustomerLocDeliveryName
						   ,NULL as SiteDeliveryGuid
						   ,(CASE WHEN item.MasterCommodity_Guid IS NULL AND item.SealNo IS NULL THEN 'Unknown' 
											   WHEN item.MasterCommodity_Guid IS NOT NULL THEN comm.CommodityName 
											   ELSE 'Seal No. ' + item.SealNo END) as ItemName 
						   ,ISNULL(CASE WHEN item.SealNo IS NULL AND item.QtyMissing > 0 THEN item.QtyMissing END,0) AS QtyShortage
						   ,ISNULL(CASE WHEN item.SealNo IS NULL AND item.QtyOverage > 0 THEN item.QtyOverage END,0) AS QtyOverage
						   ,item.SealNo
						   ,reason.ReasonTypeName
						   ,item.Remarks
						   ,item.UsernameSupervisorVerify
						   ,CONVERT(nvarchar(100),item.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),item.DatetimeSupervisorVerify,108)  AS DatetimeVerify
						   ,item.Guid
						   ,item.MasterRunResourceDaily_Guid
						   ,item.MasterCustomerLocation_InternalDepartment_Guid
						   ,item.ClientHostNameScan
						   ,item.MasterCommodity_Guid
						   ,item.DatetimeSupervisorVerify
						   ,CAST(0 as bit) as FlagNonDelivery
						   ,CAST(0 as bit) as FlagItemPartial
						   ,CAST(0 as bit) AS FlagNotAllowReturn
					FROM  TblMasterActualJobItemDiscrapencies item
					INNER JOIN TblMasterReasonType reason ON reason.Guid = item.MasterReasonType_Guid
					LEFT JOIN TblMasterCommodity comm ON comm.Guid = item.MasterCommodity_Guid
					WHERE item.MasterCustomerLocation_InternalDepartment_Guid = @InternalDepartmentGuid AND 
					item.FlagCloseCase = 0 AND 
					(item.MasterRunResourceDaily_Guid IS NULL AND item.QtyOverage > 0 AND 
					 item.MasterActualJobHeader_Guid IS NULL)
					
					UNION
					---- ###  ====  item master id =====  ###
					SELECT  NULL as JobGuid
						   ,'' as JobNo
						   ,NULL as CusLocPGuid
						   ,'' as CustomerLocPickUpName
						   ,NULL as SitePickUpGuid
						   ,con.MasterCustomerLocation_Guid as CusLocDGuid
						   ,cus.CustomerFullName +' - '+ cusloc.BranchName as CustomerLocDeliveryName
						   ,con.MasterSite_Guid as SiteDeliveryGuid
						   ,con.MasterID as ItemName 
						   ,ISNULL(CASE WHEN item.SealNo IS NOT NULL AND item.QtyMissing > 0 THEN item.QtyMissing END,0) AS QtyShortage
						   ,0 AS QtyOverage
						   ,item.SealNo
						   ,reason.ReasonTypeName
						   ,item.Remarks
						   ,item.UsernameSupervisorVerify
						   ,CONVERT(nvarchar(100),item.DatetimeSupervisorVerify,@IntDateFormatSQL) + ' ' + CONVERT(varchar(5),item.DatetimeSupervisorVerify,108)  AS DatetimeVerify
						   ,item.Guid
						   ,item.MasterRunResourceDaily_Guid
						   ,item.MasterCustomerLocation_InternalDepartment_Guid
						   ,item.ClientHostNameScan
						   ,NULL as MasterCommodity_Guid
						   ,item.DatetimeSupervisorVerify
						   ,CAST(1 as bit) as FlagNonDelivery
						   ,CAST(0 as bit) as FlagItemPartial
						   ,con.FlagMultiBranch AS FlagNotAllowReturn
					FROM  TblMasterActualJobItemDiscrapencies item
					INNER JOIN TblMasterConAndDeconsolidate_Header con ON con.Guid = item.MasterConAndDeconsolidateHeader_Guid
																	   and con.FlagConDiscrepancies  = 1
																	   and con.MasterSite_Guid = @SiteGuid
					INNER JOIN TblMasterReasonType reason ON reason.Guid = item.MasterReasonType_Guid
					INNER JOIN TblMasterCustomerLocation cusloc ON cusloc.Guid = con.MasterCustomerLocation_Guid
					INNER JOIN TblMasterCustomer cus ON cus.Guid = cusloc.MasterCustomer_Guid
					WHERE item.MasterCustomerLocation_InternalDepartment_Guid = @InternalDepartmentGuid AND 
					item.FlagCloseCase = 0 AND 
					item.MasterConAndDeconsolidateHeader_Guid IS NOT NULL
					ORDER BY item.DatetimeSupervisorVerify DESC
					
					
					IF OBJECT_ID('tempdb..#TmpJobDiscrepancies') IS NOT NULL
					BEGIN
						DROP TABLE #TmpJobDiscrepancies
					END
                ";
            }
        }
        #endregion
    }
}
