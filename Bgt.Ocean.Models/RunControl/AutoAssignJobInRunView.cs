using System;

namespace Bgt.Ocean.Models.RunControl
{
    public class ValidateJobsInRunView
    {
        public int JobTypeID { get; set; }
        public Guid JobHeaderGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public string JobNoID { get; set; }
    }
}
