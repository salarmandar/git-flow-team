using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteOptimizedLocationDetails
    {
        public List<LocationInformation> Plan { get; set; } = new List<LocationInformation>();
        public List<LocationInformation> Optimized { get; set; } = new List<LocationInformation>();
    }

    public class LocationInformation
    {
        public Guid Guid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? Order { get; set; }
    }
}
