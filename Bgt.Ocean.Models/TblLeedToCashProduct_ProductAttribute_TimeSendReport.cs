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
    
    public partial class TblLeedToCashProduct_ProductAttribute_TimeSendReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblLeedToCashProduct_ProductAttribute_TimeSendReport()
        {
            this.TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail = new HashSet<TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail>();
        }
    
        public System.Guid Guid { get; set; }
        public System.Guid LeedToCashProduct_ProductAttribute_Guid { get; set; }
        public System.Guid SystemDayOfWeek_Guid { get; set; }
        public Nullable<System.Guid> SystemLeedToCashCityType_Guid { get; set; }
        public Nullable<System.Guid> TimeRange_Guid { get; set; }
    
        public virtual TblLeedToCashProduct_ProductAttribute TblLeedToCashProduct_ProductAttribute { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail> TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail { get; set; }
    }
}
