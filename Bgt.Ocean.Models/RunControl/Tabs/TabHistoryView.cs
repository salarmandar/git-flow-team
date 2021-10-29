using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    public class TabHistoryView
    {
        public IEnumerable<HistoryView> HistoryList { get; set; }
    }

    public class HistoryView
    {
        public string Command { get; set; }
        public string Event { get; set; }
        public string UserName { get; set; }
        public string DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
    }
}
