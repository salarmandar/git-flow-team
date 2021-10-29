using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    public class TabDetailView
    {
        public string JobID { get; set; }
        public string LOB { get; set; }
        public DateTime? ServiceStopTransectionDate { get; set; }
        public DateTime? WindowsTimeServiceTimeStart { get; set; }
        public string WorkDate { get; set; }
        public string ServiceTypeName { get; set; }
        public string ScheduleTime { get; set; }
        public string ProblemCode { get; set; }

        public string CustomerName { get; set; }
        public string LocationName { get; set; }

        public string ATMID { get; set; }
        public string MachineModel { get; set; }
        public string PlaceName { get; set; }
        public string LockType1 { get; set; }
        public string TranferSafeID { get; set; }
        public string LockType2 { get; set; }
        public Guid? Machine_Guid { get; set; }
        public string ContractNo { get; set; }
    }

    public class HidePanel
    {
        public JobScreen JobScreen { get; set; }
        public IEnumerable<JobField> JobField { get; set; } = new List<JobField>();
    }

}
