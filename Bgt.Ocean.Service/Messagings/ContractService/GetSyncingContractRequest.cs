using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ContractService
{
    public class GetSyncingContractRequest : RequestBase
    {
        [Required]
        public Guid Contract_Guid { get; set; }
    }
}
