namespace Bgt.Ocean.Models.StandardTable
{
    public class MachineSubServiceTypeView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string machineServiceTypeId { get; set; }
        public string machineSubServiceTypeId { get; set; }
        public string machineSubServiceTypeName { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }

    public class MachineSubServiceTypeView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
}
