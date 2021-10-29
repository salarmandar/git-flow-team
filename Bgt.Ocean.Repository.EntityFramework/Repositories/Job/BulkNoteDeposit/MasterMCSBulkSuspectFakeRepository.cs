using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkSuspectFakeRepository : IRepository<TblMasterActualJobMCSBulkSuspectFake>
    {
        SvdBulkNoteDepositSuspectFake GetBulkNoteDepositSuspectFake(Guid? jobGuid);
    }
    public class MasterMCSBulkSuspectFakeRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkSuspectFake>, IMasterMCSBulkSuspectFakeRepository
    {
        public MasterMCSBulkSuspectFakeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositSuspectFake GetBulkNoteDepositSuspectFake(Guid? jobGuid)
        {

            SvdBulkNoteDepositSuspectFake result = new SvdBulkNoteDepositSuspectFake();
            var head = DbContext.TblMasterActualJobMCSBulkSuspectFake.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var denoList = DbContext.TblMasterActualJobMCSBulkSuspectFakeEntry
                                       .Where(o => o.MasterActualJobMCSBulkSuspectFake_Guid == head.Guid)
                                       .Select(o => new SFBNDTransectionView
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
                result.DenoList = denoList;
                
            }
            return result;
        }
    }
}
