using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Models.SystemsModel
{
    public class DayOfWeekView : ModelBase
    {
        public string MasterDayOfWeek_Name { get; set; }
        public int? MasterDayOfWeek_Sequence { get; set; }
        public int? DayOfWeekSequence { get; set; }
        public Guid Id { get; set; }
        public string DisplayText { get; set; }
        public bool FlagHoliday { get; set; }
        public bool FlagParent { get { return true; } }

    }
}
