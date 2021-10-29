using System;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public class MasterActualJobSumActualCountRepository : Repository<OceanDbEntities, TblMasterActualJobSumActualCount>, IMasterActualJobSumActualCountRepository
    {
        public MasterActualJobSumActualCountRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SvdNoteWithdrawActualCount GetActualCountDetail(Guid jobGuid)
        {
            SvdNoteWithdrawActualCount result = new SvdNoteWithdrawActualCount();
            var sum = DbContext.TblMasterActualJobSumActualCount.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (sum != null)
            {
                var casette = DbContext.TblMasterActualJobActualCount
                                       .Where(o => o.MasterActualJobSumActualCount_Guid == sum.Guid)
                                       .Select(o => new ACNWTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Count = o.Count,
                                           Reject = o.Reject,
                                           Diff = o.Diff,
                                           CassetteSequence = o.CassetteSequence
                                       });

                result.SumCount = sum.SumCount;
                result.SumReject = sum.SumReject;
                result.SumDiff = sum.SumDiff;
                result.CurrencyAbbr = sum.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.CassetteList = casette;
            }

            return result;
        }
    }

    public interface IMasterActualJobSumActualCountRepository : IRepository<TblMasterActualJobSumActualCount>
    {
        SvdNoteWithdrawActualCount GetActualCountDetail(Guid jobGuid);
    }
}
