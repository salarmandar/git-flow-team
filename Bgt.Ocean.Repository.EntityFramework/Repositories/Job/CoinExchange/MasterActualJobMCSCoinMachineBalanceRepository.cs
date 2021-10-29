using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{



    public interface IMasterActualJobMCSCoinMachineBalanceRepository : IRepository<TblMasterActualJobMCSCoinMachineBalance>
    {
        SvdCoinExchangeMachineBalance GetCoinExchangeMachineBalance(Guid? jobGuid);
    }
    public class MasterActualJobMCSCoinMachineBalanceRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinMachineBalance>, IMasterActualJobMCSCoinMachineBalanceRepository
    {
        public MasterActualJobMCSCoinMachineBalanceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }


        public SvdCoinExchangeMachineBalance GetCoinExchangeMachineBalance(Guid? jobGuid)
        {
            SvdCoinExchangeMachineBalance result = new SvdCoinExchangeMachineBalance();
            var head = DbContext.TblMasterActualJobMCSCoinMachineBalance.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var cassetList = DbContext.TblMasterActualJobMCSCoinMachineBalanceEntry
                                       .Where(o => o.MasterActualJobMCSCoinMachineBalance_Guid == head.Guid)
                                       .Select(o => new MBCassetteTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Report = o.Report,
                                           Counted = o.Counted,
                                           Diff = o.Diff,
                                           CassetteTypeID = (CassetteType)o.CassetteTypeID,
                                           CassetteSequence = o.CassetteSequence
                                       });

                result.TotalReport = head.TotalReport;
                result.TotalCounted = head.TotalCounted;
                result.TotalDiff = head.TotalDiff;
                result.BeginningAmount = head.BeginAmount;
                result.CurrencyAbbr = head.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.MachineBalanceHopperList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Hopper);
                result.MachineBalanceNoteList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Normal);
                result.MachineBalanceCoinList = cassetList.Where(o => o.CassetteTypeID == CassetteType.Coin);

            }
            return result;
        }
    }
}
