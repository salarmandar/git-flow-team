using System;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Models.ActualJob.CashAddModel;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public class MasterActualJobSumCashAddRepository : Repository<OceanDbEntities, TblMasterActualJobSumCashAdd>, IMasterActualJobSumCashAddRepository
    {
        public MasterActualJobSumCashAddRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SvdNoteWithdrawCashAdd GetCashAddDetail(Guid jobGuid)
        {
            SvdNoteWithdrawCashAdd result = new SvdNoteWithdrawCashAdd();
            var sum = DbContext.TblMasterActualJobSumCashAdd.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (sum != null)
            {
                var casette = DbContext.TblMasterActualJobCashAdd
                                       .Where(o => o.MasterActualJobSumCashAdd_Guid == sum.Guid)
                                       .Select(o => new CANWTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Loadded = o.Loadded,
                                           Surplus = o.Surplus,
                                           TotalATM = o.TotalATM,
                                           CassetteSequence = o.CassetteSequence
                                       });                

                result.CustomerOrderNo = sum.CustomerOrderNo??string.Empty;
                result.OtherMachineAmount = sum.OtherMachineAmount;
                result.SumLoadded = sum.SumLoadded;
                result.SumSurplus = sum.SumSurplus;
                result.SumTotalATM = sum.SumTotalATM;
                result.PrevSurplusAmont = sum.PrevSurplusAmont;
                result.DeliveryAmount = sum.DeliveryAmount;
                result.TotalAmountAvaliable = sum.TotalAmountAvaliable;
                result.CurrencyAbbr = sum.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.CassetteList = casette;
            }

            return result;
        }

        public CashAddDetailView GetCashAddDetail(Guid jobGuid,Guid legGuid) {
           // var result = DbContext.TblMasterActualJobCashAdd.Where(w=>w.master)

            return new CashAddDetailView();
        }
    }

    public interface IMasterActualJobSumCashAddRepository : IRepository<TblMasterActualJobSumCashAdd>
    {
        SvdNoteWithdrawCashAdd GetCashAddDetail(Guid jobGuid);
    }
}
