using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface
    public interface ISystemMaterRouteTypeOfWeekRepository : IRepository<TblSystemMaterRouteTypeOfWeek>
    {
        IEnumerable<TblSystemMaterRouteTypeOfWeek> GetMasterRouteTypeOfWeek();
    }
    #endregion



    public class SystemMaterRouteTypeOfWeekRepository : Repository<OceanDbEntities, TblSystemMaterRouteTypeOfWeek>, ISystemMaterRouteTypeOfWeekRepository
    {
        public SystemMaterRouteTypeOfWeekRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<TblSystemMaterRouteTypeOfWeek> GetMasterRouteTypeOfWeek()
        {
            return DbContext.TblSystemMaterRouteTypeOfWeek.OrderBy(o=>o.WeekTypeInt);
        }
    }
}
