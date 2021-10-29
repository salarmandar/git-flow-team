using Bgt.Ocean.Models;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class SystemConfigurationAdditionalMapper
    {
        #region Domain Email
        public static IEnumerable<EmailDomainsView> ConvertToSystemDomainEmailWhiteListView(this IEnumerable<TblSystemDomainEmailWhiteList> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemDomainEmailWhiteList>, IEnumerable<EmailDomainsView>>(model);
        }

        public static TblSystemDomainEmailWhiteList ConvertToTblSystemDomainEmailWhite(this EmailDomainsView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<EmailDomainsView, TblSystemDomainEmailWhiteList>(model);
        }

        public static SystemEmailDomainsView ConvertToSystemDomainEmailView(this EmailDomainsView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<EmailDomainsView, SystemEmailDomainsView>(model);
        }

        public static SystemEmailDomainsView ConvertToSystemDomainEmailView(this TblSystemDomainEmailWhiteList model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblSystemDomainEmailWhiteList, SystemEmailDomainsView>(model);
        }
        #endregion

        #region Global Envyronment Config
        public static IEnumerable<SystemEnvironmentView> ConvertToSystemEnvironmentViewListView(this IEnumerable<TblSystemEnvironment_Global> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemEnvironment_Global>, IEnumerable<SystemEnvironmentView>>(model);
        }

        public static TblSystemEnvironment_Global ConvertToTblSystemEnvironment_Global(this SystemEnvironmentView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<SystemEnvironmentView, TblSystemEnvironment_Global>(model);
        }

        public static SystemEnvironmentGlobalView ConvertToSystemEnvironmentGlobalView(this SystemEnvironmentView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<SystemEnvironmentView, SystemEnvironmentGlobalView>(model);
        }

        public static SystemEnvironmentView ConvertToSystemEnvironmentView(this SystemEnvironmentGlobalView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map< SystemEnvironmentGlobalView, SystemEnvironmentView>(model);
        }
        public static SystemEnvironmentGlobalView ConvertToSystemEnvironmentGlobalView(this TblSystemEnvironment_Global model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblSystemEnvironment_Global, SystemEnvironmentGlobalView>(model);
        }
        #endregion

        #region Pre Defined Emails
        public static IEnumerable<PreDefinedEmailsView> ConvertToPreDefinedEmailsView(this IEnumerable<TblMasterUserAccessGroupCountryEmail> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterUserAccessGroupCountryEmail>, IEnumerable<PreDefinedEmailsView>>(model);
        }

        public static TblMasterUserAccessGroupCountryEmail ConvertToTblMasterUserAccessGroupCountryEmail(this PreDefinedEmailsView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<PreDefinedEmailsView, TblMasterUserAccessGroupCountryEmail>(model);
        }

        public static SystemPreDefinedEmailsView ConvertToSystemPreDefinedEmailsView(this PreDefinedEmailsView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<PreDefinedEmailsView, SystemPreDefinedEmailsView>(model);
        }

        public static SystemPreDefinedEmailsView ConvertToSystemPreDefinedEmailsView(this TblMasterUserAccessGroupCountryEmail model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblMasterUserAccessGroupCountryEmail, SystemPreDefinedEmailsView>(model);
        }
        #endregion

        #region Notification Periods Config
        public static IEnumerable<NotificationConfigPeriodsView> ConvertToNotificationConfigPeriodsView(this IEnumerable<SystemNotificationConfigPeriodsView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SystemNotificationConfigPeriodsView>, IEnumerable<NotificationConfigPeriodsView>>(model);
        }

        public static SystemNotificationConfigPeriodsView ConvertToSystemNotificationConfigPeriodsView(this TblSystemNotificationConfigPeriods model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblSystemNotificationConfigPeriods, SystemNotificationConfigPeriodsView>(model);
        }

        public static IEnumerable<TblSystemNotificationConfigPeriodsUsers> ConvertToSystemNotificationConfigPeriodsView(this IEnumerable<NotificationConfigPeriodsUsers> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map< IEnumerable<NotificationConfigPeriodsUsers>, IEnumerable<TblSystemNotificationConfigPeriodsUsers>>(model);
        }
        #endregion

        #region System Log
        public static IEnumerable<ConfigurationAuditLogView> ConvertToConfigurationAuditLogListView(this IEnumerable<TblSystemConfigurationAuditLog> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemConfigurationAuditLog>, IEnumerable<ConfigurationAuditLogView>>(model);
        }
        #endregion
    }
}
