using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Consolidation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using static Bgt.Ocean.Infrastructure.Util.EnumPreVault;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation
{

    public interface IMasterConAndDeconsolidate_HeaderRepository : IRepository<TblMasterConAndDeconsolidate_Header>
    {
        Guid GetGuidStatusByStatusID(int statusID);
        IEnumerable<TblMasterConAndDeconsolidate_Header> FindByMasterID_Guids(List<Guid> masterID_Guids);
        IEnumerable<TblMasterConAndDeconsolidate_Header> GetConByGuidList(List<Guid> conGuids);
        IEnumerable<TblMasterConAndDeconsolidate_Header> GetInnerConLayer(Guid conGuid);
        TblSystemConAndDeconsolidateStatus GetStatusConByStatusGuid(Guid statusGuid);
        bool CheckDupMasterID_MultiBranch(string masterID, Guid masterID_Guid, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid);
        void UpdateStatusConsolidate_CheckOutDept(IEnumerable<Guid> ConsolidateGuid, Guid consolidateStatusGuid, string username, DateTime clientDateTime, DateTimeOffset universalDateTime);
        void UpdateConsolidateHeader(Guid conGuid, Guid statusGuid, Guid? conOuter_Guid, string newMasterID, string username, DateTime clientDateTime, DateTimeOffset universalDateTime);
        List<ConAvailableItemView> GetItemCanCon(DateTime workDate, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid);
        List<ConAvailableItemView> GetItemEditCon(DateTime workDate, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid, Guid masterID_Guid);
        List<PreVaultConsolidationLiabilityValueDetailResult> GetConWithCurrencyDetail(Guid masterID_Guid);
        IEnumerable<Guid> GetConGuidByJobsGuid(IEnumerable<Guid> jobsGuid);
        IEnumerable<Guid> GetAllJobGuidInConByJobGuid(IEnumerable<Guid> jobsGuid);
        bool HasJobNotInCon(IEnumerable<Guid> jobsGuid);
        void UpdateConToNewDailyRun(IEnumerable<Guid> jobsGuid, TblMasterDailyRunResource newDailyRun, DateTime datetimeModified, string userModify, DateTimeOffset universalDatetimeModified);
        IEnumerable<TblMasterConAndDeconsolidate_Header> GetConsolidateByJobsGuid(IEnumerable<Guid> jobsGuid);
    }

    public class MasterConAndDeconsolidate_HeaderRepository : Repository<OceanDbEntities, TblMasterConAndDeconsolidate_Header>, IMasterConAndDeconsolidate_HeaderRepository
    {
        public MasterConAndDeconsolidate_HeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterConAndDeconsolidate_Header> FindByMasterID_Guids(List<Guid> masterID_Guids)
        {
            return DbContext.TblMasterConAndDeconsolidate_Header.Where(e => masterID_Guids.Contains(e.Guid));
        }

        public IEnumerable<TblMasterConAndDeconsolidate_Header> GetConByGuidList(List<Guid> conGuids)
        {
            return DbContext.TblMasterConAndDeconsolidate_Header.Where(w => conGuids.Contains(w.Guid));
        }

        public IEnumerable<TblMasterConAndDeconsolidate_Header> GetInnerConLayer(Guid conGuid)
        {
            return DbContext.TblMasterConAndDeconsolidate_Header.Where(w => w.ConsolidationRoute_Guid == conGuid);
        }

        public Guid GetGuidStatusByStatusID(int statusID)
        {
            return DbContext.TblSystemConAndDeconsolidateStatus.Where(e => e.StatusID == statusID && e.FlagDisable == false).FirstOrDefault().Guid;
        }

        public TblSystemConAndDeconsolidateStatus GetStatusConByStatusGuid(Guid statusGuid)
        {
            return DbContext.TblSystemConAndDeconsolidateStatus.Where(e => e.Guid == statusGuid && e.FlagDisable == false).FirstOrDefault();
        }

        public void UpdateStatusConsolidate_CheckOutDept(IEnumerable<Guid> ConsolidateGuid, Guid consolidateStatusGuid, string username, DateTime clientDateTime, DateTimeOffset universalDateTime)
        {
            string guids = string.Join("','", ConsolidateGuid);
            string a = $"('{guids}')";
            string sql = $@"UPDATE TblMasterConAndDeconsolidate_Header
                                    SET  FlagInPreVault = 1,
                                         SystemCoAndDeSolidateStatus_Guid = '{consolidateStatusGuid}',
                                         UserModifed = '{username}',
                                         DatetimeModified = '{clientDateTime}',
                                         UniversalDatetimeModified = '{universalDateTime}'
                                         WHERE Guid in {a}";
            DbContext.Database.Connection.Query(sql);
        }

        public bool CheckDupMasterID_MultiBranch(string masterID, Guid masterID_Guid, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid)
        {
            Guid completeStatusGuid = GetGuidStatusByStatusID(StatusConsolidate.Completed);
            Guid deconsolidatedStatusGuid = GetGuidStatusByStatusID(StatusConsolidate.Deconsolidated);

            bool isDupMasterID = DbContext.TblMasterConAndDeconsolidate_Header.Any(e => e.MasterID == masterID
                                            && e.Guid != masterID_Guid
                                            && e.SystemCoAndDeSolidateStatus_Guid != completeStatusGuid
                                            && e.SystemCoAndDeSolidateStatus_Guid != deconsolidatedStatusGuid
                                            && e.MasterSitePathHeader_Guid == sitePathGuid
                                            && e.MasterSite_Guid == originSiteGuid
                                            && e.Destination_MasterSite_Guid == destinationSiteGuid
                                            && e.FlagMultiBranch
                                            );

            return isDupMasterID;
        }

        public void UpdateConsolidateHeader(Guid conGuid, Guid statusGuid, Guid? conOuter_Guid, string newMasterID, string username, DateTime clientDateTime, DateTimeOffset universalDateTime)
        {
            string sql;
            if (conOuter_Guid == null)
            {
                sql = $@"UPDATE TblMasterConAndDeconsolidate_Header
                            SET  MasterID = '{newMasterID}',
                                 SystemCoAndDeSolidateStatus_Guid = '{statusGuid}',
                                 UserModifed = '{username}',
                                 DatetimeModified = '{clientDateTime}',
                                 UniversalDatetimeModified = '{universalDateTime}',
                                 ConsolidationRoute_Guid = null
                                 WHERE Guid = '{conGuid}'";

            }
            else
            {
                sql = $@"UPDATE TblMasterConAndDeconsolidate_Header
                            SET  MasterID = '{newMasterID}',
                                 SystemCoAndDeSolidateStatus_Guid = '{statusGuid}',
                                 UserModifed = '{username}',
                                 DatetimeModified = '{clientDateTime}',
                                 UniversalDatetimeModified = '{universalDateTime}',
                                 ConsolidationRoute_Guid = '{conOuter_Guid}'
                                 WHERE Guid = '{conGuid}'";
            }

            DbContext.Database.Connection.Query(sql);
        }

        public List<PreVaultConsolidationLiabilityValueDetailResult> GetConWithCurrencyDetail(Guid masterID_Guid)
        {
            List<PreVaultConsolidationLiabilityValueDetailResult> response;

            string sql = @"
			    SELECT  Distinct 
			    		TblMasterConAndDeconsolidate_Header.Guid AS MasterID_Guid, 
			    		TblMasterConAndDeconsolidate_Header.MasterID, 
			    		TblMasterConAndDeconsolidate_Header.SystemCoAndDeSolidateStatus_Guid,
			    		TblSystemConAndDeconsolidateStatus.StatusID,
			    		TblSystemConAndDeconsolidateStatus.StatusName,  
			            TblMasterConAndDeconsolidate_Header.MasterCustomerLocation_Guid,
			    		NULL AS LocationName, 
			    		TblMasterConAndDeconsolidate_Header.MasterRouteGroup_Detail_Guid, 
			            TblMasterConAndDeconsolidate_Header.Workdate, 
			    		TblMasterConAndDeconsolidate_Header.UserCreated, 
			    		TblMasterConAndDeconsolidate_Header.DatetimeCreated, 
			            TblMasterConAndDeconsolidate_Header.UniversalDatetimeCreated, 
			    		TblMasterConAndDeconsolidate_Header.UserModifed, 
			    		TblMasterConAndDeconsolidate_Header.DatetimeModified, 
			            TblMasterConAndDeconsolidate_Header.UniversalDatetimeModified, 
			    		TblMasterConAndDeconsolidate_Header.SystemConsolidateSourceID, 
			            TblMasterConAndDeconsolidate_Header.MasterSite_Guid, 
			    		TblMasterConAndDeconsolidate_Header.ConsolidationRoute_Guid, 
			    		'' AS RunResourceDailyStatusID,
			    		'Multi-Branch Route Consolidation' AS ConOrDeconsolidateType,
			    		6 AS ConOrDeconsolidateTypeID,
			    		TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber AS RouteName,
			    		TblMasterConAndDeconsolidate_Header.MasterDailyRunResource_Guid,
			    		TblMasterActualJobItemsLiability.Liability AS Liability
			    		,TblMasterCurrency.MasterCurrencyAbbreviation AS CurrencyNameAbb
			    		,TblMasterActualJobItemsLiability.Guid AS Liability_Guid
			    		,TblMasterActualJobItemsSeal.MasterCustomerLocation_InternalDepartment_Guid
			    		,TblMasterConAndDeconsolidate_Header.OnwardDestination_Guid
			    FROM    TblMasterConAndDeconsolidate_Header 
			    		INNER JOIN TblSystemConAndDeconsolidateStatus ON TblMasterConAndDeconsolidate_Header.SystemCoAndDeSolidateStatus_Guid = TblSystemConAndDeconsolidateStatus.Guid
			    		INNER JOIN TblMasterDailyRunResource ON TblMasterConAndDeconsolidate_Header.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
			    		INNER JOIN TblMasterRunResource ON TblMasterRunResource.Guid = TblMasterDailyRunResource.MasterRunResource_Guid
			    		INNER JOIN TblMasterRouteGroup_Detail ON TblMasterRouteGroup_Detail.Guid = TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid
			    		LEFT JOIN TblMasterActualJobItemsSeal ON TblMasterConAndDeconsolidate_Header.Guid = TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid
			    		LEFT JOIN TblMasterActualJobItemsLiability ON TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
			    		LEFT JOIN TblMasterCurrency ON TblMasterActualJobItemsLiability.MasterCurrency_Guid = TblMasterCurrency.Guid
			    WHERE   TblMasterConAndDeconsolidate_Header.Guid = @MasterID_Guid";

            using (var dbContext = new OceanDbEntities())
            {
                response = dbContext.Database.SqlQuery<PreVaultConsolidationLiabilityValueDetailResult>(
                            sql.ToString()
                            , new SqlParameter("@MasterID_Guid", masterID_Guid)
                            ).ToList();
            }
            return response;
        }

        public List<ConAvailableItemView> GetItemCanCon(DateTime workDate, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid)
        {
            List<ConAvailableItemView> response;

            #region ## Query String
            string sql = $@"
                        	DECLARE @TotalJobs			TABLE(	JobGuid								uniqueidentifier,
                        										JobNo								nvarchar(200),
                        										ServiceJobTypeNameAbb				varchar(10),
                        										ServiceJobTypeNumber				int, 										
                        										PickUp_Location						nvarchar(500),
                        										Delivery_Location					nvarchar(500),
                        										WorkDate							nvarchar(100),
                        										CustomerGuid						uniqueidentifier,
                        										CustomerLocationGuid				uniqueidentifier, 
                                                                FlagChkCustomer                     bit,
                        										MasterRouteGroupDetail_Guid			uniqueidentifier,
                        										RouteName							nvarchar(100),
                        										DailyRunGuid						uniqueidentifier,
                        										NoOfItems							int,
                        										OnwardDestinationGuid				uniqueidentifier,
                        										InterDepartmentName					nvarchar(50)
                        										)
                        
                        	DECLARE @TotalJobsResult	TABLE(	JobGuid								uniqueidentifier,
                        										JobNo								nvarchar(200),
                        										ServiceJobTypeNameAbb				varchar(10),
                        										ServiceJobTypeNumber				int, 										
                        										PickUp_Location						nvarchar(500),
                        										Delivery_Location					nvarchar(500),
                        										WorkDate							nvarchar(100),
                        										CustomerGuid						uniqueidentifier,
                        										CustomerLocationGuid				uniqueidentifier, 
                                                                FlagChkCustomer                     bit,
                        										MasterRouteGroupDetail_Guid			uniqueidentifier,
                        										RouteName							nvarchar(100),
                        										DailyRunGuid						uniqueidentifier,
                        										NoOfItems							int,
                        										OnwardDestinationGuid				uniqueidentifier,
                        										InterDepartmentName					nvarchar(50)
                        										)
                        
                        	DECLARE @TotalJobsAndItems	TABLE(	JobGuid								uniqueidentifier,
                        										JobNo								nvarchar(200),
                        										ServiceJobTypeNameAbb				varchar(10),
                        										PickUp_Location						nvarchar(500),
                        										Delivery_Location					nvarchar(500),
                        										WorkDate							nvarchar(100),
                        										CustomerGuid						uniqueidentifier,
                        										CustomerLocationGuid				uniqueidentifier, 
                                                                FlagChkCustomer                     bit,
                        										SealNo								nvarchar(100), 
                        										SealGuid							uniqueidentifier,
                                                                MasterID_Guid                       uniqueidentifier,
                        										LiabilityGuid						uniqueidentifier,
                        										CommodityGuid						uniqueidentifier,
                        										Commodity							nvarchar(100),
                        										Liability							float, 
                        										CurrencyNameAbb						nvarchar(30),
                        										Qty									int, 
                        										GroupSeal							nvarchar(200),
                        										MasterRouteGroupDetail_Guid			uniqueidentifier,
                        										RouteName							nvarchar(100),
                        										GroupScanName						nvarchar(200),
                        										DailyRunGuid						uniqueidentifier,
                        										ColumnInReport						nvarchar(5),
                        										OnwardDestinationGuid				uniqueidentifier,
                        										InterDepartmentName					nvarchar(50)
                        										)
                        
                        	DECLARE	@TempSeal			 Table(	SealGuid				uniqueidentifier
                        										, JobGuid				uniqueidentifier
                        										, LiabilityGuid			uniqueidentifier )
                        
                        	DECLARE	@TempNon			 Table(	CommodityItemGuid		uniqueidentifier
                        										, JobGuid				uniqueidentifier
                        										, Quantity				int )
                        
                        BEGIN
                        	-- ## ==========   Find jobs in Pre-Vault Inventory   =========== ##
                        	INSERT INTO @TotalJobs
                        		SELECT      jh.Guid AS JobGuid, 
                        					jh.JobNo, 
                        					(TblSystemServiceJobType.ServiceJobTypeNameAbb) AS ServiceJobTypeNameAbb,
                        					(TblSystemServiceJobType.ServiceJobTypeID) AS ServiceJobTypeNumber, 
                        					(CASE WHEN TblSystemServiceJobType.ServiceJobTypeID = 16 THEN 
                        						(CASE WHEN jh.JobBCP_Ref_Guid IS NOT NULL 
                        							 THEN	(	select	cus.CustomerFullName + ' - ' + loc.BranchName 
                        										from	TblMasterActualJobHeader BCPJob
                        												INNER JOIN TblMasterActualJobServiceStopLegs BCPL ON BCPJob.Guid = BCPL.MasterActualJobHeader_Guid
                        												INNER JOIN TblMasterCustomerLocation loc ON BCPL.MasterCustomerLocation_Guid = loc.Guid 
                        												INNER JOIN TblMasterCustomer cus ON loc.MasterCustomer_Guid = cus.Guid 
                        										where	BCPJob.Guid = jh.JobBCP_Ref_Guid AND BCPL.SequenceStop = 1 )
                        							ELSE	(	select	TblMasterSite.SiteName + ' - ' + dept.InterDepartmentName
                        										from	TblMasterActualJobHeader jobhead 
                        												inner join TblMasterCustomerLocation_InternalDepartment dept on jobhead.BCD_MasterCustomerLocation_InternalDepartment_Guid = dept.Guid
                        												inner join TblMasterActualJobServiceStopLegs legs on legs.MasterActualJobHeader_Guid = jobhead.Guid
                        												inner join TblMasterSite on legs.MasterSite_Guid = TblMasterSite.Guid
                        										where	jobhead.Guid = jh.Guid	and legs.SequenceStop = 1) 
                        							END)
                        					ELSE (	select	cus.CustomerFullName + ' - ' + loc.BranchName 
                        							from	TblMasterActualJobServiceStopLegs legs
                        									INNER JOIN TblMasterCustomerLocation loc ON legs.MasterCustomerLocation_Guid = loc.Guid 
                        									INNER JOIN TblMasterCustomer cus ON loc.MasterCustomer_Guid = cus.Guid 
                        							where	legs.MasterActualJobHeader_Guid = jh.Guid AND legs.SequenceStop = 1 )
                        					END
                        					) AS PickUp_Location,
                        					DestCus.CustomerFullName +' - '+ DestLoc.BranchName AS Delivery_Location,
                        					DestinationLeg.ServiceStopTransectionDate AS WorkDate,
                        					DestCus.Guid AS CustomerGuid,
                        					DestLoc.Guid AS CustomerLocationGuid,
                                            DestCus.FlagChkCustomer AS FlagChkCustomer,
                        					TblMasterRouteGroup_Detail.Guid AS MasterRouteGroupDetail_Guid,					
                        					(CASE WHEN DRun.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + run.VehicleNumber, '') 
                        						 ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + run.VehicleNumber + ' (' + CONVERT(varchar(1), DRun.MasterRunResourceShift) + ')', '') END) 
                        					AS RouteName,
                        					DRun.Guid AS DailyRunGuid,
                        					jh.NoOfItems,
                        					(CASE WHEN jh.OnwardDestinationType = 1 THEN jh.OnwardDestination_Guid
                        							WHEN jh.OnwardDestinationType = 2	THEN TblSystemOnwardDestinationType.Guid END) AS OnwardDestinationGuid,
                        					(CASE WHEN jh.OnwardDestinationType = 1 THEN TblMasterCustomerLocation_InternalDepartment.InterDepartmentName
                        							WHEN jh.OnwardDestinationType = 2	THEN TblSystemOnwardDestinationType.OnwardDestinationName
                        							ELSE '' END) AS InterDepartmentName
                        		FROM        TblMasterActualJobHeader jh
                        					INNER JOIN TblSystemServiceJobType ON jh.SystemServiceJobType_Guid = TblSystemServiceJobType.Guid 
                        					INNER JOIN TblMasterActualJobServiceStopLegs EntranceLeg ON jh.Guid = EntranceLeg.MasterActualJobHeader_Guid 
                        					INNER JOIN TblMasterActualJobServiceStopLegs DestinationLeg ON jh.Guid = DestinationLeg.MasterActualJobHeader_Guid 
                        					INNER JOIN TblMasterCustomerLocation DestLoc ON DestinationLeg.MasterCustomerLocation_Guid = DestLoc.Guid 
                        					INNER JOIN TblMasterCustomer DestCus ON DestLoc.MasterCustomer_Guid = DestCus.Guid 
                        					LEFT JOIN TblMasterDailyRunResource DRun ON DestinationLeg.MasterRunResourceDaily_Guid = DRun.Guid
                        					LEFT JOIN TblMasterRouteGroup_Detail ON DRun.MasterRouteGroup_Detail_Guid = TblMasterRouteGroup_Detail.Guid
                        					LEFT JOIN TblMasterRunResource run ON DRun.MasterRunResource_Guid = run.Guid
                        					LEFT OUTER JOIN TblSystemOnwardDestinationType ON jh.OnwardDestinationType = TblSystemOnwardDestinationType.OnwardDestinationTypeID 
                        					LEFT OUTER JOIN TblMasterCustomerLocation_InternalDepartment ON jh.OnwardDestination_Guid = TblMasterCustomerLocation_InternalDepartment.Guid
                        		WHERE       jh.SystemStatusJobID IN (7,28,29) --status ก่อนทำ chk out to interB -- TFS#64881: Add filter where status 29
                        					AND jh.FlagJobDiscrepancies = 0
                        					AND jh.FlagCancelAll = 0 
                        					AND jh.FlagMultiBranch = 1
                        					AND (SELECT COUNT(1) FROM TblMasterDailyInterBranch_Job IntJob WHERE IntJob.MasterActualJobHeader_Guid = jh.Guid) = 0 -- check job doesn't check out to inter-branch
                        					AND TblSystemServiceJobType.ServiceJobTypeID IN (14, 15, 16) --BCD Job
                        					AND (EntranceLeg.SequenceStop = 1 AND EntranceLeg.MasterSite_Guid = @OriginSiteGuid) 
                        					AND (DestinationLeg.FlagDestination = 1 AND DestinationLeg.MasterSite_Guid = @DestinationSiteGuid)
                        					AND DestinationLeg.ServiceStopTransectionDate = @WorkDate
                                            AND jh.MasterSitePathHeader_Guid = @SitePathGuid
                        
                        	-- ## ==========   If it's BCD Job. Then, Get only BCD with No Missing Seal and Non-Barcode   =========== ##
                        	INSERT INTO @TempSeal													
                        	SELECT	ItemSeal.Guid		
                        			, ItemSeal.MasterActualJobHeader_Guid
                        			, ItemSeal.MasterActualJobItemsCommodity_Guid			
                        	FROM	TblMasterActualJobItemsSeal ItemSeal		
                        	WHERE	ItemSeal.MasterActualJobHeader_Guid IN ( SELECT JobGuid FROM @TotalJobs WHERE ServiceJobTypeNumber = 16 )
                        
                        	INSERT INTO @TempNon													
                        	SELECT	ItemNon.Guid		
                        			, ItemNon.MasterActualJobHeader_Guid
                        			, ItemNon.Quantity		
                        	FROM	TblMasterActualJobItemsCommodity ItemNon		
                        	WHERE	ItemNon.MasterActualJobHeader_Guid IN ( SELECT JobGuid FROM @TotalJobs WHERE ServiceJobTypeNumber = 16 )
                        
                        	INSERT INTO @TotalJobsResult
                        	SELECT * 
                        	FROM @TotalJobs t
                        	WHERE (t.ServiceJobTypeNumber IN (14,15))
                        		OR (t.ServiceJobTypeNumber = 16 AND t.NoOfItems =	(	SELECT  Count(1) 
                        																FROM	@TempSeal 
                        																WHERE	JobGuid = t.JobGuid )
                        		OR (t.ServiceJobTypeNumber = 16 AND t.NoOfItems =	(	SELECT  SUM(Quantity) 
                        																FROM	@TempNon 
                        																WHERE	JobGuid = t.JobGuid )))
                        
                        	INSERT INTO @TotalJobsAndItems
                        	-- ## ==========   Con inner layer   =========== ##
                        	SELECT		NULL AS JobGuid,
                        				'' AS JobNo,
                        				'' AS ServiceJobTypeNameAbb,
					                    '' AS PickUp_Location,
					                    '' AS Delivery_Location,
                        				con.WorkDate,
                        				NULL AS CustomerGuid,
                        				NULL AS CustomerLocationGuid,
                                        1 AS FlagChkCustomer,
                        				con.MasterID AS SealNo,
                        				con.Guid AS SealGuid,	
                                        con.Guid AS MasterID_Guid,		
                        				NULL AS LiabilityGuid,
                        				NULL AS CommodityGuid,
                        				'' AS Commodity, 								
                        				Items.Liability AS Liability,				
                        				'' AS CurrencyNameAbb, 				
                        				1 AS Qty,
                        				'Con inner layer' AS GroupSeal,
                        				TblMasterRouteGroup_Detail.Guid AS MasterRouteGroupDetail_Guid,
                        				(CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 
                        					  THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
                        					  ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END)
                        				AS RouteName,
                        				'Seal' AS GroupScanName,
                        				con.MasterDailyRunResource_Guid AS DailyRunGuid,
                        				NULL AS ColumnInReport,
                        				con.OnwardDestination_Guid AS OnwardDestinationGuid,
                        				ISNULL(IntDep.InterDepartmentName, Owd.OnwardDestinationName) AS InternalDepartmentName
                        	FROM TblMasterConAndDeconsolidate_Header con 
                        		 INNER JOIN TblSystemConAndDeconsolidateStatus status on con.SystemCoAndDeSolidateStatus_Guid = status.Guid
                        		 INNER JOIN TblMasterDailyRunResource ON con.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
                        		 INNER JOIN TblMasterRunResource ON TblMasterRunResource.Guid = TblMasterDailyRunResource.MasterRunResource_Guid
                        		 INNER JOIN TblMasterRouteGroup_Detail ON TblMasterRouteGroup_Detail.Guid = TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid
                        		 LEFT JOIN TblMasterCustomerLocation_InternalDepartment IntDep ON CON.OnwardDestination_Guid = IntDep.Guid
                        	     LEFT JOIN TblSystemOnwardDestinationType Owd ON CON.OnwardDestination_Guid = Owd.Guid
              	                    OUTER APPLY
              	                    (
              		                    SELECT	AllItems.FlagMultiBranch
              				                    , MAX(AllItems.SystemStatusJobID) AS SystemStatusJobID
              				                    , AllItems.Con_Guid
              				                    , SUM(STC) AS Liability
              				                    , NULL AS LiaGuid
              				                    , SUM(Total) AS Total
							                    , MAX(AllItems.MasterActualJobHeader_Guid) AS ItemJob_Guid
              						                    FROM(
              								                    select s.MasterActualJobHeader_Guid, s.MasterCustomerLocation_InternalDepartment_Guid, JH.FlagMultiBranch, JH.SystemStatusJobID
              											                    , s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid AS Con_Guid
              											                    , ISNULL(TblMasterActualJobItemsLiability.Liability, 0.00) AS STC
              											                    , TblMasterActualJobItemsLiability.Guid AS LiaGuid
              											                    , COUNT(s.Guid) AS Total
              									                    from TblMasterActualJobItemsSeal s
              											                    INNER JOIN TblMasterActualJobHeader JH ON s.MasterActualJobHeader_Guid = JH.Guid
              											                    LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
              									                    where s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = CON.Guid
              											                    AND s.FlagPartial = 0 
              											                    AND s.FlagSealDiscrepancies = 0
              											                    AND JH.FlagMultiBranch = 1
              									                    group by s.MasterActualJobHeader_Guid
              											                    , s.MasterCustomerLocation_InternalDepartment_Guid
              											                    , JH.FlagMultiBranch
              											                    , JH.SystemStatusJobID
              											                    , s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid
              											                    , TblMasterActualJobItemsLiability.Liability
              											                    , TblMasterActualJobItemsLiability.Guid	
											                    UNION
											                    --== Not calculate value of commodity but using Job_Guid. (Because based on previous SP result, it summarized only seal liability value) ==--
											                    select  c.MasterActualJobHeader_Guid, c.MasterCustomerLocation_InternalDepartment_Guid, JH.FlagMultiBranch, JH.SystemStatusJobID
													                    , c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid AS Con_Guid
													                    , 0 AS STC
													                    --, (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
													                    --	   else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS STC
													                    , NULL AS LiaGuid
													                    , SUM(c.Quantity) AS Total								
											                    from TblMasterActualJobItemsCommodity c							 
												                        INNER JOIN TblMasterActualJobHeader JH ON c.MasterActualJobHeader_Guid = JH.Guid
												                        INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
											                    where c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = CON.Guid
												                        AND c.FlagPartial = 0 
												                        AND c.FlagCommodityDiscrepancies = 0 
												                        AND com.FlagRequireSeal = 0												  
											                    group by c.MasterActualJobHeader_Guid
													                    , c.MasterCustomerLocation_InternalDepartment_Guid
													                    , JH.FlagMultiBranch
													                    , JH.SystemStatusJobID
													                    , c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid
													                    , com.FlagCommodityGlobal
													                    , com.CommodityAmount
													                    , com.CommodityValue
													                    , c.Quantity
													                    --, cc.CommodityAmount
													                    --, cc.CommodityValue	
											                    ) as AllItems 
              		                    GROUP BY  AllItems.FlagMultiBranch , AllItems.Con_Guid		 ) AS Items
                        	WHERE	con.MasterRouteGroup_Detail_Guid IS NOT NULL
                                    AND con.ConsolidationRoute_Guid IS NULL
                                    AND con.FlagInPreVault = 1
                        			AND con.Workdate = @WorkDate
                        			AND con.MasterSite_Guid = @OriginSiteGuid
                        			AND con.Destination_MasterSite_Guid = @DestinationSiteGuid
                        			AND con.MasterSitePathHeader_Guid = @SitePathGuid
                        			AND con.FlagMultiBranch = 1
                                    AND con.SystemConsolidateSourceID = 4
                        			
                        			AND status.StatusID = 2
                        	UNION ALL
                        
                        	-- ## ==========   Seal within Liability   =========== ##
                        	SELECT		t.JobGuid,
                        				t.JobNo,
                        				t.ServiceJobTypeNameAbb,
                        				t.PickUp_Location,
                        				t.Delivery_Location,
                        				t.WorkDate,
                        				t.CustomerGuid,
                        				t.CustomerLocationGuid,
                                        t.FlagChkCustomer,
                        				TblMasterActualJobItemsSeal.SealNo,
                        				TblMasterActualJobItemsSeal.Guid AS SealGuid,
                                        NULL AS MasterID_Guid,			
                        				TblMasterActualJobItemsLiability.Guid AS LiabilityGuid,
                        				NULL AS CommodityGuid,
                        				CASE WHEN RANK() OVER (PARTITION BY TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid ORDER BY TblMasterActualJobItemsSeal.SealNo) = 1 THEN TblMasterCommodity.CommodityName END  AS Commodity, 								
                        				CASE WHEN RANK() OVER (PARTITION BY TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid ORDER BY TblMasterActualJobItemsSeal.SealNo) = 1 THEN ISNULL(TblMasterActualJobItemsLiability.Liability, 0.00) ELSE 0.00 END AS Liability,				
                        				TblMasterCurrency.MasterCurrencyAbbreviation AS CurrencyNameAbb, 				
                        				1 AS Qty,
                        				'Seal In Liability' AS GroupSeal,
                        				t.MasterRouteGroupDetail_Guid,
                        				t.RouteName,
                        				'Seal' AS GroupScanName,
                        				t.DailyRunGuid,
                        				NULL AS ColumnInReport,
                        				t.OnwardDestinationGuid,
                        				t.InterDepartmentName
                        	FROM        TblMasterActualJobItemsSeal 
                        				INNER JOIN @TotalJobsResult AS t on t.JobGuid = TblMasterActualJobItemsSeal.MasterActualJobHeader_Guid
                        				INNER JOIN TblMasterActualJobItemsLiability ON TblMasterActualJobItemsLiability.Guid = TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid
                        				LEFT OUTER JOIN TblMasterCurrency ON TblMasterCurrency.Guid = TblMasterActualJobItemsLiability.MasterCurrency_Guid
                        				LEFT OUTER JOIN TblMasterCommodity ON TblMasterCommodity.Guid = TblMasterActualJobItemsLiability.MasterCommodity_Guid
                        	WHERE		TblMasterActualJobItemsSeal.FlagSealDiscrepancies = 0
                        				AND TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid IS NOT NULL
                        				AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL
                        				AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL
                        	UNION ALL
                        		-- ## ==========   Only Seals   =========== ##
                        	SELECT		t.JobGuid,
                        				t.JobNo,
                        				t.ServiceJobTypeNameAbb,
                        				t.PickUp_Location,
                        				t.Delivery_Location,
                        				t.WorkDate,
                        				t.CustomerGuid,
                        				t.CustomerLocationGuid,
                                        t.FlagChkCustomer,
                        				TblMasterActualJobItemsSeal.SealNo, 
                        				TblMasterActualJobItemsSeal.Guid AS SealGuid,
                                        NULL AS MasterID_Guid,
                        				NULL AS LiabilityGuid,
                        				NULL AS CommodityGuid,
                        				NULL AS Commodity, 
                        				0.00 AS Liability,
                        				'' AS CurrencyNameAbb,
                        				1 AS Qty,
                        				'Seal Item' AS GroupSeal,
                        				t.MasterRouteGroupDetail_Guid,
                        				t.RouteName,
                        				'Seal' AS GroupScanName,
                        				t.DailyRunGuid,
                        				NULL AS ColumnInReport,
                        				t.OnwardDestinationGuid,
                        				t.InterDepartmentName		
                        	FROM        TblMasterActualJobItemsSeal 
                        				INNER JOIN @TotalJobsResult AS t on t.JobGuid = TblMasterActualJobItemsSeal.MasterActualJobHeader_Guid
                        	WHERE		TblMasterActualJobItemsSeal.FlagSealDiscrepancies = 0
                        				AND TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid IS NULL
                        				AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL
                        				AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL
                        	UNION ALL
                        	-- ## ==========   Find Non barcode (Commodity)   =========== ##
                        	SELECT		t.JobGuid,
                        				t.JobNo,
                        				t.ServiceJobTypeNameAbb,
                        				t.PickUp_Location,
                        				t.Delivery_Location,
                        				t.WorkDate,
                        				t.CustomerGuid,
                        				t.CustomerLocationGuid,
                                        t.FlagChkCustomer,
                        				NULL AS SealNo, 
                        				NULL AS SealGuid,
                                        NULL AS MasterID_Guid,
                        				NULL AS LiabilityGuid,
                        				TblMasterActualJobItemsCommodity.Guid AS CommodityGuid,
                        				TblMasterCommodity.CommodityName AS Commodity, 
                        				0.00 AS Liability,
                        				'' AS CurrencyNameAbb,
                        				TblMasterActualJobItemsCommodity.Quantity AS Qty,
                        				NULL AS GroupSeal,
                        				t.MasterRouteGroupDetail_Guid,
                        				t.RouteName,
                        				'Non Barcode' AS GroupScanName,
                        				t.DailyRunGuid,
                        				TblMasterCommodity.ColumnInReport AS ColumnInReport,
                        				t.OnwardDestinationGuid,
                        				t.InterDepartmentName
                        	FROM		@TotalJobsResult AS t 
                        				INNER JOIN TblMasterActualJobItemsCommodity ON t.JobGuid = TblMasterActualJobItemsCommodity.MasterActualJobHeader_Guid
                        				INNER JOIN TblMasterCommodity ON TblMasterActualJobItemsCommodity.MasterCommodity_Guid = TblMasterCommodity.Guid
                        	WHERE		((TblMasterActualJobItemsCommodity.FlagCommodityDiscrepancies = 0) 
                        					OR (TblMasterActualJobItemsCommodity.FlagCommodityDiscrepancies = 1 AND TblMasterActualJobItemsCommodity.Quantity > 0))		
                        				AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL
                        				AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL
                        
                        	-- ## ==========   Order by work date   =========== ##
                        	SELECT		RESULT.JobGuid								
                        				,RESULT.JobNo							
                        				,RESULT.ServiceJobTypeNameAbb				
                        				,RESULT.PickUp_Location						
                        				,RESULT.Delivery_Location					
                        				,RESULT.WorkDate							
                        				,RESULT.CustomerGuid						
                        				,RESULT.CustomerLocationGuid
                                        ,RESULT.FlagChkCustomer			
                        				,RESULT.SealNo								
                        				,RESULT.SealGuid			
                                        ,RESULT.MasterID_Guid
                        				,RESULT.LiabilityGuid						
                        				,RESULT.CommodityGuid						
                        				,RESULT.Commodity							
                        				,RESULT.Liability							
                        				,RESULT.CurrencyNameAbb						
                        				,RESULT.Qty									
                        				,RESULT.GroupSeal						
                        				,RESULT.MasterRouteGroupDetail_Guid			
                        				,RESULT.RouteName							
                        				,RESULT.GroupScanName							
                        				,RESULT.DailyRunGuid	
                        				,RESULT.OnwardDestinationGuid
                        				,RESULT.InterDepartmentName
                        	FROM		@TotalJobsAndItems RESULT
                        	Order by	 
                        				CASE WHEN RESULT.SealNo IS NULL THEN 1
                        					 WHEN RESULT.SealNo IS NOT NULL THEN 2 END
                        				,RESULT.SealNo
                        				,CASE WHEN RESULT.ColumnInReport IS NOT NULL THEN 3
                        					  WHEN RESULT.ColumnInReport IS NULL THEN 4 END
                        				,RESULT.ColumnInReport,
                        				RESULT.JobNo
                        END";
            #endregion

            using (var dbContext = new OceanDbEntities())
            {
                response = dbContext.Database.SqlQuery<ConAvailableItemView>(
                            sql.ToString()
                            , new SqlParameter("@WorkDate", workDate)
                            , new SqlParameter("@OriginSiteGuid", originSiteGuid)
                            , new SqlParameter("@DestinationSiteGuid", destinationSiteGuid)
                            , new SqlParameter("@SitePathGuid", sitePathGuid)
                            ).ToList();
            }
            return response;
        }

        public List<ConAvailableItemView> GetItemEditCon(DateTime workDate, Guid originSiteGuid, Guid destinationSiteGuid, Guid sitePathGuid, Guid masterID_Guid)
        {
            List<ConAvailableItemView> response;

            #region Query String
            string sql = @"
                                DECLARE @TotalJobs			TABLE(	JobGuid								uniqueidentifier,
                             										JobNo								nvarchar(200),
                             										ServiceJobTypeNameAbb				varchar(10),
                             										ServiceJobTypeNumber				int, 										
                             										PickUp_Location						nvarchar(500),
                             										Delivery_Location					nvarchar(500),
                             										WorkDate							nvarchar(100),
                             										CustomerGuid						uniqueidentifier,
                             										CustomerLocationGuid				uniqueidentifier, 
                                                                    FlagChkCustomer                     bit,
                             										MasterRouteGroupDetail_Guid			uniqueidentifier,
                             										RouteName							nvarchar(100),
                             										DailyRunGuid						uniqueidentifier,
                             										NoOfItems							int,
                             										OnwardDestinationGuid				uniqueidentifier,
                             										InterDepartmentName					nvarchar(50)
                             										)
                             
                             	DECLARE @TotalJobsResult	TABLE(	JobGuid								uniqueidentifier,
                             										JobNo								nvarchar(200),
                             										ServiceJobTypeNameAbb				varchar(10),
                             										ServiceJobTypeNumber				int, 										
                             										PickUp_Location						nvarchar(500),
                             										Delivery_Location					nvarchar(500),
                             										WorkDate							nvarchar(100),
                             										CustomerGuid						uniqueidentifier,
                             										CustomerLocationGuid				uniqueidentifier, 
                                                                    FlagChkCustomer                     bit,
                             										MasterRouteGroupDetail_Guid			uniqueidentifier,
                             										RouteName							nvarchar(100),
                             										DailyRunGuid						uniqueidentifier,
                             										NoOfItems							int,
                             										OnwardDestinationGuid				uniqueidentifier,
                             										InterDepartmentName					nvarchar(50)
                             										)
                             
                             	DECLARE @TotalJobsAndItems	TABLE(	JobGuid								uniqueidentifier,
                             										JobNo								nvarchar(200),
                             										ServiceJobTypeNameAbb				varchar(10),
                             										PickUp_Location						nvarchar(500),
                             										Delivery_Location					nvarchar(500),
                             										WorkDate							nvarchar(100),
                             										CustomerGuid						uniqueidentifier,
                             										CustomerLocationGuid				uniqueidentifier, 
                                                                    FlagChkCustomer                     bit,
                             										SealNo								nvarchar(100), 
                             										SealGuid							uniqueidentifier,
                                                                    MasterID_Guid                       uniqueidentifier,
                             										LiabilityGuid						uniqueidentifier,
                             										CommodityGuid						uniqueidentifier,
                             										Commodity							nvarchar(100),
                             										Liability							float, 
                             										CurrencyNameAbb						nvarchar(30),
                             										Qty									int, 
                             										GroupSeal							nvarchar(200),
                             										MasterRouteGroupDetail_Guid			uniqueidentifier,
                             										RouteName							nvarchar(100),
                             										GroupScanName						nvarchar(200),
                             										DailyRunGuid						uniqueidentifier,
                             										ColumnInReport						nvarchar(5),
                             										OnwardDestinationGuid				uniqueidentifier,
                             										InterDepartmentName					nvarchar(50),
                             										ConOuterLayerGuid					uniqueidentifier
                             										)
                             
                             	DECLARE	@TempSeal			 Table(	SealGuid				uniqueidentifier
                             										, JobGuid				uniqueidentifier
                             										, LiabilityGuid			uniqueidentifier )
                             
                             	DECLARE	@TempNon			 Table(	CommodityItemGuid		uniqueidentifier
                             										, JobGuid				uniqueidentifier
                             										, Quantity				int )
                             
                             	DECLARE @IsConRoute			bit
                             			, @IsConSite		bit
                             			, @FlagDisable		bit
                             			, @DailyRunGuid		uniqueidentifier
                             			, @LocationGuid		uniqueidentifier
                             			, @OnwardGuid		uniqueidentifier
                             
                             BEGIN
                             
                             	SELECT @IsConRoute = IIF(c.MasterRouteGroup_Detail_Guid IS NOT NULL, 1, 0)
                             		   ,@IsConSite = IIF(c.MasterRouteGroup_Detail_Guid IS NULL AND c.MasterCustomerLocation_Guid IS NULL, 1, 0)
                             		   ,@FlagDisable = IIF(s.StatusID = 2, 1, 0) -- set true for status seal
                             		   , @DailyRunGuid = c.MasterDailyRunResource_Guid
                             		   , @LocationGuid = c.MasterCustomerLocation_Guid
                             		   , @OnwardGuid = c.OnwardDestination_Guid
                             	FROM TblMasterConAndDeconsolidate_Header c
                             		 INNER JOIN TblSystemConAndDeconsolidateStatus s on c.SystemCoAndDeSolidateStatus_Guid = s.Guid
                             	WHERE c.Guid = @ConGuid
                             
                             	-- ## ==========   Find jobs in Pre-Vault Inventory   =========== ##
                             	INSERT INTO @TotalJobs
                             		SELECT      jh.Guid AS JobGuid, 
                             					jh.JobNo, 
                             					(TblSystemServiceJobType.ServiceJobTypeNameAbb) AS ServiceJobTypeNameAbb,
                             					(TblSystemServiceJobType.ServiceJobTypeID) AS ServiceJobTypeNumber, 
                             					(CASE WHEN TblSystemServiceJobType.ServiceJobTypeID = 16 THEN 
                             						(CASE WHEN jh.JobBCP_Ref_Guid IS NOT NULL 
                             							 THEN	(	select	cus.CustomerFullName + ' - ' + loc.BranchName 
                             										from	TblMasterActualJobHeader BCPJob
                             												INNER JOIN TblMasterActualJobServiceStopLegs BCPL ON BCPJob.Guid = BCPL.MasterActualJobHeader_Guid
                             												INNER JOIN TblMasterCustomerLocation loc ON BCPL.MasterCustomerLocation_Guid = loc.Guid 
                             												INNER JOIN TblMasterCustomer cus ON loc.MasterCustomer_Guid = cus.Guid 
                             										where	BCPJob.Guid = jh.JobBCP_Ref_Guid AND BCPL.SequenceStop = 1 )
                             							ELSE	(	select	TblMasterSite.SiteName + ' - ' + dept.InterDepartmentName
                             										from	TblMasterActualJobHeader jobhead 
                             												inner join TblMasterCustomerLocation_InternalDepartment dept on jobhead.BCD_MasterCustomerLocation_InternalDepartment_Guid = dept.Guid
                             												inner join TblMasterActualJobServiceStopLegs legs on legs.MasterActualJobHeader_Guid = jobhead.Guid
                             												inner join TblMasterSite on legs.MasterSite_Guid = TblMasterSite.Guid
                             										where	jobhead.Guid = jh.Guid	and legs.SequenceStop = 1) 
                             							END)
                             					ELSE (	select	cus.CustomerFullName + ' - ' + loc.BranchName 
                             							from	TblMasterActualJobServiceStopLegs legs
                             									INNER JOIN TblMasterCustomerLocation loc ON legs.MasterCustomerLocation_Guid = loc.Guid 
                             									INNER JOIN TblMasterCustomer cus ON loc.MasterCustomer_Guid = cus.Guid 
                             							where	legs.MasterActualJobHeader_Guid = jh.Guid AND legs.SequenceStop = 1 )
                             					END
                             					) AS PickUp_Location,
                             					DestCus.CustomerFullName +' - '+ DestLoc.BranchName AS Delivery_Location,
                             					DestinationLeg.ServiceStopTransectionDate AS WorkDate,
                             					DestCus.Guid AS CustomerGuid,
                             					DestLoc.Guid AS CustomerLocationGuid,
                                                DestCus.FlagChkCustomer AS FlagChkCustomer,
                             					TblMasterRouteGroup_Detail.Guid AS MasterRouteGroupDetail_Guid,					
                             					(CASE WHEN DRun.MasterRunResourceShift = 1 THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + run.VehicleNumber, '') 
                             						 ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + run.VehicleNumber + ' (' + CONVERT(varchar(1), DRun.MasterRunResourceShift) + ')', '') END) 
                             					AS RouteName,
                             					DRun.Guid AS DailyRunGuid,
                             					jh.NoOfItems,
                             					(CASE WHEN jh.OnwardDestinationType = 1 THEN jh.OnwardDestination_Guid
                             							WHEN jh.OnwardDestinationType = 2	THEN TblSystemOnwardDestinationType.Guid END) AS OnwardDestinationGuid,
                             					(CASE WHEN jh.OnwardDestinationType = 1 THEN TblMasterCustomerLocation_InternalDepartment.InterDepartmentName
                             							WHEN jh.OnwardDestinationType = 2	THEN TblSystemOnwardDestinationType.OnwardDestinationName
                             							ELSE '' END) AS InterDepartmentName
                             		FROM        TblMasterActualJobHeader jh
                             					INNER JOIN TblSystemServiceJobType ON jh.SystemServiceJobType_Guid = TblSystemServiceJobType.Guid 
                             					INNER JOIN TblMasterActualJobServiceStopLegs EntranceLeg ON jh.Guid = EntranceLeg.MasterActualJobHeader_Guid 
                             					INNER JOIN TblMasterActualJobServiceStopLegs DestinationLeg ON jh.Guid = DestinationLeg.MasterActualJobHeader_Guid 
                             					INNER JOIN TblMasterCustomerLocation DestLoc ON DestinationLeg.MasterCustomerLocation_Guid = DestLoc.Guid 
                             					INNER JOIN TblMasterCustomer DestCus ON DestLoc.MasterCustomer_Guid = DestCus.Guid 
                             					LEFT JOIN TblMasterDailyRunResource DRun ON DestinationLeg.MasterRunResourceDaily_Guid = DRun.Guid
                             					LEFT JOIN TblMasterRouteGroup_Detail ON DRun.MasterRouteGroup_Detail_Guid = TblMasterRouteGroup_Detail.Guid
                             					LEFT JOIN TblMasterRunResource run ON DRun.MasterRunResource_Guid = run.Guid
                             					LEFT OUTER JOIN TblSystemOnwardDestinationType ON jh.OnwardDestinationType = TblSystemOnwardDestinationType.OnwardDestinationTypeID 
                             					LEFT OUTER JOIN TblMasterCustomerLocation_InternalDepartment ON jh.OnwardDestination_Guid = TblMasterCustomerLocation_InternalDepartment.Guid
                             		WHERE       jh.SystemStatusJobID IN (7,28,29)--status ก่อนทำ chk out to interB
                             					AND jh.FlagJobDiscrepancies = 0
                             					AND jh.FlagCancelAll = 0 
                             					AND jh.FlagMultiBranch = 1
                             					AND (SELECT COUNT(1) FROM TblMasterDailyInterBranch_Job IntJob WHERE IntJob.MasterActualJobHeader_Guid = jh.Guid) = 0 -- check job doesn't check out to inter-branch
                             					AND TblSystemServiceJobType.ServiceJobTypeID IN (14, 15, 16) --BCD Job
                             					AND (EntranceLeg.SequenceStop = 1 AND EntranceLeg.MasterSite_Guid = @OriginSiteGuid) 
                             					AND (DestinationLeg.FlagDestination = 1 AND DestinationLeg.MasterSite_Guid = @DestinationSiteGuid)
                             					AND DestinationLeg.ServiceStopTransectionDate = @WorkDate
                             					AND jh.MasterSitePathHeader_Guid = @SitePathGuid
                             					AND 1 = case when @LocationGuid IS NULL then 1 when DestLoc.Guid = @LocationGuid then 1 else 0 end
                             					AND 1 = case when @DailyRunGuid IS NULL then 1 when DRun.Guid = @DailyRunGuid then 1 else 0 end
                             					AND 1 = case when @OnwardGuid IS NULL then 1 
                             								 when jh.OnwardDestinationType = 1 and jh.OnwardDestination_Guid = @OnwardGuid then 1 
                             								 when jh.OnwardDestinationType = 2 AND TblSystemOnwardDestinationType.Guid = @OnwardGuid then 1
                             								 else 0 end
                             
                             	-- ## ==========   If it's BCD Job. Then, Get only BCD with No Missing Seal and Non-Barcode   =========== ##
                             	INSERT INTO @TempSeal													
                             	SELECT	ItemSeal.Guid		
                             			, ItemSeal.MasterActualJobHeader_Guid
                             			, ItemSeal.MasterActualJobItemsCommodity_Guid			
                             	FROM	TblMasterActualJobItemsSeal ItemSeal		
                             	WHERE	ItemSeal.MasterActualJobHeader_Guid IN ( SELECT JobGuid FROM @TotalJobs WHERE ServiceJobTypeNumber = 16 )
                             
                             	INSERT INTO @TempNon													
                             	SELECT	ItemNon.Guid		
                             			, ItemNon.MasterActualJobHeader_Guid
                             			, ItemNon.Quantity		
                             	FROM	TblMasterActualJobItemsCommodity ItemNon		
                             	WHERE	ItemNon.MasterActualJobHeader_Guid IN ( SELECT JobGuid FROM @TotalJobs WHERE ServiceJobTypeNumber = 16 )
                             
                             	INSERT INTO @TotalJobsResult
                             	SELECT * 
                             	FROM @TotalJobs t
                             	WHERE (t.ServiceJobTypeNumber IN (14,15))
                             		OR (t.ServiceJobTypeNumber = 16 AND t.NoOfItems =	(	SELECT  Count(1) 
                             																FROM	@TempSeal 
                             																WHERE	JobGuid = t.JobGuid )
                             		OR (t.ServiceJobTypeNumber = 16 AND t.NoOfItems =	(	SELECT  SUM(Quantity) 
                             																FROM	@TempNon 
                             																WHERE	JobGuid = t.JobGuid )))
                             
                             	IF(@IsConSite = 1)
                             	BEGIN
                             		INSERT INTO @TotalJobsAndItems
                             		-- ## ==========   Con inner layer   =========== ##
                             		SELECT		NULL AS JobGuid,
                             					'' AS JobNo,
                             					'' AS ServiceJobTypeNameAbb,
					                            '' AS PickUp_Location,
					                            '' AS Delivery_Location,
                             					con.WorkDate,
                             					NULL AS CustomerGuid,
                             					NULL AS CustomerLocationGuid,
                                                1 AS FlagChkCustomer,
                             					con.MasterID AS SealNo,
                             					con.Guid AS SealGuid,
			                                    con.Guid AS MasterID_Guid,
                             					NULL AS LiabilityGuid,
                             					NULL AS CommodityGuid,
                             					'' AS Commodity, 								
                             					Items.Liability AS Liability,				
                             					'' AS CurrencyNameAbb, 				
                             					1 AS Qty,
                             					'Con inner layer' AS GroupSeal,
                             					TblMasterRouteGroup_Detail.Guid AS MasterRouteGroupDetail_Guid,
                             					(CASE WHEN TblMasterDailyRunResource.MasterRunResourceShift = 1 
                             						  THEN ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber, '') 
                             						  ELSE ISNULL(TblMasterRouteGroup_Detail.MasterRouteGroupDetailName + ' - ' + TblMasterRunResource.VehicleNumber + ' (' + CONVERT(varchar(1),TblMasterDailyRunResource.MasterRunResourceShift) + ')', '') END)
                             					AS RouteName,
                             					'Seal' AS GroupScanName,
                             					con.MasterDailyRunResource_Guid AS DailyRunGuid,
                             					NULL AS ColumnInReport,
                             					con.OnwardDestination_Guid AS OnwardDestinationGuid,
                             					ISNULL(IntDep.InterDepartmentName, Owd.OnwardDestinationName) AS InternalDepartmentName,
                             					con.ConsolidationRoute_Guid AS ConOuterLayerGuid
                             		FROM TblMasterConAndDeconsolidate_Header con 
                             			 INNER JOIN TblSystemConAndDeconsolidateStatus status on con.SystemCoAndDeSolidateStatus_Guid = status.Guid
                             			 INNER JOIN TblMasterDailyRunResource ON con.MasterDailyRunResource_Guid = TblMasterDailyRunResource.Guid
                             			 INNER JOIN TblMasterRunResource ON TblMasterRunResource.Guid = TblMasterDailyRunResource.MasterRunResource_Guid
                             			 INNER JOIN TblMasterRouteGroup_Detail ON TblMasterRouteGroup_Detail.Guid = TblMasterDailyRunResource.MasterRouteGroup_Detail_Guid
                             			 LEFT JOIN TblMasterCustomerLocation_InternalDepartment IntDep ON CON.OnwardDestination_Guid = IntDep.Guid
                             		     LEFT JOIN TblSystemOnwardDestinationType Owd ON CON.OnwardDestination_Guid = Owd.Guid
              	                         OUTER APPLY
              	                         (
              		                         SELECT	AllItems.FlagMultiBranch
              				                         , MAX(AllItems.SystemStatusJobID) AS SystemStatusJobID
              				                         , AllItems.Con_Guid
              				                         , SUM(STC) AS Liability
              				                         , NULL AS LiaGuid
              				                         , SUM(Total) AS Total
							                         , MAX(AllItems.MasterActualJobHeader_Guid) AS ItemJob_Guid
              						                         FROM(
              							                          select s.MasterActualJobHeader_Guid, s.MasterCustomerLocation_InternalDepartment_Guid, JH.FlagMultiBranch, JH.SystemStatusJobID
              								                            , s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid AS Con_Guid
              								                            , ISNULL(TblMasterActualJobItemsLiability.Liability, 0.00) AS STC
              								                            , TblMasterActualJobItemsLiability.Guid AS LiaGuid
              								                            , COUNT(s.Guid) AS Total
              							                           from TblMasterActualJobItemsSeal s
              								                            INNER JOIN TblMasterActualJobHeader JH ON s.MasterActualJobHeader_Guid = JH.Guid
              								                            LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
              							                           where s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = CON.Guid
              								                            AND s.FlagPartial = 0 
              								                            AND s.FlagSealDiscrepancies = 0
              								                            AND JH.FlagMultiBranch = 1
              							                           group by s.MasterActualJobHeader_Guid
              								                            , s.MasterCustomerLocation_InternalDepartment_Guid
              								                            , JH.FlagMultiBranch
              								                            , JH.SystemStatusJobID
              								                            , s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid
              								                            , TblMasterActualJobItemsLiability.Liability
              								                            , TblMasterActualJobItemsLiability.Guid	
										                          UNION
										                          --== Not calculate value of commodity but using Job_Guid. (Because based on previous SP result, it summarized only seal liability value) ==--
										                          select  c.MasterActualJobHeader_Guid, c.MasterCustomerLocation_InternalDepartment_Guid, JH.FlagMultiBranch, JH.SystemStatusJobID
										                            , c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid AS Con_Guid
										                            , 0 AS STC
										                            --, (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
										                            --	   else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS STC
										                            , NULL AS LiaGuid
										                            , SUM(c.Quantity) AS Total								
										                          from TblMasterActualJobItemsCommodity c							 
										                               INNER JOIN TblMasterActualJobHeader JH ON c.MasterActualJobHeader_Guid = JH.Guid
										                               INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
										                          where c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = CON.Guid
										                               AND c.FlagPartial = 0 
										                               AND c.FlagCommodityDiscrepancies = 0 
										                               AND com.FlagRequireSeal = 0												  
										                          group by c.MasterActualJobHeader_Guid
										                            , c.MasterCustomerLocation_InternalDepartment_Guid
										                            , JH.FlagMultiBranch
										                            , JH.SystemStatusJobID
										                            , c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid
										                            , com.FlagCommodityGlobal
										                            , com.CommodityAmount
										                            , com.CommodityValue
										                            , c.Quantity
										                            --, cc.CommodityAmount
										                            --, cc.CommodityValue	
										                          ) as AllItems 
              		                         GROUP BY  AllItems.FlagMultiBranch , AllItems.Con_Guid		 ) AS Items
                             		WHERE	con.MasterRouteGroup_Detail_Guid IS NOT NULL
                             				AND 1 = case when con.ConsolidationRoute_Guid IS NULL then 1
                                                         when con.ConsolidationRoute_Guid = @ConGuid then 1 else 0 end
                             				AND con.Workdate = @WorkDate
                             				AND con.MasterSite_Guid = @OriginSiteGuid
                             				AND con.Destination_MasterSite_Guid = @DestinationSiteGuid
                             				AND con.MasterSitePathHeader_Guid = @SitePathGuid
                             				AND con.FlagMultiBranch = 1
                                            AND con.SystemConsolidateSourceID = 4
                             				AND con.FlagInPreVault = 1
                             				AND status.StatusID = 2
                             	END
                             
                             	INSERT INTO @TotalJobsAndItems
                             	-- ## ==========   Seal within Liability   =========== ##
                             	SELECT		t.JobGuid,
                             				t.JobNo,
                             				t.ServiceJobTypeNameAbb,
                             				t.PickUp_Location,
                             				t.Delivery_Location,
                             				t.WorkDate,
                             				t.CustomerGuid,
                             				t.CustomerLocationGuid,
                                            t.FlagChkCustomer,
                             				TblMasterActualJobItemsSeal.SealNo,
                             				TblMasterActualJobItemsSeal.Guid AS SealGuid,
			                                NULL AS MasterID_Guid,
                             				TblMasterActualJobItemsLiability.Guid AS LiabilityGuid,
                             				NULL AS CommodityGuid,
                             				CASE WHEN RANK() OVER (PARTITION BY TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid ORDER BY TblMasterActualJobItemsSeal.SealNo) = 1 THEN TblMasterCommodity.CommodityName END  AS Commodity, 								
                             				CASE WHEN RANK() OVER (PARTITION BY TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid ORDER BY TblMasterActualJobItemsSeal.SealNo) = 1 THEN ISNULL(TblMasterActualJobItemsLiability.Liability, 0.00) ELSE 0.00 END AS Liability,				
                             				TblMasterCurrency.MasterCurrencyAbbreviation AS CurrencyNameAbb, 				
                             				1 AS Qty,
                             				'Seal In Liability' AS GroupSeal,
                             				t.MasterRouteGroupDetail_Guid,
                             				t.RouteName,
                             				'Seal' AS GroupScanName,
                             				t.DailyRunGuid,
                             				NULL AS ColumnInReport,
                             				t.OnwardDestinationGuid,
                             				t.InterDepartmentName,
                             				IIF(@IsConRoute = 1, TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid, TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid) AS ConGuid
                             	FROM        TblMasterActualJobItemsSeal 
                             				INNER JOIN @TotalJobsResult AS t on t.JobGuid = TblMasterActualJobItemsSeal.MasterActualJobHeader_Guid
                             				INNER JOIN TblMasterActualJobItemsLiability ON TblMasterActualJobItemsLiability.Guid = TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid
                             				LEFT OUTER JOIN TblMasterCurrency ON TblMasterCurrency.Guid = TblMasterActualJobItemsLiability.MasterCurrency_Guid
                             				LEFT OUTER JOIN TblMasterCommodity ON TblMasterCommodity.Guid = TblMasterActualJobItemsLiability.MasterCommodity_Guid
                             	WHERE		TblMasterActualJobItemsSeal.FlagSealDiscrepancies = 0
                             				AND TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid IS NOT NULL
                             				AND 1 = case when TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL and
                             								  TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 0 AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid = @ConGuid AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 1 AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = @ConGuid AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL then 1
                             							 else 0 end
                             
                             	UNION ALL
                             		-- ## ==========   Only Seals   =========== ##
                             	SELECT		t.JobGuid,
                             				t.JobNo,
                             				t.ServiceJobTypeNameAbb,
                             				t.PickUp_Location,
                             				t.Delivery_Location,
                             				t.WorkDate,
                             				t.CustomerGuid,
                             				t.CustomerLocationGuid,
                                            t.FlagChkCustomer,
                             				TblMasterActualJobItemsSeal.SealNo, 
                             				TblMasterActualJobItemsSeal.Guid AS SealGuid,
			                                NULL AS MasterID_Guid,
                             				NULL AS LiabilityGuid,
                             				NULL AS CommodityGuid,
                             				NULL AS Commodity, 
                             				0.00 AS Liability,
                             				'' AS CurrencyNameAbb,
                             				1 AS Qty,
                             				'Seal Item' AS GroupSeal,
                             				t.MasterRouteGroupDetail_Guid,
                             				t.RouteName,
                             				'Seal' AS GroupScanName,
                             				t.DailyRunGuid,
                             				NULL AS ColumnInReport,
                             				t.OnwardDestinationGuid,
                             				t.InterDepartmentName,
                             				IIF(@IsConRoute = 1, TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid, TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid) AS ConGuid
                             	FROM        TblMasterActualJobItemsSeal 
                             				INNER JOIN @TotalJobsResult AS t on t.JobGuid = TblMasterActualJobItemsSeal.MasterActualJobHeader_Guid
                             	WHERE		TblMasterActualJobItemsSeal.FlagSealDiscrepancies = 0
                             				AND TblMasterActualJobItemsSeal.MasterActualJobItemsCommodity_Guid IS NULL
                             				AND 1 = case when TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL and
                             								  TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 0 AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid = @ConGuid AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 1 AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = @ConGuid AND TblMasterActualJobItemsSeal.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL then 1
                             							 else 0 end
                             
                             	UNION ALL
                             	-- ## ==========   Find Non barcode (Commodity)   =========== ##
                             	SELECT		t.JobGuid,
                             				t.JobNo,
                             				--t.SiteCode,
                             				--t.JobTypeGuid,
                             				t.ServiceJobTypeNameAbb,
                             				t.PickUp_Location,
                             				t.Delivery_Location,
                             				t.WorkDate,
                             				t.CustomerGuid,
                             				t.CustomerLocationGuid,
                                            t.FlagChkCustomer,
                             				NULL AS SealNo, 
                             				NULL AS SealGuid,
			                                NULL AS MasterID_Guid,
                             				NULL AS LiabilityGuid,
                             				TblMasterActualJobItemsCommodity.Guid AS CommodityGuid,
                             				TblMasterCommodity.CommodityName AS Commodity, 
                             				0.00 AS Liability,
                             				'' AS CurrencyNameAbb,
                             				TblMasterActualJobItemsCommodity.Quantity AS Qty,
                             				NULL AS GroupSeal,
                             				t.MasterRouteGroupDetail_Guid,
                             				t.RouteName,
                             				'Non Barcode' AS GroupScanName,
                             				t.DailyRunGuid,
                             				TblMasterCommodity.ColumnInReport AS ColumnInReport,
                             				t.OnwardDestinationGuid,
                             				t.InterDepartmentName,
                             				IIF(@IsConRoute = 1, TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid, TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterID_Guid) AS ConGuid
                             	FROM		@TotalJobsResult AS t 
                             				INNER JOIN TblMasterActualJobItemsCommodity ON t.JobGuid = TblMasterActualJobItemsCommodity.MasterActualJobHeader_Guid
                             				INNER JOIN TblMasterCommodity ON TblMasterActualJobItemsCommodity.MasterCommodity_Guid = TblMasterCommodity.Guid
                             	WHERE		((TblMasterActualJobItemsCommodity.FlagCommodityDiscrepancies = 0) 
                             					OR (TblMasterActualJobItemsCommodity.FlagCommodityDiscrepancies = 1 AND TblMasterActualJobItemsCommodity.Quantity > 0))	
                             				AND 1 = case when TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL and
                             								  TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 0 AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterID_Guid = @ConGuid AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NULL then 1
                             							 when @IsConRoute = 1 AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = @ConGuid AND TblMasterActualJobItemsCommodity.MasterConAndDeconsolidateHeaderMasterID_Guid IS NULL then 1
                             							 else 0 end	
                             
                             	-- ## ==========   Order by work date   =========== ##
                             	SELECT		RESULT.JobGuid								
                             				,RESULT.JobNo							
                             				,RESULT.ServiceJobTypeNameAbb				
                             				,RESULT.PickUp_Location						
                             				,RESULT.Delivery_Location					
                             				,RESULT.WorkDate							
                             				,RESULT.CustomerGuid						
                             				,RESULT.CustomerLocationGuid	
                                            ,RESULT.FlagChkCustomer			
                             				,RESULT.SealNo								
                             				,RESULT.SealGuid	
			                                ,RESULT.MasterID_Guid						
                             				,RESULT.LiabilityGuid						
                             				,RESULT.CommodityGuid						
                             				,RESULT.Commodity							
                             				,RESULT.Liability							
                             				,RESULT.CurrencyNameAbb						
                             				,RESULT.Qty									
                             				,RESULT.GroupSeal						
                             				,RESULT.MasterRouteGroupDetail_Guid			
                             				,RESULT.RouteName							
                             				,RESULT.GroupScanName							
                             				,RESULT.DailyRunGuid	
                             				,RESULT.OnwardDestinationGuid
                             				,RESULT.InterDepartmentName
                             				,CAST( IIF(RESULT.ConOuterLayerGuid IS NOT NULL, 1, 0) AS BIT) AS FlagCheckEdit
                             				,@FlagDisable AS FlagDisable	
                             	FROM		@TotalJobsAndItems RESULT
                             	Order by	 
                             				CASE WHEN RESULT.SealNo IS NULL THEN 1
                             					 WHEN RESULT.SealNo IS NOT NULL THEN 2 END
                             				,RESULT.SealNo
                             				,CASE WHEN RESULT.ColumnInReport IS NOT NULL THEN 3
                             					  WHEN RESULT.ColumnInReport IS NULL THEN 4 END
                             				,RESULT.ColumnInReport,
                             				RESULT.JobNo
                             END";
            #endregion

            using (var dbContext = new OceanDbEntities())
            {
                response = dbContext.Database.SqlQuery<ConAvailableItemView>(
                            sql.ToString()
                            , new SqlParameter("@WorkDate", workDate)
                            , new SqlParameter("@OriginSiteGuid", originSiteGuid)
                            , new SqlParameter("@DestinationSiteGuid", destinationSiteGuid)
                            , new SqlParameter("@SitePathGuid", sitePathGuid)
                            , new SqlParameter("@ConGuid", masterID_Guid)
                            ).ToList();
            }

            return response;
        }

        #region Get by job guid
        public IEnumerable<TblMasterConAndDeconsolidate_Header> GetConsolidateByJobsGuid(IEnumerable<Guid> jobsGuid)
        {
            var consGuid = GetConGuidByJobsGuid(jobsGuid);
            var consolidate = DbContext.TblMasterConAndDeconsolidate_Header
                              .Join(consGuid,
                              con => con.Guid,
                              con1 => con1,
                              (con, con1) => con).AsEnumerable();
            return consolidate;
        }

        public IEnumerable<Guid> GetConGuidByJobsGuid(IEnumerable<Guid> jobsGuid)
        {
            var sealInCon = DbContext.TblMasterActualJobItemsSeal.Where(e => (e.MasterConAndDeconsolidateHeaderMasterID_Guid != null ||
                                                                        e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null))
                            .Join(jobsGuid,
                            s => s.MasterActualJobHeader_Guid,
                            j => j,
                            (s, j) => s);

            var nonInCon = DbContext.TblMasterActualJobItemsCommodity.Where(e => (e.MasterConAndDeconsolidateHeaderMasterID_Guid != null ||
                                                                       e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null))
                           .Join(jobsGuid,
                           n => n.MasterActualJobHeader_Guid,
                           j => j,
                           (n, j) => n);

            var conRouteGuids = sealInCon.Where(e => e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null)
                                .Select(x => (Guid)x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid)
                                .Union(nonInCon.Where(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null)
                                .Select(x => (Guid)x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid)).AsEnumerable();

            var conLocGuids = sealInCon.Where(e => e.MasterConAndDeconsolidateHeaderMasterID_Guid != null)
                              .Select(x => (Guid)x.MasterConAndDeconsolidateHeaderMasterID_Guid)
                              .Union(nonInCon.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid != null)
                              .Select(x => (Guid)x.MasterConAndDeconsolidateHeaderMasterID_Guid)).AsEnumerable();

            return conRouteGuids.Union(conLocGuids);
        }

        public IEnumerable<Guid> GetAllJobGuidInConByJobGuid(IEnumerable<Guid> jobsGuid)
        {
            IEnumerable<Guid> consGuid = GetConGuidByJobsGuid(jobsGuid);
            IEnumerable<Guid> sealInJob = DbContext.TblMasterActualJobItemsSeal.Where(e => consGuid.Any(x => x == e.MasterConAndDeconsolidateHeaderMasterID_Guid)
                                                                         || consGuid.Any(x => x == e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid))
                                                                 .Select(x => (Guid)x.MasterActualJobHeader_Guid).AsEnumerable();

            IEnumerable<Guid> nonInJob = DbContext.TblMasterActualJobItemsCommodity.Where(e => consGuid.Any(x => x == e.MasterConAndDeconsolidateHeaderMasterID_Guid)
                                                                        || consGuid.Any(x => x == e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid))
                                                                 .Select(x => (Guid)x.MasterActualJobHeader_Guid).AsEnumerable();

            return sealInJob.Union(nonInJob);
        }

        public bool HasJobNotInCon(IEnumerable<Guid> jobsGuid)
        {
            IEnumerable<Guid> jobsInCon = GetAllJobGuidInConByJobGuid(jobsGuid);
            return jobsInCon.Except(jobsGuid).Any();
        }
        #endregion

        #region TruckToTruck
        public void UpdateConToNewDailyRun(IEnumerable<Guid> jobsGuid, TblMasterDailyRunResource newDailyRun, DateTime datetimeModified, string userModify, DateTimeOffset universalDatetimeModified)
        {
            var conGuids = GetConGuidByJobsGuid(jobsGuid).ToList();
            if (conGuids.Any())
            {
                var conData = FindByMasterID_Guids(conGuids);
                foreach (var item in conData)
                {
                    item.MasterDailyRunResource_Guid = newDailyRun.Guid;
                    item.MasterRouteGroup_Detail_Guid = item.MasterRouteGroup_Detail_Guid != null ? newDailyRun.MasterRouteGroup_Detail_Guid : null; //Has value for route consolidate.
                    item.DatetimeModified = datetimeModified;
                    item.UserModifed = userModify;
                    item.UniversalDatetimeModified = universalDatetimeModified;
                }
                DbContext.SaveChanges();
            }
        }
        #endregion

    }

}
