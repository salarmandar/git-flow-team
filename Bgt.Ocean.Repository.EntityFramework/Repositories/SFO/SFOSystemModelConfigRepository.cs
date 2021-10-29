using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemModelConfigRepository : IRepository<SFOTblSystemModelConfig>
    {
        SFOTblSystemModelConfig GetConfigByKey(string configKey);
    }

    public class SFOSystemModelConfigRepository : Repository<SFOLogDbEntities, SFOTblSystemModelConfig>, ISFOSystemModelConfigRepository
    {
        public SFOSystemModelConfigRepository(IDbFactory<SFOLogDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SFOTblSystemModelConfig GetConfigByKey(string configKey)
            => FindAllAsQueryable().FirstOrDefault(e => e.ConfigKey == configKey);
    }
}
