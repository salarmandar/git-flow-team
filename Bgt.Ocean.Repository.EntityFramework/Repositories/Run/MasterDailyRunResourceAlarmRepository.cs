using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    #region Interface

    public interface IMasterDailyRunResourceAlarmRepository : IRepository<TblMasterDailyRunResource_Alarm>
    {
        TblMasterDailyRunResource_Alarm GetNewDailyRunAlarmByRunGuid(Guid masterDailyRunResourceGuid);
    }

    #endregion

    public class MasterDailyRunResourceAlarmRepository 
        : Repository<OceanDbEntities, TblMasterDailyRunResource_Alarm>, IMasterDailyRunResourceAlarmRepository
    {
        public MasterDailyRunResourceAlarmRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterDailyRunResource_Alarm GetNewDailyRunAlarmByRunGuid(Guid masterDailyRunResourceGuid)
        {
            return FindAllAsQueryable(e => e.MasterDailyRunResource_Guid.Value == masterDailyRunResourceGuid && !e.FlagAcknowledged && !e.FlagDeactivated)
                    .OrderByDescending(e => e.DatetimeCreated)
                    .FirstOrDefault();
        }

        public override void Create(TblMasterDailyRunResource_Alarm entity)
        {
            entity.CleanupDirty();
            base.Create(entity);
        }

        public override void Modify(TblMasterDailyRunResource_Alarm entity)
        {
            entity.CleanupDirty();
            base.Modify(entity);
        }
    }
}
