
namespace Bgt.Ocean.Repository.EntityFramework.StringQuery.RouteOptimize
{
    public class RouteOptimizeQuery
    {
        public string GetDailyRunPlan
        {
            get
            {
                return @"SELECT		NEWID() AS NemoQueueGuid
                                    ,dailyRun.Guid AS DailyGuid
			                        ,RANK() OVER (ORDER BY dailyRun.Guid) AS OptimizationOrder
                                    ,site.Guid AS BranchGuid
			                        ,site.SiteCode AS BranchCode
			                        ,country.Guid AS CountryGuid
			                        ,country.MasterCountryAbbreviation AS CountryCode
			                        ,dailyRun.Guid AS ShiftGuid
			                        ,dailyRun.StartTime AS ShiftServiceStart
			                        ,dailyRun.EndTime AS ShiftServiceEnd
			                        ,dailyRun.WorkDate AS DateStart
			                        ,timeZone.Identifier AS TimeZone
                        FROM		TblMasterDailyRunresource dailyRun
                        INNER JOIN	TblMastersite site ON site.Guid = dailyRun.MasterSite_Guid
                        INNER JOIN	TblSystemTimezone timeZone ON timeZone.TimeZoneID = site.TimeZoneID
                        INNER JOIN	TblMasterCountry country ON country.Guid = site.MasterCountry_Guid
                        WHERE	    dailyRun.Guid = @DailyRunGuid";
            }
        }

        public string GetDailyRunPlans
        {
            get
            {
                return @"SELECT		NEWID() AS NemoQueueGuid
                                    ,dailyRun.Guid AS DailyGuid
			                        ,RANK() OVER (ORDER BY dailyRun.Guid) AS OptimizationOrder
                                    ,site.Guid AS BranchGuid
			                        ,site.SiteCode AS BranchCode
			                        ,country.Guid AS CountryGuid
			                        ,country.MasterCountryAbbreviation AS CountryCode
			                        ,dailyRun.Guid AS ShiftGuid
			                        ,dailyRun.StartTime AS ShiftServiceStart
			                        ,dailyRun.EndTime AS ShiftServiceEnd
			                        ,dailyRun.WorkDate AS DateStart
			                        ,timeZone.Identifier AS TimeZone
                        FROM		TblMasterDailyRunresource dailyRun
                        INNER JOIN	TblMastersite site ON site.Guid = dailyRun.MasterSite_Guid
                        INNER JOIN	TblSystemTimezone timeZone ON timeZone.TimeZoneID = site.TimeZoneID
                        INNER JOIN	TblMasterCountry country ON country.Guid = site.MasterCountry_Guid
                        WHERE	    dailyRun.Guid IN ({DailyRunGuid})";
            }
        }

        public string GetDailyRunServiceLocation
        {
            get
            {
                return @"SELECT		DailyRunGuid, DailyRunGuid AS ShiftGuid, JobGuid, LocationGuid, LocationCode, ServiceType, JobOrder
                        FROM
                        (
                        SELECT		leg.MasterRunResourceDaily_Guid AS DailyRunGuid
			                        ,job.Guid AS JobGuid
                                    ,loc.Guid AS LocationGuid
			                        ,ISNULL(loc.BranchCodeReference, loc.BranchName) AS LocationCode
			                        ,leg.JobOrder AS JobOrder
			                        ,lob.LobFullName + '-' + jobType.ServiceJobTypeNameAbb AS ServiceType
			                        ,RANK() OVER(PARTITION BY leg.MasterRunResourceDaily_Guid, loc.Guid, loc.BranchCodeReference, loc.BranchName, leg.JobOrder ORDER BY job.Guid) AS RowID
                        FROM		TblMasterActualJobServiceStopLegs leg
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND FlagChkCustomer = 1
                        INNER JOIN	TblSystemLineOfBusiness lob ON lob.Guid = job.SystemLineOfBusiness_Guid AND lob.FlagDisable = 0
                        INNER JOIN	TblSystemServiceJobType jobType ON jobType.Guid = job.SystemServiceJobType_Guid AND jobType.FlagDisable = 0
                        WHERE		job.FlagCancelAll = 0
                                    AND job.SystemStatusJobID <> 14
			                        AND leg.MasterRunResourceDaily_Guid = @DailyRunGuid
                        GROUP BY 	leg.MasterRunResourceDaily_Guid, loc.Guid, loc.BranchCodeReference, loc.BranchName, leg.JobOrder, job.Guid, lob.LobFullName, jobType.ServiceJobTypeNameAbb
                        ) AS temp
                        WHERE		temp.RowID = 1
                        ORDER BY	JobOrder";
            }
        }

