using System;

namespace Bgt.Ocean.Service.Messagings.CustomerService
{
  public  class CustomerBaseRequest
    {
        public Guid? SiteGuid { get; set; }
    }
    public class RequestAdhocCustomer : CustomerBaseRequest
    {
        public Guid? UserGuid { get; set; }
        public int RoleUser { get; set; }
        public Guid? DayOfWeek_Guid { get; set; }
        public Guid? JobType_Guid { get; set; }
        public Guid? subServiceType_Guid { get; set; }
        public Guid? LobGuid { get; set; }
        public string strWorkdate { get; set; }
        public bool? FlagDestination { get; set; }
        public Guid? CustomerLocaitonPK { get; set; }
        public Guid? SiteDelGuid { get; set; }

        public string DateTimeFormat { get; set; }
    }
}
