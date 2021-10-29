using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.RouteGroup
{
    public class ResponseQueryRouteGroup : BaseResponse
    {
        public List<ResponseQueryRouteGroup_Main> result { get; set; } = new List<ResponseQueryRouteGroup_Main>();
    }

    public class ResponseQueryRouteGroup_Main : BaseResponseQuery
    {
        public string brinksSite { get; set; }
        public string routeGroupDetailName { get; set; }
        public string routeGroupName { get; set; }
    }
}
