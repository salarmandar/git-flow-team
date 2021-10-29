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
    public interface IMasterMCSBulkSuspectFakeDetailRepository : IRepository<TblMasterActualJobMCSBulkSuspectFakeDetail>
    {
        SvdBulkNoteDepositSuspectFakeDetail GetBulkNoteDepositSuspectFakeDetail(Guid? jobGuid);
    }
    public class MasterMCSBulkSuspectFakeDetailRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkSuspectFakeDetail>, IMasterMCSBulkSuspectFakeDetailRepository
    {
        public MasterMCSBulkSuspectFakeDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositSuspectFakeDetail GetBulkNoteDepositSuspectFakeDetail(Guid? jobGuid)
        {

            SvdBulkNoteDepositSuspectFakeDetail result = new SvdBulkNoteDepositSuspectFakeDetail();
            var head = DbContext.TblMasterActualJobMCSBulkSuspectFake.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                result.SuspectFakeDetailList = DbContext.TblMasterActualJobMCSBulkSuspectFakeDetail
                                                        .Where(o => o.MasterActualJobMCSBulkSuspectFake_Guid == head.Guid).AsEnumerable()
                                                         .Select(o => new SuspectFakeDetailView
                                                         {
                                                             CurrencyAbbr = o.TblMasterCurrency.MasterCurrencyAbbreviation,
                                                             DenominationValue = o.DenominationValue,
                                                             DepositDate = o.DepositDate,
                                                             DepositDateStr = o.DepositDate.ChangeFromDateToString(ApiSession.UserFormatDate),
                                                             SerialNo = o.SerialNo
                                                         });

                result.SuspectFakeBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.SuspectFakeBag, Capability.BulkNoteDeposit);

            }

            return result;
        }
    }
}
