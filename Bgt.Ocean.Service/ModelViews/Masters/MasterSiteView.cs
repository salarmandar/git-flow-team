using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class BrinksSiteView
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterCountry_Guid { get; set; }
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public string Description { get; set; }
        public bool FlagDisable { get; set; }
        public bool FlagSendSignature { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<int> TimeZoneID { get; set; }
        public Nullable<System.Guid> MasterCustomer_Guid { get; set; }
        public Nullable<System.Guid> MasterSiteHub_Guid { get; set; }
        public string AlphaCode { get; set; }
        public Nullable<decimal> FixedCostPerMonth { get; set; }
        public string TermConditionBody { get; set; }
        public bool FlagIntegrationSite { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
