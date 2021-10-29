using System;

namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class SystemDayOfWeekView
    {
        public System.Guid Guid { get; set; }
        public string MasterDayOfWeek_Name { get; set; }
        public Nullable<int> MasterDayOfWeek_Sequence { get; set; }
        public Nullable<System.Guid> SystemDisplayTextControls_Guid { get; set; }
        public string MasterDayOfWeekNameDisplayText { get; set; }
        public bool FlagDisable { get; set; }
    }
}
