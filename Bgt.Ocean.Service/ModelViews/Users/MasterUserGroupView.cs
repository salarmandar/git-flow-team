using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class MasterUserGroupView
    {
        public Guid MasterUser_Guid { get; set; }
        public IEnumerable<MasterGroupView> UserGroupList { get; set; }
    }
}
