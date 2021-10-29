using System;

namespace Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional
{
    public class ConfigurationAuditLogView
    {
        /// <summary>
        /// Gets or sets Log_Guid.
        /// </summary>
        public Guid? LogGuid { get; set; }

        /// <summary>
        /// Type of Event in number
        /// 1 = Add
        /// 2 = Edit
        /// 3 = Delete
        /// 0 = Undefined
        /// </summary>
        public int EventID { get; set; }

        /// <summary>
        /// Type of event in string
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// user who made the change
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Name of function
        /// </summary>
        public string ConfigType { get; set; }

        /// <summary>
        /// Gets or sets AppKey.
        /// </summary>
        public string LogAppKey { get; set; }

        /// <summary>
        /// Gets or sets AppValue1.
        /// </summary>
        public string LogAppValue1 { get; set; }

        /// <summary>
        /// Gets or sets AppValue2.
        /// </summary>
        public string LogAppValue2 { get; set; }

        /// <summary>
        /// Gets or sets AppValue3.
        /// </summary>
        public string LogAppValue3 { get; set; }

        /// <summary>
        /// Gets or sets AppValue4.
        /// </summary>
        public string LogAppValue4 { get; set; }

        /// <summary>
        /// Gets or sets AppValue5.
        /// </summary>
        public string LogAppValue5 { get; set; }

        /// <summary>
        /// Gets or sets AppValue6.
        /// </summary>
        public string LogAppValue6 { get; set; }

        /// <summary>
        /// Gets or sets AppValue7.
        /// </summary>
        public string LogAppValue7 { get; set; }

        /// <summary>
        /// Gets or sets AppValue8.
        /// </summary>
        public string LogAppValue8 { get; set; }

        /// <summary>
        /// Gets or sets AppValue9.
        /// </summary>
        public string LogAppValue9 { get; set; }

        /// <summary>
        /// Gets or sets AppDescription.
        /// </summary>
        public string LogAppDescription { get; set; }

        /// <summary>
        /// Date of change
        /// </summary>
        public string Date { get; set; }
    }
}
