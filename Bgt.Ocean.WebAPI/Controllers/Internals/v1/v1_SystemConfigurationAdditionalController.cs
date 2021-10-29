using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.ModelViews.Users;
using Bgt.Ocean.Service.Messaging.SystemConfigurationAdditional;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_SystemConfigurationAdditionalController : ApiControllerBase
    {
        private readonly ISystemDomainEmailService _systemDomainEmailService;
        private readonly ISystemEnvironmentService _systemEnvironmentGlobalService;
        private readonly IMasterUserAccessGroupCountryEmailService _masterUserAccessGroupCountryEmailService;
        private readonly ISystemNotificationPeriodsService _systemNotificationConfigPeriodsService;
        private readonly ISystemConfigurationAuditLogService _systemConfigurationAuditLogService;

        public v1_SystemConfigurationAdditionalController(ISystemDomainEmailService systemDomainEmailService, 
            ISystemEnvironmentService systemEnvironmentGlobalService,
            IMasterUserAccessGroupCountryEmailService masterUserAccessGroupCountryEmailService,
            ISystemNotificationPeriodsService systemNotificationConfigPeriodsService,
            ISystemConfigurationAuditLogService systemConfigurationAuditLogService) {
            _systemDomainEmailService = systemDomainEmailService;
            _systemEnvironmentGlobalService = systemEnvironmentGlobalService;
            _masterUserAccessGroupCountryEmailService = masterUserAccessGroupCountryEmailService;
            _systemNotificationConfigPeriodsService = systemNotificationConfigPeriodsService;
            _systemConfigurationAuditLogService = systemConfigurationAuditLogService;
        }

        #region Email Domains
        [HttpGet]
        public IEnumerable<EmailDomainsView> GetEmailDomainsInfo()
        {
            return _systemDomainEmailService.GetEmailDomainsInfo();
        }

        [HttpPost]
        public SystemMessageView CreateUpdateEmailDomains(EmailDomainsRequest EmailDomain)
        {
            return _systemDomainEmailService.CreateUpdateEmailDomains(EmailDomain);
        }

        [HttpPost]
        public SystemMessageView DeleteEmailDomain(EmailDomainsRequest EmailDomain)
        {
            return _systemDomainEmailService.DeleteEmailDomain(EmailDomain);
        }
        #endregion

        #region Pre-Defined Emails
        [HttpGet]
        public IEnumerable<PreDefinedEmailsView> GetPreDefinedEmailsInfo()
        {
            return _masterUserAccessGroupCountryEmailService.GetPreDefinedEmailsInfo();
        }

        [HttpPost]
        public SystemMessageView CreateUpdatePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails)
        {
            return _masterUserAccessGroupCountryEmailService.CreateUpdatePreDefinedEmails(preDefinedEmails);
        }

        [HttpPost]
        public SystemMessageView DeletePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails)
        {
            return _masterUserAccessGroupCountryEmailService.DeletePreDefinedEmails(preDefinedEmails);
        }
        #endregion

        #region System Environment Global
        [HttpGet]
        public IEnumerable<SystemEnvironmentView> GetSystemEnvironmentInfo()
        {
            return _systemEnvironmentGlobalService.GetSystemEnvironmentInfo();
        }

        [HttpPost]
        public SystemMessageView CreateUpdateSystemEnvironmentGlobal(SystemEnvironmentRequest systemEnvironmentGlobal)
        {
            return _systemEnvironmentGlobalService.CreateUpdateSystemEnvironment(systemEnvironmentGlobal);
        }

        [HttpPost]
        public SystemMessageView DeleteSystemEnvironmentGlobal(SystemEnvironmentRequest systemEnvironmentGlobal)
        {
            return _systemEnvironmentGlobalService.DeleteSystemEnvironmentGlobal(systemEnvironmentGlobal);
        }
        #endregion

        #region Notification Periods
        [HttpGet]
        public IEnumerable<NotificationConfigPeriodsView> GetNotificationPeriodsInfo(Guid countryGuid, bool flagDisable)
        {
            return _systemNotificationConfigPeriodsService.GetNotificationPeriodsInfo(countryGuid, flagDisable);
        }

        [HttpPost]
        public SystemMessageView CreateUpdateNotificationPeriods(NotificationConfigPeriodsRequest notificationPeriods)
        {
            return _systemNotificationConfigPeriodsService.CreateUpdateNotificationPeriods(notificationPeriods);
        }

        [HttpGet]
        public NotificationConfigPeriodsView SearchGetNofiticacionConfigPeriodById(Guid Guid)
        {
            return _systemNotificationConfigPeriodsService.SearchGetNofiticacionConfigPeriodById(Guid);
        }

        [HttpPost]
        public SystemMessageView DisableOrEnableNotificationConfigPeriod(NotificationConfigPeriodsRequest notificationPeriods)
        {
            return _systemNotificationConfigPeriodsService.DisableOrEnableNotificationConfigPeriod(notificationPeriods);
        }

        [HttpGet]
        public IEnumerable<UserView> GetInfoGlobalAdmin()
        {
            return _systemNotificationConfigPeriodsService.GetInfoGlobalAdmin();
        }
        #endregion

        #region System Log
        [HttpGet]
        public IEnumerable<ConfigurationAuditLogView> GetSystemLogInfo()
        {
            return _systemConfigurationAuditLogService.GetSystemLogInfo();
        }
        #endregion

    }
}
