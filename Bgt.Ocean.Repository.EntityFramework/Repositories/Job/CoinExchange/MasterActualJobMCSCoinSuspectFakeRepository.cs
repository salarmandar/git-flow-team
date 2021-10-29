using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSCoinSuspectFakeRepository : IRepository<TblMasterActualJobMCSCoinSuspectFake>
    {
        SvdCoinExchangeSuspectFake GetCoinExchangeSuspectFake(Guid? jobGuid);
    }
    public class MasterActualJobMCSCoinSuspectFakeRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinSuspectFake>, IMasterActualJobMCSCoinSuspectFakeRepository
    {
        public MasterActualJobMCSCoinSuspectFakeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdCoinExchangeSuspectFake GetCoinExchangeSuspectFake(Guid? jobGuid)
        {

            SvdCoinExchangeSuspectFake result = new SvdCoinExchangeSuspectFake();
            var head = DbContext.TblMasterActualJobMCSCoinSuspectFake.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var denoList = DbContext.TblMasterActualJobMCSCoinSuspectFakeEntry
                                       .Where(o => o.MasterActualJobMCSCoinSuspectFake_Guid == head.Guid)
                                       .Select(o => new SuspectFakeTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Report = o.Report,
                                           Counted = o.Counted,
                                           Diff = o.Diff
                                       });

                result.TotalReport = head.TotalReport;
                result.TotalCounted = head.TotalCounted;
                result.TotalDiff = head.TotalDiff;
                result.SuspectFakeList = denoList;

                result.SuspectFakeDetailList = DbContext.TblMasterActualJobMCSCoinSuspectFakeDetail
                                                        .Where(o => o.MasterActualJobMCSCoinSuspectFake_Guid == head.Guid).AsEnumerable()
                                                         .Select(o => new SuspectFakeDetailView
                                                         {
                                                             CurrencyAbbr = o.TblMasterCurrency.MasterCurrencyAbbreviation,
                                                             DenominationValue = o.DenominationValue,
                                                             DepositDate = o.DepositDate,
                                                             DepositDateStr = o.DepositDate.ChangeFromDateToString(ApiSession.UserFormatDate),
                                                             SerialNo = o.SerialNo
                                                         });

                result.SuspectFakeBagList = DbContext.TblMasterActualJobMCSItemSeal
                                                    .Where(o => o.MasterActualJobHeader_Guid == jobGuid
                                                             && o.TblSystemSealType.SealTypeID == (int)SealTypeID.SuspectFakeBag
                                                             && o.SFOTblSystemMachineCapability.CapabilityID == (int)Capability.CoinExchange)
                                                    .Select(o => new SealBagView { SealNo = o.SealNo });

            }
            return result;
        }
    }
}
