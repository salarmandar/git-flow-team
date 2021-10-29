using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.StandardTable.MachineServiceType
{
    public class ResponseQueryMachineServiceType : BaseResponse
    {
        public List<ResponseQueryMachineServiceType_Main> result { get; set; } = new List<ResponseQueryMachineServiceType_Main>();
    }

    public class ResponseQueryMachineServiceType_Main : BaseResponseQuery
    {
        public string machineServiceTypeId { get; set; }
        public string machineServiceTypeName { get; set; }
    }
}
