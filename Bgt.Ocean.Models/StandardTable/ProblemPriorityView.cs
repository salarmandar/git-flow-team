namespace Bgt.Ocean.Models.StandardTable
{
    public class ProblemPriorityView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string priorityId { get; set; }
        public string description { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }

    public class ProblemPriorityView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
}
