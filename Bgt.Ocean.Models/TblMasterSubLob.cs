//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bgt.Ocean.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblMasterSubLob
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterCustomer_Guid { get; set; }
        public string SubLobName { get; set; }
        public Nullable<System.Guid> SystemLineOfBusiness { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModify { get; set; }
        public Nullable<System.DateTime> DatetimeModify { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModify { get; set; }
        public Nullable<System.Guid> SystemReportContent_Guid { get; set; }
    
        public virtual TblMasterCustomer TblMasterCustomer { get; set; }
        public virtual TblSystemLineOfBusiness TblSystemLineOfBusiness { get; set; }
    }
}
