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
    
    public partial class TblMasterHistory_ActualJob
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterActualJobHeader_Guid { get; set; }
        public Nullable<int> MsgID { get; set; }
        public string MsgParameter { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public bool FlagIsStaging { get; set; }
    }
}
