using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class ProvinceStateView
    {
        public Guid ProvinceStateGuid { get; set; }
        public Guid CountryGuid { get; set; }
        public string ProvinceStateName { get; set; }
        public string ProvinceStateNameAbb { get; set; }
        public Guid? TimeZoneGuid { get; set; }
        public Guid? CountryRegion { get; set; }
        public bool? FlagHaveState { get; set; }
    }
}
