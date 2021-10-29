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
    
    public partial class TblMasterRouteGroup_Detail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblMasterRouteGroup_Detail()
        {
            this.TblMasterRoute_OptimizationStatus = new HashSet<TblMasterRoute_OptimizationStatus>();
            this.TblTransactionRouteOptimizationHeader_Detail = new HashSet<TblTransactionRouteOptimizationHeader_Detail>();
            this.TblMasterConAndDeconsolidate_Header = new HashSet<TblMasterConAndDeconsolidate_Header>();
        }
    
        public System.Guid Guid { get; set; }
        public System.Guid MasterRouteGroup_Guid { get; set; }
        public System.Guid MasterSite_Guid { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string Description { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModifed { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModifed { get; set; }
        public bool FlagAutoConsolDeliveryLocPickup { get; set; }
        public string ReferenceId { get; set; }
        public bool FlagNotSendDailyPlanReport { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterRoute_OptimizationStatus> TblMasterRoute_OptimizationStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblTransactionRouteOptimizationHeader_Detail> TblTransactionRouteOptimizationHeader_Detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterConAndDeconsolidate_Header> TblMasterConAndDeconsolidate_Header { get; set; }
    }
}
