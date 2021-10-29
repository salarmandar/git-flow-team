using System;

namespace Bgt.Ocean.Service.Messagings.Consolidation
{
    public class ConsolidationMainRequest
    {
        public string workDate { get; set; }
        public Guid siteGuid { get; set; }
        public string userFormatDate { get; set; }
    }
}
