using System;

namespace Bgt.Ocean.Models.SystemConfigurationAdditional
{
    public class SystemNotificationConfigPeriodsView
    {
        public System.Guid NotificationConfigPeriodsGuid { get; set; }
        public System.DateTime InitialDate { get; set; }
        public System.DateTime FinalDate { get; set; }
        public int DaysBeforeDueDate { get; set; }
        public int NotifyBeforeDueDate { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public bool FlagDisable { get; set; }
        public Guid Country { get; set; }
        public string PeriodTitle { get; set; }
        public System.Guid SystemEnvironmentMasterCountry_Guid { get; set; }
        public string textMailList { get; set; }
    }

    public class SystemEmailDomainsView
    {
        /// <summary>
        /// Gets or sets AllowedDomain_Guid.
        /// </summary>
        public Guid? AllowedDomain_Guid { get; set; }

        /// <summary>
        /// Gets or sets AllowedDomain.
        /// </summary>
        public string AllowedDomain { get; set; }

    }

    public class SystemPreDefinedEmailsView
    {
        /// <summary>
        /// Gets or sets UserAccessGroupCountryEmailGuid.
        /// </summary>
        public Guid? UserAccessGroupCountryEmailGuid { get; set; }

        /// <summary>
        /// Gets or sets CountryGuid.
        /// </summary>
        public Guid CountryGuid { get; set; }

        /// <summary>
        /// Gets or sets AllowedEmailList.
        /// </summary>
        public string AllowedEmailList { get; set; }

        /// <summary>
        /// Gets or sets Country Name.
        /// </summary>
        public string CountryName { get; set; }
    }

    public class SystemEnvironmentGlobalView
    {
        /// <summary>
        /// Gets or sets SystemEnvironment_Guid.
        /// </summary>
        public Guid? SystemEnvironmentGuid { get; set; }

        /// <summary>
        /// Gets or sets AppKey.
        /// </summary>
        public string SystemEnvironmentAppKey { get; set; }

        /// <summary>
        /// Gets or sets AppValue1.
        /// </summary>
        public string SystemEnvironmentAppValue1 { get; set; }

        /// <summary>
        /// Gets or sets AppValue2.
        /// </summary>
        public string SystemEnvironmentAppValue2 { get; set; }

        /// <summary>
        /// Gets or sets AppValue3.
        /// </summary>
        public string SystemEnvironmentAppValue3 { get; set; }

        /// <summary>
        /// Gets or sets AppValue4.
        /// </summary>
        public string SystemEnvironmentAppValue4 { get; set; }

        /// <summary>
        /// Gets or sets AppValue5.
        /// </summary>
        public string SystemEnvironmentAppValue5 { get; set; }

        /// <summary>
        /// Gets or sets AppValue6.
        /// </summary>
        public string SystemEnvironmentAppValue6 { get; set; }

        /// <summary>
        /// Gets or sets AppValue7.
        /// </summary>
        public string SystemEnvironmentAppValue7 { get; set; }

        /// <summary>
        /// Gets or sets AppValue8.
        /// </summary>
        public string SystemEnvironmentAppValue8 { get; set; }

        /// <summary>
        /// Gets or sets AppValue9.
        /// </summary>
        public string SystemEnvironmentAppValue9 { get; set; }

        /// <summary>
        /// Gets or sets AppDescription.
        /// </summary>
        public string SystemEnvironmentAppDescription { get; set; }
    }
}
