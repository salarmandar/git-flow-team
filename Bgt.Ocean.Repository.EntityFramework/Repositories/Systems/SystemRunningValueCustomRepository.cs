using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemRunningValueCustomRepository : IRepository<TblSystemRunningValue_Custom>
    {
        int SetRunningIndependent(string runningKey, RunNumberRefModel refRequest, int runningRequest);
    }
    public class SystemRunningValueCustomRepository : Repository<OceanDbEntities, TblSystemRunningValue_Custom>, ISystemRunningValueCustomRepository
    {
        public SystemRunningValueCustomRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
      
        public int SetRunningIndependent(string runningKey, RunNumberRefModel refRequest, int runningRequest)
        {
            int running = 1;
            using (var db = new OceanDbEntities())
            {

                //if type is datetime plz convert to format yyyy-MM-dd

                var query = db.TblSystemRunningValue_Custom.Where(f => f.RunningKey == runningKey && f.ReferenceValue1 == refRequest.referenceValue1
                                                                                                  && f.ReferenceValue2 == refRequest.referenceValue2
                                                                                                  && f.ReferenceValue3 == refRequest.referenceValue3
                                                                                                  && f.ReferenceValue4 == refRequest.referenceValue4
                                                                                                  && f.ReferenceValue5 == refRequest.referenceValue5);
                var data = query.AsEnumerable();
                if (data.Any())
                {
                    running = data.FirstOrDefault().RunningValue1;
                    data.FirstOrDefault().RunningValue1 += runningRequest;
                }
                else
                {
                    var insert = new TblSystemRunningValue_Custom();
                    insert.Guid = System.Guid.NewGuid();
                    insert.RunningValue1 = runningRequest + 1;
                    insert.RunningKey = runningKey;
                    insert.ReferenceValue1 = refRequest.referenceValue1;
                    insert.ReferenceValue2 = refRequest.referenceValue2;
                    insert.ReferenceValue3 = refRequest.referenceValue3;
                    insert.ReferenceValue4 = refRequest.referenceValue4;
                    insert.ReferenceValue5 = refRequest.referenceValue5;
                    db.TblSystemRunningValue_Custom.Add(insert);
                }
                db.SaveChanges();

            }
            return running;
        }

    }

}
