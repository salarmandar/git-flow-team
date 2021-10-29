using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterLogVerifyKeyRepository : IRepository<TblMasterLogVerifyKey>
    {
        TblMasterLogVerifyKey GetActiveKey(string key);
    }

    public class MasterLogVerifyKeyRepository : Repository<OceanDbEntities, TblMasterLogVerifyKey>, IMasterLogVerifyKeyRepository
    {
        public MasterLogVerifyKeyRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterLogVerifyKey GetActiveKey(string key)
        {
            return DbContext.TblMasterLogVerifyKey.FirstOrDefault(o => o.Verify_key == key && o.Action);
        }
    }
}
