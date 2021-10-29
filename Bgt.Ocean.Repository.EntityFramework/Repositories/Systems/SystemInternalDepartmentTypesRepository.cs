using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region Interface
    public interface ISystemInternalDepartmentTypesRepository : IRepository<TblSystemInternalDepartmentType>
    {

    }
    #endregion
    public class SystemInternalDepartmentTypesRepository : Repository<OceanDbEntities, TblSystemInternalDepartmentType>, ISystemInternalDepartmentTypesRepository
    {
        public SystemInternalDepartmentTypesRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
