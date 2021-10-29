using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Models.Denomination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IMasterDenominationRepository : IRepository<TblMasterDenomination>
    {
        IEnumerable<DenominationView> GetDenoByCurrency(Guid currencyGuid, int denoType, string denoUnit);
        IEnumerable<DenominationModel> GetDenoByCurrency(Guid currencyGuid);
        IEnumerable<DenominationDetailView> GetDenominationByDenoUnit(Guid? currencyGuid, Guid? languageGuid, IEnumerable<Guid> denoGuids);
    }
    public class MasterDenominationRepository : Repository<OceanDbEntities, TblMasterDenomination>, IMasterDenominationRepository
    {
        public MasterDenominationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<DenominationView> GetDenoByCurrency(Guid currencyGuid, int denoType, string denoUnit)
        {
            var type = DbContext.TblSystemDenominationType.SingleOrDefault(s => s.DenominationTypeID == denoType).Guid;
            var unit = DbContext.TblSystemDenominationUnit.FirstOrDefault(f => f.DenominationUnit == denoUnit && f.SystemDenominationTypeID == denoType).Guid;
            var deno = DbContext.TblMasterDenomination.Where(w => w.MasterCurrency_Guid == currencyGuid && !w.FlagDisable && w.SystemDenominationType_Guid == type && w.SystemDenominationDefaultUnit == unit)
                                                                                        .GroupBy(g => new
                                                                                        {
                                                                                            g.Guid,
                                                                                            g.MasterCurrency_Guid,
                                                                                            g.DenominationValue,
                                                                                            g.DenominationText
                                                                                        })
                                                                                        .Select(s => new DenominationView
                                                                                        {
                                                                                            Guid = s.Key.Guid,
                                                                                            MasterCurrency_Guid = s.Key.MasterCurrency_Guid,
                                                                                            DenominationValue = s.Key.DenominationValue,
                                                                                            DenominationText = s.Key.DenominationText
                                                                                        });

            return deno;
        }
        public IEnumerable<DenominationModel> GetDenoByCurrency(Guid currencyGuid)
        {
            var deno = DbContext.TblMasterDenomination.Where(w => w.MasterCurrency_Guid == currencyGuid && !w.FlagDisable)
                        .Join(DbContext.TblSystemDenominationType,
                                                                            d => d.SystemDenominationType_Guid,
                                                                            t => t.Guid,
                                                                            (d, t) => new { d, t })
                                                                            .Select(s => new DenominationModel
                                                                            {
                                                                                Guid = s.d.Guid,
                                                                                MasterCurrency_Guid = s.d.MasterCurrency_Guid,
                                                                                DenominationValue = s.d.DenominationValue,
                                                                                DenominationName = s.d.DenominationText,
                                                                                DenominationTypeID = s.t.DenominationTypeID,
                                                                                DenominationType = s.t.DenominationTypeName,
                                                                                UserCreated = s.d.UserCreated,
                                                                                DatetimeCreated = s.d.DatetimeCreated,
                                                                                UniversalDatetimeCreated = s.d.UniversalDatetimeCreated,
                                                                                UserModifed = s.d.UserModifed,
                                                                                DatetimeModified = s.d.DatetimeModified,
                                                                                UniversalDatetimeModified = s.d.UniversalDatetimeModified
                                                                            }
                                                                            );

            return deno;
        }

        public IEnumerable<DenominationDetailView> GetDenominationByDenoUnit(Guid? currencyGuid, Guid? languageGuid, IEnumerable<Guid> denoGuids)
        {

            var result = (from denom in DbContext.TblMasterDenomination
                          join sysUnit in DbContext.TblSystemDenominationUnit on denom.SystemDenominationDefaultUnit equals sysUnit.Guid
                          join sysType in DbContext.TblSystemDenominationType on denom.SystemDenominationType_Guid equals sysType.Guid
                          join langUnit in DbContext.TblSystemDisplayTextControlsLanguage on sysUnit.SystemDisplayTextControls_Guid equals langUnit.Guid
                          join langType in DbContext.TblSystemDisplayTextControlsLanguage on sysType.SystemDisplayTextControls_Guid equals langType.Guid
                          where denom.MasterCurrency_Guid == currencyGuid && !sysUnit.FlagDisable && langUnit.SystemLanguageGuid == languageGuid && langType.SystemLanguageGuid == languageGuid
                          && denoGuids.Contains(sysUnit.Guid) && !denom.FlagDisable
                          select new DenominationDetailView
                          {
                              DenoGuid = denom.Guid,
                              DenoName = denom.DenominationText,
                              DenoValue = denom.DenominationValue ?? 0,
                              Type = langType.DisplayText,
                              UnitType = (DenoUnit)sysType.DenominationTypeID,
                              DenoUnitGuid = sysUnit.Guid
                          });
            return result.OrderBy(o => o.UnitType).ThenByDescending(t => t.DenoValue).ToList();
        }
    }
}
