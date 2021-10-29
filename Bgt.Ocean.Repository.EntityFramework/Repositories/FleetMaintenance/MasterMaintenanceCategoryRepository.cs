
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterMaintenanceCategoryRepository : IRepository<TblMasterMaintenanceCategory>
    {
        IEnumerable<MaintenanceCategoryView> GetMaintenanceCategory(Guid? brinksCompanyGuid);
        IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryItems(Guid? brinksCompanyGuid, Guid? MaintenanceCategoryGuid, string currencyAbb);
    }

    public class MasterMaintenanceCategoryRepository : Repository<OceanDbEntities, TblMasterMaintenanceCategory>, IMasterMaintenanceCategoryRepository
    {
        public MasterMaintenanceCategoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        public IEnumerable<MaintenanceCategoryView> GetMaintenanceCategory(Guid? brinksCompanyGuid)
        {

            var result = DbContext.TblMasterMaintenanceCategory.Where(o =>
                          o.FlagDisable == false && o.MasterCustomer_Guid == brinksCompanyGuid)
                          .Select(o => new MaintenanceCategoryView
                          {
                              MasterMaintenanceCategoryGuid = o.Guid,
                              MaintenanceCategoryName = o.MaintenanceCategoryName
                          });

            return result;
        }

        public IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryItems(Guid? brinksCompanyGuid, Guid? MaintenanceCategoryGuid, string currencyAbb)
        {

            var result = DbContext.TblMasterMaintenanceCategory
                          .Join(DbContext.TblMasterMaintenanceCategory_Items
                          , mc => mc.Guid
                          , mci => mci.MasterMaintenanceCategory_Guid
                          , (mc, mci) => new { mc, mci })
                          .Where(o =>
                          o.mc.FlagDisable == false && o.mci.FlagDisable == false && o.mc.MasterCustomer_Guid == brinksCompanyGuid
                          && (MaintenanceCategoryGuid == null || o.mci.MasterMaintenanceCategory_Guid == MaintenanceCategoryGuid)
                           )
                          .Select(o => new MaintenanceCategoryDetailView
                          {
                              MasterMaintenanceCategory_Guid = o.mc.Guid,
                              MaintenanceCategoryName = o.mc.MaintenanceCategoryName,
                              Description = o.mci.ItemName,
                              DiscountType = EnumDiscountType.Undefined,
                              CurrencyText = currencyAbb
                          }).OrderBy(o => o.Description);

            return result;
        }

    }
}
