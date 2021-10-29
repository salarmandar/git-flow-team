using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterMachineCassetteRepository : IRepository<SFOTblMasterMachine_Cassette>
    {
        IEnumerable<DenominationOnMachineCassetteView> Func_GetDenoOnMachineCassetteByCustomerLocationGuid(Guid customerLocationGuid, CassetteType cassetteTypeId);
        IEnumerable<DenominationOnMachineCassetteView> Func_GetCassetteInMachine(Guid customerLocationGuid, params CassetteType[] arrayCassetteTypeId);
        IEnumerable<DenominationOnMachineCassetteView> Func_GetDefaultCassetteInModelMachine(Guid customerLocationGuid, CassetteType cassetteTypeId, Capability capabilityId);
    }
    public class SFOMasterMachineCassetteRepository : Repository<OceanDbEntities, SFOTblMasterMachine_Cassette>, ISFOMasterMachineCassetteRepository
    {
        public SFOMasterMachineCassetteRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
        public IEnumerable<DenominationOnMachineCassetteView> Func_GetDenoOnMachineCassetteByCustomerLocationGuid(Guid customerLocationGuid, CassetteType cassetteTypeId)
        {
            int casseteId = (int)cassetteTypeId;
            //validate         
            if (casseteId == (int)CassetteType.AllIn)
            {
                var machineCassette = DbContext.SFOTblMasterMachine_Cassette.Where(w => w.SFOTblMasterMachine_Guid == customerLocationGuid
                                                   && !w.TblMasterCurrency.FlagDisable
                                                   && w.SFOTblMasterCassette.TblSystemCassetteType.InternalID == casseteId
                                                  )
                                   .Select(s => new DenominationOnMachineCassetteView
                                   {
                                       MachineCassetteGuid = s.SFOTblMasterCassette_Guid,
                                       Seq = s.CassetteSequence,
                                       CasseteName = s.SFOTblMasterCassette.CassetteName,
                                       CassetteTypeID = (CassetteType)s.SFOTblMasterCassette.TblSystemCassetteType.InternalID,
                                       CurrencyGuid = s.TblMasterCurrency_Guid.HasValue ? s.TblMasterCurrency_Guid.Value : Guid.Empty,
                                       CurrencyAbb = s.TblMasterCurrency.MasterCurrencyAbbreviation,
                                       CurrencyDescription = s.TblMasterCurrency.MasterCurrencyDescription
                                   });
                return machineCassette;
            }
            else
            {
                var machineCassette = DbContext.SFOTblMasterMachine_Cassette.Where(w => w.SFOTblMasterMachine_Guid == customerLocationGuid
                                                   && !w.TblMasterCurrency.FlagDisable
                                                   && !w.TblMasterDenomination.FlagDisable
                                                   && w.SFOTblMasterCassette.TblSystemCassetteType.InternalID == casseteId
                                                  )
                                   .Select(s => new DenominationOnMachineCassetteView
                                   {
                                       MachineCassetteGuid = s.SFOTblMasterCassette_Guid,
                                       Seq = s.CassetteSequence,
                                       CasseteName = s.SFOTblMasterCassette.CassetteName,
                                       CassetteTypeID = (CassetteType)s.SFOTblMasterCassette.TblSystemCassetteType.InternalID,
                                       DeNoGuid = s.TblMasterDenomination_Guid.HasValue ? s.TblMasterDenomination_Guid.Value : Guid.Empty,
                                       DeNoText = s.TblMasterDenomination.DenominationText,
                                       DeNoValue = s.TblMasterDenomination.DenominationValue,
                                       CurrencyGuid = s.TblMasterCurrency_Guid.HasValue ? s.TblMasterCurrency_Guid.Value : Guid.Empty,
                                       CurrencyAbb = s.TblMasterCurrency.MasterCurrencyAbbreviation,
                                       CurrencyDescription = s.TblMasterCurrency.MasterCurrencyDescription

                                   });
                return machineCassette;
            }
        }
        public IEnumerable<DenominationOnMachineCassetteView> Func_GetDefaultCassetteInModelMachine(Guid customerLocationGuid, CassetteType cassetteTypeId,Capability capabilityId)
        {
            return DbContext.SFOTblMasterMachine.FirstOrDefault(w => w.Guid == customerLocationGuid)
                               ?.SFOTblMasterMachineModelType
                               .SFOTblMasterMachineModelType_DefaultCassette.Where(w => w.SFOTblSystemMachineCapability.CapabilityID == (int)capabilityId)
                               .Select(f => new DenominationOnMachineCassetteView
                               {
                                   MachineCassetteGuid = f.SFOTblMasterCassette.Guid,
                                   Seq = f.Sequence,
                                   CasseteName = f.SFOTblMasterCassette.CassetteName,
                                   CassetteTypeID = (CassetteType)f.SFOTblMasterCassette.TblSystemCassetteType.InternalID,
                                   CurrencyGuid = f.TblMasterCurrency.Guid,
                                   CurrencyAbb = f.TblMasterCurrency.MasterCurrencyAbbreviation,
                                   CurrencyDescription = f.TblMasterCurrency.MasterCurrencyDescription
                               });
        }
        public IEnumerable<DenominationOnMachineCassetteView> Func_GetCassetteInMachine(Guid customerLocationGuid, params CassetteType[] arrayCassetteTypeId)
        {
            var machineCassette = DbContext.SFOTblMasterMachine_Cassette.Where(w => w.SFOTblMasterMachine_Guid == customerLocationGuid
                                                                                && !w.TblMasterCurrency.FlagDisable
                                                                                && (!w.TblMasterDenomination_Guid.HasValue || !w.TblMasterDenomination.FlagDisable))
                                .Join(arrayCassetteTypeId, c => c.SFOTblMasterCassette.TblSystemCassetteType.InternalID, ac => (int)ac, (c, ac) => new { c, ac })

                                              .Select(s => new DenominationOnMachineCassetteView
                                              {
                                                  MachineCassetteGuid = s.c.SFOTblMasterCassette_Guid,
                                                  Seq = s.c.CassetteSequence,
                                                  CasseteName = s.c.SFOTblMasterCassette.CassetteName,
                                                  CassetteTypeID = (CassetteType)s.c.SFOTblMasterCassette.TblSystemCassetteType.InternalID,
                                                  CassetteTypeIdGuid = s.c.SFOTblMasterCassette.TblSystemCassetteType_Guid,
                                                  DeNoGuid = s.c.TblMasterDenomination_Guid.HasValue ? s.c.TblMasterDenomination_Guid.Value : Guid.Empty,
                                                  DeNoText = s.c.TblMasterDenomination.DenominationText,
                                                  DeNoValue = s.c.TblMasterDenomination.DenominationValue,
                                                  CurrencyGuid = s.c.TblMasterCurrency_Guid.HasValue ? s.c.TblMasterCurrency_Guid.Value : Guid.Empty,
                                                  CurrencyAbb = s.c.TblMasterCurrency.MasterCurrencyAbbreviation,
                                                  CurrencyDescription = s.c.TblMasterCurrency.MasterCurrencyDescription,
                                                  Amount = s.c.TblMasterDenomination.Amount

                                              });
            return machineCassette;
        }
    }
}
