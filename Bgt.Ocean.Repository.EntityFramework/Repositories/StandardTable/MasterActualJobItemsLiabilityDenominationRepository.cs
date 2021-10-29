using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Denomination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IMasterActualJobItemsLiabilityDenominationRepository : IRepository<TblMasterActualJobItemsLiability_Denomination>
    {
        IEnumerable<TblMasterActualJobItemsLiability_Denomination> FindByListGuid(IEnumerable<Guid?> listGuid);

        IEnumerable<TblMasterActualJobItemsLiability_Denomination> FindByLiabilityGuid(Guid? liabilityGuid);

        IEnumerable<DenominationDetailView> GetDenominationByDenoUnit(Guid? liabilityGuid, Guid? languageGuid, IEnumerable<Guid> denoGuids);
    }
    public class MasterActualJobItemsLiabilityDenominationRepository : Repository<OceanDbEntities, TblMasterActualJobItemsLiability_Denomination>, IMasterActualJobItemsLiabilityDenominationRepository
    {
        public MasterActualJobItemsLiabilityDenominationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<TblMasterActualJobItemsLiability_Denomination> FindByListGuid(IEnumerable<Guid?> listGuid)
        {

            return DbContext.TblMasterActualJobItemsLiability_Denomination.Where(o => listGuid.Contains(o.Guid));
        }

        public IEnumerable<TblMasterActualJobItemsLiability_Denomination> FindByLiabilityGuid(Guid? liabilityGuid)
        {
            return DbContext.TblMasterActualJobItemsLiability_Denomination.Where(o => o.MasterActualJobItemsLiability_Guid == liabilityGuid);
        }

        public IEnumerable<DenominationDetailView> GetDenominationByDenoUnit(Guid? liabilityGuid, Guid? languageGuid, IEnumerable<Guid> denoGuids)
        {

            var result = (from liabilityDenom in DbContext.TblMasterActualJobItemsLiability_Denomination
                          join sysUnit in DbContext.TblSystemDenominationUnit on liabilityDenom.SystemDenominationUnit_Guid equals sysUnit.Guid
                          join sysType in DbContext.TblSystemDenominationType on sysUnit.SystemDenominationTypeID equals sysType.DenominationTypeID
                          join langUnit in DbContext.TblSystemDisplayTextControlsLanguage on sysUnit.SystemDisplayTextControls_Guid equals langUnit.Guid
                          join langType in DbContext.TblSystemDisplayTextControlsLanguage on sysType.SystemDisplayTextControls_Guid equals langType.Guid
                          where !sysUnit.FlagDisable
                          && langUnit.SystemLanguageGuid == languageGuid 
                          && langType.SystemLanguageGuid == languageGuid
                          && denoGuids.Contains(sysUnit.Guid)
                          && liabilityDenom.MasterActualJobItemsLiability_Guid == liabilityGuid
                          select new DenominationDetailView
                          {
                              DenoGuid = liabilityDenom.MasterDenomination_Guid,
                              LiabilityDenoGuid = liabilityDenom.Guid,
                              DenoName = liabilityDenom.DenominationText,
                              DenoValue = liabilityDenom.DenominationValue,
                              Type = langType.DisplayText,
                              UnitType = (DenoUnit)sysType.DenominationTypeID,
                              Qty = liabilityDenom.Qty ?? 0,
                              Value = liabilityDenom.Value ?? 0,
                              DenoUnitGuid = sysUnit.Guid,
                              ItemState = EnumState.Unchanged,
                          });
            return result.OrderBy(o => o.UnitType).ThenByDescending(t => t.DenoValue).ToList();
        }

    }
}
