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
    
    public partial class TblLeedToCashQuotation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblLeedToCashQuotation()
        {
            this.TblLeedToCashQuotation_Clause = new HashSet<TblLeedToCashQuotation_Clause>();
            this.TblLeedToCashQuotation_Customer_Mapping = new HashSet<TblLeedToCashQuotation_Customer_Mapping>();
            this.TblLeedToCashQuotation_Location_Mapping = new HashSet<TblLeedToCashQuotation_Location_Mapping>();
            this.TblLeedToCashQuotation_PricingRule_Mapping = new HashSet<TblLeedToCashQuotation_PricingRule_Mapping>();
            this.TblLeedToCashQuotation_Product = new HashSet<TblLeedToCashQuotation_Product>();
            this.TblLeedToCashQuotation_ProductAttribute = new HashSet<TblLeedToCashQuotation_ProductAttribute>();
            this.TblMasterCustomerContract = new HashSet<TblMasterCustomerContract>();
            this.TblLeedToCashQuotation_History = new HashSet<TblLeedToCashQuotation_History>();
        }
    
        public System.Guid Guid { get; set; }
        public System.Guid MasterCountry_Guid { get; set; }
        public int QuotationOrder { get; set; }
        public string QuotationID { get; set; }
        public string Description { get; set; }
        public System.Guid MasterCustomer_Guid { get; set; }
        public System.Guid SystemLeedToCashQuotationStatus_Guid { get; set; }
        public System.Guid SystemLeedToCashQuotationType_Guid { get; set; }
        public int RevisionNo { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> RefQuotation_Guid { get; set; }
        public bool FlagRequiredApproved { get; set; }
        public string RemarksApproval { get; set; }
        public Nullable<System.Guid> MasterBrinksComp_Guid { get; set; }
        public Nullable<System.Guid> SalesPerson_Guid { get; set; }
        public string OpportunityID { get; set; }
        public bool FlagGetFromCopyData { get; set; }
        public Nullable<bool> FlagDuplicatedData { get; set; }
        public Nullable<decimal> PercentDiscount { get; set; }
        public Nullable<System.Guid> AddendumToContract_Guid { get; set; }
        public Nullable<System.Guid> MasterUser_Guid { get; set; }
        public Nullable<System.DateTimeOffset> AutoAcceptanceDate { get; set; }
        public Nullable<System.DateTime> RateExpiredDateForAcceptance { get; set; }
        public string CustomerInfo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_Clause> TblLeedToCashQuotation_Clause { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_Customer_Mapping> TblLeedToCashQuotation_Customer_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_Location_Mapping> TblLeedToCashQuotation_Location_Mapping { get; set; }
        public virtual TblMasterCustomer TblMasterCustomer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_PricingRule_Mapping> TblLeedToCashQuotation_PricingRule_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_Product> TblLeedToCashQuotation_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_ProductAttribute> TblLeedToCashQuotation_ProductAttribute { get; set; }
        public virtual TblMasterCountry TblMasterCountry { get; set; }
        public virtual TblMasterCustomer TblMasterCustomer1 { get; set; }
        public virtual TblMasterUser TblMasterUser { get; set; }
        public virtual TblMasterUser TblMasterUser1 { get; set; }
        public virtual TblSystemLeedToCashQuotationType TblSystemLeedToCashQuotationType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterCustomerContract> TblMasterCustomerContract { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashQuotation_History> TblLeedToCashQuotation_History { get; set; }
    }
}
