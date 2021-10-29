namespace Bgt.Ocean.Models.StandardTable
{
    public class ProblemView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string machineServiceTypeId { get; set; }
        public string problemGuid { get; set; }
        public string problemId { get; set; }
        public string problemName { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }

    public class ProblemView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }

    public class ProblemView_SubProblem
    {
        public string guid { get; set; }
        public string subProblemId { get; set; }
        public string subProblemName { get; set; }
    }
}
