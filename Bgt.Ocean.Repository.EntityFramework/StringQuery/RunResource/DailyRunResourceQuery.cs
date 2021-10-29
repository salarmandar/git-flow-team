
namespace Bgt.Ocean.Repository.EntityFramework.StringQuery.RunResource
{
    public class DailyRunResourceQuery
    {
        /// <summary>
        /// WorkDate = @WorkDate,
        /// MasterSite_Guid = @SiteGuid
        /// </summary>
        public string GetDailyRunResource_Ready
        {
            get
            {
                return @"SELECT		DailyRunResource.Guid
			                        ,RouteGroup.MasterRouteGroupName
			                        ,RouteDetail.MasterRouteGroupDetailName
			                        ,CASE 
				                        WHEN DailyRunResource.MasterRunResourceShift IS NOT NULL AND DailyRunResource.MasterRunResourceShift <> 1 THEN RunResource.VehicleNumber + ' (S'+ CONVERT(NVARCHAR, DailyRunResource.MasterRunResourceShift)+')'
				                        ELSE ISNULL(RunResource.VehicleNumber,'') 
			                        END AS VehicleNumberFullName
                        FROM		TblMasterDailyRunResource DailyRunResource 
                        INNER JOIN	TblMasterRunResource RunResource ON RunResource.Guid = DailyRunResource.MasterRunResource_Guid
                        INNER JOIN	TblMasterRouteGroup_Detail RouteDetail ON RouteDetail.Guid = DailyRunResource.MasterRouteGroup_Detail_Guid
                        INNER JOIN	TblMasterRouteGroup RouteGroup ON RouteGroup.Guid = RouteDetail.MasterRouteGroup_Guid
                        WHERE		DailyRunResource.FlagDisable = 0 
			                        AND RunResource.Flag3Party = 0
			                        AND DailyRunResource.RunResourceDailyStatusID = 1
			                        AND DailyRunResource.WorkDate = @WorkDate
			                        AND DailyRunResource.MasterSite_Guid = @SiteGuid";
            }
        }

        public string GetDailyRunResource_Fullname
        {
            get
            {
                return @"SELECT		CASE 
				                        WHEN DailyRunResource.MasterRunResourceShift IS NOT NULL AND DailyRunResource.MasterRunResourceShift <> 1 THEN RouteDetail.MasterRouteGroupDetailName + ' - ' + RunResource.VehicleNumber + ' (S'+ CONVERT(NVARCHAR, DailyRunResource.MasterRunResourceShift)+')'
				                        ELSE RouteDetail.MasterRouteGroupDetailName + ' - ' + ISNULL(RunResource.VehicleNumber,'') 
			                        END AS VehicleNumberFullName
                        FROM		TblMasterDailyRunResource DailyRunResource 
                        INNER JOIN	TblMasterRunResource RunResource ON RunResource.Guid = DailyRunResource.MasterRunResource_Guid
                        INNER JOIN	TblMasterRouteGroup_Detail RouteDetail ON RouteDetail.Guid = DailyRunResource.MasterRouteGroup_Detail_Guid
                        INNER JOIN	TblMasterRouteGroup RouteGroup ON RouteGroup.Guid = RouteDetail.MasterRouteGroup_Guid
                        WHERE		DailyRunResource.Guid = @DailyRunGuid";
            }
        }

