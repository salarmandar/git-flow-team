using System;

namespace Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog
{
    public class ChangeInfo
    {
        public string LabelKey { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string OldValueRaw { get; set; }
        public string NewValue { get; set; }
        public string NewValueRaw { get; set; }
        public Guid? LogProcessGuid { get; set; }
        public Guid? LogCategoryGuid { get; set; }
    }
}
