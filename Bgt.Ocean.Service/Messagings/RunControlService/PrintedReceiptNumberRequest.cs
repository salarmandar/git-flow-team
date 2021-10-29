using System;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class PrintedReceiptNumberRequest
    {
        public Guid CustomerLocation_Guid { get; set; }
        public int? SequenceStop { get; set; }
        public DateTime? ServiceStopTransectionDate { get; set; }
        public string BranchCodeReference { get; set; }
        public string SiteCode { get; set; }
        public string JobNo { get; set; }
        public string PrintedReceiptNumber { get; set; }
    }
}
