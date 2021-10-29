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
    
    public partial class TblLeedToCashProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblLeedToCashProduct()
        {
            this.TblLeedToCashMasterClauseScopeDetail = new HashSet<TblLeedToCashMasterClauseScopeDetail>();
            this.TblLeedToCashProduct_Clause = new HashSet<TblLeedToCashProduct_Clause>();
            this.TblLeedToCashProduct_ProductAttribute = new HashSet<TblLeedToCashProduct_ProductAttribute>();
            this.TblLeedToCashProduct_ServiceType = new HashSet<TblLeedToCashProduct_ServiceType>();
            this.TblLeedToCashQuotation_Product = new HashSet<TblLeedToCashQuotation_Product>();
            this.TblMasterCustomerContract_ServiceLocation = new HashSet<TblMasterCustomerContract_ServiceLocation>();
            this.TblPricingRule = new HashSet<TblPricingRule>();
        }
    
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public System.Guid SystemLeedToCashStatus_Guid { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductLevel { get; set; }
        public bool FlagProductLeaf { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> Product_Guid { get; set; }
        public Nullable<int> ProductIndex { get; set; }
        public string ProductID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidTo { get; set; }
        public Nullable<System.Guid> MasterCustomer_Guid { get; set; }
        public bool IsCopyData { get; set; }
        public bool IsBidProduct { get; set; }
        public bool FlagRemove { get; set; }
        public string WordShownInTariff { get; set; }
        public Nullable<System.Guid> SystemLineOfBusiness_Guid { get; set; }
        public string LOBFullName { get; set; }
        public string ReportContentName { get; set; }
        public Nullable<int> ReportContentID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashMasterClauseScopeDetail> TblLeedToCashMasterClauseScopeDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashProduct_Clause> TblLeedToCashProduct_Clause { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashProduct_ProductAttribute> TblLeedToCashProduct_ProductAttribute { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashProduct_ServiceType> TblLeedToCashProduct_ServiceType { get; set; }
        public virtual TblMasterCountry TblMasterCountry { get; set; }
        public virtual TblMasterCustomer TblMasterCustomer { get; set; }
        public virtual TblSystemLeedToCashStatus TblSystemLeedToCashStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_Product> TblLeedToCashQuotation_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterCustomerContract_ServiceLocation> TblMasterCustomerContract_ServiceLocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblPricingRule> TblPricingRule { get; set; }
    }
}
