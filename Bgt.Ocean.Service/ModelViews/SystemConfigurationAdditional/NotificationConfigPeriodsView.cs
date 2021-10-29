using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional
{
    public class NotificationConfigPeriodsView
    {
        public System.Guid NotificationConfigPeriodsGuid { get; set; }
        public System.DateTime InitialDate { get; set; }
        public System.DateTime FinalDate { get; set; }
        /// <summary>
        /// Days for notification GlobalUserAdmon
        /// </summary>
        public int DaysBeforeDueDate { get; set; }
        /// <summary>
        /// Days for Notification LocalUser this parameter will been used for WinService Notification.
        /// </summary>
        public int NotifyBeforeDueDate { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public bool FlagDisable { get; set; }
        public Guid Country { get; set; }
        public string PeriodTitle { get; set; }
        public List<NotificationConfigPeriodsUsers> NotificationPeriodDetail { get; set; }
        public System.Guid SystemEnvironmentMasterCountry_Guid { get; set; }
    }

    public class NotificationConfigPeriodsUsers
    {
        public Guid GlobalUserGuid { get; set; }
        public Guid SystemNotificationConfigPeriodsGuid { get; set; }
        public string Email { get; set; }
        public string UserCreated { get; set; }
        public bool FlagDisable { get; set; }
        public string UserName { get; set; }
        public bool IsExternalGlobalAdmin { get; set; }
    }
}