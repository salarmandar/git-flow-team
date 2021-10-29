using Bgt.Ocean.Models.Customer;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.BrinksService
{
    public class CustomerContractRequest : RequestBase
    {
        public Guid? lobGuid { get; set; }
        public Guid? servicetypeGuid { get; set; }
        public Guid? subservicetypeGuid { get; set; }
        public IEnumerable<Guid> locationPKGuids { get; set; } = new List<Guid>();
        public IEnumerable<Guid> locationDLGuids { get; set; } = new List<Guid>();
        public DateTime? workdatePK { get; set; }
        public DateTime? workdateDL { get; set; }
    }

    public class CustomerContractResponse : BaseResponse
    {
        public IEnumerable<CustomerContractView> ContractList { get; set; }
    }


}
