using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    public interface IMasterDailyRunResourceHistoryRepository : IRepository<TblMasterHistory_DailyRunResource>
    {
        void CreateDailyRunLog(MasterDailyRunLogView logView);
    }

    public class MasterDailyRunResourceHistoryRepository : Repository<OceanDbEntities, TblMasterHistory_DailyRunResource>, IMasterDailyRunResourceHistoryRepository
    {
        public MasterDailyRunResourceHistoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void CreateDailyRunLog(MasterDailyRunLogView logView)
        {
            if (logView.MasterDailyRunResourceGuid == Guid.Empty)
                return;

            var newDailyRunLog = new TblMasterHistory_DailyRunResource
            {
                Guid = Guid.NewGuid(),
                MsgParameter = logView.JSONParameter,
                MasterDailyRunResource_Guid = logView.MasterDailyRunResourceGuid,
                MsgID = logView.MsgID,
                DatetimeCreated = logView.ClientDate,
                UniversalDatetimeCreated = DateTime.UtcNow,
                UserCreated = logView.UserCreated
            };

            Create(newDailyRunLog);
        }
    }
}
