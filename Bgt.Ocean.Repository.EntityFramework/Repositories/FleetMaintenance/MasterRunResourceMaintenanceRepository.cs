
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterRunResourceMaintenanceRepository : IRepository<TblMasterRunResource_Maintenance>
    {
        IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryDetailByMaintenanceGuid(Guid? maintenanceGuid, EnumMaintenanceState state);
    }

    public class MasterRunResourceMaintenanceRepository : Repository<OceanDbEntities, TblMasterRunResource_Maintenance>, IMasterRunResourceMaintenanceRepository
    {
        public MasterRunResourceMaintenanceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryDetailByMaintenanceGuid(Guid? maintenanceGuid, EnumMaintenanceState state)
        {
            var result = Enumerable.Empty<MaintenanceCategoryDetailView>();
            var maintenance = DbContext.TblMasterRunResource_Maintenance.FirstOrDefault(o => o.Guid == maintenanceGuid);
            if (maintenance != null)
            {
                var CurrencyAC = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == maintenance.CurrencyActual_Guid)?.MasterCurrencyAbbreviation;
                var CurrencyES = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == maintenance.CurrencyEstimate_Guid)?.MasterCurrencyAbbreviation;

                result = DbContext.TblMasterRunResource_Maintenance_Detail.Where(o => o.MasterRunResource_Maintenance_Guid == maintenanceGuid)
                                     .Join(DbContext.TblMasterMaintenanceCategory
                                     , m => m.MasterMaintenanceCategory_Guid
                                     , c => c.Guid
                                     , (m, c) => new { m, c })
                                   .AsEnumerable()
                                   .Select(o =>
                                   {
                                       var detail = new MaintenanceCategoryDetailView
                                       {
                                           MaintenanceDetailGuid = o.m.Guid,
                                           Description = o.m.Description,
                                           MasterMaintenanceCategory_Guid = o.m.MasterMaintenanceCategory_Guid,
                                           MaintenanceCategoryName = o.c.MaintenanceCategoryName,
                                           State = state
                                       };

                                       if (state == EnumMaintenanceState.Actual)
                                       {
                                           detail.Discount = o.m.Discount_Actual ?? 0;
                                           detail.DiscountType = (EnumDiscountType)o.m.Discount_Type_Acutal;
                                           detail.PartQty = o.m.Part_Qty_Actual ?? 0;
                                           detail.UnitPrice = o.m.Unite_Price_Actual ?? 0;
                                           detail.Total = Convert.ToDouble((o.m.Total_Actual ?? 0));
                                           detail.CurrencyText = CurrencyAC;
                                       }
                                       else
                                       {
                                           detail.Discount = o.m.Discount_Estimate ?? 0;
                                           detail.DiscountType = (EnumDiscountType)o.m.Discount_Type_Estimate;
                                           detail.PartQty = o.m.Part_Qty_Estimate ?? 0;
                                           detail.UnitPrice = o.m.Unite_Price_Estimate ?? 0;
                                           detail.Total = Convert.ToDouble((o.m.Total_Estimate ?? 0));
                                           detail.CurrencyText = CurrencyES;
                                       }
                                       return detail;
                                   }).OrderBy(o => o.MaintenanceCategoryName).ThenBy(o => o.Description);
            }

            return result;
        }
    }
}
