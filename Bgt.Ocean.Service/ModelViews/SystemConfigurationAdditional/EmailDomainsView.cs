using System;

namespace Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional
{
    public class EmailDomainsView
    {
        /// <summary>
        /// Gets or sets AllowedDomain_Guid.
        /// </summary>
        public Guid? AllowedDomain_Guid { get; set; }

        /// <summary>
        /// Gets or sets AllowedDomain.
        /// </summary>
        public string AllowedDomain { get; set; }

        public EmailDomainsView() { }
    }
}