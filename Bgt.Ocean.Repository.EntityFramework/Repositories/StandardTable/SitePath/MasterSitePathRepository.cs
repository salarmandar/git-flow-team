using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath
{
    public interface IMasterSitePathRepository : IRepository<TblMasterSitePathHeader>
    {
        List<SitePathView> GetSitePathInfoList(Guid MasterCountry_Guid, bool? FlagDisable);
        SitePathView GetSitePathInfoById(Guid SitePathGuid);
        bool ValidateDuplicateSitePath(SitePathView sitePath, out int error);

        IEnumerable<BrinksSiteBySitePath> getShortInfoSitePathDetail(Guid? SitePathHeader_Guid);

        IEnumerable<TblMasterSitePathHeader> getSitePathHeaderFromTblMasterSitePath(Guid? pickupSiteGuid, Guid? deliverySiteGuid);

        IEnumerable<TblMasterSitePathHeader> getAllSitePathNameFromCustomerLocation(int? jobType, Guid? pickupLocationGuid, Guid? deliveryLocationGuid, Guid? pickupSiteGuid, Guid? deliverySiteGuid);

    }
    public class MasterSitePathRepository : Repository<OceanDbEntities, TblMasterSitePathHeader>, IMasterSitePathRepository
    {
        public MasterSitePathRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public List<SitePathView> GetSitePathInfoList(Guid MasterCountry_Guid, bool? FlagDisable)
        {
            bool includeAll = (!FlagDisable.HasValue || !FlagDisable.Value) ? false : true;
            List<SitePathView> Result = null;
            Result = (from s in DbContext.TblMasterSitePathHeader
                                         join n in DbContext.TblMasterSiteNetwork on s.MasterSiteNetwork_Guid equals n.Guid
                                         join c in DbContext.TblMasterCountry on n.MasterCountry_Guid equals c.Guid
                                         where n.MasterCountry_Guid == MasterCountry_Guid && (includeAll || !s.FlagDisble)
                                         select new SitePathView
                                         {
                                            SitePathGuid = s.Guid,
                                            SitePathName = s.SitePathName,
                                            SiteNetworkGuid = s.MasterSiteNetwork_Guid,
                                            SiteNetworkName = n.SiteNetworkName,
                                            MasterCountryGuid = n.MasterCountry_Guid,
                                            MasterCountryName = c.MasterCountryName,
                                            FlagDisable = s.FlagDisble,
                                            UserCreated = s.UserCreated,
                                            DatetimeCreated = s.DatetimeCreted,
                                            UserModified = s.UserModified,
                                            DatetimeModified = s.DatetimeModified
                                        }).ToList();
            return Result;
        }

        public SitePathView GetSitePathInfoById(Guid SitePathGuid)
        {
            SitePathView Result = null;
            Result = (from s in DbContext.TblMasterSitePathHeader
                                         join n in DbContext.TblMasterSiteNetwork on s.MasterSiteNetwork_Guid equals n.Guid
                                         join c in DbContext.TblMasterCountry on n.MasterCountry_Guid equals c.Guid
                                         where s.Guid == SitePathGuid && s.FlagDisble == false
                                         select new SitePathView
                                         {
                                             SitePathGuid = s.Guid,
                                             SitePathName = s.SitePathName,
                                             SiteNetworkGuid = s.MasterSiteNetwork_Guid,
                                             SiteNetworkName = n.SiteNetworkName,
                                             MasterCountryGuid = n.MasterCountry_Guid,
                                             MasterCountryName = c.MasterCountryName,
                                             FlagDisable = s.FlagDisble,
                                             UserCreated = s.UserCreated,
                                             DatetimeCreated = s.DatetimeCreted,
                                             UserModified = s.UserModified,
                                             DatetimeModified = s.DatetimeModified,
                                             BrinksSitelist = from bsm in DbContext.TblMasterSitePathDetail
                                                              join bs in DbContext.TblMasterSite on bsm.MasterSite_Guid equals bs.Guid
                                                              where bsm.MasterSitePathHeader_Guid == s.Guid
                                                              orderby bsm.SequenceIndex ascending
                                                              select new BrinksSiteBySitePath
                                                              {
                                                                  Guid = bsm.Guid,
                                                                  SiteGuid = bsm.MasterSite_Guid,
                                                                  SiteName = (bs.SiteName ?? ""),
                                                                  SiteCodeName = (bs.SiteCode ?? "") + " - " + (bs.SiteName ?? ""),
                                                                  SecuenceIndex = bsm.SequenceIndex,
                                                                  FlagDestination = bsm.FlagDestination
                                                              },
                                             CustomerLocationlist = from spd in DbContext.TblMasterSitePathDestination
                                                                    join cl in DbContext.TblMasterCustomerLocation on spd.MasterCustomerLocation_Guid equals cl.Guid
                                                                    join c in DbContext.TblMasterCustomer on cl.MasterCustomer_Guid equals c.Guid
                                                                    where spd.MasterSitePathHeader_Guid == s.Guid
                                                                    select new LocationViewBySitePath
                                                                    {
                                                                        Guid = spd.Guid,
                                                                        CustomerGuid = c.Guid,
                                                                        CustomerName = c.CustomerFullName,
                                                                        LocationGuid = cl.Guid,
                                                                        LocationName = cl.BranchName
                                                                    },
                                         }).FirstOrDefault();
            return Result;
        }

        public bool ValidateDuplicateSitePath(SitePathView sitePath, out int error)
        {
            error = 0;
            var validSiteName = (from s in DbContext.TblMasterSitePathHeader
                      join n in DbContext.TblMasterSiteNetwork on s.MasterSiteNetwork_Guid equals n.Guid
                      where s.SitePathName == sitePath.SitePathName && 
                      n.MasterCountry_Guid == sitePath.MasterCountryGuid && s.Guid != sitePath.SitePathGuid
                      select new SitePathView
                      {
                          SitePathGuid = s.Guid
                      }).FirstOrDefault();
            if (validSiteName == null)
            {
                int count = 0;
                Guid originSite = sitePath.BrinksSitelist.OrderBy(x => x.SecuenceIndex).FirstOrDefault().SiteGuid;
                Guid destinationSite = sitePath.BrinksSitelist.LastOrDefault().SiteGuid;
                int destinationIndex = Convert.ToInt32(sitePath.BrinksSitelist.OrderBy(x => x.SecuenceIndex).LastOrDefault().SecuenceIndex);
                var repeteadSites = (from s in DbContext.TblMasterSitePathHeader
                                  join bsm in DbContext.TblMasterSitePathDetail on s.Guid equals bsm.MasterSitePathHeader_Guid
                                  join dsm in DbContext.TblMasterSitePathDetail on s.Guid equals dsm.MasterSitePathHeader_Guid
                                  where (bsm.MasterSite_Guid == originSite && bsm.SequenceIndex == 1) &&
                                  (dsm.MasterSite_Guid == destinationSite && dsm.SequenceIndex == destinationIndex && dsm.FlagDestination) &&
                                  bsm.MasterSitePathHeader_Guid == dsm.MasterSitePathHeader_Guid &&
                                  s.Guid != sitePath.SitePathGuid
                                  select new
                                  {
                                      SitePathGuid = s.Guid,
                                  }).ToList();
                foreach (var sitePathRepeated in repeteadSites)
                {
                    count = destinationIndex;
                    foreach (BrinksSiteBySitePath brinkSite in sitePath.BrinksSitelist)
                    {
                        var validateSite = (from bsm in DbContext.TblMasterSitePathDetail
                                            where bsm.MasterSite_Guid == brinkSite.SiteGuid && bsm.SequenceIndex == brinkSite.SecuenceIndex &&
                                            bsm.MasterSitePathHeader_Guid == sitePathRepeated.SitePathGuid
                                            select new
                                            {
                                                Guid = bsm.Guid
                                            }).FirstOrDefault();
                        if(validateSite != null)
                        {
                            count--;
                        }
                    }
                    if(count == 0)
                    {
                        error = 2;
                        return false;
                    }
                }
            }
            else
            {
                error = 1;
                return false;
            }
            return true;
        }

        

        public IEnumerable<BrinksSiteBySitePath> getShortInfoSitePathDetail(Guid? SitePathHeader_Guid)
        {
            var query_sitepath = from tblHeader in DbContext.TblMasterSitePathHeader.Where(o => !o.FlagDisble)
                                 join tblDetail in DbContext.TblMasterSitePathDetail on tblHeader.Guid equals tblDetail.MasterSitePathHeader_Guid
                                 join tblSite in DbContext.TblMasterSite on tblDetail.MasterSite_Guid equals tblSite.Guid
                                 where tblHeader.Guid == SitePathHeader_Guid
                                 select new BrinksSiteBySitePath
                                 {
                                     SecuenceIndex = tblDetail.SequenceIndex ?? 0,
                                     Guid = tblDetail.Guid,
                                     SiteGuid = tblSite.Guid,
                                     SiteName = (tblSite.SiteName ?? ""),
                                     SiteCodeName = (tblSite.SiteCode ?? "") + " - " + (tblSite.SiteName ?? ""),
                                 };
            return query_sitepath.AsEnumerable().OrderBy(o => o.SecuenceIndex);
        }

        public IEnumerable<TblMasterSitePathHeader> getSitePathHeaderFromTblMasterSitePath(Guid? pickupSiteGuid, Guid? deliverySiteGuid)
        {
            var query_sitepath = from tblHeader in DbContext.TblMasterSitePathHeader.Where(o => !o.FlagDisble)
                                 join tblDetail_P in DbContext.TblMasterSitePathDetail on tblHeader.Guid equals tblDetail_P.MasterSitePathHeader_Guid
                                 join tblDetail_D in DbContext.TblMasterSitePathDetail on tblHeader.Guid equals tblDetail_D.MasterSitePathHeader_Guid
                                 where (tblDetail_P.SequenceIndex == 1 && tblDetail_P.MasterSite_Guid == pickupSiteGuid) &&
                                        (tblDetail_D.FlagDestination && tblDetail_D.MasterSite_Guid == deliverySiteGuid)
                                 select tblHeader;
            return query_sitepath.AsEnumerable();
        }

        public IEnumerable<TblMasterSitePathHeader> getAllSitePathNameFromCustomerLocation(int? jobType, Guid? pickupLocationGuid, Guid? deliveryLocationGuid, Guid? pickupSiteGuid, Guid? deliverySiteGuid)
        {
            IQueryable<TblMasterSitePathHeader> query = null;
            if (jobType == 14) //Get SitePath from P MultiBranch, no need deliveryLocationGuid, if not return blank
            {
                query = from tblHeader in DbContext.TblMasterSitePathHeader.Where(o => !o.FlagDisble)
                        join locDest in DbContext.TblMasterCustomerLocation_LocationDestination on tblHeader.Guid equals locDest.MasterSitePathHeader_Guid
                        join locD in DbContext.TblMasterCustomerLocation on locDest.MasterCustomerLocationDes_Guid equals locD.Guid
                        where locDest.MasterCustomerLocation_InternalDepartment_Guid != null &&
                                locDest.MasterCustomerLocation_Guid == pickupLocationGuid &&
                                locD.MasterSite_Guid == deliverySiteGuid
                        select tblHeader;
            }
            else if  (jobType == 15) //Get SitePath from TV MultiBranch, need deliveryLocationGuid, if not return blank
            {
                query = from tblHeader in DbContext.TblMasterSitePathHeader.Where(o => !o.FlagDisble)
                        join locDest in DbContext.TblMasterCustomerLocation_LocationDestination on tblHeader.Guid equals locDest.MasterSitePathHeader_Guid
                        where locDest.MasterCustomerLocation_InternalDepartment_Guid == null &&
                                locDest.MasterCustomerLocation_Guid == pickupLocationGuid &&
                                locDest.MasterCustomerLocationDes_Guid == deliveryLocationGuid
                        select tblHeader;
            }
            //No JobType, gets TblSitePath


            if (query != null && query.Any())
            {
                return query.AsEnumerable();
            }

            //if cannot get from CustomerLocation's Destination -> get from TblSitePath
            return getSitePathHeaderFromTblMasterSitePath(pickupSiteGuid, deliverySiteGuid);
        }
    }
}
