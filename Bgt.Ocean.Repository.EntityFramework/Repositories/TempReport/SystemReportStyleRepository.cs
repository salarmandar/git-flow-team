using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport
{
    public interface ISystemReportStyleRepository : IRepository<TblSystemReportStyle>
    {
        IEnumerable<TblSystemReportStyle> FindReportStyleByStyleID(int[] typeID);
    }

    public class SystemReportStyleRepository : Repository<OceanDbEntities, TblSystemReportStyle>, ISystemReportStyleRepository
    {
        public SystemReportStyleRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblSystemReportStyle> FindReportStyleByStyleID(int[] typeID)
        {
            return DbContext.TblSystemReportStyle.Where(o => typeID.Any(x=> x == o.ReportStyleID));
        }
    }
}
