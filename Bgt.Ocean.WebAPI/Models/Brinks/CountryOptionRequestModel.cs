using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bgt.Ocean.WebAPI.Models.Brinks
{
    public class CountryOptionRequestModel
    {
        public Guid? SiteGuid { get; set; }
        public Guid? countryGuid { get; set; }
        public List<string> Appkey { get; set; } = new List<string>();
    }
}