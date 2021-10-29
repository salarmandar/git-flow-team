using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterRunResourceGasolineExpenseRepository : IRepository<TblMasterRunResource_GasolineExpense>
    {
        IEnumerable<TblMasterRunResource_GasolineExpense> GetGasolineInfoList(Guid runResourceGuid, Guid siteGuid);
    }
    public class MasterRunResourceGasolineExpenseRepository : Repository<OceanDbEntities, TblMasterRunResource_GasolineExpense>, IMasterRunResourceGasolineExpenseRepository
    {
        public MasterRunResourceGasolineExpenseRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }


        public IEnumerable<TblMasterRunResource_GasolineExpense> GetGasolineInfoList(Guid runResourceGuid,Guid siteGuid)
        {
            return DbContext.TblMasterRunResource_GasolineExpense
                   .Where(x=>x.MasterRunResource_Guid == runResourceGuid && x.MasterSite_Guid == siteGuid);
        }
    }
}
