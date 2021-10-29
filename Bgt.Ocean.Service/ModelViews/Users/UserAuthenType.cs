using static Bgt.Ocean.Infrastructure.Util.EnumUser;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class UserAuthenType
    {
        public AuthenType AuthenType { get; set; }
        public string FullUsername { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
    }
}
