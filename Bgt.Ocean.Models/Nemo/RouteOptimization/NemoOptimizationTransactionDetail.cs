using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationTransactionDetail
    {
        public Guid NemoQueueOptimizationDetail_Guid { get; set; }
        public int Sequence { get; set; }
        public Guid OldLocation_Guid { get; set; }
        public string OldLocation_Code { get; set; }
        public string OldLocation_Name { get; set; }
        public string OldLocation_Lattitude { get; set; }
        public string OldLocation_Longtitude { get; set; }
        public int? NewSequence { get; set; }
        public Guid? NewLocation_Guid { get; set; }
        public string NewLocation_Code { get; set; }
        public string NewLocation_Name { get; set; }
        public string NewLocation_Lattitude { get; set; }
        public string NewLocation_Longtitude { get; set; }
    }
}
