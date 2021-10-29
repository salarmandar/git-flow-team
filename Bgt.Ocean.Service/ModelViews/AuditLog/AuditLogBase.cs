using System;

namespace Bgt.Ocean.Service.ModelViews.AuditLog
{
    public class AuditLogBase
    {
        public Guid AuditLogGuid { get; set; }
        public int? MsgID { get; set; }
        public string Message { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
    }
}
