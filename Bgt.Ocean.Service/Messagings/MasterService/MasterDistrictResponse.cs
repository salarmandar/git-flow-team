using System;

namespace Bgt.Ocean.Service.Messagings.MasterService
{
    public class MasterDistrictResponse
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterCity_Guid { get; set; }
        public Nullable<System.Guid> MasterCountry_State_Guid { get; set; }
        public string MasterDistrictName { get; set; }
        public string MasterDistrictAbbreviation { get; set; }
        public string Postcode { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> SystemLeedToCashCityType_Guid { get; set; }
    }
}
