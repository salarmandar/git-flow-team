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
    
    public partial class TblMasterActualJobMCSRecyclingCashRecycling
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblMasterActualJobMCSRecyclingCashRecycling()
        {
            this.TblMasterActualJobMCSRecyclingCashRecyclingEntry = new HashSet<TblMasterActualJobMCSRecyclingCashRecyclingEntry>();
        }
    
        public System.Guid Guid { get; set; }
        public System.Guid MasterActualJobHeader_Guid { get; set; }
        public System.Guid MasterActualJobServiceStopLegs_Guid { get; set; }
        public System.Guid MasterCurrency_Guid { get; set; }
        public string CurrencyAbbr { get; set; }
        public decimal TotalLoaded { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public decimal TotalRecycled { get; set; }
        public decimal DeliveryAmount { get; set; }
        public string CustomerOrderNo { get; set; }
    
        public virtual TblMasterActualJobServiceStopLegs TblMasterActualJobServiceStopLegs { get; set; }
        public virtual TblMasterCurrency TblMasterCurrency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSRecyclingCashRecyclingEntry> TblMasterActualJobMCSRecyclingCashRecyclingEntry { get; set; }
        public virtual TblMasterActualJobHeader TblMasterActualJobHeader { get; set; }
    }
}
