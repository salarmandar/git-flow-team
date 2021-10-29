using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance
{
    public interface IVaultBalanceSealAndMasterRepository : IRepository<TblVaultBalanceSealAndMaster>
    {
    }
    public class VaultBalanceSealAndMasterRepository : Repository<OceanDbEntities, TblVaultBalanceSealAndMaster>, IVaultBalanceSealAndMasterRepository
    {
        public VaultBalanceSealAndMasterRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
