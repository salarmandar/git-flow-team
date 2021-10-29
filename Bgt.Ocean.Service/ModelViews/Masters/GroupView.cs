using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class GroupView
    {
        public Guid Guid { get; set; }
        public GroupView(Guid id)
        {
            Guid = id;
        }

        public GroupView()
        {

        }

        public int RoleType { get; set; }
        public string Email { get; set; }
        public List<string> Emails { get; set; }
        public Guid? MasterCountry_Guid { get; set; }
        public string GroupName { get; set; }
    }
}
