using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.MachineSubServiceType
{
    public class ResponseQueryMachineSubServiceType : BaseResponse
    {
        public List<ResponseQueryMachineSubServiceType_Main> result { get; set; } = new List<ResponseQueryMachineSubServiceType_Main>();
    }

    public class ResponseQueryMachineSubServiceType_Main : BaseResponseQuery
    {
        public string machineServiceTypeId { get; set; }
        public string machineSubServiceTypeId { get; set; }
        public string machineSubServiceTypeName { get; set; }
    }
}
