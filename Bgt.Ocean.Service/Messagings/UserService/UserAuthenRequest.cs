using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class UserAuthenRequest
    {
        /// <summary>
        /// Application username
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Application password
        /// </summary>
        [Required]
        public string Password { get; set; }

        public DateTime? ClientDateTime { get; set; }
        public string ClientSessionId { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        [Required]
        public int ApplicationId { get; set; }
    }
}
