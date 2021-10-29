using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Products
{
    public class ProductView
    {
        public Guid Guid { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDisplayName { get; set; }
        public string LOBDisplayName { get; set; }
        public Guid? SystemLineOfBusiness_Guid { get; set; }
        public IEnumerable<Guid> ServiceType_Guids { get; set; }
        public string Description { get; set; }
    }
}
