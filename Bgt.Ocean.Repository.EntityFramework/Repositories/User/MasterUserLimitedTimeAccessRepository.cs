using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.User
{
    #region Interface
    public interface IMasterUserLimitedTimeAccessRepository : IRepository<TblMasterUserLimitedTimeAccess>
    {

    }
    #endregion

    #region Functions
    public class MasterUserLimitedTimeAccessRepository : Repository<OceanDbEntities, TblMasterUserLimitedTimeAccess>, IMasterUserLimitedTimeAccessRepository
    {
        public MasterUserLimitedTimeAccessRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
    #endregion
}
