using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Models.Systems
{
    public class CountryOptionView
    {
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<System.Guid> SystemEnvironmentMasterCountry_Guid { get; set; }
        public string Appkey { get; set; }
        public string MasterSite_Guid { get; set; }
        public string MasterCountry_Guid { get; set; }
        public string AppValue1 { get; set; }
        public string AppValue2 { get; set; }
        public string AppValue3 { get; set; }
        public string AppValue4 { get; set; }
        public string AppValue5 { get; set; }
        public string AppValue6 { get; set; }
        public string AppValue7 { get; set; }
        public string AppValue8 { get; set; }
        public string AppValue9 { get; set; }
        public Nullable<bool> FlagDataDefault { get; set; }
    }
}
