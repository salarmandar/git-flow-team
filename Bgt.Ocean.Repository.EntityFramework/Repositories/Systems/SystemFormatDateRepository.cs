using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemFormatDateRepository : IRepository<TblSystemFormat_Date>
    {
        int FindByUserFormatDate(string userFormatDate);
    }

    public class SystemFormatDateRepository : Repository<OceanDbEntities, TblSystemFormat_Date>, ISystemFormatDateRepository
    {
        public SystemFormatDateRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        { }

        public int FindByUserFormatDate(string userFormatDate)
        {
           return  DbContext.TblSystemFormat_Date.Where(e => e.FormatDate.Equals(userFormatDate)).FirstOrDefault()?.FormatSQLCode ?? 101;
        }
    }
}
