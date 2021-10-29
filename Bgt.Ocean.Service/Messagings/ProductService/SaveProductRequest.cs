using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.ProductService
{
    public class SaveProductRequest : RequestBase
    {
        public Guid Country_Guid { get; set; }
        public Guid Company_Guid { get; set; }
        public Guid? Guid { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public Guid SystemLineOfBusiness_Guid { get; set; }
        public IEnumerable<Guid> ServiceType_Guids { get; set; }
        public string Description { get; set; }
    }
}
