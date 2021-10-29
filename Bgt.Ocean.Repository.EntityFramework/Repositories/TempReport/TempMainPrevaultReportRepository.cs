using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport
{
    public interface ITempMainPrevaultReportRepository : IRepository<TblTempMainPrevaultReport>
    {

    }
    public class TempMainPrevaultReportRepository : Repository<OceanDbEntities, TblTempMainPrevaultReport>, ITempMainPrevaultReportRepository
    {
        public TempMainPrevaultReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
