using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOTblSystemLogCategoryRepository : IRepository<SFOTblSystemLogCategory>
    {
        SFOTblSystemLogCategory FindCategoryByCode(string categoryCode);
    }
    public class SFOTblSystemLogCategoryRepository : Repository<OceanDbEntities, SFOTblSystemLogCategory>, ISFOTblSystemLogCategoryRepository
    {
        public SFOTblSystemLogCategoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        public SFOTblSystemLogCategory FindCategoryByCode(string categoryCode)
        {
            return DbContext.SFOTblSystemLogCategory.FirstOrDefault(f => f.CategoryCode == categoryCode);
        }
    }      
}
