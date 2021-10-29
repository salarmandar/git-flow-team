using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring
{
    public interface ISmartBillingScheduleDayMappingRepository : IRepository<TblSmartBillingSchedule_Day_Mapping>
    {
    }
    public class SmartBillingScheduleDayMappingRepository : Repository<OceanDbEntities, TblSmartBillingSchedule_Day_Mapping>, ISmartBillingScheduleDayMappingRepository
    {
        public SmartBillingScheduleDayMappingRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
