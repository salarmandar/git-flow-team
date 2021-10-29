using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSRecyclingCashRecyclingRepository : IRepository<TblMasterActualJobMCSRecyclingCashRecycling>
    {
        SvdRecyclingCashRecycling GetRecyclingCashRecycling(Guid? jobGuid);
    }

    public class MasterActualJobMCSRecyclingCashRecyclingRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingCashRecycling>, IMasterActualJobMCSRecyclingCashRecyclingRepository
    {
        public MasterActualJobMCSRecyclingCashRecyclingRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
        public SvdRecyclingCashRecycling GetRecyclingCashRecycling(Guid? jobGuid)
        {

            SvdRecyclingCashRecycling result = new SvdRecyclingCashRecycling();
            var head = DbContext.TblMasterActualJobMCSRecyclingCashRecycling.FirstOrDefault(o => o.MasterActualJobHeader_Guid == jobGuid);
            if (head != null)
            {
                var casette = DbContext.TblMasterActualJobMCSRecyclingCashRecyclingEntry
                                       .Where(o => o.MasterActualJobMCSRecyclingCashRecycling_Guid == head.Guid)
                                       .Select(o => new RRCTransectionView
                                       {
                                           CurrencyAbbr = o.CurrencyAbbr,
                                           DenominationValue = o.DenominationValue,
                                           Amount = o.Amount,
                                           Loaded = o.Loaded,
                                           Recyled = o.Recyled,
                                           CassetteSequence = o.CassetteSequence
                                       });

                result.CustomerOrderNo = head.CustomerOrderNo??"";
                result.DeliveryAmount = head.DeliveryAmount;
                result.CurrencyAbbr = head.TblMasterCurrency.MasterCurrencyAbbreviation;
                result.TotalAmount = head.TotalAmount;
                result.TotalLoaded = head.TotalLoaded;
                result.TotalRecycled = head.TotalRecycled;
                result.CassetteList = casette;
                result.ReturnCashBagList = DbContext.TblMasterActualJobMCSItemSeal.GetSealBagView(jobGuid, SealTypeID.ReturnCashBag, Capability.Recycling);
            }
            return result;
        }
    }
}
