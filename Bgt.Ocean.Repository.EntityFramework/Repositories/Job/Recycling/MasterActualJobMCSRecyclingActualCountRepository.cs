using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSRecyclingActualCountRepository : IRepository<TblMasterActualJobMCSRecyclingActualCount>
    {
        SvdRecyclingActualCount GetRecyclingActualCount(Guid? jobGuid);
    }
    public class MasterActualJobMCSRecyclingActualCountRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingActualCount>, IMasterActualJobMCSRecyclingActualCountRepository
    {
        public MasterActualJobMCSRecyclingActualCountRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdRecyclingActualCount GetRecyclingActualCount(Guid? jobGuid)
        {

            SvdRecyclingActualCount result = new SvdRecyclingActualCount();
            var head = DbContext.TblMasterActualJobMCSRecyclingActualCount.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var casette = DbContext.TblMasterActualJobMCSRecyclingActualCountEntry
                                       .Where(o => o.MasterActualJobMCSRecyclingActualCount_Guid == head.Guid)
                                       .Select(o => new ACRCTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Amount = o.Amount,
                                           Counted = o.Counted,
                                           Reject = o.Reject,
                                           Diff = o.Diff,
                                           CassetteSequence = o.CassetteSequence
                                       });
                result.TotalAmount = head.TotalAmount;
                result.TotalDiff = head.TotalDiff.GetValueOrDefault();
                result.TotalReject = head.TotalReject.GetValueOrDefault();
                result.ActualCountList = casette;
            }
            return result;
        }
    }
}
