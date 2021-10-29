using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SiteNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterSiteNetworkRepository : IRepository<TblMasterSiteNetwork>
    {
        PopupSiteDestinationView GetDestinationSiteByOriginSiteOrLocation(Guid Site_Guid, Guid? OriginCustomerLocation_Guid, int ServiceJobTypeID);
        List<SiteNetworkMemberView> GetSiteNetworkInfoList(Guid MasterCountry_Guid, bool? FlagDisable);
        SiteNetworkMemberView GetSiteNetworkInfoById(Guid SiteNetworkGuid);
    }
    public class MasterSiteNetworkRepository : Repository<OceanDbEntities, TblMasterSiteNetwork>, IMasterSiteNetworkRepository
    {
        public MasterSiteNetworkRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public PopupSiteDestinationView GetDestinationSiteByOriginSiteOrLocation(Guid Site_Guid, Guid? OriginCustomerLocation_Guid, int ServiceJobTypeID)
        {
            //(1).multiBr jobs must have site path
            //(2).multiBr not allow same site
            //(3).P-multiBr condition : destination is Brinks
            //(4).TV-multiBr condition : destination is customer
            //(5).BCD-MultiBr condition : destination is customer

            PopupSiteDestinationView result = new PopupSiteDestinationView();
            IQueryable<Guid> SiteGuids = null;

            #region GET country
            var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid;
            if (MasterCountry_Guid == null)
                return result;
            #endregion

            #region GET pre-defined destination site has sitepath
            if (ServiceJobTypeID == IntTypeJob.TV_MultiBr)
            {
                SiteGuids = from loc_brinks in DbContext.TblMasterCustomerLocation_BrinksSite
                            join locDest in DbContext.TblMasterCustomerLocation_LocationDestination on loc_brinks.MasterCustomerLocation_Guid equals locDest.MasterCustomerLocationDes_Guid //pre-defined table
                            join loc in DbContext.TblMasterCustomerLocation on loc_brinks.MasterCustomerLocation_Guid equals loc.Guid
                            join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                            where locDest.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                                  && loc_brinks.MasterSite_Guid != Site_Guid //(2)
                                  && cust.FlagChkCustomer == true //(4) 
                                  && loc_brinks.FlagDefaultBrinksSite
                                  && locDest.MasterSitePathHeader_Guid != null //(1)
                            select loc_brinks.MasterSite_Guid;
            }


            if (ServiceJobTypeID == IntTypeJob.P_MultiBr)
            {

                SiteGuids = from locDest in DbContext.TblMasterCustomerLocation_LocationDestination
                            join loc in DbContext.TblMasterCustomerLocation on locDest.MasterCustomerLocationDes_Guid equals loc.Guid //pre-defined table
                            join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                            where locDest.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                                  && loc.MasterSite_Guid != Site_Guid //(2)
                                  && cust.FlagChkCustomer == false //(3) 
                                  && loc.MasterSite_Guid != null
                                  && locDest.MasterSitePathHeader_Guid != null //(1)
                            select loc.MasterSite_Guid.Value;
            }

            if (ServiceJobTypeID == IntTypeJob.BCD_MultiBr)
            {

                SiteGuids = (from siteHeader in DbContext.TblMasterSitePathHeader
                             join siteNetWork in DbContext.TblMasterSiteNetwork on siteHeader.MasterSiteNetwork_Guid equals siteNetWork.Guid
                             join sitepathBCD in DbContext.TblMasterSitePathDestination on siteHeader.Guid equals sitepathBCD.MasterSitePathHeader_Guid  //pre-defined table (1),(2) 
                             join destSite in DbContext.TblMasterSitePathDetail on
                             //Multi condition join
                             new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = Site_Guid, FlagDestination = true } equals
                             new { MasterSitePathHeader_Guid = destSite.MasterSitePathHeader_Guid, MasterSite_Guid = destSite.MasterSite_Guid, FlagDestination = destSite.FlagDestination }
                             join originSite in DbContext.TblMasterSitePathDetail on siteHeader.Guid equals originSite.MasterSitePathHeader_Guid
                             where originSite.SequenceIndex == 1 && !siteHeader.FlagDisble && !siteNetWork.FlagDisable &&
                                   sitepathBCD.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                             select originSite.MasterSite_Guid).Distinct();

            }


            #endregion

            #region GET no pre-defined site has sitepath
            if (SiteGuids == null || !SiteGuids.Any())
            {

                if (ServiceJobTypeID == IntTypeJob.BCD_MultiBr)
                {
                    //#get origin site
                    SiteGuids = (from siteHeader in DbContext.TblMasterSitePathHeader
                                 join siteNetWork in DbContext.TblMasterSiteNetwork on siteHeader.MasterSiteNetwork_Guid equals siteNetWork.Guid
                                 join destSite in DbContext.TblMasterSitePathDetail on
                                 //Multi condition join
                                 new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = Site_Guid, FlagDestination = true } equals
                                 new { MasterSitePathHeader_Guid = destSite.MasterSitePathHeader_Guid, MasterSite_Guid = destSite.MasterSite_Guid, FlagDestination = destSite.FlagDestination }
                                 join originSite in DbContext.TblMasterSitePathDetail on siteHeader.Guid equals originSite.MasterSitePathHeader_Guid
                                 where originSite.SequenceIndex == 1 && !siteHeader.FlagDisble && !siteNetWork.FlagDisable
                                 select originSite.MasterSite_Guid).Distinct();
                }
                else
                {
                    //#get destination site
                    SiteGuids = (from siteHeader in DbContext.TblMasterSitePathHeader
                                 join siteNetWork in DbContext.TblMasterSiteNetwork on siteHeader.MasterSiteNetwork_Guid equals siteNetWork.Guid
                                 join originSite in DbContext.TblMasterSitePathDetail on
                                 //Multi condition join
                                 new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = Site_Guid, seq = 1 } equals
                                 new { MasterSitePathHeader_Guid = originSite.MasterSitePathHeader_Guid, MasterSite_Guid = originSite.MasterSite_Guid, seq = originSite.SequenceIndex.Value }
                                 join destSite in DbContext.TblMasterSitePathDetail on siteHeader.Guid equals destSite.MasterSitePathHeader_Guid
                                 where destSite.FlagDestination && !siteHeader.FlagDisble && !siteNetWork.FlagDisable
                                 select destSite.MasterSite_Guid).Distinct();
                }
            }
            #endregion

            #region GET data

            var allData = from CUS in DbContext.TblMasterCustomer
                          join LOC in DbContext.TblMasterCustomerLocation on CUS.Guid equals LOC.MasterCustomer_Guid
                          join SITE in DbContext.TblMasterSite on LOC.MasterSite_Guid equals SITE.Guid
                          join DESTSITE in SiteGuids on LOC.MasterSite_Guid equals DESTSITE
                          join COUNTRY in DbContext.TblMasterCountry on SITE.MasterCountry_Guid equals COUNTRY.Guid
                          where SITE.FlagDisable == false
                          && CUS.FlagChkCustomer == false
                          && CUS.Flag3Party == false
                          && CUS.FlagDefaultSystem == false
                          && SITE.Guid != Site_Guid //(2)
                          select new
                          {
                              siteNetworkMember = new SiteDestinationMemberView
                              {
                                  SiteGuid = SITE.Guid,
                                  SiteName = (SITE.SiteCode ?? "") + " - " + (SITE.SiteName ?? ""),
                                  MasterCountryGuid = SITE.MasterCountry_Guid,
                                  MasterCountryName = COUNTRY.MasterCountryName,
                                  CompanyGuid = CUS.Guid,
                                  CompanyName = CUS.CustomerFullName
                              },
                              country = new DestinationCountryView
                              {
                                  MasterCountryGuid = COUNTRY.Guid,
                                  FlagHaveState = COUNTRY.FlagHaveState ?? false,
                                  FlagInputCityManual = COUNTRY.FlagInputCityManual ?? false,
                                  MasterCountryName = COUNTRY.MasterCountryName

                              },
                              brinksCompany = new BrinksCompanyView
                              {
                                  CompanyGuid = CUS.Guid,
                                  CompanyName = CUS.CustomerFullName,
                                  MasterCountryGuid = COUNTRY.Guid
                              },
                          };

            result.SiteNetworkMemberlist = allData.Select(o => o.siteNetworkMember)
                                                  .GroupBy(g => new { g.SiteGuid })
                                                  .Select(g => g.FirstOrDefault())
                                                  .OrderBy(o => o.SiteName);

            result.Countrylist = allData.Select(o => o.country)
                                        .GroupBy(g => new { g.MasterCountryGuid })
                                        .Select(g => g.FirstOrDefault())
                                        .OrderBy(o => o.MasterCountryName);

            result.BrinksCompanylist = allData.Select(o => o.brinksCompany)
                                      .GroupBy(g => new { g.CompanyGuid })
                                      .Select(g => g.FirstOrDefault())
                                      .OrderBy(o => o.CompanyName);
            #endregion

            return result;
        }

        public List<SiteNetworkMemberView> GetSiteNetworkInfoList(Guid MasterCountry_Guid, bool? FlagDisable)
        {
            bool includeAll = (!FlagDisable.HasValue || !FlagDisable.Value) ? false : true;
            List<SiteNetworkMemberView> Result = (from s in DbContext.TblMasterSiteNetwork
                                                  join c in DbContext.TblMasterCountry on s.MasterCountry_Guid equals c.Guid
                                                  where s.MasterCountry_Guid == MasterCountry_Guid && (includeAll || !s.FlagDisable)
                                                  select new SiteNetworkMemberView
                                                  {
                                                      SiteGuid = s.Guid,
                                                      SiteName = s.SiteNetworkName,
                                                      MasterCountryGuid = s.MasterCountry_Guid,
                                                      MasterCountryName = c.MasterCountryName,
                                                      FlagDisable = s.FlagDisable,
                                                      UserCreated = s.UserCreated,
                                                      DatetimeCreated = s.DatetimeCreated,
                                                      UserModified = s.UserModified,
                                                      DatetimeModified = s.DatetimeModified,
                                                      BrinksSitelist = from bsm in DbContext.TblMasterSiteNetworkMember
                                                                       join bs in DbContext.TblMasterSite on bsm.MasterSite_Guid equals bs.Guid
                                                                       join mc in DbContext.TblMasterCustomer on bs.MasterCustomer_Guid equals mc.Guid
                                                                       where bsm.MasterSiteNetwork_Guid == s.Guid
                                                                       select new BrinksSiteBySiteNetwork
                                                                       {
                                                                           Guid = bsm.Guid,
                                                                           SiteGuid = bsm.MasterSite_Guid,
                                                                           SiteName = (bs.SiteName ?? ""),
                                                                           SiteCodeName = (bs.SiteCode ?? "") + " - " + (bs.SiteName ?? ""),
                                                                           CompanyGuid = mc.Guid,
                                                                           CompanyName = mc.CustomerFullName
                                                                       },
                                                      FlagHaveSitePath = (from p in DbContext.TblMasterSitePathHeader
                                                                          where p.MasterSiteNetwork_Guid == s.Guid && !p.FlagDisble
                                                                          select p.Guid).Any()
                                                  }).ToList();
            return Result;
        }

        public SiteNetworkMemberView GetSiteNetworkInfoById(Guid SiteNetworkGuid)
        {
            SiteNetworkMemberView Result = (from s in DbContext.TblMasterSiteNetwork
                                            join c in DbContext.TblMasterCountry on s.MasterCountry_Guid equals c.Guid
                                            where s.Guid == SiteNetworkGuid
                                            select new SiteNetworkMemberView
                                            {
                                                SiteGuid = s.Guid,
                                                SiteName = s.SiteNetworkName,
                                                MasterCountryGuid = s.MasterCountry_Guid,
                                                MasterCountryName = c.MasterCountryName,
                                                FlagDisable = s.FlagDisable,
                                                UserCreated = s.UserCreated,
                                                DatetimeCreated = s.DatetimeCreated,
                                                UserModified = s.UserModified,
                                                DatetimeModified = s.DatetimeModified,
                                                BrinksSitelist = from bsm in DbContext.TblMasterSiteNetworkMember
                                                                 join bs in DbContext.TblMasterSite on bsm.MasterSite_Guid equals bs.Guid
                                                                 join mc in DbContext.TblMasterCustomer on bs.MasterCustomer_Guid equals mc.Guid
                                                                 where bsm.MasterSiteNetwork_Guid == s.Guid
                                                                 orderby bsm.DatetimeCreated ascending
                                                                 select new BrinksSiteBySiteNetwork
                                                                 {
                                                                     Guid = bsm.Guid,
                                                                     SiteGuid = bsm.MasterSite_Guid,
                                                                     SiteName = (bs.SiteName ?? ""),
                                                                     SiteCodeName = (bs.SiteCode ?? "") + " - " + (bs.SiteName ?? ""),
                                                                     CompanyGuid = mc.Guid,
                                                                     CompanyName = mc.CustomerFullName
                                                                 },
                                                FlagHaveSitePath = (from p in DbContext.TblMasterSitePathHeader
                                                                    where p.MasterSiteNetwork_Guid == s.Guid && !p.FlagDisble
                                                                    select p.Guid).Any()
                                            }).FirstOrDefault();
            return Result;
        }
    }
}
