using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.Problem
{
    public class ResponseQueryProblem : BaseResponse
    {
        public List<ResponseQueryProblem_Main> result { get; set; } = new List<ResponseQueryProblem_Main>();
    }

    public class ResponseQueryProblem_Main : BaseResponseQuery
    {
        public string machineServiceTypeId { get; set; }
        public string problemGuid { get; set; }
        public string problemId { get; set; }
        public string problemName { get; set; }
        public List<ResponseQueryProblem_SubProblem> subProblem { get; set; } = new List<ResponseQueryProblem_SubProblem>();
    }

    public class ResponseQueryProblem_SubProblem
    {
        public string guid { get; set; }
        public string subProblemId { get; set; }
        public string subProblemName { get; set; }
    }
}
