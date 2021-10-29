using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class CheckRateExceedRequest : RequestBase
    {
        [Required]
        public Guid Quotation_Guid { get; set; }

        /// <summary>
        /// User as sale person
        /// </summary>
        [Required]
        public Guid MasterUser_Guid { get; set; }
    }
}
