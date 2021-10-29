using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.RunResourceType
{
    public class ResponseQueryRunResourceType : BaseResponse
    {
        public List<ResponseQueryRunResourceType_Main> result { get; set; } = new List<ResponseQueryRunResourceType_Main>();
    }

    public class ResponseQueryRunResourceType_Main : BaseResponseQuery
    {
        public string runResourceTypeName { get; set; }
    }
}
