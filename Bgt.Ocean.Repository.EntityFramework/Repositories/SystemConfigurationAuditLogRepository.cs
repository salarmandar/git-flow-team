using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface ISystemConfigurationAuditLogRepository : IRepository<TblSystemConfigurationAuditLog>
    {
        void CreateLogDomainMail(SystemEmailDomainsView newEmailDomain, string eventType, string user, SystemEmailDomainsView OldEmailDomain = null);
        void CreateLogPredefinedMail(SystemPreDefinedEmailsView newPreDefinedEmails, string eventType, string user, SystemPreDefinedEmailsView oldPreDefinedEmails = null);
        void CreateLogSystemEnvironmentGlobal(SystemEnvironmentGlobalView newSystemEnvironmentGlobalView, string eventType, string user, SystemEnvironmentGlobalView oldSystemEnvironmentGlobalView = null);
        void CreateLogConfigNotificationPeriods(SystemNotificationConfigPeriodsView newNotificationPeriods, string eventType, string user, SystemNotificationConfigPeriodsView oldNotificationPeriods = null);
        IEnumerable<TblSystemConfigurationAuditLog> GetSystemLogList();
    }
    public class SystemConfigurationAuditLogRepository : Repository<OceanDbEntities, TblSystemConfigurationAuditLog>, ISystemConfigurationAuditLogRepository
    {
        private Dictionary<string, string> _columnsName = new Dictionary<string, string>()
        {
            {"AllowedDomain", "Domain"},
            { "AllowedEmailList", "E-mails"},
            {"CountryName", "Country"},
            {"SystemEnvironmentAppKey", "Appkey"},
            {"SystemEnvironmentAppDescription", "Description"},
            {"SystemEnvironmentAppValue1", "Value1"},
            {"SystemEnvironmentAppValue2", "Value2"},
            {"SystemEnvironmentAppValue3", "Value3"},
            {"SystemEnvironmentAppValue4", "Value4"},
            {"SystemEnvironmentAppValue5", "Value5"},
            {"SystemEnvironmentAppValue6", "Value6"},
            {"SystemEnvironmentAppValue7", "Value7"},
            {"SystemEnvironmentAppValue8", "Value8"},
            {"SystemEnvironmentAppValue9", "Value9"},
            {"InitialDate", "Initial Date"},
            {"FinalDate", "Final Date"},
            {"DaysBeforeDueDate", "Days Notification Global Admin"},
            {"NotifyBeforeDueDate", "Days Local Admin User"},
            {"PeriodTitle", "Period Title"},
            {"FlagDisable", "Disable"},
            {"textMailList", "Email List"}
        };
        public SystemConfigurationAuditLogRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {

        }

        private string GetLogText(string configType, string eventType, string keyValue, string columnName, string newValue, string oldValue = "")
        {
            switch (eventType)
            {
                case "Add":
                    return string.Format("{0}: {1} has been added. {2} {3}", configType, keyValue, columnName, newValue);
                case "Edit":
                    if(oldValue == "" || oldValue == string.Empty)
                    {
                        oldValue = "(empty)";
                    }
                    return string.Format("{0}: {1} has been changed {2} from {3} to {4}", configType, keyValue, columnName, oldValue, newValue);
                case "Delete":
                    return string.Format("{0}: {1} has been deleted. {2} {3}", configType, keyValue, columnName, newValue);
                case "Enable":
                    return string.Format("{0}: {1} has been enabled", configType, keyValue);
                case "Disable":
                    return string.Format("{0}: {1} has been disabled", configType, keyValue);
                default:
                    return "";
            }
        }

        private void SetGeneralLog <SystemView>(string configType, string eventType, string keyValue, string user, SystemView newValue, SystemView oldValue)
        {
            var logMessage = "";
            foreach (PropertyInfo info in newValue.GetType().GetProperties())
            {
                string columnName = _columnsName.FirstOrDefault(x => x.Key == info.Name).Value;
                if (columnName != null)
                {
                    string newValueText = info.GetValue(newValue) != null && info.GetValue(newValue).ToString() != "" ? 
                        (info.PropertyType != typeof(DateTime) ? info.GetValue(newValue).ToString() : ((DateTime)info.GetValue(newValue)).ToString("dd/MM/yyyy")) : "(empty)";
                    string oldValueText = oldValue != null && info.GetValue(oldValue) != null && info.GetValue(oldValue).ToString() != "" ? 
                        (info.PropertyType != typeof(DateTime) ? info.GetValue(oldValue).ToString() : ((DateTime)info.GetValue(oldValue)).ToString("dd/MM/yyyy")) : "(empty)";
                    if(newValueText != oldValueText)
                    {
                        logMessage = GetLogText(configType, eventType, keyValue, columnName, newValueText, (oldValue != null) ? oldValueText : "");
                        CreateLogSystemConfiguration(getEnventId(eventType), eventType, configType, user,(eventType!="Edit"? newValueText : oldValueText), logMessage);
                    }
                }
            }
        }

        public void CreateLogDomainMail(SystemEmailDomainsView newEmailDomain, string eventType, string user, SystemEmailDomainsView OldEmailDomain = null)
        {
            SetGeneralLog("DomainEmail", eventType, newEmailDomain.AllowedDomain, user, newEmailDomain, OldEmailDomain);
        }

        public void CreateLogPredefinedMail(SystemPreDefinedEmailsView newPreDefinedEmails, string eventType, string user, SystemPreDefinedEmailsView oldPreDefinedEmails = null)
        {
            SetGeneralLog("PreDefinedEmail", eventType, newPreDefinedEmails.AllowedEmailList, user, newPreDefinedEmails, oldPreDefinedEmails);
        }

        public void CreateLogSystemEnvironmentGlobal(SystemEnvironmentGlobalView newSystemEnvironmentGlobalView, string eventType, string user, SystemEnvironmentGlobalView oldSystemEnvironmentGlobalView = null)
        {
            SetGeneralLog("SystemEnvironmentGlobal", eventType, newSystemEnvironmentGlobalView.SystemEnvironmentAppKey, user, newSystemEnvironmentGlobalView, oldSystemEnvironmentGlobalView);
        }

        public void CreateLogConfigNotificationPeriods(SystemNotificationConfigPeriodsView newNotificationPeriods, string eventType, string user, SystemNotificationConfigPeriodsView oldNotificationPeriods = null)
        {
            SetGeneralLog("NotificationPeriods", eventType, newNotificationPeriods.PeriodTitle, user, newNotificationPeriods, oldNotificationPeriods);
        }

        private int getEnventId(string eventType)
        {
            switch (eventType)
            {
                case "Add":
                    return 1;
                case "Edit":
                    return 2;
                case "Delete":
                    return 3;
                case "Enable":
                    return 4;
                case "Disable":
                    return 5;
                default:
                    return 0;
            }
        }

        private void CreateLogSystemConfiguration(int systemEventId, string eventType, string configType, string user,
            string appKey = null, string appDescription = null)
        {
            TblSystemConfigurationAuditLog newLog = new TblSystemConfigurationAuditLog();
            newLog.Guid = Guid.NewGuid();
            newLog.SystemEvet_ID = systemEventId;
            newLog.Event = eventType;
            newLog.ConfigType = configType;
            newLog.UseID = user;
            newLog.AppKey = (appKey != null ? appKey : "");
            newLog.AppDescription = appDescription;
            newLog.DateTimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            Create(newLog);
        }


        public IEnumerable<TblSystemConfigurationAuditLog> GetSystemLogList()
        {
            return DbContext.TblSystemConfigurationAuditLog.OrderByDescending(x => x.DateTimeCreated).ToList();
        }
    }

}
