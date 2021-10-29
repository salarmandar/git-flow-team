using Bgt.Ocean.Infrastructure.Helpers;
using System.Configuration;
using System.Text;

namespace Bgt.Ocean.Repository.EntityFramework.StringQuery.Masterroute
{
    public class UpdateSeqIndexQuery
    {

        public string QueryUpdateSeqIndex { get { return SetQueryUpdate(); } }
        public string QueryGetMasterRouteJob { get { return SetQueryGetMasterRouteJob(); } }
        public string QueryUpdateMassUpdate { get { return SetQueryUpdateMassUpdate(); } }
        public string QueryCheckMassUpdateJobUnderOptimize { get { return GetQueryCheckMassUpdateJobUnderOptimize(); } }
        public string schema { get { return ConfigurationManager.AppSettings["EnvSTG"].ToBoolean() ? "[stg]" : "[dbo]"; } }

        private string SetQueryUpdateMassUpdate()
        {
            StringBuilder queryStr = new StringBuilder();
            queryStr.Append(@"
                         BEGIN TRY
	                     DECLARE 
									@CountServiceType	INT,
									@CountDayOfWeek		INT,
									@CountLob			INT,
									@CheckTypeJob		INT = (CAST( @IsDeliveryLocation AS INT) +CAST( @IsPickUpLocation AS INT)),
									@IsJobInterBranch	Bit = IIF(@PickUpSiteGuid = @DeliverySiteGuid,0,1),
									@UtcDate		    Datetimeoffset = getutcdate(),
									@AllowContract      bit = 0,
									@CountContract      int
								DECLARE
									@TmpLob		TABLE(
										Guid			UNIQUEIDENTIFIER,
										LobNameAbb		nvarchar(50)
									)
								DECLARE
									@TmpServiceType		TABLE(
										Guid			UNIQUEIDENTIFIER,
										ServiceTypeId	Int,
										ServiceTypeNameAbb	nvarchar(50)

									)
								DECLARE
									@TmpDayOfWeek		TABLE(
										Guid			UNIQUEIDENTIFIER,
										DayOfWeekSeq	INT,
										WeekName		nvarchar(50)
									)

								DECLARE
									@TmpWeekType		TABLE(
										Guid			UNIQUEIDENTIFIER,
										WeekTypeName	nvarchar(50)
									)
								DECLARE 
									@TmpMinSeviceHour	TABLE(
										ServiceJobTypeGuid			UNIQUEIDENTIFIER,
										LobGuid					UNIQUEIDENTIFIER,
										DayOfWeekSeq			Int,
										ServiceHour				Datetime
									)
								DECLARE 
									@TmpMaxSeviceHour	TABLE(
										ServiceJobTypeGuid		UNIQUEIDENTIFIER,
										LobGuid					UNIQUEIDENTIFIER,
										DayOfWeekSeq			Int,
										ServiceHour				Datetime
									)
								DECLARE @TblServiceTypeExclude	TABLE(
										ServiceTypeIdExclude  int
								)
								INSERT INTO @TblServiceTypeExclude VALUES (14),(15),(16)
	
								INSERT INTO @TmpMinSeviceHour
									SELECT SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence,min(ServiceHourStart) 
									FROM TblMasterCustomerLocation_ServiceHour sh
									INNER JOIN TblSystemDayOfWeek dow ON sh.SystemDayOfWeek_Guid = dow.Guid
									WHERE MasterCustomerLocation_Guid = @LocReplaceGuid
									GROUP BY SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence

								INSERT INTO @TmpMaxSeviceHour
									SELECT SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence,Max(ServiceHourStop) 
									FROM TblMasterCustomerLocation_ServiceHour sh
									INNER JOIN TblSystemDayOfWeek dow ON sh.SystemDayOfWeek_Guid = dow.Guid
									WHERE MasterCustomerLocation_Guid = @LocReplaceGuid 
									GROUP BY SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence


                               {0}
	/*

								INSERT INTO @TmpLob 
								select Guid,LOBAbbrevaitionName from TblSystemLineOfBusiness  -- where LOBAbbrevaitionName = 'CIT'

								INSERT INTO @TmpServiceType
								SELECT GUID,ServiceJobTypeID,ServiceJobTypeNameAbb FROM TblSystemServiceJobType where ServiceJobTypeID in (2,3)

								INSERT INTO @TmpDayOfWeek 
								SELECT Guid,MasterDayOfWeek_Sequence,MasterDayOfWeek_Name FROM TblSystemDayOfWeek -- where MasterDayOfWeek_Sequence =2

                         INSERT INTO @TmpWeekType 
                            SELECT Guid,WeekTypeName FROM TblSystemMaterRouteTypeOfWeek WHERE WeekTypeInt IN (1,2,3)
	*/							
							

								-- Validate exclude multi branch 
								DELETE @TmpServiceType WHERE EXISTS(Select * from @TblServiceTypeExclude WHERE ServiceTypeId = ServiceTypeIdExclude)

								SELECT @CountServiceType = COUNT(1) FROM @TmpServiceType
								SELECT @CountDayOfWeek = COUNT(1) FROM @TmpDayOfWeek
								SELECT @CountLob = COUNT(1) FROM @TmpLob

								IF OBJECT_ID (N'tempdb..#TmpJobTarget', N'U')
										 IS NOT NULL
									  DROP TABLE #TmpJobTarget;

								IF OBJECT_ID (N'tempdb..#TmpJobMultiStops', N'U')
									  IS NOT NULL
								   DROP TABLE #TmpJobMultiStops;
								
								IF OBJECT_ID (N'tempdb..#TmpTblResponse', N'U')
									  IS NOT NULL
								   DROP TABLE #TmpTblResponse;
								

								CREATE TABLE [dbo].#TmpJobTarget (
									LegGuid							Uniqueidentifier,
									JobHeadGuid						Uniqueidentifier,
									MasterRoutGuid					Uniqueidentifier,
									MasterRoutDeliveryGuid			Uniqueidentifier,
									RouteGroupDetailGuid			Uniqueidentifier,
									LocationGuid					Uniqueidentifier,
									MasterSiteGuid					Uniqueidentifier,
									DayOfWeekSeq					Int,
									LobGuid							Uniqueidentifier,
									ServiceJobTypeGuid				Uniqueidentifier,
									ServiceJobTypeId				int,
									AcitonGuid						Uniqueidentifier,
									ActionNameAbb					Nvarchar(10) collate database_default,
									SchduleTime						Datetime,
									FlagInterBranch					Bit,
									SeqStop							int,
									JobOrder						int,
									InsertFrom						nvarchar(50) collate database_default
								)


								CREATE TABLE #TmpJobMultiStops (
									LegGuid							Uniqueidentifier,
									JobHeadGuid						Uniqueidentifier,
									MasterRoutGuid					Uniqueidentifier,
									MasterRoutDeliveryGuid			Uniqueidentifier,
									RouteGroupDetailGuid			Uniqueidentifier,
									LocationGuid					Uniqueidentifier,
									MasterSiteGuid					Uniqueidentifier,
									DayOfWeekSeq					Int,
									LobGuid							Uniqueidentifier,
									ServiceJobTypeGuid				Uniqueidentifier,
									ServiceJobTypeId				int,
									AcitonGuid						Uniqueidentifier,
									ActionNameAbb					Nvarchar(10) collate database_default,
									SchduleTime						Datetime,
									FlagInterBranch					Bit,
									SeqStop							int,
									JobOrder						int
									
								)
								CREATE TABLE #TmpTblResponse (
								MasterRouteGuid				Uniqueidentifier,
								RouteGroupDetailGuid		Uniqueidentifier
								)
								

							
								/*P , D*/

	
									INSERT INTO #TmpJobMultiStops(
										LegGuid					,
										JobHeadGuid				,
										MasterRoutGuid			,
										MasterRoutDeliveryGuid	,
										RouteGroupDetailGuid	,
										LocationGuid			,
										MasterSiteGuid			,
										DayOfWeekSeq			,
										LobGuid					,
										ServiceJobTypeGuid		,
										ServiceJobTypeId		,
										AcitonGuid				,
										ActionNameAbb			,
										SchduleTime				,
										FlagInterBranch			,
										SeqStop					,
										JobOrder						
										
								)
									SELECT	L.Guid,
											H.Guid,
											r.Guid,
											l.MasterRouteDeliveryLeg_Guid,
											l.MasterRouteGroupDetail_Guid,
											L.MasterCustomerLocation_Guid,
											L.MasterSite_Guid,
											L.DayOfWeek_Sequence,
											h.SystemLineOfBusiness_Guid,
											h.SystemServiceJobType_Guid,
											ty.ServiceJobTypeID,
											a.Guid,
											a.ActionNameAbbrevaition ,
											l.SchduleTime,
											h.FlagJobInterBranch,
											l.SequenceStop,
											l.JobOrder
										
									FROM TblMasterRouteJobHeader H
									INNER JOIN TblSystemServiceJobType ty ON ty.Guid = h.SystemServiceJobType_Guid AND 1= (CASE	WHEN EXISTS (SELECT * FROM @TmpServiceType where Guid = H.SystemServiceJobType_Guid) THEN 1  -- JobType
																																WHEN @CountServiceType = 0 AND EXISTS(SELECT * FROM TblSystemServiceJobType tt WHERE tt.ServiceJobTypeID IN (0,1,2,3,4,5,11,12) AND tt.Guid = H.SystemServiceJobType_Guid) THEN 1 ELSE 0 END) --AND ServiceJobTypeID IN (2,3)
																			AND 1 =(CASE	WHEN EXISTS (SELECT * FROM @TmpLob WHERE Guid = h.SystemLineOfBusiness_Guid) THEN 1 -- LOB.
																							WHEN @CountLob = 0 THEN 1 ELSE 0 END)
									INNER JOIN TblMasterRoute R  ON H.FlagDisable = 0 AND R.Guid = H.MasterRoute_Guid 
																					AND EXISTS(SELECT * FROM @TmpWeekType WHERE Guid = r.SystemMaterRouteTypeOfWeek_Guid) AND (FlagHoliday = @FlagHolliday OR FlagHoliday = 0)
																						AND 1 = (CASE	WHEN EXISTS(SELECT * FROM @TmpDayOfWeek WHERE Guid = R.MasterDayOfweek_Guid) THEN 1 
																										WHEN @CountDayOfWeek = 0 THEN 1
																										ELSE 0
																								END)
									INNER JOIN TblMasterRouteJobServiceStopLegs L ON H.Guid = L.MasterRouteJobHeader_Guid  														
																						AND 'Action' = (CASE	
																												WHEN @IsPickUpLocation = 1 AND @PickupLocGuid =L. MasterCustomerLocation_Guid AND L.SequenceStop % 2 = 1 AND L.MasterSite_Guid = @PickUpSiteGuid AND  h.FlagJobInterBranch = @IsJobInterBranch THEN 'Action' 
																												WHEN @IsPickUpLocation = 1 AND @PickupLocGuid IS NULL AND L.SequenceStop % 2 = 1 AND L.MasterSite_Guid = @PickUpSiteGuid THEN 'Action'
																												-- Delivery 
																												WHEN @IsDeliveryLocation = 1 AND @DeliveryLocaGuid = L. MasterCustomerLocation_Guid AND L.SequenceStop % 2 = 0 AND L.MasterSite_Guid = @DeliverySiteGuid   
                                                                                                                        AND 1 = (CASE	WHEN @PickUpSiteGuid = @DeliverySiteGuid THEN 1  
																																	WHEN @PickUpSiteGuid != @DeliverySiteGuid AND ServiceJobTypeID = 2 AND H.FlagJobInterBranch = 1 THEN 1
																															END) THEN 'Action'
																												WHEN @IsDeliveryLocation = 1 AND @DeliveryLocaGuid IS NULL AND L.SequenceStop % 2 = 0 AND L.MasterSite_Guid = @DeliverySiteGuid 
																													AND 1 = (CASE	WHEN @PickUpSiteGuid = @DeliverySiteGuid THEN 1  
																																	WHEN @PickUpSiteGuid != @DeliverySiteGuid AND ServiceJobTypeID = 2 AND H.FlagJobInterBranch = 1 THEN 1
																															END) THEN 'Action'																				                          
																												WHEN @IsDeliveryLocation = 0 AND L.SequenceStop = 3 AND ServiceJobTypeID = 0 AND L.MasterSite_Guid = @DeliverySiteGuid AND h.FlagJobInterBranch = 1 THEN 'Action' --P Inter branch.

																										END)																							
									INNER JOIN TblSystemJobAction A ON A.Guid = L.CustomerLocationAction_Guid 

									/*========================= P,D Same site ===============================*/
									INSERT INTO #TmpJobTarget
									SELECT *,'step 1' FROM #TmpJobMultiStops WHERE ServiceJobTypeId NOT IN (2,3) AND FlagInterBranch = 0
	
									/*============================ PickUp Inter Branch ============================*/
									IF(@PickUpSiteGuid <> @DeliverySiteGuid)
									BEGIN                          
		
										INSERT #TmpJobTarget
												SELECT 
													P.LegGuid					PLegGuid ,
													P.JobHeadGuid				PJobHeadGuid ,
													P.MasterRoutGuid			PMasterRoutGuid ,
													P.MasterRoutDeliveryGuid	PMasterRoutDeliveryGuid ,
													P.RouteGroupDetailGuid		PRouteGroupDetailGuid ,
													P.LocationGuid				PLocationGuid ,
													P.MasterSiteGuid			PMasterSiteGuid,
													P.DayOfWeekSeq				PDayOfWeekSeq ,
													P.LobGuid					PLobGuid 	,
													P.ServiceJobTypeGuid		PServiceJobTypeGuid 	,
													P.ServiceJobTypeId			PServiceJobTypeId ,
													P.AcitonGuid				PAcitonGuid 	,
													P.ActionNameAbb				PActionNameAbb ,
													P.SchduleTime				PSchduleTime	,
													P.FlagInterBranch			PFlagInterBranch	, 
													P.SeqStop					PSeqStop,
													P.JobOrder					PJobOrder,
													'Step 2-P_inter'
												FROM #TmpJobMultiStops p 
												INNER JOIN #TmpJobMultiStops D ON P.JobHeadGuid = D.JobHeadGuid  AND P.LegGuid <> D.LegGuid
												WHERE p.SeqStop = 1 and d.SeqStop = 3 
														AND p.ServiceJobTypeId = 0 AND D.ServiceJobTypeId =0 
														AND p.FlagInterBranch =1 AND d.FlagInterBranch = 1 
														AND p.MasterSiteGuid = @PickUpSiteGuid 
														and d.MasterSiteGuid = @DeliverySiteGuid
		
											--INSERT #TmpJobTarget
			
				

									END

									/*================= T,TV ======================*/
									IF(EXISTS(SELECT TOP 1 1 FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3)))
									BEGIN
										IF((SELECT COUNT(1) FROM (SELECT SeqStop FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3) GROUP BY SeqStop) c) = 1)
										BEGIN
			
											--INSERT INTO #TmpJobTarget
											--SELECT * ,'Step 2-1' FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3) 
											--									AND	 1 = CASE	WHEN ServiceJobTypeId =3 THEN 1 -- T
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 1 THEN 1 -- Tv[P]
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 0 AND FlagInterBranch = 0 AND @PickupLocGuid = @DeliverySiteGuid THEN 1 --Tv[D] Same
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 0 AND FlagInterBranch = 1 AND @PickupLocGuid != @DeliverySiteGuid THEN 1 -- Tv[D] Interbranch
											--										  END 
											print '1'
			

										END
										ELSE
										BEGIN

			
											;WITH JobTSet as(
												SELECT 
													P.LegGuid					PLegGuid ,
													P.JobHeadGuid				PJobHeadGuid ,
													P.MasterRoutGuid			PMasterRoutGuid ,
													P.MasterRoutDeliveryGuid	PMasterRoutDeliveryGuid ,
													P.RouteGroupDetailGuid		PRouteGroupDetailGuid ,
													P.LocationGuid				PLocationGuid ,
													P.MasterSiteGuid			PMasterSiteGuid,
													P.DayOfWeekSeq				PDayOfWeekSeq ,
													P.LobGuid					PLobGuid 	,
													P.ServiceJobTypeGuid		PServiceJobTypeGuid 	,
													P.ServiceJobTypeId			PServiceJobTypeId ,
													P.AcitonGuid				PAcitonGuid 	,
													P.ActionNameAbb				PActionNameAbb ,
													P.SchduleTime				PSchduleTime	,
													P.FlagInterBranch			PFlagInterBranch	, 
													P.SeqStop					PSeqStop	,
													P.JobOrder					PJobOrder, 
													D.LegGuid					DLegGuid	,
													D.JobHeadGuid				DJobHeadGuid	,
													D.MasterRoutGuid			DMasterRoutGuid,
													D.MasterRoutDeliveryGuid	DMasterRoutDeliveryGuid,
													D.RouteGroupDetailGuid		DRouteGroupDetailGuid,
													D.LocationGuid				DLocationGuid,
													D.MasterSiteGuid			DMasterSiteGuid,
													D.DayOfWeekSeq				DDayOfWeekSeq,
													D.LobGuid					DLobGuid	,
													D.ServiceJobTypeGuid		DServiceJobTypeGuid	,
													D.ServiceJobTypeId			DServiceJobTypeId,
													D.AcitonGuid				DAcitonGuid	,
													D.ActionNameAbb				DActionNameAbb,
													D.SchduleTime				DSchduleTime,
													D.FlagInterBranch			DFlagInterBranch,
													D.SeqStop				    DSeqStop, 
													D.JobOrder					DJobOrder
													
												FROM #TmpJobMultiStops p 
												INNER JOIN #TmpJobMultiStops D ON P.JobHeadGuid = D.JobHeadGuid  AND P.LegGuid <> D.LegGuid
												WHERE p.SeqStop = 1 and d.SeqStop = 2 AND p.ServiceJobTypeId IN (2,3) AND D.ServiceJobTypeId in (2,3)
											)
											INSERT #TmpJobTarget
												SELECT 
													PLegGuid				,
													PJobHeadGuid			,
													PMasterRoutGuid			,
													PMasterRoutDeliveryGuid	,
													PRouteGroupDetailGuid	,
													PLocationGuid			,
													PMasterSiteGuid			,
													PDayOfWeekSeq			,
													PLobGuid				,
													PServiceJobTypeGuid		,
													PServiceJobTypeId		,
													PAcitonGuid				,
													PActionNameAbb			,
													PSchduleTime			,
													PFlagInterBranch		,
													PSeqStop				,
													PJobOrder				,
													'Step 2-P'
												FROM JobTSet 
												UNION 
												SELECT 
													DLegGuid				,
													DJobHeadGuid			,
													DMasterRoutGuid			,
													DMasterRoutDeliveryGuid	,
													DRouteGroupDetailGuid	,
													DLocationGuid			,
													DMasterSiteGuid			,
													DDayOfWeekSeq			,
													DLobGuid					,
													DServiceJobTypeGuid		,
													DServiceJobTypeId		,
													DAcitonGuid				,
													DActionNameAbb			,
													DSchduleTime			,
													DFlagInterBranch		,
													DSeqStop				,
													DJobOrder				,
													'Step 2-D'
												FROM JobTSet
			
		
											--order by pJobHeadGuid,PSeqStop
										END
									END

								DECLARE 
									@TotalJobTarget				Int,
									@TotalJobWithServiceHour	Int
								SELECT @TotalJobTarget = COUNT(1)  FROM #TmpJobTarget t	

								SELECT  @TotalJobWithServiceHour = COUNT(1)
								FROM #TmpJobTarget t 
								INNER JOIN @TmpMinSeviceHour minH ON t.DayOfWeekSeq = minh.DayOfWeekSeq AND T.ServiceJobTypeGuid = minH.ServiceJobTypeGuid AND t.LobGuid = minH.LobGuid  AND ( T.SchduleTime >= minH.ServiceHour OR T.SchduleTime= '1900-01-01 00:00:00.000')
								INNER JOIN @TmpMaxSeviceHour maxH ON t.DayOfWeekSeq = maxH.DayOfWeekSeq AND T.ServiceJobTypeGuid = maxH.ServiceJobTypeGuid AND t.LobGuid = maxH.LobGuid AND (T.SchduleTime <= maxH.ServiceHour OR T.SchduleTime= '1900-01-01 00:00:00.000')

								SELECT 
								@CountContract = COUNT(	1)
								FROM TblMasterCustomerContract CustomerContract
								INNER JOIN TblMasterCustomerContract_ServiceLocation CTS 
										ON     CustomerContract.Guid = CTS.MasterCustomerContract_Guid
										AND CustomerContract.FlagDisable = 0
								INNER JOIN TblMasterCustomerLocation_BrinksSite Loc on CTS.MasterCustomerLocation_Guid = Loc.MasterCustomerLocation_Guid
								WHERE (   CustomerContract.ContractExpiredDate >= getdate()
										OR CustomerContract.ContractExpiredDate IS NULL) and CTS.MasterCustomerLocation_Guid = @LocReplaceGuid  
										AND MasterSite_Guid = @SiteReplaceGuid

								SELECT @AllowContract=  AppValue1
								FROM [dbo].[GetCountryOption]('FlagAllowCreateJobWithoutContract',@SiteReplaceGuid, NULL)
                                
                                DECLARE 
									@CheckDestination int =0,
									@DestinationOfReplaceLoca	Uniqueidentifier
								
								/*==================Validate Destination location==================*/
								IF @ReplaceType = 'D' AND EXISTS(SELECT TOP 1 * FROM #TmpJobTarget WHERE ServiceJobTypeId IN(2,3))
								BEGIN
									SELECT TOP 1 @CheckDestination = COUNT(1) FROM  #TmpJobTarget t
									OUTER APPLY(
										SELECT 
										SUM(IIF(d.MasterCustomerLocationDes_Guid = @LocReplaceGuid,1,0)) AS X 
										FROM TblMasterCustomerLocation_LocationDestination d 
										WHERE  d.MasterCustomerLocation_Guid = t.LocationGuid-- AND d.MasterCustomerLocationDes_Guid = @LocReplaceGuid
									) dest
									WHERE ServiceJobTypeId IN (2,3) AND SeqStop = 1  and x = 0 
								END
								ELSE IF  @ReplaceType = 'P' AND EXISTS(SELECT TOP 1 * FROM #TmpJobTarget WHERE ServiceJobTypeId IN(2,3))
								BEGIN	
									IF (EXISTS (SELECT TOP 1 MasterCustomerLocationDes_Guid 
												FROM TblMasterCustomerLocation_LocationDestination 
												WHERE MasterCustomerLocation_Guid = @LocReplaceGuid AND MasterCustomerLocation_InternalDepartment_Guid IS NULL)
										)
									BEGIN										
										SELECT TOP 1 @CheckDestination = COUNT(1) FROM #TmpJobTarget WHERE ServiceJobTypeId IN (2,3) AND SeqStop = 2 
										 AND LocationGuid not in (
											SELECT MasterCustomerLocationDes_Guid 
											FROM TblMasterCustomerLocation_LocationDestination 
											WHERE MasterCustomerLocation_Guid = @LocReplaceGuid AND MasterCustomerLocation_InternalDepartment_Guid IS NULL
										)
										GROUP BY LocationGuid	
									END
								END

								IF(@TotalJobTarget = @TotalJobWithServiceHour AND (@CountContract> 0 OR @AllowContract = 'true') AND @CheckDestination = 0)
								BEGIN	
									BEGIN TRAN
									--select * from TblMasterHistory_MasterRouteJOB
									Declare @param nvarchar(max),
											@CustomerContract	Uniqueidentifier
									SET @param='Pickup : [' +IIF(@IsPickUpLocation=0,'Brink''s','Location' )+']  Delivery : ['+IIF(@IsDeliveryLocation=0,'Brink''s','Location' )+']  Day of week :['+STUFF((
											SELECT ' ' + WeekName
											FROM @TmpDayOfWeek
											FOR XML PATH('')
											), 1, 1, '')+'] Lob : ['+
											STUFF((
											SELECT ' ' + LobNameAbb
											FROM @TmpLob
											FOR XML PATH('')
											), 1, 1, '')+'] ServiceJobType : ['+
											STUFF((
											SELECT ' ' + ServiceTypeNameAbb
											FROM @TmpServiceType
											FOR XML PATH('')
											), 1, 1, '')+
											'] Week Type :['+
											STUFF((
											SELECT ' ' + WeekTypeName
											FROM @TmpWeekType
											FOR XML PATH('')
											), 1, 1, '')
											+'] '+IIF(@FlagHolliday = 0 ,'','Holiday : [Check]')+']'
									
									INSERT INTO TblMasterHistory_MasterRouteJob
									SELECT NEWID(),ISNULL(MasterRoutDeliveryGuid,MasterRoutGuid),JobHeadGuid,LegGuid, '867',CONCAT(LocationGuid,',',@LocReplaceGuid,',',@param,',',@UserModify),@UserModify,@ClientDate,@UtcDate 
									FROM #TmpJobTarget where ActionNameAbb = @ReplaceType
							
									SELECT 
									@CustomerContract =CTS.MasterCustomerLocation_Guid
									FROM TblMasterCustomerContract CustomerContract

									INNER JOIN TblMasterCustomerContract_ServiceLocation CTS 
											ON     CustomerContract.Guid = CTS.MasterCustomerContract_Guid
											AND CustomerContract.FlagDisable = 0
									INNER JOIN TblMasterCustomerLocation Loc on CTS.MasterCustomerLocation_Guid = Loc.Guid
									WHERE (   CustomerContract.ContractExpiredDate >= getdate()
											OR CustomerContract.ContractExpiredDate IS NULL) 
											and CTS.MasterCustomerLocation_Guid = @LocReplaceGuid
											and MasterSite_Guid = @SiteReplaceGuid


									UPDATE TblMasterRouteJobHeader
									SET MasterCustomerContract_Guid = @CustomerContract,
                                        OnwardDestinationType = NULL,	
										OnwardDestination_Guid = NULL,
										UserModifed = @UserModify,
										DatetimeModified = @ClientDate,
										UniversalDatetimeModified = @UtcDate								
									WHERE EXISTS (SELECT LegGuid FROM #TmpJobTarget where ActionNameAbb = @ReplaceType and Guid = LegGuid)

									UPDATE TblMasterRouteJobServiceStopLegs 
									SET MasterCustomerLocation_Guid = @LocReplaceGuid , 
										MasterSite_Guid = @SiteReplaceGuid	,
                                        SeqIndex = JobOrder +9999,
                                        JobOrder = 0
									
									WHERE EXISTS (SELECT LegGuid FROM #TmpJobTarget where ActionNameAbb = @ReplaceType and Guid = LegGuid)
									
														INSERT INTO #TmpTblResponse(MasterRouteGuid,RouteGroupDetailGuid)
									SELECT DISTINCT ISNULL(MasterRoutDeliveryGuid,MasterRoutGuid) As MasterRouteGuid,RouteGroupDetailGuid 
									
									FROM #TmpJobTarget where ActionNameAbb = @ReplaceType
                                    

                                    -- Audit Log
                                    DECLARE 
                                    @NullToken UNIQUEIDENTIFIER = NEWID(),
                                    @SystemLogProcessGuid UNIQUEIDENTIFIER,                             
                                    @New_LocationGuid UNIQUEIDENTIFIER,
                                    @New_LocationName NVARCHAR(200),
                                    @ProcessCode      NVARCHAR(200) = 'MASTER_ROUTE_JOB',
                                    @CategoryCode     NVARCHAR(200) = 'MRJ_Mass_Update'

                                    SET @SystemLogProcessGuid			= (SELECT TOP 1 GUID FROM [sfo].SFOTblSystemLogProcess WHERE ProcessCode = @ProcessCode)

                                    SELECT 
                                     @New_LocationGuid = Loc.Guid							
                                    ,@New_LocationName = Cus.CustomerFullName+' - '+ Loc.BranchName 
                                    FROM TblMasterCustomerLocation Loc 
                                    INNER JOIN TblMasterCustomer Cus ON Loc.MasterCustomer_Guid = Cus.Guid AND Loc.Guid = @LocReplaceGuid

                                    ;WITH jobs AS(
                                    SELECT * FROM
	                                    (
	                                    SELECT 
	                                     Loc.Guid							AS Old_LocationGuid
	                                    ,Cus.CustomerFullName+' - '+ Loc.BranchName AS Old_LocationName
	                                    ,T.JobHeadGuid						AS MasterJobGuid
	                                    ,T.JobOrder							AS JobOrder
	                                    FROM #TmpJobTarget T
	                                    INNER JOIN TblMasterCustomerLocation Loc ON Loc.Guid  = T.LocationGuid
	                                    INNER JOIN TblMasterCustomer Cus ON Loc.MasterCustomer_Guid = Cus.Guid 
                                        WHERE T.ActionNameAbb = @ReplaceType
	                                    ) j,
	                                    -- CATEGORY
	                                    (SELECT  GUID			AS SystemLogCategoryGuid
			                                    ,SeqIndexShow 
			                                    ,CASE  
				                                    WHEN SeqIndexShow = 1 THEN 5095
				                                    WHEN SeqIndexShow = 2 THEN 5096
				                                    WHEN SeqIndexShow = 3 THEN 5097
			                                    END AS SystemMsgID
		                                    FROM [sfo].SFOTblSystemLogCategory 
		                                    WHERE CategoryCode = @CategoryCode 
                                                  AND 1 = CASE	WHEN @ReplaceType = 'P' AND SeqIndexShow IN (1,2) THEN 1 
                                                                WHEN @ReplaceType = 'D' AND SeqIndexShow IN (1,3) THEN 1 
					                                      ELSE 0 END 
	                                    ) c
                                    )
                                    INSERT INTO TblMasterRouteTransactionLog
                                    (Guid,SystemLogCategory_Guid,SystemLogProcess_Guid,ReferenceValue_Guid,UserCreated,DateTimeCreated,UniversalDatetimeCreated,Remark,SystemMsgID,JSONValue)
                                    SELECT
                                     NEWID()					AS GUID
                                    ,j.SystemLogCategoryGuid	AS SystemLogCategory_Guid
                                    ,@SystemLogProcessGuid		AS SystemLogProcess_Guid
                                    ,j.MasterJobGuid			AS ReferenceValue_Guid
                                    ,@UserModify				AS UserCreated
                                    ,@ClientDate				AS DateTimeCreated
                                    ,@UtcDate					AS UniversalDatetimeCreated
                                    ,'Master Job Mass Update'	AS Remark
                                    ,j.SystemMsgID				AS SystemMsgID
                                    ,m.JSONValue				AS JSONValue
                                    FROM jobs j
                                    OUTER APPLY(
                                    SELECT CASE		WHEN j.SystemMsgID = 5095 
				                                    THEN  FORMATMESSAGE(@msg_3p
					                                    ,IIF(@ReplaceType = 'P','Pickup Leg','Delivery Leg') + ' Location'
					                                    ,CAST(ISNULL(j.Old_LocationName,'-')				AS nvarchar(200))
					                                    ,CAST(ISNULL(@New_LocationName,'-')				    AS nvarchar(200)))
				                                    WHEN j.SystemMsgID = 5096 
				                                    THEN  FORMATMESSAGE(@msg_1p
					                                    ,CAST(ISNULL(j.JobOrder,'-')						AS nvarchar(200)))
				                                    WHEN j.SystemMsgID = 5097
				                                    THEN  FORMATMESSAGE(@msg_1p
					                                    ,CAST(ISNULL(j.JobOrder,'-')						AS nvarchar(200)))
		                                    END
                                    AS JSONValue
                                    --Skip Unchanged
                                    ,CASE WHEN ISNULL(J.Old_LocationGuid,@NullToken) = ISNULL(@New_LocationGuid,@NullToken)
                                    THEN 1
                                    ELSE 0 END AS FlagUnchanged
                                    ) m
                                    WHERE m.FlagUnchanged = 0

									/*Show data*/
									SELECT * FROM #TmpTblResponse
									SELECT 0 as MsgID ,'Success' AS MsgDetail	

								--
									COMMIT TRAN
								END
								ELSE
								BEGIN       
									SELECT * FROM #TmpTblResponse                         
									SELECT -790 as MsgID ,'Record not equal' AS MsgDetail
								END
			END TRY
			BEGIN CATCH
					ROLLBACK TRAN
							SELECT * FROM #TmpTblResponse
							SELECT -184 AS 'MsgId' ,ERROR_MESSAGE() AS 'Msg'
							INSERT INTO TblSystemLog_HistoryError
								(GUID ,ErrorDescription ,FunctionName ,PageName ,InnerError,ClientIP ,ClientName ,DatetimeCreated,FlagSendEmail)
							SELECT								
								Guid = NEWID(),
								ErrorDescription  =ERROR_MESSAGE(),
								FunctionName = 'Update master-route SeqIndex ',
								PageName = 'MasterRoute [Re-SeqInde]',
								InnerError =CONCAT('Error : [Id : ',ERROR_NUMBER(),' Line : ',ERROR_LINE(),' Message : ',ERROR_MESSAGE() ,']'),
								ClientIP = @@SERVERNAME,
								ClientName = '',
								DatetimeCreated =GETUTCDATE(),
								FlagSendEmail = 1
				
                        IF OBJECT_ID (N'tempdb..#TmpJobTarget', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpJobTarget;
                        IF OBJECT_ID (N'tempdb..#TmpJobMultiStops', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpJobMultiStops; 
                        IF OBJECT_ID (N'tempdb..#TmpTblResponse', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpTblResponse;
            END CATCH                          
                        IF OBJECT_ID (N'tempdb..#TmpJobTarget', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpJobTarget;

                        IF OBJECT_ID (N'tempdb..#TmpJobMultiStops', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpJobMultiStops; 
                        IF OBJECT_ID (N'tempdb..#TmpTblResponse', N'U')
                        IS NOT NULL
                        DROP TABLE #TmpTblResponse;");
            return queryStr.ToString();
        }

        private string SetQueryGetMasterRouteJob()
        {
            StringBuilder queryStr = new StringBuilder();
            queryStr.Append(@"BEGIN TRY ");
            queryStr.Append(@"DECLARE @temp	table (		
				                    LegGuid							uniqueidentifier															
				                    ,LocationName						nvarchar(600)
				                    ,JobOrder							int
				                    ,SeqIndex							int
				                    ,MasterRouteGroupDetail_Guid		uniqueidentifier										
				                    ,SequenceStop						int
				                    ,JobTypeNameAbb						nvarchar(10),
				                    JobAction							nvarchar(10),
				                    OnwardType					int,
				                    OnwardName							nvarchar(400) default('')
				                    )
                            DECLARE @TmpSourceJob TABLE (
				                    Guid							uniqueidentifier,	
				                    SeqIndex							int
				            )	

                            DECLARE
                                    @MasterSiteGuid                             uniqueidentifier = null,
                                    @StrMasterDayOfWeek_Sequence                nvarchar(100) = null,
                                    @DefalutGuid                                uniqueidentifier
                            DECLARE @TmpRouteGroupDetail

                                    TABLE(
                                        RouteGrouptDetailGuid               uniqueidentifier
                                    )
                            BEGIN
                                --SET ANSI_WARNINGS OFF;
                                        SELECT @DefalutGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

                                INSERT INTO @TmpRouteGroupDetail
                                    SELECT ISNULL(splitdata, @DefalutGuid) FROM[dbo].[fnSplitString](@MasterRouteGroupDetail_Guid, ',')

                                {0}                                

                                SELECT @MasterSiteGuid = r.MasterSite_Guid
                                        , @StrMasterDayOfWeek_Sequence = w.MasterDayOfWeek_Sequence

                                FROM TblMasterRoute r INNER JOIN TblSystemDayOfWeek w on r.MasterDayOfweek_Guid = w.Guid

                                WHERE r.Guid = @MasterRoute_Guid


                                INSERT INTO @temp
                                    -- งานทั้งหมดที่อยู่ใน Route เป็นเคสปกติ
                                    SELECT  L.Guid AS LegGuid
				                            , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
				                            , L.JobOrder
				                            , L.SeqIndex
				                            , L.MasterRouteGroupDetail_Guid
				                            , L.SequenceStop
				                            , JT.ServiceJobTypeNameAbb
				                            , ac.ActionNameAbbrevaition
				                            ,ISNULL(h.OnwardDestinationType, 3)
				                            , own.InterDepartmentName
                                    FROM    TblMasterRouteJobHeader H

                                            INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid

                                            INNER JOIN TblSystemJobAction ac ON ac.Guid = L.CustomerLocationAction_Guid

                                            INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid

                                            INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid

                                            INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid

                                            INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid AND C.FlagDisable = 0

                                            INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid   AND CustomreLocation.FlagDefaultBrinksSite = 1

                                                                                                                AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid

                                            LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid

                                            LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid

                                            LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid

                                    WHERE H.FlagDisable = 0

                                            AND h.MasterRoute_Guid = @MasterRoute_Guid

                                            AND L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence AND(L.FlagDeliveryLegForTV = 0    OR(L.FlagDeliveryLegForTV = 1 AND H.DayInVault = 0))

                                                                                                AND L.MasterSite_Guid = @MasterSiteGuid

                                            AND EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid = ISNULL(l.MasterRouteGroupDetail_Guid, @DefalutGuid))							
				                            AND H.Guid NOT IN(SELECT  Header.Guid

                                                                FROM    TblMasterRoute MRoute

                                                                        INNER JOIN TblMasterRouteJobHeader Header ON Header.MasterRoute_Guid = MRoute.Guid

                                                                        INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid

                                                                        INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid

                                                                WHERE   MRoute.Guid = @MasterRoute_Guid

                                                                        AND CusLocation.FlagDisable = 1)
                                           AND SeqIndex {1} @TargetSeqIndex AND JobOrder {1} @JobOrder


                                    UNION
                                    --2

                                    SELECT L.Guid AS LegGuid
					                            , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
					                            , L.JobOrder
					                            , L.SeqIndex
					                            , L.MasterRouteGroupDetail_Guid
					                            , L.SequenceStop
					                            , JT.ServiceJobTypeNameAbb
					                            , ac.ActionNameAbbrevaition
					                            ,ISNULL(h.OnwardDestinationType, 3)
					                            , own.InterDepartmentName
                                    FROM    TblMasterRoute R

                                            INNER JOIN TblMasterRouteJobHeader H ON H.MasterRoute_Guid = R.Guid

                                            INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid

                                            INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid

                                            INNER JOIN TblSystemJobAction ac ON ac.Guid = L.CustomerLocationAction_Guid

                                            INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid

                                            INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid

                                            INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid

                                            INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid

                                            LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid

                                            LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid

                                            LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid

                                    WHERE R.MasterSite_Guid = @MasterSiteGuid

                                            AND L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence

                                            AND R.FlagDisable = 0

                                            AND H.FlagDisable = 0

                                            AND CustomreLocation.FlagDefaultBrinksSite = 1

                                            AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid

                                            AND(FlagDeliveryLegForTV = 1 AND L.MasterRouteDeliveryLeg_Guid = @MasterRoute_Guid)

                                            AND EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid = (ISNULL(l.MasterRouteGroupDetail_Guid, @DefalutGuid)))
				                            AND H.Guid NOT IN(SELECT  Header.Guid

                                                                FROM    TblMasterRoute MRoute

                                                                        INNER JOIN TblMasterRouteJobHeader Header ON Header.MasterRoute_Guid = MRoute.Guid

                                                                        INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid

                                                                        INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid

                                                                WHERE   MRoute.MasterSite_Guid = @MasterSiteGuid

                                                                        AND JobLeg.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence

                                                                        AND CusLocation.FlagDisable = 1)
                                           AND SeqIndex {1} @TargetSeqIndex AND JobOrder {1} @JobOrder

                                    UNION
                                        --3

                                        SELECT L.Guid AS LegGuid
					                            , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
					                            , L.JobOrder
					                            , L.SeqIndex
						
					                            , L.MasterRouteGroupDetail_Guid
					                            , L.SequenceStop
					                            , JT.ServiceJobTypeNameAbb
					                            , ac.ActionNameAbbrevaition
					                            ,ISNULL(h.OnwardDestinationType, 3)
					                            , own.InterDepartmentName
                                        FROM    TblMasterRoute R

                                                INNER JOIN TblMasterRouteJobHeader H ON H.MasterRoute_Guid = R.Guid

                                                INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid

                                                INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid

                                                INNER JOIN TblSystemJobAction ac ON ac.Guid = L.CustomerLocationAction_Guid

                                                INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid

                                                INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid

                                                INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid

                                                INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid

                                                LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid

                                                LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid

                                                LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid

                                        WHERE L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence

                                                AND L.MasterSite_Guid = @MasterSiteGuid

                                                AND R.FlagDisable = 0

                                                AND H.FlagDisable = 0

                                                AND L.SequenceStop = 2
                                                --AND CustomreLocation.FlagDefaultBrinksSite = 1

                                                AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid

                                                AND L.MasterRouteDeliveryLeg_Guid = @MasterRoute_Guid

                                                AND EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid = (ISNULL(l.MasterRouteGroupDetail_Guid, @DefalutGuid)))
					                            AND H.Guid NOT IN(SELECT  Header.Guid

                                                                    FROM    TblMasterRouteJobHeader Header

                                                                            INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid

                                                                            INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid

                                                                    WHERE   Header.Guid = H.Guid

                                                                            AND JobLeg.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence

                                                                            AND CusLocation.FlagDisable = 1)
                                             AND   SeqIndex {1} @TargetSeqIndex AND JobOrder {1} @JobOrder
                               
                                IF(@OptionChange = 1)
								BEGIN
									DECLARE @amtData int
										SELECT @amtData= (COUNT(1)-1)+@TargetSeqIndex FROM @TmpSourceJob
									;WITH TmpData as (
										SELECT LegGuid,JobOrder,(ROW_NUMBER() OVER(ORDER BY JobOrder , SeqIndex)+@amtData) as SeqIndex2,SeqIndex FROM @temp 
										WHERE NOT EXISTS(select * from @TmpSourceJob where Guid= LegGuid)								 
									 )
									UPDATE @temp set SeqIndex = s.SeqIndex2
									FROM @temp t 
									INNER JOIN TmpData s ON s.LegGuid = t.LegGuid
									UPDATE @temp set SeqIndex = s.SeqIndex
									FROM @temp t 
									INNER JOIN @TmpSourceJob s ON t.LegGuid = s.Guid
								END
                                ELSE 
								BEGIN
									;WITH TmpData as (
										SELECT LegGuid,JobOrder,(ROW_NUMBER() OVER(ORDER BY JobOrder , SeqIndex)) as SeqIndex2,SeqIndex FROM @temp 
										WHERE NOT EXISTS(select * from @TmpSourceJob where Guid= LegGuid)								 
									 )
									UPDATE @temp set SeqIndex = s.SeqIndex2
									FROM @temp t 
									INNER JOIN TmpData s ON s.LegGuid = t.LegGuid
									UPDATE @temp set SeqIndex = s.SeqIndex
									FROM @temp t 
									INNER JOIN @TmpSourceJob s ON t.LegGuid = s.Guid
								END
								BEGIN TRAN 
								INSERT INTO TblMasterHistory_MasterRouteJob
									(
										MasterRoute_Guid					   ,
										MasterRouteJobHeader_Guid			   ,
										MasterRouteJobServiceStopLegs_Guid	   ,
										MsgID								   ,
										MsgParaeter							   ,
										UserCreated							   ,
										DatetimeCreated						   ,
										UniversalDatetimeCreated
									)
                                  select 										
										@MasterRoute_Guid as MasterRoute_Guid,
										l.MasterRouteJobHeader_Guid AS MasterRouteJobHeader_Guid, 
										l.Guid AS MasterRouteJobHeader_Guid,
										858,
										CONCAT(l.SeqIndex,',',t.SeqIndex,',',@UserModifed)
										,@UserModifed,
										@ClientDate,
										GETUTCDATE()									
										FROM TblMasterRouteJobServiceStopLegs l
										INNER JOIN @temp t ON  l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 

									/**/
									INSERT INTO [dbo].[TblMasterHistory_MasterRoute]
										([Guid]
										,[MasterRoute_Guid]
										,[MsgID]
										,[MsgParameter]
										,[UserCreated]
										,[DatetimeCreated]
										,[UniversalDatetimeCreated])
									SELECT 
										NEWID() as 'Guid',
										@MasterRoute_Guid as MasterRoute_Guid,									
										857 AS MsgId,
										@UserModifed AS 'MsgParameter'
										,@UserModifed as UserCreate,
										@ClientDate as DataTimeCreate,
										GETUTCDATE()									

									UPDATE TblMasterRouteJobHeader SET UserModifed = @UserModifed ,DatetimeModified = @ClientDate 
									FROM TblMasterRouteJobHeader h WHERE EXISTS (
										SELECT DISTINCT l.MasterRouteJobHeader_Guid  FROM TblMasterRouteJobServiceStopLegs l
										INNER JOIN @temp t ON h.Guid = l.MasterRouteJobHeader_Guid AND   l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 
									)

									Update TblMasterRouteJobServiceStopLegs				
									SET SeqIndex = t.SeqIndex										
									FROM TblMasterRouteJobServiceStopLegs l
									INNER JOIN @temp t ON  l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 
		                
									COMMIT TRAN
                                 SELECT 0 AS 'MsgId' ,'Success' AS 'Msg'
                            END
");
            queryStr.Append(@"END TRY 
                              BEGIN CATCH
                                    ROLLBACK TRAN
                                    SELECT -184 AS 'MsgId' ,ERROR_MESSAGE() AS 'Msg'

		                            INSERT INTO TblSystemLog_HistoryError
		                                (GUID ,ErrorDescription ,FunctionName ,PageName ,InnerError,ClientIP ,ClientName ,DatetimeCreated,FlagSendEmail)
		                           	SELECT								
		                                Guid = NEWID(),
		                                ErrorDescription  =ERROR_MESSAGE(),
		                                FunctionName = 'Update master-route SeqIndex ',
		                                PageName = 'MasterRoute [Re-SeqInde]',
									    InnerError =CONCAT('Error : [Id : ',ERROR_NUMBER(),' Line : ',ERROR_LINE(),' Message : ',ERROR_MESSAGE() ,']'),
		                                ClientIP = @@SERVERNAME,
		                                ClientName = '',
		                                DatetimeCreated =GETUTCDATE(),
		                                FlagSendEmail = 1
                              END CATCH;  ");
            return queryStr.ToString();
        }

        private string SetQueryUpdate()
        {
            StringBuilder queryStr = new StringBuilder();
            queryStr.Append(@"Begin Tran ");
            queryStr.Append(@"DECLARE @temp			table (		
										LegGuid							uniqueidentifier	,														
										LocationName						nvarchar(600),
										JobOrder							int,
										SeqIndex							int,
										MasterRouteGroupDetail_Guid		uniqueidentifier,										
										SequenceStop						int,
										JobTypeNameAbb						nvarchar(10),
                                        JobTypeID                         int,
										JobAction							nvarchar(10),
										OnwardType					    int,
										OnwardName							nvarchar(400) default('')
										)	");
            queryStr.Append(@"DECLARE	
		                    @MsgID										int = 0,
		                    @ErrorMasssage								nvarchar(600),
		                    @intErrorCode								int,
		                    @MasterSiteGuid								uniqueidentifier = null,
		                    @StrMasterDayOfWeek_Sequence				nvarchar(100) = null,	
		                    @RouteGroupDetailGuid						uniqueidentifier ,
		                    @isHaveRouteGroupNull						bit = 0,
		                    @SeqD										int,
		                    @HaveJobNum									int,
		                    @DefalutGuid								uniqueidentifier,
		                    @joborder									int ,
		                    @SchduleTime								datetime,
		                    @joborderLoop								int ,
		                    @SeqIndexLoop								int,
		                    @SchduleTimeLoop							datetime,
		                    @maxJobOrder								int=0");
            queryStr.Append(@"DECLARE @TmpRouteGroupDetail TABLE(
		                     RouteGrouptDetailGuid							uniqueidentifier	
                            ) ");
            queryStr.Append(@"BEGIN TRY ");
            queryStr.Append(@"BEGIN
		                        SET ANSI_WARNINGS OFF; 
		                        SELECT @DefalutGuid=CAST(CAST(0 AS binary) AS uniqueidentifier)
		                        INSERT INTO @TmpRouteGroupDetail 
			                        SELECT ISNULL(splitdata,@DefalutGuid) FROM [dbo].[fnSplitString](@MasterRouteGroupDetail_Guid, ',')

		                        SELECT	@MasterSiteGuid = r.MasterSite_Guid
				                        ,@StrMasterDayOfWeek_Sequence = w.MasterDayOfWeek_Sequence
		                        FROM	TblMasterRoute r INNER JOIN TblSystemDayOfWeek w on r.MasterDayOfweek_Guid = w.Guid
		                        WHERE   r.Guid = @MasterRoute_Guid

	
		                        INSERT INTO @temp
			                        -- งานทั้งหมดที่อยู่ใน Route เป็นเคสปกติ
			                        SELECT	L.Guid AS LegGuid			
					                        , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
					                        , L.JobOrder
					                        , L.SeqIndex					
					                        , L.MasterRouteGroupDetail_Guid					
					                        , L.SequenceStop
					                        , JT.ServiceJobTypeNameAbb	
                                            , JT.ServiceJobTypeID	
					                        , ac.ActionNameAbbrevaition			
					                        ,ISNULL( h.OnwardDestinationType,0)
					                        , own.InterDepartmentName
			                        FROM	TblMasterRouteJobHeader H 
					                        INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid																		                       
					                        INNER JOIN TblSystemJobAction ac ON ac.Guid =  L.CustomerLocationAction_Guid						
					                        INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid
					                        INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid					
					                        INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid
					                        INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid AND C.FlagDisable = 0											
					                        INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid	AND CustomreLocation.FlagDefaultBrinksSite = 1	
																						                        AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid		
					                        LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid 	
					                        LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid
					                        LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid
			                        WHERE	H.FlagDisable = 0		
											AND h.MasterRoute_Guid = @MasterRoute_Guid 
											AND L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence AND ( L.FlagDeliveryLegForTV = 0	OR  (L.FlagDeliveryLegForTV = 1 AND H.DayInVault = 0	) )	
																		                        AND L.MasterSite_Guid = @MasterSiteGuid	
											AND  EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid =ISNULL(l.MasterRouteGroupDetail_Guid,@DefalutGuid))							
					                        AND H.Guid NOT IN ( SELECT	Header.Guid
										                        FROM	TblMasterRoute MRoute
												                        INNER JOIN TblMasterRouteJobHeader Header ON Header.MasterRoute_Guid = MRoute.Guid
												                        INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid 
												                        INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid
										                        WHERE	MRoute.Guid = @MasterRoute_Guid
												                        AND CusLocation.FlagDisable = 1 )				
												
			                        UNION		
			                        --2
			                        SELECT		L.Guid AS LegGuid			
						                        , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
						                        , L.JobOrder
						                        , L.SeqIndex
						                        , L.MasterRouteGroupDetail_Guid	
						                        , L.SequenceStop
						                        , JT.ServiceJobTypeNameAbb
                                                , JT.ServiceJobTypeID
						                        , ac.ActionNameAbbrevaition
						                        ,ISNULL( h.OnwardDestinationType,0)
						                        , own.InterDepartmentName
			                        FROM	TblMasterRoute R
					                        INNER JOIN TblMasterRouteJobHeader H ON H.MasterRoute_Guid = R.Guid
					                        INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid
					                        INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid
					                        INNER JOIN TblSystemJobAction ac ON ac.Guid =  L.CustomerLocationAction_Guid		
					                        INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid					
					                        INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid
					                        INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid
					                        INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid	
					                        LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid 
					                        LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid
					                        LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid
			                        WHERE	R.MasterSite_Guid = @MasterSiteGuid
					                        AND L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence	
					                        AND R.FlagDisable = 0		 
					                        AND H.FlagDisable = 0	
					                        AND CustomreLocation.FlagDefaultBrinksSite = 1	
					                        AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid	
					                        AND (FlagDeliveryLegForTV = 1 AND L.MasterRouteDeliveryLeg_Guid = @MasterRoute_Guid)		
					                        AND  EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid =(ISNULL(l.MasterRouteGroupDetail_Guid,@DefalutGuid)))
					                        AND H.Guid NOT IN ( SELECT	Header.Guid
										                        FROM	TblMasterRoute MRoute
												                        INNER JOIN TblMasterRouteJobHeader Header ON Header.MasterRoute_Guid = MRoute.Guid
												                        INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid 
												                        INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid
										                        WHERE	MRoute.MasterSite_Guid = @MasterSiteGuid
												                        AND JobLeg.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence
												                        AND CusLocation.FlagDisable = 1 )
			                        UNION		
				                        --3
				                        SELECT	  L.Guid AS LegGuid			
						                        , C.CustomerFullName + ' - ' + CL.BranchName AS LocationName--, CL.BranchName AS LocationName
						                        , L.JobOrder
						                        , L.SeqIndex
						
						                        , L.MasterRouteGroupDetail_Guid
						                        , L.SequenceStop
						                        , JT.ServiceJobTypeNameAbb
                                                , JT.ServiceJobTypeID
						                        , ac.ActionNameAbbrevaition
						                        ,ISNULL( h.OnwardDestinationType,0)						
						                        , own.InterDepartmentName
				                        FROM	TblMasterRoute R
						                        INNER JOIN TblMasterRouteJobHeader H ON H.MasterRoute_Guid = R.Guid
						                        INNER JOIN TblMasterRouteJobServiceStopLegs L ON L.MasterRouteJobHeader_Guid = H.Guid
						                        INNER JOIN TblSystemJobAction A ON L.CustomerLocationAction_Guid = A.Guid
						                        INNER JOIN TblSystemJobAction ac ON ac.Guid =  L.CustomerLocationAction_Guid		
						                        INNER JOIN TblSystemServiceJobType JT ON H.SystemServiceJobType_Guid = JT.Guid					
						                        INNER JOIN TblMasterCustomerLocation CL ON L.MasterCustomerLocation_Guid = CL.Guid
						                        INNER JOIN TblMasterCustomer C ON CL.MasterCustomer_Guid = C.Guid
						                        INNER JOIN TblMasterCustomerLocation_BrinksSite CustomreLocation ON CustomreLocation.MasterCustomerLocation_Guid = L.MasterCustomerLocation_Guid AND CustomreLocation.MasterSite_Guid = L.MasterSite_Guid	
						                        LEFT JOIN TblMasterRouteGroup_Detail G ON L.MasterRouteGroupDetail_Guid = G.Guid 
						                        LEFT JOIN TblMasterRouteGroup on TblMasterRouteGroup.Guid = G.MasterRouteGroup_Guid
						                        LEFT JOIN TblMasterCustomerLocation_InternalDepartment own ON own.Guid = h.OnwardDestination_Guid
				                        WHERE	L.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence
						                        AND L.MasterSite_Guid = @MasterSiteGuid	
						                        AND R.FlagDisable = 0		 
						                        AND H.FlagDisable = 0	
						                        AND L.SequenceStop = 2
						                        --AND CustomreLocation.FlagDefaultBrinksSite = 1	
						                        AND CustomreLocation.MasterSite_Guid = @MasterSiteGuid	
						                        AND L.MasterRouteDeliveryLeg_Guid = @MasterRoute_Guid		
						                        AND  EXISTS (SELECT RouteGrouptDetailGuid FROM @TmpRouteGroupDetail WHERE RouteGrouptDetailGuid =(ISNULL(l.MasterRouteGroupDetail_Guid,@DefalutGuid)))
						                        AND H.Guid NOT IN ( SELECT	Header.Guid
											                        FROM	TblMasterRouteJobHeader Header 
													                        INNER JOIN  TblMasterRouteJobServiceStopLegs JobLeg ON  JobLeg.MasterRouteJobHeader_Guid = Header.Guid 
													                        INNER JOIN  TblMasterCustomerLocation CusLocation ON JobLeg.MasterCustomerLocation_Guid = CusLocation.Guid
											                        WHERE	Header.Guid = H.Guid 
													                        AND JobLeg.DayOfWeek_Sequence = @StrMasterDayOfWeek_Sequence
													                        AND CusLocation.FlagDisable = 1 )
			
			                        ;WITH NewSeq AS(
				                        select LegGuid,ROW_NUMBER() OVER(partition by MasterRouteGroupDetail_Guid,JobOrder ORDER BY JobAction,JobTypeID,OnwardType desc,OnwardName) as NewSeq from @temp
			                        )
			                        UPDATE @temp 
			                        SET SeqIndex =NewSeq
			                        FROM @temp t 
			                        INNER JOIN NewSeq tt ON t.LegGuid = tt.LegGuid			                        
							        
                                    /*toodo  insert log job history */
									INSERT INTO TblMasterHistory_MasterRouteJob
									(
										MasterRoute_Guid					   ,
										MasterRouteJobHeader_Guid			   ,
										MasterRouteJobServiceStopLegs_Guid	   ,
										MsgID								   ,
										MsgParaeter							   ,
										UserCreated							   ,
										DatetimeCreated						   ,
										UniversalDatetimeCreated
									)
                                  select 										
										@MasterRoute_Guid as MasterRoute_Guid,
										l.MasterRouteJobHeader_Guid AS MasterRouteJobHeader_Guid, 
										l.Guid AS MasterRouteJobHeader_Guid,
										858,
										CONCAT(l.SeqIndex,',',t.SeqIndex,',',@UserModifed)
										,@UserModifed,
										@ClientDate,
										GETUTCDATE()									
										FROM TblMasterRouteJobServiceStopLegs l
										INNER JOIN @temp t ON  l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 

									/**/
									INSERT INTO TblMasterHistory_MasterRoute
										(Guid
										,MasterRoute_Guid
										,MsgID
										,MsgParameter
										,UserCreated
										,DatetimeCreated
										,UniversalDatetimeCreated)
									SELECT 
										NEWID() as 'Guid',
										@MasterRoute_Guid as MasterRoute_Guid,									
										857 AS MsgId,
										@UserModifed AS 'MsgParameter'
										,@UserModifed as UserCreate,
										@ClientDate as DataTimeCreate,
										GETUTCDATE()


									UPDATE TblMasterRouteJobHeader SET UserModifed = @UserModifed,DatetimeModified = @ClientDate 
									FROM TblMasterRouteJobHeader h WHERE EXISTS (
										SELECT DISTINCT l.MasterRouteJobHeader_Guid  FROM TblMasterRouteJobServiceStopLegs l
										INNER JOIN @temp t ON h.Guid = l.MasterRouteJobHeader_Guid AND   l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 
									)

									Update TblMasterRouteJobServiceStopLegs				
									SET SeqIndex = t.SeqIndex										
									FROM TblMasterRouteJobServiceStopLegs l
									INNER JOIN @temp t ON  l.Guid = t.LegGuid  AND l.SeqIndex <> t.SeqIndex 
		                
                                -- UPDATE Route Optimize
						        DECLARE @BreakStatus uniqueidentifier =(SELECT top 1 Guid FROM TblSystemRouteOptimizationStatus WHERE RouteOptimizationStatusID = 6 AND FlagDisable != 1)
						        DECLARE @Completed uniqueidentifier =(SELECT top 1 Guid FROM TblSystemRouteOptimizationStatus WHERE RouteOptimizationStatusID = 3 AND FlagDisable != 1)

						
							        ;WITH TmpRouteGroupDetailName AS (
								        SELECT 
									        RGD.MasterRouteGroupDetailName
								        FROM TblMasterRoute_OptimizationStatus ROS 
								        INNER JOIN @TmpRouteGroupDetail TRGD ON ROS.MasterRoute_Guid = @MasterRoute_Guid AND ROS.MasterRouteGroupDetail_Guid = TRGD.RouteGrouptDetailGuid 
								        INNER JOIN TblMasterRouteGroup_Detail RGD ON RGD.Guid = TRGD.RouteGrouptDetailGuid
								        WHERE ROS.SystemRouteOptimizationStatus_Guid = @Completed
							        )

						        INSERT INTO [TblMasterRouteTransactionLog]
							        ([Guid]
							        ,[SystemLogCategory_Guid]
							        ,[SystemLogProcess_Guid]
							        ,[ReferenceValue_Guid]
							        ,[UserCreated]
							        ,[DatetimeCreated]
							        ,[UniversalDatetimeCreated]
							        ,[Remark]
							        ,[SystemMsgID]
							        ,[JSONValue])
						        SELECT
						        NEWID(),
							        '0837C72C-7C57-43A5-9A6B-DCEB02DE279D',
							        'B4F771C0-3A36-4FBD-857E-C4A23A92A65F',
							        @MasterRoute_Guid
							        ,@UserModifed
							        ,@ClientDate
							        ,GETUTCDATE()
							        ,'Master-route Level Sp. Set Job-order.'
							        ,6126
							        ,CONCAT('[""',(SELECT STUFF(
                                    (SELECT  ',' + MasterRouteGroupDetailName

                                        FROM(SELECT DISTINCT

                                                     MasterRouteGroupDetailName

                                                FROM TmpRouteGroupDetailName sh


                                            ) AS ServiceJobType

                                        ORDER BY MasterRouteGroupDetailName

                            FOR XML PATH('')), 1, 1, '')),'""]') as JsonValue
                            UPDATE TblMasterRoute_OptimizationStatus 
						    SET SystemRouteOptimizationStatus_Guid =  (@BreakStatus)	
						    FROM TblMasterRoute_OptimizationStatus mos 
						    INNER JOIN @TmpRouteGroupDetail tmp ON mos.MasterRouteGroupDetail_Guid = tmp.RouteGrouptDetailGuid and mos.MasterRoute_Guid = @MasterRoute_Guid

                            COMMIT TRAN
                            SELECT 0 AS 'MsgId' ,'Success' AS 'Msg'
                        END 
");

            queryStr.Append(@"END TRY 
                              BEGIN CATCH
                                    ROLLBACK TRAN
                                    SELECT -184 AS 'MsgId' ,ERROR_MESSAGE() AS 'Msg'

		                            INSERT INTO TblSystemLog_HistoryError
		                                (GUID ,ErrorDescription ,FunctionName ,PageName ,InnerError,ClientIP ,ClientName ,DatetimeCreated,FlagSendEmail)
		                           	SELECT								
		                                Guid = NEWID(),
		                                ErrorDescription  =ERROR_MESSAGE(),
		                                FunctionName = 'Update master-route SeqIndex ',
		                                PageName = 'MasterRoute [Re-SeqInde]',
									    InnerError =CONCAT('Error : [Id : ',ERROR_NUMBER(),' Line : ',ERROR_LINE(),' Message : ',ERROR_MESSAGE() ,']'),
		                                ClientIP = @@SERVERNAME,
		                                ClientName = '',
		                                DatetimeCreated =GETUTCDATE(),
		                                FlagSendEmail = 1
                              END CATCH;  ");

            return queryStr.ToString().Replace("[dbo]", schema);
        }
        private string GetQueryCheckMassUpdateJobUnderOptimize()
        {
            string query = @"			             DECLARE 
									@CountServiceType	INT,
									@CountDayOfWeek		INT,
									@CountLob			INT,
									@CheckTypeJob		INT = (CAST( @IsDeliveryLocation AS INT) +CAST( @IsPickUpLocation AS INT)),
									@IsJobInterBranch	Bit = IIF(@PickUpSiteGuid = @DeliverySiteGuid,0,1),
									@UtcDate		    Datetimeoffset = getutcdate(),
									@AllowContract      bit = 0,
									@CountContract      int
								DECLARE
									@TmpLob		TABLE(
										Guid			UNIQUEIDENTIFIER,
										LobNameAbb		nvarchar(50)
									)
								DECLARE
									@TmpServiceType		TABLE(
										Guid			UNIQUEIDENTIFIER,
										ServiceTypeId	Int,
										ServiceTypeNameAbb	nvarchar(50)

									)
								DECLARE
									@TmpDayOfWeek		TABLE(
										Guid			UNIQUEIDENTIFIER,
										DayOfWeekSeq	INT,
										WeekName		nvarchar(50)
									)

								DECLARE
									@TmpWeekType		TABLE(
										Guid			UNIQUEIDENTIFIER,
										WeekTypeName	nvarchar(50)
									)
								--DECLARE 
								--	@TmpMinSeviceHour	TABLE(
								--		ServiceJobTypeGuid			UNIQUEIDENTIFIER,
								--		LobGuid					UNIQUEIDENTIFIER,
								--		DayOfWeekSeq			Int,
								--		ServiceHour				Datetime
								--	)
								DECLARE 
									@TmpMaxSeviceHour	TABLE(
										ServiceJobTypeGuid		UNIQUEIDENTIFIER,
										LobGuid					UNIQUEIDENTIFIER,
										DayOfWeekSeq			Int,
										ServiceHour				Datetime
									)
								DECLARE @TblServiceTypeExclude	TABLE(
										ServiceTypeIdExclude  int
								)
								INSERT INTO @TblServiceTypeExclude VALUES (14),(15),(16)
	
								--INSERT INTO @TmpMinSeviceHour
								--	SELECT SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence,min(ServiceHourStart) 
								--	FROM TblMasterCustomerLocation_ServiceHour sh
								--	INNER JOIN TblSystemDayOfWeek dow ON sh.SystemDayOfWeek_Guid = dow.Guid
								--	WHERE MasterCustomerLocation_Guid = @LocReplaceGuid
								--	GROUP BY SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence

								--INSERT INTO @TmpMaxSeviceHour
								--	SELECT SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence,Max(ServiceHourStop) 
								--	FROM TblMasterCustomerLocation_ServiceHour sh
								--	INNER JOIN TblSystemDayOfWeek dow ON sh.SystemDayOfWeek_Guid = dow.Guid
								--	WHERE MasterCustomerLocation_Guid = @LocReplaceGuid 
								--	GROUP BY SystemServiceJobType_Guid,sh.MasterLineOfBusiness_Guid,MasterDayOfWeek_Sequence

								     
                               {0}
	/*

								INSERT INTO @TmpLob 
								select Guid,LOBAbbrevaitionName from TblSystemLineOfBusiness  -- where LOBAbbrevaitionName = 'CIT'

								INSERT INTO @TmpServiceType
								SELECT GUID,ServiceJobTypeID,ServiceJobTypeNameAbb FROM TblSystemServiceJobType where ServiceJobTypeID in (2,3)

								INSERT INTO @TmpDayOfWeek 
								SELECT Guid,MasterDayOfWeek_Sequence,MasterDayOfWeek_Name FROM TblSystemDayOfWeek -- where MasterDayOfWeek_Sequence =2

                         INSERT INTO @TmpWeekType 
                            SELECT Guid,WeekTypeName FROM TblSystemMaterRouteTypeOfWeek WHERE WeekTypeInt IN (1,2,3)
	*/							
							

								-- Validate exclude multi branch 
								DELETE @TmpServiceType WHERE EXISTS(Select * from @TblServiceTypeExclude WHERE ServiceTypeId = ServiceTypeIdExclude)

								SELECT @CountServiceType = COUNT(1) FROM @TmpServiceType
								SELECT @CountDayOfWeek = COUNT(1) FROM @TmpDayOfWeek
								SELECT @CountLob = COUNT(1) FROM @TmpLob

								IF OBJECT_ID (N'tempdb..#TmpJobTarget', N'U')
										 IS NOT NULL
									  DROP TABLE #TmpJobTarget;

								IF OBJECT_ID (N'tempdb..#TmpJobMultiStops', N'U')
									  IS NOT NULL
								   DROP TABLE #TmpJobMultiStops;
								
								IF OBJECT_ID (N'tempdb..#TmpTblResponse', N'U')
									  IS NOT NULL
								   DROP TABLE #TmpTblResponse;
								

								CREATE TABLE [dbo].#TmpJobTarget (
									LegGuid							Uniqueidentifier,
									JobHeadGuid						Uniqueidentifier,
									MasterRoutGuid					Uniqueidentifier,
									MasterRoutDeliveryGuid			Uniqueidentifier,
									RouteGroupDetailGuid			Uniqueidentifier,
									LocationGuid					Uniqueidentifier,
									MasterSiteGuid					Uniqueidentifier,
									DayOfWeekSeq					Int,
									LobGuid							Uniqueidentifier,
									ServiceJobTypeGuid				Uniqueidentifier,
									ServiceJobTypeId				int,
									AcitonGuid						Uniqueidentifier,
									ActionNameAbb					Nvarchar(10) collate database_default,
									SchduleTime						Datetime,
									FlagInterBranch					Bit,
									SeqStop							int,
									JobOrder						int,
									InsertFrom						nvarchar(50) collate database_default
								)


								CREATE TABLE #TmpJobMultiStops (
									LegGuid							Uniqueidentifier,
									JobHeadGuid						Uniqueidentifier,
									MasterRoutGuid					Uniqueidentifier,
									MasterRoutDeliveryGuid			Uniqueidentifier,
									RouteGroupDetailGuid			Uniqueidentifier,
									LocationGuid					Uniqueidentifier,
									MasterSiteGuid					Uniqueidentifier,
									DayOfWeekSeq					Int,
									LobGuid							Uniqueidentifier,
									ServiceJobTypeGuid				Uniqueidentifier,
									ServiceJobTypeId				int,
									AcitonGuid						Uniqueidentifier,
									ActionNameAbb					Nvarchar(10) collate database_default,
									SchduleTime						Datetime,
									FlagInterBranch					Bit,
									SeqStop							int,
									JobOrder						int
									
								)
								CREATE TABLE #TmpTblResponse (
								MasterRouteGuid				Uniqueidentifier,
								RouteGroupDetailGuid		Uniqueidentifier
								)
								

							
								/*P , D*/

	
									INSERT INTO #TmpJobMultiStops(
										LegGuid					,
										JobHeadGuid				,
										MasterRoutGuid			,
										MasterRoutDeliveryGuid	,
										RouteGroupDetailGuid	,
										LocationGuid			,
										MasterSiteGuid			,
										DayOfWeekSeq			,
										LobGuid					,
										ServiceJobTypeGuid		,
										ServiceJobTypeId		,
										AcitonGuid				,
										ActionNameAbb			,
										SchduleTime				,
										FlagInterBranch			,
										SeqStop					,
										JobOrder						
										
								)
									SELECT	L.Guid,
											H.Guid,
											r.Guid,
											l.MasterRouteDeliveryLeg_Guid,
											l.MasterRouteGroupDetail_Guid,
											L.MasterCustomerLocation_Guid,
											L.MasterSite_Guid,
											L.DayOfWeek_Sequence,
											h.SystemLineOfBusiness_Guid,
											h.SystemServiceJobType_Guid,
											ty.ServiceJobTypeID,
											a.Guid,
											a.ActionNameAbbrevaition ,
											l.SchduleTime,
											h.FlagJobInterBranch,
											l.SequenceStop,
											l.JobOrder
										
									FROM TblMasterRouteJobHeader H
									INNER JOIN TblSystemServiceJobType ty ON ty.Guid = h.SystemServiceJobType_Guid AND 1= (CASE	WHEN EXISTS (SELECT * FROM @TmpServiceType where Guid = H.SystemServiceJobType_Guid) THEN 1  -- JobType
																																WHEN @CountServiceType = 0 AND EXISTS(SELECT * FROM TblSystemServiceJobType tt WHERE tt.ServiceJobTypeID IN (0,1,2,3,4,5,11,12) AND tt.Guid = H.SystemServiceJobType_Guid) THEN 1 ELSE 0 END) --AND ServiceJobTypeID IN (2,3)
																			AND 1 =(CASE	WHEN EXISTS (SELECT * FROM @TmpLob WHERE Guid = h.SystemLineOfBusiness_Guid) THEN 1 -- LOB.
																							WHEN @CountLob = 0 THEN 1 ELSE 0 END)
									INNER JOIN TblMasterRoute R  ON H.FlagDisable = 0 AND R.Guid = H.MasterRoute_Guid 
																					AND EXISTS(SELECT * FROM @TmpWeekType WHERE Guid = r.SystemMaterRouteTypeOfWeek_Guid) AND (FlagHoliday = @FlagHolliday OR FlagHoliday = 0)
																						AND 1 = (CASE	WHEN EXISTS(SELECT * FROM @TmpDayOfWeek WHERE Guid = R.MasterDayOfweek_Guid) THEN 1 
																										WHEN @CountDayOfWeek = 0 THEN 1
																										ELSE 0
																								END)
									INNER JOIN TblMasterRouteJobServiceStopLegs L ON H.Guid = L.MasterRouteJobHeader_Guid  														
																						AND 'Action' = (CASE	
																												WHEN @IsPickUpLocation = 1 AND @PickupLocGuid =L. MasterCustomerLocation_Guid AND L.SequenceStop % 2 = 1 AND L.MasterSite_Guid = @PickUpSiteGuid AND  h.FlagJobInterBranch = @IsJobInterBranch THEN 'Action' 
																												WHEN @IsPickUpLocation = 1 AND @PickupLocGuid IS NULL AND L.SequenceStop % 2 = 1 AND L.MasterSite_Guid = @PickUpSiteGuid THEN 'Action'
																												-- Delivery 
																												WHEN @IsDeliveryLocation = 1 AND @DeliveryLocaGuid = L. MasterCustomerLocation_Guid AND L.SequenceStop % 2 = 0 AND L.MasterSite_Guid = @DeliverySiteGuid   
                                                                                                                        AND 1 = (CASE	WHEN @PickUpSiteGuid = @DeliverySiteGuid THEN 1  
																																	WHEN @PickUpSiteGuid != @DeliverySiteGuid AND ServiceJobTypeID = 2 AND H.FlagJobInterBranch = 1 THEN 1
																															END) THEN 'Action'
																												WHEN @IsDeliveryLocation = 1 AND @DeliveryLocaGuid IS NULL AND L.SequenceStop % 2 = 0 AND L.MasterSite_Guid = @DeliverySiteGuid 
																													AND 1 = (CASE	WHEN @PickUpSiteGuid = @DeliverySiteGuid THEN 1  
																																	WHEN @PickUpSiteGuid != @DeliverySiteGuid AND ServiceJobTypeID = 2 AND H.FlagJobInterBranch = 1 THEN 1
																															END) THEN 'Action'																				                          
																												WHEN @IsDeliveryLocation = 0 AND L.SequenceStop = 3 AND ServiceJobTypeID = 0 AND L.MasterSite_Guid = @DeliverySiteGuid AND h.FlagJobInterBranch = 1 THEN 'Action' --P Inter branch.

																										END)																							
									INNER JOIN TblSystemJobAction A ON A.Guid = L.CustomerLocationAction_Guid 

									/*========================= P,D Same site ===============================*/
									INSERT INTO #TmpJobTarget
									SELECT *,'step 1' FROM #TmpJobMultiStops WHERE ServiceJobTypeId NOT IN (2,3) AND FlagInterBranch = 0
	
									/*============================ PickUp Inter Branch ============================*/
									IF(@PickUpSiteGuid <> @DeliverySiteGuid)
									BEGIN                          
		
										INSERT #TmpJobTarget
												SELECT 
													P.LegGuid					PLegGuid ,
													P.JobHeadGuid				PJobHeadGuid ,
													P.MasterRoutGuid			PMasterRoutGuid ,
													P.MasterRoutDeliveryGuid	PMasterRoutDeliveryGuid ,
													P.RouteGroupDetailGuid		PRouteGroupDetailGuid ,
													P.LocationGuid				PLocationGuid ,
													P.MasterSiteGuid			PMasterSiteGuid,
													P.DayOfWeekSeq				PDayOfWeekSeq ,
													P.LobGuid					PLobGuid 	,
													P.ServiceJobTypeGuid		PServiceJobTypeGuid 	,
													P.ServiceJobTypeId			PServiceJobTypeId ,
													P.AcitonGuid				PAcitonGuid 	,
													P.ActionNameAbb				PActionNameAbb ,
													P.SchduleTime				PSchduleTime	,
													P.FlagInterBranch			PFlagInterBranch	, 
													P.SeqStop					PSeqStop,
													P.JobOrder					PJobOrder,
													'Step 2-P_inter'
												FROM #TmpJobMultiStops p 
												INNER JOIN #TmpJobMultiStops D ON P.JobHeadGuid = D.JobHeadGuid  AND P.LegGuid <> D.LegGuid
												WHERE p.SeqStop = 1 and d.SeqStop = 3 
														AND p.ServiceJobTypeId = 0 AND D.ServiceJobTypeId =0 
														AND p.FlagInterBranch =1 AND d.FlagInterBranch = 1 
														AND p.MasterSiteGuid = @PickUpSiteGuid 
														and d.MasterSiteGuid = @DeliverySiteGuid
		
											--INSERT #TmpJobTarget
			
				

									END

									/*================= T,TV ======================*/
									IF(EXISTS(SELECT TOP 1 1 FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3)))
									BEGIN
										IF((SELECT COUNT(1) FROM (SELECT SeqStop FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3) GROUP BY SeqStop) c) = 1)
										BEGIN
			
											--INSERT INTO #TmpJobTarget
											--SELECT * ,'Step 2-1' FROM #TmpJobMultiStops WHERE ServiceJobTypeId in (2,3) 
											--									AND	 1 = CASE	WHEN ServiceJobTypeId =3 THEN 1 -- T
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 1 THEN 1 -- Tv[P]
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 0 AND FlagInterBranch = 0 AND @PickupLocGuid = @DeliverySiteGuid THEN 1 --Tv[D] Same
											--												WHEN ServiceJobTypeId = 2 AND SeqStop % 2 = 0 AND FlagInterBranch = 1 AND @PickupLocGuid != @DeliverySiteGuid THEN 1 -- Tv[D] Interbranch
											--										  END 
											print '1'
			

										END
										ELSE
										BEGIN

			
											;WITH JobTSet as(
												SELECT 
													P.LegGuid					PLegGuid ,
													P.JobHeadGuid				PJobHeadGuid ,
													P.MasterRoutGuid			PMasterRoutGuid ,
													P.MasterRoutDeliveryGuid	PMasterRoutDeliveryGuid ,
													P.RouteGroupDetailGuid		PRouteGroupDetailGuid ,
													P.LocationGuid				PLocationGuid ,
													P.MasterSiteGuid			PMasterSiteGuid,
													P.DayOfWeekSeq				PDayOfWeekSeq ,
													P.LobGuid					PLobGuid 	,
													P.ServiceJobTypeGuid		PServiceJobTypeGuid 	,
													P.ServiceJobTypeId			PServiceJobTypeId ,
													P.AcitonGuid				PAcitonGuid 	,
													P.ActionNameAbb				PActionNameAbb ,
													P.SchduleTime				PSchduleTime	,
													P.FlagInterBranch			PFlagInterBranch	, 
													P.SeqStop					PSeqStop	,
													P.JobOrder					PJobOrder, 
													D.LegGuid					DLegGuid	,
													D.JobHeadGuid				DJobHeadGuid	,
													D.MasterRoutGuid			DMasterRoutGuid,
													D.MasterRoutDeliveryGuid	DMasterRoutDeliveryGuid,
													D.RouteGroupDetailGuid		DRouteGroupDetailGuid,
													D.LocationGuid				DLocationGuid,
													D.MasterSiteGuid			DMasterSiteGuid,
													D.DayOfWeekSeq				DDayOfWeekSeq,
													D.LobGuid					DLobGuid	,
													D.ServiceJobTypeGuid		DServiceJobTypeGuid	,
													D.ServiceJobTypeId			DServiceJobTypeId,
													D.AcitonGuid				DAcitonGuid	,
													D.ActionNameAbb				DActionNameAbb,
													D.SchduleTime				DSchduleTime,
													D.FlagInterBranch			DFlagInterBranch,
													D.SeqStop				    DSeqStop, 
													D.JobOrder					DJobOrder
													
												FROM #TmpJobMultiStops p 
												INNER JOIN #TmpJobMultiStops D ON P.JobHeadGuid = D.JobHeadGuid  AND P.LegGuid <> D.LegGuid
												WHERE p.SeqStop = 1 and d.SeqStop = 2 AND p.ServiceJobTypeId IN (2,3) AND D.ServiceJobTypeId in (2,3)
											)
											INSERT #TmpJobTarget
												SELECT 
													PLegGuid				,
													PJobHeadGuid			,
													PMasterRoutGuid			,
													PMasterRoutDeliveryGuid	,
													PRouteGroupDetailGuid	,
													PLocationGuid			,
													PMasterSiteGuid			,
													PDayOfWeekSeq			,
													PLobGuid				,
													PServiceJobTypeGuid		,
													PServiceJobTypeId		,
													PAcitonGuid				,
													PActionNameAbb			,
													PSchduleTime			,
													PFlagInterBranch		,
													PSeqStop				,
													PJobOrder				,
													'Step 2-P'
												FROM JobTSet 
												UNION 
												SELECT 
													DLegGuid				,
													DJobHeadGuid			,
													DMasterRoutGuid			,
													DMasterRoutDeliveryGuid	,
													DRouteGroupDetailGuid	,
													DLocationGuid			,
													DMasterSiteGuid			,
													DDayOfWeekSeq			,
													DLobGuid					,
													DServiceJobTypeGuid		,
													DServiceJobTypeId		,
													DAcitonGuid				,
													DActionNameAbb			,
													DSchduleTime			,
													DFlagInterBranch		,
													DSeqStop				,
													DJobOrder				,
													'Step 2-D'
												FROM JobTSet
			
		
											--order by pJobHeadGuid,PSeqStop
										END
									END


;WITH TmpGroupMasterRoute AS (
		SELECT DISTINCT ISNULL(MasterRoutDeliveryGuid,MasterRoutGuid)MasterRoutGuid,RouteGroupDetailGuid   from #TmpJobTarget		
	),TmpJoinOptimize AS(
		SELECT tr.*,st.RouteOptimizationStatusName from TmpGroupMasterRoute tr
		JOIN TblMasterRoute_OptimizationStatus rop ON tr.MasterRoutGuid = rop.MasterRoute_Guid and tr.RouteGroupDetailGuid = rop.MasterRouteGroupDetail_Guid
		JOIN TblSystemRouteOptimizationStatus st ON rop.SystemRouteOptimizationStatus_Guid = st.Guid and st.FlagDisable != 1
		
		WHERE st.RouteOptimizationStatusID in (1,2,4)
	)
	SELECT t.MasterRoutGuid,R.MasterRouteName,t.RouteGroupDetailGuid,RGD.MasterRouteGroupDetailName,t.RouteOptimizationStatusName FROM TmpJoinOptimize t
	JOIN TblMasterRoute R ON R.Guid = t.MasterRoutGuid 
	JOIN TblMasterRouteGroup_Detail RGD ON RGD.Guid = t.RouteGroupDetailGuid
	
	IF OBJECT_ID (N'tempdb..#TmpJobTarget', N'U')
				IS NOT NULL
			DROP TABLE #TmpJobTarget;

	IF OBJECT_ID (N'tempdb..#TmpJobMultiStops', N'U')
			IS NOT NULL
		DROP TABLE #TmpJobMultiStops;
								
	IF OBJECT_ID (N'tempdb..#TmpTblResponse', N'U')
			IS NOT NULL
		DROP TABLE #TmpTblResponse;";
            return query;
        }

    }

}
