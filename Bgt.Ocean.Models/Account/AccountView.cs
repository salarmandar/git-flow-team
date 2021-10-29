using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Models.Account
{
    public class AccountBaseModel
    {
        [Required]
        public int ApplicationID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class ChangePasswordViewModel: AccountBaseModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string VerifyKey { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public Guid UserGuid { get; set; }
    }
}
