using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class DistrictView : ModelBase
    {
        public Guid? MasterCity_Guid { get; set; }
        public Guid? MasterCountry_State_Guid { get; set; }
        public string MasterDistrictName { get; set; }
        public string MasterDistrictAbbreviation { get; set; }
        public string Postcode { get; set; }
        public string ProvinceStateName { get; set; }
    }
}
