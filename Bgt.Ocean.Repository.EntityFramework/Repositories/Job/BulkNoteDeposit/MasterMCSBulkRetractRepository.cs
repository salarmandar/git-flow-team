using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkRetractRepository : IRepository<TblMasterActualJobMCSBulkRetract>
    {
        SvdBulkNoteDepositRetract GetBulkNoteDepositRetract(Guid? jobGuid);
    }
    public class MasterMCSBulkRetractRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkRetract>, IMasterMCSBulkRetractRepository
    {
        public MasterMCSBulkRetractRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositRetract GetBulkNoteDepositRetract(Guid? jobGuid)
        {

            SvdBulkNoteDepositRetract result = new SvdBulkNoteDepositRetract();
            var head = DbContext.TblMasterActualJobMCSBulkRetract.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var denoList = DbContext.TblMasterActualJobMCSBulkRetractEntry
                                       .Where(o => o.MasterMCSBulkRetract_Guid == head.Guid)
                                       .Select(o => new RBNDTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Report = o.Report,
                                           Counted = o.Counted,
                                           Diff = o.Diff
                                       });

                var DocumentT1List = DbContext.TblMasterActualJobMCSBulkT1Detail
                                              .Where(o => o.MasterActualJobMCSBulkRetract_Guid == head.Guid)
                                              .Select(o => new RBNDDocumentT1View
                                              {
                                                  NumberOfNote = o.NumberOfNote,
                                                  ReasonName = o.ReasonName                                                  
                                              });

                result.TotalReport = head.TotalReport;
                result.TotalCounted = head.TotalCounted;
                result.TotalDiff = head.TotalDiff;
                result.TotalDocument = head.TotalDocument;
                result.DenoList = denoList;

                result.DocumentT1DetailList = DocumentT1List;

                result.RetractReturnBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.RetrackBag, Capability.BulkNoteDeposit);
                result.DocumentT1BagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.T1Bag, Capability.BulkNoteDeposit);
            }
            return result;
        }
    }
}
