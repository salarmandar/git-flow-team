using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.CustomerLocation
{
    public class LocationEmailRequest
    {
        public Guid SiteGuid { get; set; }
        public List<Guid> CustomerGuid { get; set; } = new List<Guid>();
        public Guid EmailActionGuid { get; set; }

    }
}
