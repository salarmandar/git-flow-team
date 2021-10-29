using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSCoinCashAddRepository : IRepository<TblMasterActualJobMCSCoinCashAdd>
    {
        SvdCoinExchangeCashAdd GetCoinExchangeCashAdd(Guid? jobGuid);
    }
    public class MasterActualJobMCSCoinCashAddRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinCashAdd>, IMasterActualJobMCSCoinCashAddRepository
    {
        public MasterActualJobMCSCoinCashAddRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdCoinExchangeCashAdd GetCoinExchangeCashAdd(Guid? jobGuid)
        {
            SvdCoinExchangeCashAdd result = new SvdCoinExchangeCashAdd();
            var head = DbContext.TblMasterActualJobMCSCoinCashAdd.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var cassetList = DbContext.TblMasterActualJobMCSCoinCashAddEntry
                                       .Where(o => o.MasterActualJobMCSCoinCashAdd_Guid == head.Guid)
                                       .Select(o => new CACassetteTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Loaded = o.Loaded,
                                           TotalATM = o.ATM,
                                           Surplus = o.Surplus,
                                           CassetteTypeID = (CassetteType)o.CassetteTypeID,
                                           CassetteSequence = o.CassetteSequence
                                       });

                result.CoinOrderNo = head.CoinOrderNo ?? "";
                result.NoteOrderNo = head.NoteOrderNo ?? "";

                result.PrevSurplusAmount = head.PrevSurplusAmount;
                result.DeliveryAmount = head.DeliveryAmount;
                result.TotalAmountAvailable = head.TotalAmountAvailable;
                result.CurrencyAbbr = head.TblMasterCurrency.MasterCurrencyAbbreviation;

                result.TotalLoadded = head.TotalLoaded;
                result.TotalATM = head.TotalATM;
                result.TotalSurplus = head.TotalSurplus;
                result.CashAddHopperList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Hopper);
                result.CashAddNoteList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Normal);
                result.CashAddCoinList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Coin);

            }
            return result;
        }
    }


}
