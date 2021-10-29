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
    
    public partial class TblLeedToCashMasterClause
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblLeedToCashMasterClause()
        {
            this.TblLeedToCashMasterClauseScope = new HashSet<TblLeedToCashMasterClauseScope>();
            this.TblLeedToCashProduct_Clause = new HashSet<TblLeedToCashProduct_Clause>();
        }
    
        public System.Guid Guid { get; set; }
        public string ClauseID { get; set; }
        public string ClauseName { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public Nullable<System.Guid> MasterClauseCat_Guid { get; set; }
        public Nullable<int> SeqNo { get; set; }
        public Nullable<System.DateTime> DateOfIssue { get; set; }
        public Nullable<System.DateTime> DateOfExpiry { get; set; }
        public Nullable<bool> FlagGlobal { get; set; }
        public string ClauseDetail { get; set; }
        public string ClauseDetailFieldID { get; set; }
        public string Description { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public System.Guid SystemLeedToCashStatus_Guid { get; set; }
        public bool IsCopyData { get; set; }
    
        public virtual TblLeedToCashMasterCategoryClause TblLeedToCashMasterCategoryClause { get; set; }
        public virtual TblMasterCountry TblMasterCountry { get; set; }
        public virtual TblSystemLeedToCashStatus TblSystemLeedToCashStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashMasterClauseScope> TblLeedToCashMasterClauseScope { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashProduct_Clause> TblLeedToCashProduct_Clause { get; set; }
    }
}
