using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance
{
    public interface IVaultBalanceDetailRepository : IRepository<TblVaultBalanceDetail>
    {

    }
    public class VaultBalanceDetailRepository : Repository<OceanDbEntities, TblVaultBalanceDetail>, IVaultBalanceDetailRepository
    {
        public VaultBalanceDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
