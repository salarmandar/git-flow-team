using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobItemsLiabilityRepository : IRepository<TblMasterActualJobItemsLiability>
    {
    }
    public class MasterActualJobItemsLiabilityRepository : Repository<OceanDbEntities, TblMasterActualJobItemsLiability>, IMasterActualJobItemsLiabilityRepository
    {
        public MasterActualJobItemsLiabilityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
