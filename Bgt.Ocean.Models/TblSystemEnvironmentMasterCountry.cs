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
    
    public partial class TblSystemEnvironmentMasterCountry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblSystemEnvironmentMasterCountry()
        {
            this.TblSystemNotificationConfigPeriods = new HashSet<TblSystemNotificationConfigPeriods>();
            this.TblSystemEnvironmentMasterCountryTreeView = new HashSet<TblSystemEnvironmentMasterCountryTreeView>();
        }
    
        public System.Guid Guid { get; set; }
        public string AppKey { get; set; }
        public System.Guid SystemDisplayTextControls_Guid { get; set; }
        public string AppDescription { get; set; }
        public bool FlagNotApplyToSite { get; set; }
        public Nullable<int> FieldTypeID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSystemNotificationConfigPeriods> TblSystemNotificationConfigPeriods { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSystemEnvironmentMasterCountryTreeView> TblSystemEnvironmentMasterCountryTreeView { get; set; }
    }
}
