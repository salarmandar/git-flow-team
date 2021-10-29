using System;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Infrastructure.Util;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public class MasterActualJobSumCashReturnRepository : Repository<OceanDbEntities, TblMasterActualJobSumCashReturn>, IMasterActualJobSumCashReturnRepository
    {
        public MasterActualJobSumCashReturnRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SvdNoteWithdrawCashReturn GetCashReuturnDetail(Guid jobGuid)
        {
            SvdNoteWithdrawCashReturn result = new SvdNoteWithdrawCashReturn();
            var sum = DbContext.TblMasterActualJobSumCashReturn.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (sum != null)
            {
                var casette = DbContext.TblMasterActualJobCashReturn
                                       .Where(o => o.MasterActualJobSumCashReturn_Guid == sum.Guid)
                                       .Select(o => new CRNWTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Diff = o.Diff,
                                           Return = o.Return,
                                           Stay = o.Stay
                                       });

                result.SumDiff = sum.SumDiff;
                result.SumReturn = sum.SumReturn;
                result.SumStay = sum.SumStay;
                result.ReturnAmount = sum.ReturnAmount;
                result.StayBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.StayBag, Capability.NoteWithdraw);
                result.ReturnBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.ReturnBag, Capability.NoteWithdraw);
                result.CurrencyAbbr = sum.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.CassetteList = casette;
            }

            return result;
        }
    }

    public interface IMasterActualJobSumCashReturnRepository : IRepository<TblMasterActualJobSumCashReturn>
    {
        SvdNoteWithdrawCashReturn GetCashReuturnDetail(Guid jobGuid);
    }
}
