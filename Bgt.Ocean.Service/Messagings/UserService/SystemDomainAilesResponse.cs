using System;

namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class SystemDomainAilesResponse
    {
        public System.Guid Guid { get; set; }
        public System.Guid SystemDomain_Guid { get; set; }
        public string AilesName { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    }
}
