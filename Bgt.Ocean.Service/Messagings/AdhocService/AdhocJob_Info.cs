using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{

    public class CreateMultiJobRequest
    {
        public IEnumerable<Guid> MasterCustomerLocationGuids { get; set; }
        public Guid BrinksSiteGuid { get; set; }
        public int MaxStop { get; set; }
        public bool IsCreateToRun { get; set; }
        public DateTime UnassignedDate { get; set; }
        public string UnassignedBy { get; set; }
    }
    public class AdhocJob_Info //For create Job P, D
    {
        public Guid JobGuid { get; set; }
        public Guid LocationGuid { get; set; }
        public string JobNo { get; set; }
        public int LocationSeq { get; set; }
        public DateTime? UnassignedDate { get; set; }
        public string UnassignedBy { get; set; }
    }
}
