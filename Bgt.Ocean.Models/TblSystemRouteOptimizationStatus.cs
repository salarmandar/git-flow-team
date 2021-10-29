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
    
    public partial class TblSystemRouteOptimizationStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblSystemRouteOptimizationStatus()
        {
            this.TblTransactionRouteOptimizationHeader = new HashSet<TblTransactionRouteOptimizationHeader>();
            this.TblMasterRoute_OptimizationStatus = new HashSet<TblMasterRoute_OptimizationStatus>();
            this.TblMasterActualJobServiceStopLegs = new HashSet<TblMasterActualJobServiceStopLegs>();
        }
    
        public System.Guid Guid { get; set; }
        public int RouteOptimizationStatusID { get; set; }
        public string RouteOptimizationStatusName { get; set; }
        public Nullable<System.Guid> SystemDisplayTextControlsLanguage_Guid { get; set; }
        public bool FlagForRequest { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblTransactionRouteOptimizationHeader> TblTransactionRouteOptimizationHeader { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterRoute_OptimizationStatus> TblMasterRoute_OptimizationStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobServiceStopLegs> TblMasterActualJobServiceStopLegs { get; set; }
    }
}
