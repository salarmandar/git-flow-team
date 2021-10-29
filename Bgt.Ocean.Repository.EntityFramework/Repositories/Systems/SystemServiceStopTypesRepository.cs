using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region Interface

    public interface ISystemServiceStopTypesRepository : IRepository<TblSystemServiceStopType>
    {
        TblSystemServiceStopType GetServiceStopTypeByID(int internalID);
    }

    #endregion

    public class SystemServiceStopTypesRepository : Repository<OceanDbEntities, TblSystemServiceStopType>, ISystemServiceStopTypesRepository
    {
        public SystemServiceStopTypesRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemServiceStopType GetServiceStopTypeByID(int internalID)
        {
            return DbContext.TblSystemServiceStopType.FirstOrDefault(o=>o.InternalID == internalID);
        }
    }
}
