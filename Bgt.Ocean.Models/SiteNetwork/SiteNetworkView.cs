using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.SiteNetwork
{

    public class PopupSiteDestinationView
    {
        public IEnumerable<SiteDestinationMemberView> SiteNetworkMemberlist { get; set; }

        public IEnumerable<BrinksCompanyView> BrinksCompanylist { get; set; }

        public IEnumerable<DestinationCountryView> Countrylist { get; set; }
    }

    public class SiteDestinationMemberView
    {
        public Guid SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid MasterCountryGuid { get; set; }
        public string MasterCountryName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
    }
    public class BrinksCompanyView
    {
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
        public Guid MasterCountryGuid { get; set; }
    }

    public class DestinationCountryView
    {
        public Guid MasterCountryGuid { get; set; }
        public bool FlagHaveState { get; set; }
        public bool FlagInputCityManual { get; set; }
        public string MasterCountryName { get; set; }

    }

    public class SiteNetworkMemberView
    {
        public Guid SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid MasterCountryGuid { get; set; }
        public string MasterCountryName { get; set; }
        public IEnumerable<BrinksSiteBySiteNetwork> BrinksSitelist { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public bool FlagHaveSitePath { get; set; }
    }

    public class BrinksSiteBySiteNetwork
    {
        public Guid Guid { get; set; }
        public Guid SiteGuid { get; set; }
        public string SiteName { get; set; }
        public string SiteCodeName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
    }
}
