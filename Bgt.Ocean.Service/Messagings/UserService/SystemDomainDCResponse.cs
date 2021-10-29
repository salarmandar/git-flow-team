using System;

namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class SystemDomainDCResponse
    {
        public System.Guid Guid { get; set; }
        public System.Guid SystemDomain_Guid { get; set; }
        public string DcName { get; set; }
        public string LdapAuthPath { get; set; }
        public Nullable<int> SeqDC { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    }
}
