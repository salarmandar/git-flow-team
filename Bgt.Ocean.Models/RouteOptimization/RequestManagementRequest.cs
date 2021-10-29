using Bgt.Ocean.Models.BaseModel;
using System;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class RequestManagementRequest : PagingBase
    {
        public Guid SiteGuid { get; set; }
        public Guid? StatusGuid { get; set; }
        public Guid? RouteTypeGuid { get; set; }
    }

    public class CancelRequest
    {
        public Guid RequestGuid { get; set; }
    }

    public class UpdateCancelRequest :CancelRequest
    {
        public string UserModify { get; set; }
        public DateTime ClientDate { get; set; }
        public DateTimeOffset UtcDate { get; set; }
        public Guid? UserLanguageGuid { get; set; }
        public int StatusFrom { get; set; }
        public int StatusTo { get; set; }
    }

    public class UpdateRouteOptimizeRequest
    {
        public string RequestId { get; set; }
        public string User { get; set; }
        public Guid StatusUpdateGuid { get; set; }
        public DateTime ClientDate { get; set; }
        public DateTimeOffset UtcDate { get; set; }
        public int[] ConditionOptimizeStatusId { get; set; }
        public string ErrorDetail { get; set; }
        //string reqId, string user, Guid statusGuid, DateTime clientDate, DateTimeOffset utcDate
    }
}
