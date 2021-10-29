using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Collections.Generic;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterMenuDetailRepository : IRepository<TblMasterMenuDetail>
    {
        IEnumerable<MasterMenuDetailResult> Func_Menu_Get(string strMasterUserGuid, int? intFlagMenuWithoutMoublie, int? applicationID);
    }

    public class MasterMenuDetailRepository : Repository<OceanDbEntities, TblMasterMenuDetail>, IMasterMenuDetailRepository
    {
        public MasterMenuDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<MasterMenuDetailResult> Func_Menu_Get(string strMasterUserGuid, int? intFlagMenuWithoutMoublie, int? applicationID)
        {
            return DbContext.Up_OceanOnlineMVC_MasterMenuDetail_Get(strMasterUserGuid, intFlagMenuWithoutMoublie, applicationID);
        }
    }
}
