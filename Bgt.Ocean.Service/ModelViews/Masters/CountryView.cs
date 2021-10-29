using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class CountryView
    {
        public Guid MasterCountry_Guid { get; set; }
        public string MasterCountryName { get; set; }
        public string MasterCountryAbbreviation { get; set; }
        public bool? FlagInputCityManual { get; set; }
        public bool? FlagHaveState { get; set; }
    }
}
