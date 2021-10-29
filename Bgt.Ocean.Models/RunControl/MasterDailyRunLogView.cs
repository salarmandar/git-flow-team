using System;

namespace Bgt.Ocean.Models.RunControl
{
    public class MasterDailyRunLogView
    {
        public Guid MasterDailyRunResourceGuid { get; set; }
        public int MsgID { get; set; }
        public string JSONParameter { get; set; }
        public string UserCreated { get; set; }
        public DateTime ClientDate { get; set; }
    }
}
