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
    
    public partial class TblSystemTripIndicator
    {
        public System.Guid Guid { get; set; }
        public Nullable<int> IndicatorID { get; set; }
        public string IndicatorName { get; set; }
        public Nullable<System.Guid> SystemDisplayTextControls_Guid { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public string ReferenceId { get; set; }
    }
}
