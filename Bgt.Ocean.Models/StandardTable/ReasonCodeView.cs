namespace Bgt.Ocean.Models.StandardTable
{
    public class ReasonCodeView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string reasonTypeCategoryId { get; set; }
        public string reasonTypeCategoryName { get; set; }
        public string reasonId { get; set; }
        public string reasonName { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }

    public class ReasonCodeView_Request
    {
        public string countryAbb { get; set; }
        public string reasonTypeCategoryId { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
}
