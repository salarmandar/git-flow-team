
namespace Bgt.Ocean.WebAPI.Models.AuditLog.CustomerLocation
{
    public class AuditLogCustomerLocationActionModel : BaseAuditLogModel
    {
        public string LocationName { get; set; }
        public string Action { get; set; }
    }
}