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
    
    public partial class TblMasterCity
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterCountry_Guid { get; set; }
        public Nullable<System.Guid> MasterLocalRegion_Guid { get; set; }
        public Nullable<System.Guid> SystemTimeZone_Guid { get; set; }
        public string MasterCityName { get; set; }
        public string MasterCityAbbreviation { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<int> CityIndex { get; set; }
        public string ReferenceId { get; set; }
    }
}
