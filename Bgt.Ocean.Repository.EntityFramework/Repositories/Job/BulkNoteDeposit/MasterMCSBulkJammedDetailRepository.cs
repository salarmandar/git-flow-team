using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterMCSBulkJammedDetailRepository : IRepository<TblMasterActualJobMCSBulkJammedDetail>
    {
        SvdBulkNoteDepositJammedDetail GetBulkNoteDepositJammedDetail(Guid? jobGuid);
    }
    public class MasterMCSBulkJammedDetailRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkJammedDetail>, IMasterMCSBulkJammedDetailRepository
    {
        public MasterMCSBulkJammedDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositJammedDetail GetBulkNoteDepositJammedDetail(Guid? jobGuid)
        {
            SvdBulkNoteDepositJammedDetail result = new SvdBulkNoteDepositJammedDetail();
            var head = DbContext.TblMasterActualJobMCSBulkJammed.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                result.JammedDetailList = DbContext.TblMasterActualJobMCSBulkJammedDetail.Where(o => o.MasterActualJobMCSBulkJammed_Guid == head.Guid)
                                                             .Select(o => new JBNDDetailView
                                                             {
                                                                 JammedReason = o.TblMasterReasonType.ReasonTypeName,
                                                                 NoOfNote = o.NumberOfNote
                                                             });
                result.JammedBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.JammedBag, Capability.BulkNoteDeposit);
            }
            return result;
        }
    }
}
