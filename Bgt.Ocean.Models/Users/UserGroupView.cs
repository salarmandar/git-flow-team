using Bgt.Ocean.Models.Masters;
using System;

namespace Bgt.Ocean.Models.Users
{
    public class UserGroupView
    {
        public Guid MasterUser_Guid { get { return User.Guid; } }

        public UserView User { get; set; }

        public Guid MasterGroup_Guid { get { return Group.Guid; } }

        public GroupView Group { get; set; }
    }
}
