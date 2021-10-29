using System.Collections.Generic;
using System.Net.Mail;

namespace Bgt.Ocean.Models.Email
{
    public class EmailModel
    {
        public EmailAddress EmailFrom { get; set; }
        public List<EmailAddress> EmailTo { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> EmailCC { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> EmailBCC { get; set; } = new List<EmailAddress>();

        public string Subject { get; set; }
        public string Body { get; set; }
        public AlternateView BodyAlternateView { get; set; }

        public int TimeOut { get; set; } = 5000;
    }

    public class EmailAddress
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
