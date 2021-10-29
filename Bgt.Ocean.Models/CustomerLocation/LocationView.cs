using System;

namespace Bgt.Ocean.Models.CustomerLocation
{
    public class LocationView
    {
        public Guid Guid { get; set; }
        public Guid MasterCustomer_Guid { get; set; }
        public string CustomerFullName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public Guid? MasterCity_Guid { get; set; }
        public Guid? MasterDistrict_Guid { get; set; }
        public Guid? MasterServiceHour_Guid { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    }
}
