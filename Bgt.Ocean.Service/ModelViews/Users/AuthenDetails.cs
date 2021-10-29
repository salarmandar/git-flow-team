using System;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class AuthenDetails
    {
        public Bgt.Ocean.Infrastructure.Util.EnumUser.AuthenType AuthenType { get; set; }
        public String FullUsername { get; set; }
        public String Username { get; set; }
        public String Domain { get; set; }
        public String Password { get; set; }
    }
}
