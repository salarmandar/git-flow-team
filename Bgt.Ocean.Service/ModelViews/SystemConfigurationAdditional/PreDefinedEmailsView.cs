using Bgt.Ocean.Service.ModelViews.Masters;
using System;

namespace Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional
{
    public class PreDefinedEmailsView
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
        /// Gets or sets Country.
        /// </summary>
        public CountryView Country { get; set; }

        /// <summary>
        /// Gets or sets AllowedEmailList.
        /// </summary>
        public string AllowedEmailList { get; set; }
    }
}