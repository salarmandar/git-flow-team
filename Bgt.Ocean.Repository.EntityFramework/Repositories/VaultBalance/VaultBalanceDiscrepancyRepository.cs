using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PreVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance
{
    public interface IVaultBalanceDiscrepancyRepository : IRepository<TblVaultBalance_Discrepancy>
    {
        DiscrepancyCloseCaseLogModel getdateforLogClosecase(Guid DiscrepancyGuid);
    }
    public class VaultBalanceDiscrepancyRepository : Repository<OceanDbEntities, TblVaultBalance_Discrepancy>, IVaultBalanceDiscrepancyRepository
    {
        public VaultBalanceDiscrepancyRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public DiscrepancyCloseCaseLogModel getdateforLogClosecase(Guid DiscrepancyGuid)
        {
            var query = (from discre in DbContext.TblVaultBalance_Discrepancy
                         join JH in DbContext.TblMasterActualJobHeader on discre.MasterActualJobHeader_Guid equals JH.Guid
                         into JHL
                         from JH in JHL.DefaultIfEmpty()
                         join site in DbContext.TblMasterSite on discre.MasterSite_Guid equals site.Guid
                         into siteL
                         from site in siteL.DefaultIfEmpty()
                         join Location in DbContext.TblMasterCustomerLocation on discre.MasterCustomerLocation_Delivery_Guid equals Location.Guid
                         into LocationL
                         from Location in LocationL.DefaultIfEmpty()
                         join vault in DbContext.TblMasterCustomerLocation_InternalDepartment on discre.MasterCustomerLocation_InternalDepartment_Guid equals vault.Guid
                         into vaultL
                         from vault in vaultL.DefaultIfEmpty()
                         where discre.Guid == DiscrepancyGuid
                         select new DiscrepancyCloseCaseLogModel
                         {
                             SealNo = discre.SealNo,
                             JobNo = JH.JobNo,
                             SiteName = site.SiteName,
                             BranchName = Location.BranchName,
                             InterDepartmentName = vault.InterDepartmentName
                         }).FirstOrDefault();

            return query;
        }
    }
}
