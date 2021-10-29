using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    public class SvdChecklist
    {
        public IEnumerable<CheckListItem> CheckList { get; set; } = new List<CheckListItem>();
        public string Remarks { get; set; }
    }

    public class CheckListItem
    {
        public bool IsChecked { get; set; }
        public string Item { get; set; }
    }
}
