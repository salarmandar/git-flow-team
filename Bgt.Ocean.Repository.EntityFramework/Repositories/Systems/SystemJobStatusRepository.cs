using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
  

    public interface ISystemJobStatusRepository : IRepository<TblSystemJobStatus>
    {
        TblSystemJobStatus FindByStatusID(int? id);
    }

    public class SystemJobStatusRepository : Repository<OceanDbEntities, TblSystemJobStatus>, ISystemJobStatusRepository
    {
        public SystemJobStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemJobStatus FindByStatusID(int? id)
        {
            return DbContext.TblSystemJobStatus.FirstOrDefault(e => e.StatusJobID == id);
        }

    }
}
