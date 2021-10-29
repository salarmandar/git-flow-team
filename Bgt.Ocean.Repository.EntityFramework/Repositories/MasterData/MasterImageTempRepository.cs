using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface IMasterImageTempRepository : IRepository<TblMasterImageTemp>
    {
        IEnumerable<TblMasterImageTemp> FindByGuids(IEnumerable<Guid> guids);
    }
    public class MasterImageTempRepository : Repository<OceanDbEntities, TblMasterImageTemp>, IMasterImageTempRepository
    {
        public MasterImageTempRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterImageTemp> FindByGuids(IEnumerable<Guid> guids)
        {
            return DbContext.TblMasterImageTemp.Where(w => guids.Any(a => a == w.Guid));
        }
    }
}
