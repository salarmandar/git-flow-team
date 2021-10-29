using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IWebAPIUserRepository : IRepository<TblWebAPIUser>
    {
        void CreateHistory(TblSystemApiHistory history);
    }

    public class WebAPIUserRepository : Repository<OceanDbEntities, TblWebAPIUser>, IWebAPIUserRepository
    {
        public WebAPIUserRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void CreateHistory(TblSystemApiHistory history)
        {
            DbContext.TblSystemApiHistory.Add(history);
        }
    }
}
