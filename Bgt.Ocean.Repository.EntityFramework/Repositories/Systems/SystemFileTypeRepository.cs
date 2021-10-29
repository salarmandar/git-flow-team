using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemFileTypeRepository : IRepository<TblSystemFileType> { }
    public class SystemFileTypeRepository : Repository<OceanDbEntities, TblSystemFileType>, ISystemFileTypeRepository
    {
        public SystemFileTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
