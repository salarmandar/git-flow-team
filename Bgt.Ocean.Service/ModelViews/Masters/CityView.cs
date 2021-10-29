using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class CityView
    {
        public Guid Guid { get; set; }
        public string MasterCityName { get; set; }
        public Guid? MasterProvince_Guid { get; set; }
        public Guid? MasterCountry_Guid { get; set; }
    }
}
