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
    
    public partial class TblMasterDailyEmployee
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterEmployee_Guid { get; set; }
        public Nullable<System.Guid> MasterDailyRunResource_Guid { get; set; }
        public Nullable<int> EmployeeDailyStatusID { get; set; }
        public Nullable<int> RoleInRunResourceID { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> StopTime { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }
        public Nullable<int> WorkSeq { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public bool FlagSecondUIAuthen { get; set; }
    }
}
