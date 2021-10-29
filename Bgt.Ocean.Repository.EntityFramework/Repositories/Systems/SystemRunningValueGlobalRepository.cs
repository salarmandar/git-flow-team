using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface

    public interface ISystemRunningValueGlobalRepository : IRepository<TblSystemRunningVaule_Global>
    {
        TblSystemRunningVaule_Global GetServiceRequestRunning();
        int SetRunningIndependent(string runningKey, int runningRequest);
    }

    #endregion

    public class SystemRunningValueGlobalRepository : Repository<OceanDbEntities, TblSystemRunningVaule_Global>, ISystemRunningValueGlobalRepository
    {
        public SystemRunningValueGlobalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemRunningVaule_Global GetServiceRequestRunning()
        {
            var running = FindAll(e => e.RunningKey == "ServiceRequestRunning").FirstOrDefault();
            return running;
        }

        public int SetRunningIndependent(string runningKey, int runningRequest)
        {
            int running = 0;
            using (var db = new OceanDbEntities())
            {
                var data = db.TblSystemRunningVaule_Global.First(f => f.RunningKey == runningKey);
                running = data.RunningVaule1;
                data.RunningVaule1 += runningRequest;
                db.SaveChanges();
            }
            return running;
        }

    }
}
