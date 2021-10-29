using System;

namespace Bgt.Ocean.Models.Users
{
    public class UserView : ModelBase
    {
        public UserView() { }
        public UserView(Guid id)
        {
            Guid = id;
        }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int? TotalRecord { get; set; }
    }
}
