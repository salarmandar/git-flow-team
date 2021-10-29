using System;

namespace Bgt.Ocean.Models.Customer
{
  public  class AdhocCustomerRequest
    {
        public Guid? SiteGuid { get; set; }
        public Guid? UserGuid { get; set; }
        public int RoleUser { get; set; }
        public Guid? DayOfWeek_Guid { get; set; }
        public Guid? JobType_Guid { get; set; }
        public Guid? LobGuid { get; set; }
        public string strWorkDate { get; set; }
        public bool? FlagDestination { get; set; }
        public Guid? CustomerLocaitonPK { get; set; }
        public Guid? SiteDelGuid { get; set; }
        public Guid? subServiceType_Guid { get; set; }
        public string DateTimeFormat { get; set; }
    }
}
