using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkDepositReportRepository : IRepository<TblMasterActualJobMCSBulkDepositReport>
    {
        SvdBulkNoteDepositDepositReport GetBulkNoteDepositDepositReport(Guid? jobGuid);
    }
    public class MasterMCSBulkDepositReportRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkDepositReport>, IMasterMCSBulkDepositReportRepository
    {
        public MasterMCSBulkDepositReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositDepositReport GetBulkNoteDepositDepositReport(Guid? jobGuid)
        {

            SvdBulkNoteDepositDepositReport result = new SvdBulkNoteDepositDepositReport();
            var head = DbContext.TblMasterActualJobMCSBulkDepositReport.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var casette = DbContext.TblMasterActualJobMCSBulkDepositReportEntry
                                       .Where(o => o.MasterMCSBulkDepositReport_Guid == head.Guid).AsEnumerable()
                                       .Select(o => new DCRBNDTransectionView
                                       {
                                           CassetteName = o.CassetteName,
                                           NumberOfNote = o.NumberOfNote,
                                           Amount = o.Amount,
                                           CassetteSequence = o.CassetteSequence
                                       });
                result.TotalAmount = head.TotalAmount;
                result.CassetteList = casette;

                result.DepositReturnBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.DepositReturnBag, Capability.BulkNoteDeposit);
            }
            return result;
        }
    }
}
