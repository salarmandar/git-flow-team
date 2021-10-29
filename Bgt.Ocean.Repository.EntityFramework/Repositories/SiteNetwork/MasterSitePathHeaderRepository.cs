using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.CustomerLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SiteNetwork
{
    public interface IMasterSitePathHeaderRepository : IRepository<TblMasterSitePathHeader>
    {
        IEnumerable<DropdownSitePathView> GetAllSitePath(Guid OriginSite_Guid, Guid DestinationSite_Guid);
        IEnumerable<DropdownSitePathView> GetSitePathHaveItem(Guid OriginSite_Guid, Guid DestinationSite_Guid, DateTime workDate);
    }

    public class MasterSitePathHeaderRepository : Repository<OceanDbEntities, TblMasterSitePathHeader>, IMasterSitePathHeaderRepository
    {
        public MasterSitePathHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<DropdownSitePathView> GetAllSitePath(Guid OriginSite_Guid, Guid DestinationSite_Guid)
        {
            IEnumerable<DropdownSitePathView> result = from siteHeader in DbContext.TblMasterSitePathHeader
                                                       join originSite in DbContext.TblMasterSitePathDetail on
                                                       //Multi condition join
                                                       new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = OriginSite_Guid, seq = 1 } equals
                                                       new { MasterSitePathHeader_Guid = originSite.MasterSitePathHeader_Guid, MasterSite_Guid = originSite.MasterSite_Guid, seq = originSite.SequenceIndex.Value }
                                                       join destSite in DbContext.TblMasterSitePathDetail on siteHeader.Guid equals destSite.MasterSitePathHeader_Guid
                                                       where destSite.FlagDestination && destSite.MasterSite_Guid == DestinationSite_Guid && !siteHeader.FlagDisble
                                                       select new DropdownSitePathView
                                                       {
                                                           SitePath_Guid = siteHeader.Guid,
                                                           SitePathName = siteHeader.SitePathName,
                                                       };
            return result;
        }

        public IEnumerable<DropdownSitePathView> GetSitePathHaveItem(Guid OriginSite_Guid, Guid DestinationSite_Guid, DateTime workDate)
        {
             var data = (from siteHeader in DbContext.TblMasterSitePathHeader
                           join originSite in DbContext.TblMasterSitePathDetail on
                           //Multi condition join
                           new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = OriginSite_Guid, seq = 1 } equals
                           new { MasterSitePathHeader_Guid = originSite.MasterSitePathHeader_Guid, MasterSite_Guid = originSite.MasterSite_Guid, seq = originSite.SequenceIndex.Value }
                           join destSite in DbContext.TblMasterSitePathDetail on siteHeader.Guid equals destSite.MasterSitePathHeader_Guid
                           join jobHead in DbContext.TblMasterActualJobHeader on siteHeader.Guid equals jobHead.MasterSitePathHeader_Guid
                           join destLeg in DbContext.TblMasterActualJobServiceStopLegs on 
                           new { JobGuid = jobHead.Guid, FlagDestination = true } equals
                           new { JobGuid = destLeg.MasterActualJobHeader_Guid.Value, FlagDestination = destLeg.FlagDestination }
                           where destSite.FlagDestination && destSite.MasterSite_Guid == DestinationSite_Guid && !siteHeader.FlagDisble
                                 && jobHead.FlagMultiBranch
                                 && (jobHead.SystemStatusJobID == 7 || jobHead.SystemStatusJobID == 28 || jobHead.SystemStatusJobID == 29) 
                                 && destLeg.ServiceStopTransectionDate == workDate
                                 && (   DbContext.TblMasterActualJobItemsSeal.Any(a => a.MasterActualJobHeader_Guid == jobHead.Guid)
                                        || DbContext.TblMasterActualJobItemsCommodity.Any(a => a.MasterActualJobHeader_Guid == jobHead.Guid)    )
                           select new 
                           {
                               SitePath_Guid = siteHeader.Guid,
                               SitePathName = siteHeader.SitePathName,
                           }).Distinct();
            IEnumerable<DropdownSitePathView> result = data.Select(s => new DropdownSitePathView { SitePath_Guid = s.SitePath_Guid, SitePathName = s.SitePathName });
            return result;
        }
    }
}
