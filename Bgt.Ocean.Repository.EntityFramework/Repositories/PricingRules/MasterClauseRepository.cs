using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IMasterClauseRepository : IRepository<TblLeedToCashMasterClause>
    {
    }

    public class MasterClauseRepository : Repository<OceanDbEntities, TblLeedToCashMasterClause>, IMasterClauseRepository
    {
        public MasterClauseRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
