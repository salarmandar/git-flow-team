using Bgt.Ocean.Models.SiteNetwork;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.StandardTable
{
    public class SitePathView
    {
        public Guid SitePathGuid { get; set; }
        public string SitePathName { get; set; }
        public Guid MasterCountryGuid { get; set; }
        public string MasterCountryName { get; set; }
        public Guid SiteNetworkGuid { get; set; }
        public string SiteNetworkName { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public IEnumerable<BrinksSiteBySitePath> BrinksSitelist { get; set; }
        public IEnumerable<LocationViewBySitePath> CustomerLocationlist { get; set; }
    }


    public class BrinksSiteBySitePath : BrinksSiteBySiteNetwork
    {
        public int? SecuenceIndex { get; set; }
        public bool FlagDestination { get; set; }
    }

    public class LocationViewBySitePath
    {
        public Guid? Guid { get; set; }
        public Guid CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public Guid LocationGuid { get; set; }
        public string LocationName { get; set; }
    }
}
