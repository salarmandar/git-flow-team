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
    
    public partial class SFOTblTransactionGenericLog
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> SystemLogCategory_Guid { get; set; }
        public Nullable<System.Guid> SystemLogProcess_Guid { get; set; }
        public string ReferenceValue { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DateTimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public string Description { get; set; }
        public string SystemMsgID { get; set; }
        public string LabelIndex { get; set; }
        public string JSONValue { get; set; }
    
        public virtual SFOTblSystemLogCategory SFOTblSystemLogCategory { get; set; }
        public virtual SFOTblSystemLogProcess SFOTblSystemLogProcess { get; set; }
    }
}
