using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.ModelViews.Masters;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messaging.SystemConfigurationAdditional
{
    public class SystemConfigurationAdditionalRequest : RequestBase
    {
        public List<EmailDomainsView> EmailDomainsListModel { get; set; } = new List<EmailDomainsView>();
        public List<PreDefinedEmailsView> PreDefinedEmailsListModel { get; set; } = new List<PreDefinedEmailsView>();
        public List<SystemEnvironmentView> SystemEnvironmentListModel { get; set; } = new List<SystemEnvironmentView>();
        public List<CountryView> selectedCountries { get; set; } = new List<CountryView>();
        public List<NotificationConfigPeriodsView> NotificationConfigPeriodsModel { get; set; } = new List<NotificationConfigPeriodsView>();

        public NotificationConfigPeriodsView NotificationConfigPeriod;
    }

    public class SystemConfigurationAdditionalResponse
    {
        public Guid? Guid { get; set; } // For response new Guid       
        public SystemMessageView tblMessage { get; set; }
        public List<EmailDomainsView> EmailDomainsListModel { get; set; } = new List<EmailDomainsView>();
        public List<PreDefinedEmailsView> PreDefinedEmailsListModel { get; set; } = new List<PreDefinedEmailsView>();
        public List<SystemEnvironmentView> SystemEnvironmentListModel { get; set; } = new List<SystemEnvironmentView>();
        public List<NotificationConfigPeriodsView> NotificationConfigPeriodsList { get; set; } = new List<NotificationConfigPeriodsView>();
        public NotificationConfigPeriodsView NotificationConfigPeriodsModel { get; set; } = new NotificationConfigPeriodsView();
    }

    public class EmailDomainsRequest : EmailDomainsView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    public class PreDefinedEmailsRequest : PreDefinedEmailsView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    public class SystemEnvironmentRequest : SystemEnvironmentView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    public class NotificationConfigPeriodsRequest : NotificationConfigPeriodsView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    


}
