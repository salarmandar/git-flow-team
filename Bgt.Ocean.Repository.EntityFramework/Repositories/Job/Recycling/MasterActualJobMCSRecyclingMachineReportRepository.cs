using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{


    public interface IMasterActualJobMCSRecyclingMachineReportRepository : IRepository<TblMasterActualJobMCSRecyclingMachineReport>
    {
        SvdRecyclingMachineReportWODispense GetRecyclingMachineReportWODispense(Guid? jobGuid);
    }
    public class MasterActualJobMCSRecyclingMachineReportRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingMachineReport>, IMasterActualJobMCSRecyclingMachineReportRepository
    {
        public MasterActualJobMCSRecyclingMachineReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SvdRecyclingMachineReportWODispense GetRecyclingMachineReportWODispense(Guid? jobGuid)
        {

            SvdRecyclingMachineReportWODispense result = new SvdRecyclingMachineReportWODispense();
            var head = DbContext.TblMasterActualJobMCSRecyclingMachineReport.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var casette = DbContext.TblMasterActualJobMCSRecyclingMachineReportEntry
                                       .Where(o => o.MasterActualJobMCSRecyclingMachineReport_Guid == head.Guid)
                                       .Select(o => new MRDRCTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Amount = o.Amount,
                                           Reject = o.Reject,
                                           Remain = o.Remain,
                                           CassetteSequence = o.CassetteSequence
                                       });
                result.TotalAmount = head.TotalAmount;
                result.TotalReject = head.TotalReject;
                result.TotalRemain = head.TotalRemain;
                result.MachinReportDispenseList = casette;
            }
            return result;
        }
    }
}
