using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class SaveCopyProductRequest : RequestBase
    {
        public Guid Quotation_Guid { get; set; }
        public Guid MasterCountry_Guid { get; set; }
        public Guid SourceProduct_Guid { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> SystemServiceType_Guids { get; set; }
    }
}
