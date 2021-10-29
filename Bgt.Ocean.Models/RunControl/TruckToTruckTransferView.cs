using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    public class TruckToTruckTransferResponse
    {
        public bool isSuccess { get; set; } = true;
        public string message { get; set; } = string.Empty;
    }

    public class TruckToTruckHoldoverJobResponse : TruckToTruckTransferResponse
    {
        public List<Guid> jobGuidList { get; set; }
    }

}
