using System;

namespace Bgt.Ocean.Service.ModelViews.Customer
{
    public class AdhocCustomerView
    {
        public System.Guid Guid { get; set; }
        public string CustomerFullName { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public string MasterCountryName { get; set; }
        public Nullable<bool> FlagHaveState { get; set; }
        public Nullable<bool> FlagInputCityManual { get; set; }
        public Nullable<bool> FlagLocationDestination { get; set; }
        public Nullable<int> SiteGuid { get; set; }
        public string SiteCodeName { get; set; }
    }

    public class CustomerAdhocView
    {
        public Guid Guid { get; set; }
        public string CustomerFullName { get; set; }
        public bool FlagLocationDestination { get; set; }
    }
}
