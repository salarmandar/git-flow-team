using System;

namespace Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog
{
    public class ComparableConfigProperty
    {
        public string LabelKey { get; set; }
        public Guid? LogCategoryGuid { get; internal set; }
        public Guid? LogProcessGuid { get; internal set; }
        public string PropertyNameSource { get; set; }
        public string PropertyNameTarget { get; set; }
        public string QueryStrSetValue { get; set; }
        public int? Sequence { get; set; }
    }
}
