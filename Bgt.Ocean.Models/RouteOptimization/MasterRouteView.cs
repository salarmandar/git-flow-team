using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class RequestManagementDetailResponse
    {
        public Guid Guid { get; set; }
        public string SiteName { get; set; }
        public string RequestType { get; set; }
        public string RequestID { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public string RequestUser { get; set; }
        public DateTime RequestDatetime { get; set; }
        public string RequestDatetimeStr { get; set; }
        public DateTime? CompleteDatetime { get; set; }
        public string CompleteDatetimeStr { get; set; }
        public string CancelUser { get; set; }
        public DateTime? CancelDatetime { get; set; }
        public string CancelDatetimeStr { get; set; }
    }

    public class MasterRouteRequestDetailResponse : RequestManagementDetailResponse
    {
        public string DayOfWeek { get; set; }
        public bool FlagHoliday { get; set; }
        public string Week { get; set; }
        public string MasterRouteName { get; set; }
    }
    public class RequestRouteOptimizeView
    {
        public string RouteGroup { get; set; }
        public string RouteGroupDetail { get; set; }
        public int Stops { get; set; }
        public int Locations { get; set; }
        public int Jobs { get; set; }
        public int OptimizationStatusID { get; set; }
        public string OptimizationStatus { get; set; }
    }

  
    public class MasterRouteOptimizationViewModel
    {
        public Guid MasterRouteGroupDetailGuid { get; set; }
        public string RouteGroup { get; set; }
        public string RouteGroupDetail { get; set; }
        public int Stops { get; set; }
        public string StopsStr { get; set; }
        public int Locations { get; set; }
        public string LocationsStr { get; set; }
        public int Jobs { get; set; }
        public string JobsStr { get; set; }
        public int OptimizationStatusID { get; set; }
        public string OptimizationStatus { get; set; }
    }

    public class MasterJobRequestOptimizeViewModel
    {
        public Guid RouteGroupDetailGuid { get; set; }
        public Guid JobHeaderGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public Guid  SystemServiceJobTypeGuid { get; set; }
        public Guid LocationGuid { get; set; }
        public int? JobOrder { get; set; }
    }

    public class ValidateSameRouteGroupDetailNameInprogress
    {
        public string RouteGroupDetailName { get; set; }
        public string RequestId { get; set; }
    }
}
