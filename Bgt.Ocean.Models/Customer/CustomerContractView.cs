using System;

namespace Bgt.Ocean.Models.Customer
{
    public class CustomerContractView
    {
        public Guid? ContractGuid { get; set; }
        public string ContractNo { get; set; }
        public string ContractFullName { get; set; }
        public string strExpiredDate { get; set; }
    }
}
