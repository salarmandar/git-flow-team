using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSCoinCashReturnRepository : IRepository<TblMasterActualJobMCSCoinCashReturn>
    {
        SvdCoinExchangeCashReturn GetCoinExchangeCashReturn(Guid? jobGuid);
    }
    public class MasterActualJobMCSCoinCashReturnRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinCashReturn>, IMasterActualJobMCSCoinCashReturnRepository
    {
        public MasterActualJobMCSCoinCashReturnRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdCoinExchangeCashReturn GetCoinExchangeCashReturn(Guid? jobGuid)
        {
            SvdCoinExchangeCashReturn result = new SvdCoinExchangeCashReturn();
            var head = DbContext.TblMasterActualJobMCSCoinCashReturn.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var cassetList = DbContext.TblMasterActualJobMCSCoinCashReturnEntry
                                       .Where(o => o.MasterActualJobMCSCoinCashReturn_Guid == head.Guid)
                                       .Select(o => new CRCXTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Return = o.Return,
                                           Stay = o.Stay,
                                           Diff = o.Diff,
                                           CassetteTypeID = (CassetteType)o.CassetteTypeID,
                                           CassetteSequence = o.CassetteSequence
                                       });

                result.TotalReturn = head.TotalReturn;
                result.TotalStay = head.TotalStay;
                result.TotalDiff = head.TotalDiff;
                result.CashReturnHopperList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Hopper);
                result.CashReturnNoteList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Normal);
                result.CashReturnCoinList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Coin);

                result.ReturnBagList = DbContext.TblMasterActualJobMCSItemSeal
                                                    .Where(o => o.MasterActualJobHeader_Guid == jobGuid
                                                             && (o.TblSystemSealType.SealTypeID == (int)SealTypeID.ReturnCashBag || o.TblSystemSealType.SealTypeID == (int)SealTypeID.ReturnCoinBag)
                                                             && o.SFOTblSystemMachineCapability.CapabilityID == (int)Capability.CoinExchange).AsEnumerable()
                                                    .Select(o => new ReturnBagView { SealNo = o.SealNo , Commodity = Enum.GetName(typeof(CommoditySealtype), o.TblSystemSealType.SealTypeID) });
                result.StayBagList = DbContext.TblMasterActualJobMCSItemSeal
                                                    .Where(o => o.MasterActualJobHeader_Guid == jobGuid
                                                             && o.TblSystemSealType.SealTypeID == (int)SealTypeID.StayBag
                                                             && o.SFOTblSystemMachineCapability.CapabilityID == (int)Capability.CoinExchange)
                                                    .Select(o => new SealBagView { SealNo = o.SealNo });

            }
            return result;
        }
    }
}
