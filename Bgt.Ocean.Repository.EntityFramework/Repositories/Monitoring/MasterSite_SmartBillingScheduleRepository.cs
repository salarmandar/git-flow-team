using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring
{
    public interface IMasterSite_SmartBillingScheduleRepository: IRepository<TblMasterSite_SmartBillingSchedule>
    {
    }
    public class MasterSite_SmartBillingScheduleRepository : Repository<OceanDbEntities, TblMasterSite_SmartBillingSchedule>, IMasterSite_SmartBillingScheduleRepository
    {
        public MasterSite_SmartBillingScheduleRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
