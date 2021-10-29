using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemJobActionsRepository : IRepository<TblSystemJobAction>
    {
        TblSystemJobAction FindByAbbrevaition(string abbrevaition);
    }

    public class SystemJobActionsRepository : Repository<OceanDbEntities, TblSystemJobAction>, ISystemJobActionsRepository
    {
        public SystemJobActionsRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemJobAction FindByAbbrevaition(string abbrevaition)
        {
            return DbContext.TblSystemJobAction.FirstOrDefault(e => e.ActionNameAbbrevaition == abbrevaition);
        }
    }
}
