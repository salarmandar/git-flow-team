using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.StandardTable
{
    public class BaseRequestQuery
    {
        /// <summary>
        /// Country abbreviation (Ex. US, CA, MX)
        /// </summary>
        [Required]
        public string countryAbb { get; set; }

        /// <summary>
        /// Required format yyyy-MM-dd HH:mm (Ex. 2018-12-31 08:30) in UTC +0
        /// </summary>
        public string createdDatetimeFrom { get; set; }

        /// <summary>
        /// Required format yyyy-MM-dd HH:mm (Ex. 2018-12-31 23:30) in UTC +0
        /// </summary>
        public string createdDatetimeTo { get; set; }
    }
}
