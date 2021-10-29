using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance
{
    public interface IVaultBalanceNonbarcodeRepository : IRepository<TblVaultBalanceNonbarcode>
    {
    }
    public class VaultBalanceNonbarcodeRepository : Repository<OceanDbEntities, TblVaultBalanceNonbarcode>, IVaultBalanceNonbarcodeRepository
    {
        public VaultBalanceNonbarcodeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