        public string GetMasterRoute
        {
            get {
                return @"SELECT		DISTINCT *
                        FROM
                        (
                        SELECT		DISTINCT 
			                        dow.MasterDayOfWeek_Sequence,
			                        dow.MasterDayOfWeek_Name,
			                        tow.WeekTypeInt,
			                        tow.WeekTypeName,
                                    route.Guid AS RouteGuid,
			                        route.MasterRouteName,
                                    rGroup.Guid AS RouteGroupGuid,
			                        rGroup.MasterRouteGroupName,
                                    rDetail.Guid AS RouteGroupDetailGuid,
			                        rDetail.MasterRouteGroupDetailName
                        FROM		TblMasterRoute route
                        INNER JOIN	TblSystemMaterRouteTypeOfWeek tow ON tow.Guid = route.SystemMaterRouteTypeOfWeek_Guid
                        INNER JOIN	TblSystemDayOfWeek dow ON dow.Guid = route.MasterDayOfweek_Guid
                        INNER JOIN	TblMasterRouteJobHeader job ON job.MasterRoute_Guid = route.Guid
                        INNER JOIN	TblMasterRouteJobServiceStopLegs leg ON leg.MasterRouteJobHeader_Guid = job.Guid
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail on rDetail.Guid = leg.MasterRouteGroupDetail_Guid
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid
                        WHERE		route.FlagDisable = 0
			                        AND job.FlagDisable = 0
			                        AND route.MasterSite_Guid = @SiteGuid
                        UNION
                        SELECT		DISTINCT 
			                        dow.MasterDayOfWeek_Sequence,
			                        dow.MasterDayOfWeek_Name,
			                        tow.WeekTypeInt,
			                        tow.WeekTypeName,
                                    route.Guid AS RouteGuid,
			                        route.MasterRouteName,
                                    rGroup.Guid AS RouteGroupGuid,
			                        rGroup.MasterRouteGroupName,
                                    rDetail.Guid AS RouteGroupDetailGuid,
			                        rDetail.MasterRouteGroupDetailName
                        FROM		TblMasterRoute route
                        INNER JOIN	TblSystemMaterRouteTypeOfWeek tow ON tow.Guid = route.SystemMaterRouteTypeOfWeek_Guid
                        INNER JOIN	TblSystemDayOfWeek dow ON dow.Guid = route.MasterDayOfweek_Guid
                        INNER JOIN	TblMasterRouteJobServiceStopLegs leg ON leg.MasterRouteDeliveryLeg_Guid = route.Guid
                        INNER JOIN	TblMasterRouteJobHeader job ON job.Guid = leg.MasterRouteJobHeader_Guid
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail on rDetail.Guid = leg.MasterRouteGroupDetail_Guid
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid
                        WHERE		route.FlagDisable = 0
			                        AND job.FlagDisable = 0
			                        AND route.MasterSite_Guid = @SiteGuid
                        ) AS temp";
            }
        }

        #region Daily Plan
        public string GetDailyPlanCustomer
        {
            get {
                return @"SELECT		DISTINCT 
                                    cus.Guid
			                        ,cus.CustomerFullName
                        FROM		TblMasterDailyRunResource run
                        INNER JOIN	TblMasterActualJobServiceStopLegs leg ON leg.MasterRunResourceDaily_Guid = run.Guid
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid AND job.SystemStatusJobID <> 14
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND cus.FlagChkCustomer = 1 
                        WHERE		run.FlagDisable = 0
			                        AND run.RunResourceDailyStatusID = 2
			                        AND run.MasterSite_Guid = @SiteGuid
			                        AND run.WorkDate = @WorkDate
                        ORDER BY	cus.CustomerFullName";
            }
        }

        public string GetDailyPlanRouteGroup
        {
            get
            {
                return @"SELECT		DISTINCT 
                                    rGroup.Guid
			                        ,rGroup.MasterRouteGroupName as RouteGroupName
                        FROM		TblMasterDailyRunResource run
                        INNER JOIN	TblMasterActualJobServiceStopLegs leg ON leg.MasterRunResourceDaily_Guid = run.Guid
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid AND job.SystemStatusJobID <> 14
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND cus.FlagChkCustomer = 1 
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail ON rDetail.Guid = leg.MasterRouteGroupDetail_Guid AND rDetail.FlagDisable = 0
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid AND rGroup.FlagDisable = 0
                        WHERE		run.FlagDisable = 0
			                        AND run.RunResourceDailyStatusID = 2
			                        AND run.MasterSite_Guid = @SiteGuid
			                        AND run.WorkDate = @WorkDate
                                    @@FilterCustomers
                        ORDER BY	rGroup.MasterRouteGroupName";
            }
        }

