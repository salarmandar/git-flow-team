using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{


    public interface IMasterActualJobMCSCoinBulkNoteCollectRepository : IRepository<TblMasterActualJobMCSCoinBulkNoteCollect>
    {
        SvdCoinExchangeBulkNote GetCoinExchangeBulkNote(Guid? jobGuid);
    }
    public class MasterActualJobMCSCoinBulkNoteCollectRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinBulkNoteCollect>, IMasterActualJobMCSCoinBulkNoteCollectRepository
    {
        public MasterActualJobMCSCoinBulkNoteCollectRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
        public SvdCoinExchangeBulkNote GetCoinExchangeBulkNote(Guid? jobGuid)
        {
            SvdCoinExchangeBulkNote result = new SvdCoinExchangeBulkNote();
            var head = DbContext.TblMasterActualJobMCSCoinBulkNoteCollect.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var denoList = DbContext.TblMasterActualJobMCSCoinBulkNoteCollectEntry
                                       .Where(o => o.MasterActualJobMCSCoinBulkNoteCollect_Guid == head.Guid)
                                       .Select(o => new BNCXTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           AllIn = o.All_In,
                                           Amount = o.Amount
                                       });

                result.TotalAmount = head.TotalAmount;
                result.BulkNoteCollectionList = denoList;
                result.BulkNoteBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.BulkNoteBag, Capability.CoinExchange);
            }
            return result;
        }
    }
}