        public string GetDailyRunServiceLocations
        {
            get
            {
                return @"SELECT		DailyRunGuid, DailyRunGuid AS ShiftGuid, JobGuid, LocationGuid, LocationCode, ServiceType, JobOrder
                        FROM
                        (
                        SELECT		leg.MasterRunResourceDaily_Guid AS DailyRunGuid
			                        ,job.Guid AS JobGuid
                                    ,loc.Guid AS LocationGuid
			                        ,ISNULL(loc.BranchCodeReference, loc.BranchName) AS LocationCode
			                        ,leg.JobOrder AS JobOrder
			                        ,lob.LobFullName + '-' + jobType.ServiceJobTypeNameAbb AS ServiceType
			                        ,RANK() OVER(PARTITION BY leg.MasterRunResourceDaily_Guid, loc.Guid, loc.BranchCodeReference, loc.BranchName, leg.JobOrder ORDER BY job.Guid) AS RowID
                        FROM		TblMasterActualJobServiceStopLegs leg
                        INNER JOIN	TblMasterActualJobHeader job ON job.Guid = leg.MasterActualJobHeader_Guid
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND FlagChkCustomer = 1
                        INNER JOIN	TblSystemLineOfBusiness lob ON lob.Guid = job.SystemLineOfBusiness_Guid AND lob.FlagDisable = 0
                        INNER JOIN	TblSystemServiceJobType jobType ON jobType.Guid = job.SystemServiceJobType_Guid AND jobType.FlagDisable = 0
                        WHERE		job.FlagCancelAll = 0
                                    AND job.SystemStatusJobID <> 14
			                        AND leg.MasterRunResourceDaily_Guid IN ({DailyRunGuid})
                        GROUP BY 	leg.MasterRunResourceDaily_Guid, loc.Guid, loc.BranchCodeReference, loc.BranchName, leg.JobOrder, job.Guid, lob.LobFullName, jobType.ServiceJobTypeNameAbb
                        ) AS temp
                        WHERE		temp.RowID = 1
                        ORDER BY	JobOrder";
            }
        }

        public string GetMasterRoutePlan
        {
            get
            {
                return @"SELECT		NEWID() AS NemoQueueGuid
                                    ,@RouteDetailGuid AS DailyGuid
			                        ,RANK() OVER (ORDER BY route.Guid) AS OptimizationOrder
                                    ,site.Guid AS BranchGuid
			                        ,site.SiteCode AS BranchCode
			                        ,country.Guid AS CountryGuid
			                        ,country.MasterCountryAbbreviation AS CountryCode
			                        ,route.Guid AS ShiftGuid
			                        ,CONVERT(DATETIME, '1900-01-01 00:00:00.000') AS ShiftServiceStart
			                        ,CONVERT(DATETIME, '1900-01-01 23:59:00.000') AS ShiftServiceEnd
			                        ,NULL AS DateStart
			                        ,timeZone.Identifier AS TimeZone
                        FROM		TblMasterRoute route
                        INNER JOIN	TblMastersite site ON site.Guid = route.MasterSite_Guid
                        INNER JOIN	TblSystemTimezone timeZone ON timeZone.TimeZoneID = site.TimeZoneID
                        INNER JOIN	TblMasterCountry country ON country.Guid = site.MasterCountry_Guid
                        WHERE	    route.Guid = @RouteGuid";
            }
        }

        public string GetMasterRouteLocation
        {
            get
            {
                return @"SELECT		leg.MasterRouteGroupDetail_Guid AS DailyRunGuid
			                        ,route.Guid AS ShiftGuid
			                        ,jobHeader.Guid AS JobGuid
                                    ,loc.Guid AS LocationGuid
			                        ,ISNULL(loc.BranchCodeReference, loc.BranchName) AS LocationCode
			                        ,leg.JobOrder AS JobOrder
			                        ,lob.LobFullName + '-' + jobType.ServiceJobTypeNameAbb AS ServiceType
			                        ,RANK() OVER(PARTITION BY leg.MasterRouteGroupDetail_Guid, route.Guid, loc.Guid, loc.BranchCodeReference, loc.BranchName, leg.JobOrder ORDER BY jobHeader.Guid) AS RowID
                        FROM		TblMasterRouteJobHeader jobHeader
                        INNER JOIN	TblMasterRouteJobServiceStopLegs leg ON leg.MasterRouteJobHeader_Guid = jobHeader.Guid
                        INNER JOIN	TblMasterRoute route ON (route.Guid = jobHeader.MasterRoute_Guid)
                        INNER JOIN	TblSystemServiceJobType jobType ON jobType.Guid = jobHeader.SystemServiceJobType_Guid
                        INNER JOIN	TblSystemJobAction act ON act.Guid = leg.CustomerLocationAction_Guid
                        INNER JOIN	TblSystemLineOfBusiness lob on lob.Guid = jobHeader.SystemLineOfBusiness_Guid
                        INNER JOIN	TblMasterRouteGroup_Detail rDetail ON rDetail.Guid = leg.MasterRouteGroupDetail_Guid AND rDetail.FlagDisable = 0
                        INNER JOIN	TblMasterRouteGroup rGroup ON rGroup.Guid = rDetail.MasterRouteGroup_Guid AND rGroup.FlagDisable = 0

                        INNER JOIN	TblMasterSite site ON site.Guid = route.MasterSite_Guid
                        INNER JOIN	TblMasterCustomerLocation loc ON loc.Guid = leg.MasterCustomerLocation_Guid AND loc.FlagDisable = 0
                        INNER JOIN	TblMasterCustomer cus ON cus.Guid = loc.MasterCustomer_Guid AND cus.FlagDisable = 0 AND FlagChkCustomer = 1

                        WHERE		route.Guid = @RouteGuid
			                        AND rDetail.Guid = @RouteDetailGuid
			                        AND leg.FlagDeliveryLegForTV = 0
			                        AND jobHeader.FlagDisable = 0";
            }
        }
    }
}
