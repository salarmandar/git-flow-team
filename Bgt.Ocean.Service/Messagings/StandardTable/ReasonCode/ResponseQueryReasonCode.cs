using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.ReasonCode
{
    public class ResponseQueryReasonCode : BaseResponse
    {
        public List<ResponseQueryReasonCode_Main> result { get; set; } = new List<ResponseQueryReasonCode_Main>();
    }

    public class ResponseQueryReasonCode_Main : BaseResponseQuery
    {
        public string reasonTypeCategoryId { get; set; }
        public string reasonTypeCategoryName { get; set; }
        public string reasonId { get; set; }
        public string reasonName { get; set; }
    }
}
