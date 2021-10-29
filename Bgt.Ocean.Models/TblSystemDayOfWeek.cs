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
    
    public partial class TblSystemDayOfWeek
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblSystemDayOfWeek()
        {
            this.TblMasterCustomerLocation_ServiceHour = new HashSet<TblMasterCustomerLocation_ServiceHour>();
            this.TblMasterUserLimitedTimeAccess = new HashSet<TblMasterUserLimitedTimeAccess>();
            this.TblSmartBillingSchedule_Day_Mapping = new HashSet<TblSmartBillingSchedule_Day_Mapping>();
        }
    
        public System.Guid Guid { get; set; }
        public string MasterDayOfWeek_Name { get; set; }
        public Nullable<int> MasterDayOfWeek_Sequence { get; set; }
        public Nullable<System.Guid> SystemDisplayTextControls_Guid { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public string ReferenceId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterCustomerLocation_ServiceHour> TblMasterCustomerLocation_ServiceHour { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterUserLimitedTimeAccess> TblMasterUserLimitedTimeAccess { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSmartBillingSchedule_Day_Mapping> TblSmartBillingSchedule_Day_Mapping { get; set; }
    }
}
