using System;

namespace Bgt.Ocean.Models.RunControl
{
  public  class JobLegsView
    {
        public Guid JobGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public string JobNo { get; set; }
        public string CustomerLocateionName { get; set; }
    }
}
