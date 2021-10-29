using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.AuditLog
{
    #region interface

    public interface ISystemCustomerLocationAuditLogRepository : IRepository<TblSystemCustomerLocation_Audit_Log> { }

    #endregion

    public class SystemCustomerLocationAuditLogRepository : Repository<SFOLogDbEntities, TblSystemCustomerLocation_Audit_Log>, ISystemCustomerLocationAuditLogRepository
    {
        public SystemCustomerLocationAuditLogRepository(IDbFactory<SFOLogDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
