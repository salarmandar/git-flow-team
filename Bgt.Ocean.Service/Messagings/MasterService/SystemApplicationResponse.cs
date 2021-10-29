using System;

namespace Bgt.Ocean.Service.Messagings.MasterService
{
    public class SystemApplicationResponse
    {
        public System.Guid Guid { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> TokenID { get; set; }
        public Nullable<System.DateTime> TokenExpireDate { get; set; }
        public Nullable<bool> FlagAppInternal { get; set; }

    }
}
