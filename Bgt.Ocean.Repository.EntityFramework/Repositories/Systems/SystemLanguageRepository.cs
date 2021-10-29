using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemLanguageRepository : IRepository<TblSystemLanguage>
    {
    }

    public class SystemLanguageRepository : Repository<OceanDbEntities, TblSystemLanguage>, ISystemLanguageRepository
    {
        public SystemLanguageRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
