using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobItemUnknowRepository : IRepository<TblMasterActualJobItemUnknow>
    {
    }
    public class MasterActualJobItemUnknowRepository : Repository<OceanDbEntities, TblMasterActualJobItemUnknow>, IMasterActualJobItemUnknowRepository
    {
        public MasterActualJobItemUnknowRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
