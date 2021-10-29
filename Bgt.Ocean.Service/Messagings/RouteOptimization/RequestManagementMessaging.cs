using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.RouteOptimization;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RouteOptimization
{
    #region Response
    public class CancelRequestResponse : BaseResponse { }

    public class GetRouteOptimizePathResponse : BaseResponse { }
    public class MasterRouteOptimizationResponse : BaseResponse
    {

        public IEnumerable<MasterRouteOptimizeView> MasterRouteOptimizationList { get; set; }
    }
    public class ValidateResponse : BaseResponse { }
    public class MasterRouteOptimizeView
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

    public class GenerateRoadnetResponse : BaseResponse
    {
        public int TotalRequestGenerate { get; set; }
    }

    public class ReceiveRoadnetResponse : BaseResponse
    {
        public int TotalFail { get; set; }
        public int TotalSuccess { get; set; }
        public List< OptAdvMsgModel> DataResponse { get; set; }
    
    }
    #endregion Response

    #region Request
    public class MasterRouteInsertRequest : RequestBase
    {
        public Guid SiteGuid { get; set; }
        public Guid RequestTypeGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public bool FlagHoliday { get; set; }
        public Guid WeekGuid { get; set; }
        public Guid MasterRouteGuid { get; set; }
        public List<MasterRouteOptimizeView> MasterRouteSelectedList { get; set; }
    }

    public class MasterRouteNameRequest
    {
        public Guid SiteGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public Guid WeekGuid { get; set; }
        public bool FlagHoliday { get; set; } = false;
    }

    public class MasterRouteOptimizationRequest
    {
        public Guid SiteGuid { get; set; }
    }

    public class MasterRouteOptimizationListRequest : PagingBase
    {
        public Guid RequestGuid { get; set; }
    }

    public class MasterRouteOptimizationRequestListRequest : PagingBase
    {
        public Guid SiteGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public Guid WeekGuid { get; set; }
        public Guid MasterRouteGuid { get; set; }
        public bool FlagHoliday { get; set; }
        public bool FlagShowAllStatus { get; set; }
        public Guid? UserLangquage { get; set; }

    }
    #endregion request

    public class RoadNetAuditLogJobLevelView {

        public Guid MasterJobHeaderGuid { get; set; }
        public Guid  MasterJobLegGuid { get; set; }
        public string SourceRouteGroupDetailName { get; set; }
        public string DestinationRouteGroupDetailName { get; set; }
        public int SourceJobOrder { get; set; }
        public int DestinationJobOrder { get; set; }
        public string RequestId { get; set; }
        public string UserRequest { get; set; }
        public string LegAction { get; set; }
    }

    public class RoadNetAuditLogJobMasterRouteLevelView
    {

        public Guid MasterRouteGuid { get; set; }       
        public string RequestId { get; set; }
        public string RouteGroupName { get; set; }
        public string UserRequest { get; set; }

    }


    public class RoadNetJobHistoryLevelView
    {

        public Guid JobHeaderGuid { get; set; }
        public int SourceJobOrder { get; set; }
        public int DestinationJobOrder { get; set; }
        public string RequestId { get; set; }
        public string UserRequest { get; set; }
        public string LegAction { get; set; }
    }

    public class RoadNetDailyRunLevelView
    {

        public Guid DailyRunGuid { get; set; }
        public string RequestId { get; set; }
        public string UserRequest { get; set; }

    }

    public class RouteOptimizeErrorDetailView
    {
        public Guid RequestGuid { get; set; }
        public string RequestId { get; set; }
        public string ErrorDetail { get; set; }
    }
}
