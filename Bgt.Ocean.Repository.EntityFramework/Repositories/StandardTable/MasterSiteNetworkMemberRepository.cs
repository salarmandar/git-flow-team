using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IMasterSiteNetworkMemberRepository : IRepository<TblMasterSiteNetworkMember>
    {
    }
    public class MasterSiteNetworkMemberRepository : Repository<OceanDbEntities, TblMasterSiteNetworkMember>, IMasterSiteNetworkMemberRepository
    {
        public MasterSiteNetworkMemberRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
