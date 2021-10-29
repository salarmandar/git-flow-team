using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterGasolineVendorRepository : IRepository<TblMasterGasloine_Vendor>
    {
        IEnumerable<TblMasterGasloine_Vendor> GetGasolineVendorByBrinkCompany(Guid brinkCompanyGuid);
        TblMasterGasloine_Vendor GetGasolineVendorDefaultByBrinkCompany(Guid brinkCompanyGuid);
    }
    public class MasterGasolineVendorRepository : Repository<OceanDbEntities, TblMasterGasloine_Vendor>, IMasterGasolineVendorRepository
    {
        public MasterGasolineVendorRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterGasloine_Vendor> GetGasolineVendorByBrinkCompany(Guid brinkCompanyGuid)
        {
            return DbContext.TblMasterGasloine_Vendor.Where(x => x.MasterCustomer_Guid == brinkCompanyGuid && x.FlagDisable != true);
        }

        public TblMasterGasloine_Vendor GetGasolineVendorDefaultByBrinkCompany(Guid brinkCompanyGuid)
        {
            return DbContext.TblMasterGasloine_Vendor.FirstOrDefault(x => x.MasterCustomer_Guid == brinkCompanyGuid && x.FlagDisable != true && x.FlagDefault);
        }

    }
}
