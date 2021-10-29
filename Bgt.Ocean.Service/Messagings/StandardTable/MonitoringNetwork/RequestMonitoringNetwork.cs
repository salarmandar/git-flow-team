using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.StandardTable.MonitoringNetwork
{

    public class GetMonitoringNetworkRequest
    {
        public Guid countryGuid { get; set; }
        public string monitoringNetworkName { get; set; }
        public bool flagDisable { get; set; }
    }

    public class EnableDisableNetworkRequest
    {
        [Required]
        public Guid guid { get; set; }
        [Required]
        public bool flagDisable { get; set; }        
    }

}
