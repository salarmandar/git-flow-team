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
    
    public partial class SFOTblSystemDataConfiguration_Country
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> SFOSystemDataConfiguration_Guid { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public string DataValue1 { get; set; }
        public string DataValue2 { get; set; }
        public string DataValue3 { get; set; }
        public string DataValue4 { get; set; }
        public string DataValue5 { get; set; }
        public string DataValue6 { get; set; }
        public string DataValue7 { get; set; }
        public string DataValue8 { get; set; }
        public string DataValue9 { get; set; }
        public bool FlagDefault { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModify { get; set; }
        public Nullable<System.DateTime> DatetimeModify { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModify { get; set; }
    
        public virtual TblMasterCountry TblMasterCountry { get; set; }
        public virtual SFOTblSystemDataConfiguration SFOTblSystemDataConfiguration { get; set; }
    }
}
