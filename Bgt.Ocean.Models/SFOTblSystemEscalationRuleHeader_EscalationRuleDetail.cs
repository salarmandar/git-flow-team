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
    
    public partial class SFOTblSystemEscalationRuleHeader_EscalationRuleDetail
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> SFOSystemEscalationRuleHeader_Guid { get; set; }
        public Nullable<System.Guid> SFOSystemEscalationRuleDetail_Guid { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    
        public virtual SFOTblSystemEscalationRuleDetail SFOTblSystemEscalationRuleDetail { get; set; }
        public virtual SFOTblSystemEscalationRuleHeader SFOTblSystemEscalationRuleHeader { get; set; }
    }
}
