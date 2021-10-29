using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.Email;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Bgt.Ocean.Service.Implementations.Email
{
    #region Interface
    public interface IEmailService
    {
        bool SendEmail(EmailModel mail);
    }
    #endregion

    #region Class
    public class EmailService : IEmailService
    {
        #region Objects & Variables
        private readonly ISystemService _systemService;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        #endregion

        #region Constructor
        public EmailService(ISystemService systemService, ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository)
        {
            _systemService = systemService;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
        }
        #endregion

        #region Functions
        public bool SendEmail(EmailModel mail)
        {
            try
            {
                var configSMTP = _systemEnvironment_GlobalRepository.FindByAppKey("OO_EMAIL_CONFIGURATION");
                SmtpClient smtp = new SmtpClient
                {
                    Host = configSMTP.AppValue1,
                    EnableSsl = false
                };

                var email = EmailData_Mapping(mail);
                if (email != null)
                {
                    email.From = new MailAddress(configSMTP.AppValue2, configSMTP.AppValue3);
                    smtp.Send(email);
                    smtp.Dispose();
                    email.Dispose();
                }

                _systemService.CreateLogActivity(SystemActivityLog.SendEmail, string.Format("Successed on Sending Email {0}.", new object[] { mail.Subject }), "Email Service", HttpContext.Current.Request.UserHostAddress, ApplicationKey.OceanOnline);
                return true;
            }
            catch (Exception ex)
            {
                _systemService.CreateLogActivity(SystemActivityLog.SendEmail, string.Format("Failed on Sending Email {0}.", new object[] { mail.Subject }), "Email Service", HttpContext.Current.Request.UserHostAddress, ApplicationKey.OceanOnline);
                return false;
            }
        }
        #endregion

        #region Private Functions
        private MailMessage EmailData_Mapping(EmailModel mail)
        {
            MailMessage mailMessage = new MailMessage()
            {
                Subject = mail.Subject,
                Body = mail.Body,
            };

            mailMessage.IsBodyHtml = true;
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mail.Body, Encoding.UTF8, MediaTypeNames.Text.Html));

            foreach (var m in mail.EmailTo)
            {
                mailMessage.To.Add(m.Email);
            }

            return mailMessage;
        }
        #endregion
    }
    #endregion
}
