using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface

    public interface ISystemGlobalUnitRepository : IRepository<TblSystemGlobalUnit>
    {
        IEnumerable<TblSystemGlobalUnit> FindWeightUnit();
        string GetUnitNameAbb(Guid unitGuid);
    }

    #endregion

    public class SystemGlobalUnitRepository : Repository<OceanDbEntities, TblSystemGlobalUnit>, ISystemGlobalUnitRepository
    {
        public SystemGlobalUnitRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblSystemGlobalUnit> FindWeightUnit()
        {
            var weightUnitTypeGuid = Guid.Parse("21f5421d-1ce9-4480-89d6-025076b63a3d");
            return DbContext.TblSystemGlobalUnit.Where(x => x.SystemGlobalUnitType_Guid == weightUnitTypeGuid && !x.FlagDisable).OrderBy(e => e.UnitName);

        }

        public string GetUnitNameAbb(Guid unitGuid)
        {
            using (var context = new OceanDbEntities())
            {
                return context.TblSystemGlobalUnit.FirstOrDefault(o => o.Guid == unitGuid)?.UnitNameAbbrevaition;
            }
        }
    }
}
