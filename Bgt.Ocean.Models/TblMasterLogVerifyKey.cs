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
    
    public partial class TblMasterLogVerifyKey
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterUser_Guid { get; set; }
        public string Verify_key { get; set; }
        public bool Action { get; set; }
        public System.DateTime KeyExpire { get; set; }
        public Nullable<int> AttempCount { get; set; }
    }
}
