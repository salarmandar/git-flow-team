using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSBulkJammedRepository : IRepository<TblMasterActualJobMCSBulkJammed>
    {
        SvdBulkNoteDepositJammed GetBulkNoteDepositJammed(Guid? jobGuid);
    }
    public class MasterActualJobMCSBulkJammedRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkJammed>, IMasterActualJobMCSBulkJammedRepository
    {
        public MasterActualJobMCSBulkJammedRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdBulkNoteDepositJammed GetBulkNoteDepositJammed(Guid? jobGuid)
        {
            SvdBulkNoteDepositJammed result = new SvdBulkNoteDepositJammed();
            var head = DbContext.TblMasterActualJobMCSBulkJammed.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var denoList = DbContext.TblMasterActualJobMCSBulkJammedEntry
                                       .Where(o => o.MasterActualJobMCSBulkJammed_Guid == head.Guid)
                                       .Select(o => new JBNDTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Jammed = o.Jammed,
                                           Amount = o.Amount
                                       });

                result.TotalAmount = head.TotalAmount;
                result.DenoList = denoList;
            }
            return result;
        }
    }
}
