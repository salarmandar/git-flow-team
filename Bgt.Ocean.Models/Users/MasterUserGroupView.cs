using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Users
{
    public class MasterUserGroupView
    {
        public Guid MasterUser_Guid { get; set; }
        public IEnumerable<MasterGroupView> UserGroupList { get; set; }
    }
}
