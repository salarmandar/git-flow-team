using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Models.StandardTable
{
    public class MasterMonitoringNetworkView
    {
        public Guid? guid { get; set; }        
        [Required]
        public string monitoringNetworkName { get; set; }        
        public string phoneNumber { get; set; }
        [Required]
        public Guid masterCountry_Guid { get; set; }   
        public bool? flagDisable { get; set; }
        public string userCreated { get; set; }
        public string datetimeCreated { get; set; }
        public string userModified { get; set; }
        public string datetimeModified { get; set; }
    }


}
