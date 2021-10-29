using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class MasterGroupView
    {
        public Guid Guid { get; set; }
        public Guid MasterCountry_Guid { get; set; }
        public string GroupName { get; set; }
        public int RoleType { get; set; }
        public Guid SystemRoleGroupType_Guid { get; set; }
        public bool FlagDisable { get; set; }
        public IEnumerable<Guid> MasterSiteGuidList { get; set; }

        #region Alarm Attribute

        public bool FlagAllowAcknowledge { get; set; }
        public bool FlagAllowDeactivate { get; set; }

        #endregion
    }
}
