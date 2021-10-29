using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterHistoryActualJobOnDailyRunResourceRepository : IRepository<TblMasterHistory_ActaulJobOnDailyRunResource>
    {
        IEnumerable<TblMasterHistory_ActaulJobOnDailyRunResource> GetCheckHistoryActaulJobOnDailyRunResourceHaveRunAndJob(Guid jobGuid, Guid runGuid);
    }

    #endregion

    public class MasterHistoryActualJobOnDailyRunResourceRepository : Repository<OceanDbEntities, TblMasterHistory_ActaulJobOnDailyRunResource>, IMasterHistoryActualJobOnDailyRunResourceRepository
    {
        public MasterHistoryActualJobOnDailyRunResourceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterHistory_ActaulJobOnDailyRunResource> GetCheckHistoryActaulJobOnDailyRunResourceHaveRunAndJob(Guid jobGuid, Guid runGuid)
        {
            return DbContext.TblMasterHistory_ActaulJobOnDailyRunResource.Where(o => o.MasterActualJobHeader_Guid == jobGuid
                              && o.MasterRunResourceDaily_Guid == runGuid);
        }
    }
}
