using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class UserApplicationView
    {
        public Guid? UserGuid { get; set; }
        public IEnumerable<Guid> ApplicationGuids { get; set; }
        public Guid? Token { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
