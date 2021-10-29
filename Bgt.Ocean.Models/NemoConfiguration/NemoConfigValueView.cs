using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.NemoConfiguration
{
    public class NemoConfigValueView
    {
        public NemoCountryValueView CountryConfig { get; set; }
        public NemoSiteValueView SiteConfig { get; set; }
        public IEnumerable<NemoTrafficFactorValueView> TrafficConfig { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid? SiteGuid { get; set; }
    }

    public class NemoApplyToSiteView
    {
        public Guid CountryGuid { get; set; }
        public IEnumerable<Guid> SiteGuids { get; set; }
        public DateTime ClientDateTime { get; set; }
        public DateTimeOffset UniversalDateTime { get; set; }
        public string UserName { get; set; }
    }

    public class SiteForApplyView
    {
        public Guid SiteGuid { get; set; }
        public String SiteName { get; set; }
        public bool FlagSiteApply { get; set; }
    }
}
