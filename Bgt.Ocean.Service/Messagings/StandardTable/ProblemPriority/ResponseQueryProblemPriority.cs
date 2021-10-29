using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.ProblemPriority
{
    public class ResponseQueryProblemPriority : BaseResponse
    {
        public List<ResponseQueryProblemPriority_Main> result { get; set; } = new List<ResponseQueryProblemPriority_Main>();
    }

    public class ResponseQueryProblemPriority_Main : BaseResponseQuery
    {
        public string priorityId { get; set; }
        public string description { get; set; }
    }
}
