using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterHistoryActualJobRepository : IRepository<TblMasterHistory_ActualJob>
    {
        TblMasterHistory_ActualJob FindByJobMsg(Guid jobGuid, int msgID);
        IEnumerable<TblMasterHistory_ActualJob> FindByJob(Guid jobGuid);
    }

    public class MasterHistoryActualJobRepository : Repository<OceanDbEntities, TblMasterHistory_ActualJob>, IMasterHistoryActualJobRepository
    {
        public MasterHistoryActualJobRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterHistory_ActualJob FindByJobMsg(Guid jobGuid, int msgID)
        {
            return DbContext.TblMasterHistory_ActualJob.Where(e => e.MasterActualJobHeader_Guid == jobGuid && e.MsgID == msgID).FirstOrDefault();
        }

        public IEnumerable<TblMasterHistory_ActualJob> FindByJob(Guid jobGuid)
        {
            return DbContext.TblMasterHistory_ActualJob.Where(e => e.MasterActualJobHeader_Guid == jobGuid);
        }
    }
}
