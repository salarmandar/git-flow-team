namespace Bgt.Ocean.Models.StandardTable
{
    public class MachineServiceTypeView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string machineServiceTypeId { get; set; }
        public string machineServiceTypeName { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }

    public class MachineServiceTypeView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
}
