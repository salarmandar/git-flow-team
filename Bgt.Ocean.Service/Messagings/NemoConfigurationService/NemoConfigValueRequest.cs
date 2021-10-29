using Bgt.Ocean.Service.Messagings;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.NemoConfiguration
{
    public class NemoConfigValueRequest : RequestBase
    {
        public NemoCountryValueView CountryConfig { get; set; }
        public NemoSiteValueView SiteConfig { get; set; }
        public IEnumerable<NemoTrafficFactorValueView> TrafficConfig { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid? SiteGuid { get; set; }
    }

    public class NemoApplyToSiteRequest : RequestBase
    {
        public Guid CountryGuid { get; set; }
        public IEnumerable<Guid> SiteGuids { get; set; }
    }

    public class SiteForApplyRequest
    {
        public Guid SiteGuid { get; set; }
        public String SiteName { get; set; }
        public bool FlagSiteApply { get; set; }
    }
}
