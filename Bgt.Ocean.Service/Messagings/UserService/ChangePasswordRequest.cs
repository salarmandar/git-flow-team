﻿
namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class ChangePasswordRequest
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
