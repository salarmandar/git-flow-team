using System;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public class MasterActualJobSumMachineReportRepository : Repository<OceanDbEntities, TblMasterActualJobSumMachineReport>, IMasterActualJobSumMachineReportRepository
    {
        public MasterActualJobSumMachineReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public SvdNoteWithdrawMachineReport GetMachineReportDetail(Guid jobGuid)
        {
            SvdNoteWithdrawMachineReport result = new SvdNoteWithdrawMachineReport();
            var sum = DbContext.TblMasterActualJobSumMachineReport.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (sum != null)
            {
                var casette = DbContext.TblMasterActualJobMachineReport
                                       .Where(o => o.MasterActualJobSumMachineReport_Guid == sum.Guid)
                                       .Select(o => new MRNWTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Dispense = o.Dispense,
                                           Reject = o.Reject,
                                           Remain = o.Remain,
                                           CassetteSequence = o.CassetteSequence
                                       });
                result.SumDispense = sum.SumDispense;
                result.SumReject = sum.SumReject;
                result.SumRemain = sum.SumRemain;
                result.BeginningAmount = sum.BeginningAmount;
                result.CurrencyAbbr = sum.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.CassetteList = casette;
            }

            return result;
        }
    }

    public interface IMasterActualJobSumMachineReportRepository : IRepository<TblMasterActualJobSumMachineReport>
    {
        SvdNoteWithdrawMachineReport GetMachineReportDetail(Guid jobGuid);
    }
}
