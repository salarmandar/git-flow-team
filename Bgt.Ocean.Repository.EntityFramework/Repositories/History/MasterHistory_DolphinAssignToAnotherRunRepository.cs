using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterHistory_DolphinAssignToAnotherRunRepository : IRepository<TblMasterHistory_DolphinAssignToAnotherRun>
    {
        void insertNewHistory(Guid JobHeader_Guid, Guid DailyRun_Guid);
    }

    public class MasterHistory_DolphinAssignToAnotherRunRepository : Repository<OceanDbEntities, TblMasterHistory_DolphinAssignToAnotherRun>, IMasterHistory_DolphinAssignToAnotherRunRepository 
    {
        public MasterHistory_DolphinAssignToAnotherRunRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void insertNewHistory(Guid JobHeader_Guid, Guid DailyRun_Guid)
        {
            var newHistory = new TblMasterHistory_DolphinAssignToAnotherRun()
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = JobHeader_Guid,
                MasterRunResourceDaily_Guid = DailyRun_Guid,
                FlagDolphinRemove = false
            };
            Create(newHistory);
        }
    }
}