        public string GetDailyPlanRouteDetail
        {
            get
            {
                return @"SELECT		DISTINCT 
                                    rDetail.Guid
			                        ,rDetail.MasterRouteGroupDetailName AS RouteGroupDetailName
                        FROM		TblMasterDailyRunResource run
                        INNER JOIN	TblMasterActualJobServiceStopLegs leg ON leg.MasterRunResourceDaily_Guid = run.Guid
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid AND job.SystemStatusJobID <> 14
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND cus.FlagChkCustomer = 1 
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail ON rDetail.Guid = leg.MasterRouteGroupDetail_Guid AND rDetail.FlagDisable = 0
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid AND rGroup.FlagDisable = 0
                        WHERE		run.FlagDisable = 0
			                        AND run.RunResourceDailyStatusID = 2
			                        AND run.MasterSite_Guid = @SiteGuid
			                        AND run.WorkDate = @WorkDate
                                    @@FilterCustomers
                                    @@FilterRouteGroups
                        ORDER BY	rDetail.MasterRouteGroupDetailName";
            }
        }

        public string GetDailyPlanDataList
        {
            get
            {
                return @"SELECT     TOP @@MaxRow *
                        FROM (
                        SELECT		DISTINCT 
                                    leg.MasterRunResourceDaily_Guid AS Guid
									,cus.Guid AS CustomerGuid
									,cus.CustomerFullName AS CustomerName
									,rGroup.MasterRouteGroupName AS RouteGroupName  
									,rDetail.MasterRouteGroupDetailName AS RouteGroupDetailName
			                        ,runR.VehicleNumber AS RunResourceName
                        FROM		TblMasterDailyRunResource run
                        INNER JOIN	TblMasterActualJobServiceStopLegs leg ON leg.MasterRunResourceDaily_Guid = run.Guid
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid AND job.SystemStatusJobID <> 14
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND cus.FlagChkCustomer = 1 
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail ON rDetail.Guid = leg.MasterRouteGroupDetail_Guid AND rDetail.FlagDisable = 0
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid AND rGroup.FlagDisable = 0
						INNER JOIN	TblMasterRunResource runR ON runR.Guid = run.MasterRunResource_Guid
                        WHERE		run.FlagDisable = 0
                                    AND rDetail.FlagNotSendDailyPlanReport = 0
			                        AND run.RunResourceDailyStatusID = 2
			                        AND run.MasterSite_Guid = @SiteGuid
			                        AND run.WorkDate = @WorkDate
                                    @@FilterCustomers
                                    @@FilterRouteGroups
                                    @@FilterRouteDetails
									
                        ) temp
                        ORDER BY	temp.CustomerName, temp.RouteGroupName, temp.RouteGroupDetailName, temp.RunResourceName";
            }
        }

       public string GetDailyPlanEmailList
        {
            get
            {
                return @"SELECT     DISTINCT 
                                    CustomerGuid
									,CustomerName
									,LocationName
									,LEFT(Email, Len(Email)-1) AS Email
						FROM (
						SELECT		cus.Guid AS CustomerGuid
									,cus.CustomerFullName AS CustomerName
									,loc.BranchName AS LocationName
									,(SELECT		emailLoc.Email + ','
										FROM		TblMasterCustomerLocation_EmailAction emailLoc
										INNER JOIN	TblSystemEmailAction emailAction ON emailAction.Guid = emailLoc.SystemEmailAction_Guid 
										WHERE		emailLoc.MasterCustomer_Guid = cus.Guid 
							                        AND (emailLoc.MasterCustomerLocation_Guid = loc.Guid or emailLoc.MasterCustomerLocation_Guid is null)
													AND emailAction.ActionID = 2
										ORDER BY	emailLoc.Email
										FOR XML PATH ('')) AS Email 
                        FROM		TblMasterDailyRunResource run
                        INNER JOIN	TblMasterActualJobServiceStopLegs leg ON leg.MasterRunResourceDaily_Guid = run.Guid
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid AND job.SystemStatusJobID <> 14
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND cus.FlagChkCustomer = 1 
						
                        WHERE		run.FlagDisable = 0									
									@@FilterCustomers
									@@FilterDailyRuns
						) temp	
                        ORDER BY	temp.CustomerName, temp.LocationName  ";
            }
        }
        #endregion
    }
}
