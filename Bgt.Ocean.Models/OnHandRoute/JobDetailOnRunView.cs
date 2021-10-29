using System;

namespace Bgt.Ocean.Models.OnHandRoute
{
    public class JobDetailOnRunView
    {
        public Guid DailyRunGuid { get; set; }
        public Guid JobGuid { get; set; }
        public int JobSeq { get; set; }
        public string JobID { get; set; }
        public int JobTypeID { get; set; }
        public string JobType { get; set; }
        public int JobStatusID { get; set; }
        public string JobStatus { get; set; }
        public string JobAction { get; set; }
        public string LOB { get; set; }
        public string MachineID { get; set; }
        public string LocationName { get; set; }
        public int GroupJobTypeID { get; set; }
        public string GroupJobTypeName { get; set; }
    }
}
