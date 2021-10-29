using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Group
{
    public class MasterGroupAlarmModel
    {
        public Guid Guid { get; set; }
        public bool FlagAllowAcknowledge { get; set; }
        public bool FlagAllowDeactivate { get; set; }
        public IEnumerable<Guid> MasterSiteHandleList { get; set; }
    }
}
