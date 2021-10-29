
namespace Bgt.Ocean.WebAPI.Models.AuditLog
{
    public class BaseAuditLogModel
    {
        public string ReferenceValue { get; set; }
        public string UserCreated { get; set; }
    }

    public class AuditLogUpdateModel<TSource, TTarget> : BaseAuditLogModel
        where TSource : class
        where TTarget : class
    {
        public string PrefixDescription { get; set; }
        public string ConfigKey { get; set; }
        
        public TSource SourceModel { get; set; }
        public TTarget TargetModel { get; set; }

    }
}