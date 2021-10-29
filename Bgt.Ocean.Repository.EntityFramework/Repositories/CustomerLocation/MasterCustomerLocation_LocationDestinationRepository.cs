using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Models.CustomerLocation;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Infrastructure.Util;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation
{
    public interface IMasterCustomerLocationLocationDestinationRepository : IRepository<TblMasterCustomerLocation_LocationDestination>
    {
        IEnumerable<AdhocDestinationResult> Func_AdhocDestination(Guid siteGuid, Guid? siteDelGuid, Guid? locationGuid, int jobTypeID);

        MultiBrDetailView GetAdhocMultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid, Guid SystemServiceJobType_Guid);
        MultiBrDetailView GetAdhocAllCustomerBySite(Guid Site_Guid, Guid? OriginSite_Guid);
        MultiBrDetailView GetAdhocAllLocationByCustomer(Guid Site_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid);

        MultiBrDetailView GetMultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid);
        MultiBrDetailView GetBCDMutiBrPickupDetailByDestinationLocation(Guid DestLocation_Guid, Guid DestSite_Guid);
        MultiBrDetailView GetMasterRouteAllCustomerBySite(Guid Site_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid, Guid? OriginSite_Guid, string strJobActionAbb);
        MultiBrDetailView GetMasterRouteAllLocationByCustomer(Guid Site_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid, string strJobActionAbb);
        Guid? GetCustomerContractGuid(Guid? Customer, Guid? LocationGuid, DateTime? WorkDate);
        bool GetMasterRouteFlagJobWithoutContract(Guid? Site_Guid);
    }

    public class MasterCustomerLocationLocationDestinationRepository : Repository<OceanDbEntities, TblMasterCustomerLocation_LocationDestination>, IMasterCustomerLocationLocationDestinationRepository
    {
        public MasterCustomerLocationLocationDestinationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<AdhocDestinationResult> Func_AdhocDestination(Guid siteGuid, Guid? siteDelGuid, Guid? locationGuid, int jobTypeID)
        {
            return DbContext.Up_OceanOnlineMVC_Adhoc_Destination_Get(siteGuid, locationGuid, siteDelGuid, jobTypeID);
        }

        #region ### Adhoc : No service hour and contract

        #region -Has predefined from stardard table master data
        public MultiBrDetailView GetAdhocMultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid, Guid SystemServiceJobType_Guid)
        {

            MultiBrDetailView result = new MultiBrDetailView();
            //Check Service Job Type
            var jobtype = DbContext.TblSystemServiceJobType.FirstOrDefault(o => o.Guid == SystemServiceJobType_Guid)?.ServiceJobTypeID;
            switch (jobtype)
            {
                case IntTypeJob.TV_MultiBr:
                    result = GetAdhocTVmultiBrDeliveryDetailByOriginLocation(OriginCustomerLocation_Guid, OriginSite_Guid);
                    break;
                case IntTypeJob.P_MultiBr:
                    break;
                case IntTypeJob.BCD_MultiBr:
                    break;
            }


            return result;
        }

        #endregion

        #region -No predefined
        /// <summary>
        /// No MasterCustomer_Guid --> Get Only Customer 
        /// </summary>
        /// <returns></returns>
        public MultiBrDetailView GetAdhocAllCustomerBySite(Guid Site_Guid, Guid? OriginSite_Guid)
        {
            return GetAdhocAllCustomerAndLocation(Site_Guid, null, OriginSite_Guid);
        }

        /// <summary>
        /// Has MasterCustomer_Guid --> Get Only Location 
        /// </summary>
        /// <returns></returns>
        public MultiBrDetailView GetAdhocAllLocationByCustomer(Guid Site_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid)
        {
            return GetAdhocAllCustomerAndLocation(Site_Guid, MasterCustomer_Guid, OriginSite_Guid);
        }
        #endregion

        #endregion

        #region ### Master Route : Has service hour and contract

        #region -Has predefined from stardard table master data
        public MultiBrDetailView GetMultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid)
        {
            MultiBrDetailView result = new MultiBrDetailView();
            //Check Service Job Type
            var jobtype = DbContext.TblSystemServiceJobType.FirstOrDefault(o => o.Guid == SystemServiceJobType_Guid)?.ServiceJobTypeID;
            switch (jobtype)
            {
                case IntTypeJob.TV_MultiBr:
                    result = GetTVmultiBrDeliveryDetailByOriginLocation(OriginCustomerLocation_Guid, OriginSite_Guid, SystemSubServiceType_Guid, SystemServiceJobType_Guid, MasterLineOfBusiness_Guid, SystemDayOfWeek_Guid, WorkDate, FlagShowAll, MasterRoute_Guid);
                    break;
                case IntTypeJob.P_MultiBr:
                    result = GetPmultiBrDeliveryDetailByOriginLocation(OriginCustomerLocation_Guid, OriginSite_Guid);
                    break;
                case IntTypeJob.BCD_MultiBr:
                    break;
            }


            return result;
        }

        public MultiBrDetailView GetBCDMutiBrPickupDetailByDestinationLocation(Guid DestLocation_Guid, Guid DestSite_Guid)
        {
            MultiBrDetailView result = new MultiBrDetailView();

            var allDest = (from bcdDest in DbContext.TblMasterSitePathDestination
                           join siteHeader in DbContext.TblMasterSitePathHeader on bcdDest.MasterSitePathHeader_Guid equals siteHeader.Guid
                           join destSite in DbContext.TblMasterSitePathDetail on
                            //Multi condition join
                            new { MasterSitePathHeader_Guid = siteHeader.Guid, MasterSite_Guid = DestSite_Guid, IsDestination = true } equals
                            new { MasterSitePathHeader_Guid = destSite.MasterSitePathHeader_Guid, MasterSite_Guid = destSite.MasterSite_Guid, IsDestination = destSite.FlagDestination }

                           join originSite in DbContext.TblMasterSitePathDetail on destSite.MasterSitePathHeader_Guid equals originSite.MasterSitePathHeader_Guid
                           join site in DbContext.TblMasterSite on originSite.MasterSite_Guid equals site.Guid
                           where bcdDest.MasterCustomerLocation_Guid == DestLocation_Guid
                                  && !siteHeader.FlagDisble
                                  && originSite.SequenceIndex == 1
                           select new
                           {
                               sites = new DropdownListSiteView
                               {
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               },
                               sitepath = new DropdownSitePathView
                               {
                                   SiteGuid = site.Guid,
                                   LocationGuid = bcdDest.MasterCustomerLocation_Guid,
                                   SitePath_Guid = bcdDest.MasterSitePathHeader_Guid,
                                   SitePathName = bcdDest.TblMasterSitePathHeader.SitePathName
                               }
                           }).Distinct();
            //SET site
            result.SiteList = allDest.Select(o => o.sites)
                                .GroupBy(g => new { g.SiteGuid })
                                .Select(g => g.FirstOrDefault());

            if (result.SiteList.Count() != 1)
            {
                result.SiteList = new List<DropdownListSiteView>();
            }

            //SET site path
            result.SitePathList = allDest.Select(o => o.sitepath);

            return result;
        }
        #endregion

        #region -No predefined
        /// <summary>
        /// No MasterCustomer_Guid --> Get Only Customer 
        /// </summary>
        /// <returns></returns>
        public MultiBrDetailView GetMasterRouteAllCustomerBySite(Guid Site_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid, Guid? OriginSite_Guid, string strJobActionAbb)
        {

            return GetMasterRouteAllCustomerAndLocation(Site_Guid, SystemSubServiceType_Guid, SystemServiceJobType_Guid, MasterLineOfBusiness_Guid, SystemDayOfWeek_Guid, WorkDate, FlagShowAll, MasterRoute_Guid, null, OriginSite_Guid, strJobActionAbb);
        }

        /// <summary>
        /// Has MasterCustomer_Guid --> Get Only Location 
        /// </summary>
        /// <returns></returns>
        public MultiBrDetailView GetMasterRouteAllLocationByCustomer(Guid Site_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid, string strJobActionAbb)
        {
            return GetMasterRouteAllCustomerAndLocation(Site_Guid, SystemSubServiceType_Guid, SystemServiceJobType_Guid, MasterLineOfBusiness_Guid, SystemDayOfWeek_Guid, WorkDate, FlagShowAll, MasterRoute_Guid, MasterCustomer_Guid, OriginSite_Guid, strJobActionAbb);
        }

        #endregion

        #endregion

        #region *** Shared function

        #region ## Master Route
        public Guid? GetCustomerContractGuid(Guid? Customer, Guid? LocationGuid, DateTime? WorkDate)
        {
            var contractGuid = (from cont in DbContext.TblMasterCustomerContract
                                join cont_sv in DbContext.TblMasterCustomerContract_ServiceLocation on cont.Guid equals cont_sv.MasterCustomerContract_Guid
                                where cont.MasterCustomer_Guid == Customer
                                && (LocationGuid == null || cont_sv.MasterCustomerLocation_Guid == LocationGuid)
                                && (cont.ContractExpiredDate >= WorkDate || cont.ContractExpiredDate == null)
                                && !cont.FlagDisable
                                select cont.Guid)?.FirstOrDefault();

            return contractGuid == Guid.Empty ? null : contractGuid;
        }

        public bool GetMasterRouteFlagJobWithoutContract(Guid? Site_Guid)
        {

            var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid;
            return DbContext.Up_OceanOnlineMVC_CountryOption_Get(EnumAppKey.FlagAllowCreateJobWithoutContract, Site_Guid, MasterCountry_Guid)
                                            .FirstOrDefault().AppValue1.ToLower() == "true";
        }

        private MultiBrDetailView GetMasterRouteAllCustomerAndLocation(Guid Site_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid, string strJobActionAbb)
        {
            #region #### Condition
            bool FlagGetOnlyCustomer = MasterCustomer_Guid == null;
            #endregion

            #region #### Variable
            MultiBrDetailView result = new MultiBrDetailView();
            var allowJobType = new int[] { IntTypeJob.P_MultiBr, IntTypeJob.BCD_MultiBr, IntTypeJob.TV_MultiBr };
            var locType = new int[] { 1, 2, 3 };
            bool FlagJobWithoutContract = GetMasterRouteFlagJobWithoutContract(Site_Guid);
            Guid? MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid;
            Guid? Action_Guid = DbContext.TblSystemJobAction.FirstOrDefault(e => e.ActionNameAbbrevaition == strJobActionAbb)?.Guid;

            #endregion

            #region #### LINQ MAIN QUERY
            //Check Service Job Type
            var jobtype = DbContext.TblSystemServiceJobType.FirstOrDefault(o => o.Guid == SystemServiceJobType_Guid)?.ServiceJobTypeID;
            if (!allowJobType.Any(o => o == jobtype))
                return result;

            //Destination is Customer
            var allDest = (
                           from loc in DbContext.TblMasterCustomerLocation
                           join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                           join loc_brinks in DbContext.TblMasterCustomerLocation_BrinksSite on loc.Guid equals loc_brinks.MasterCustomerLocation_Guid
                           join site in DbContext.TblMasterSite on loc_brinks.MasterSite_Guid equals site.Guid
                           join sv_hour in DbContext.TblMasterCustomerLocation_ServiceHour on loc.Guid equals sv_hour.MasterCustomerLocation_Guid
                           where
                                 //::advance condition
                                 (//without Contract
                                 loc_brinks.MasterSite_Guid == Site_Guid && (MasterCustomer_Guid == null || cust.Guid == MasterCustomer_Guid)
                                 ) && (FlagJobWithoutContract ||
                                 (//with Contract
                                  from cont in DbContext.TblMasterCustomerContract
                                  join cont_sv in DbContext.TblMasterCustomerContract_ServiceLocation on cont.Guid equals cont_sv.MasterCustomerContract_Guid
                                  where
                                  //check location 
                                  cont_sv.MasterCustomerLocation_Guid == loc.Guid
                                  //check country
                                  && cust.MasterCountry_Guid == MasterCountry_Guid
                                  //check lob,sv_type,subsv_type
                                  && cont_sv.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                  && cont_sv.SystemLineOfBusiness_Guid == MasterLineOfBusiness_Guid
                                  && (SystemSubServiceType_Guid == null || cont_sv.SystemSubServiceType_Guid == SystemSubServiceType_Guid)
                                  // check contract expired
                                  && (cont.ContractExpiredDate >= WorkDate || cont.ContractExpiredDate == null)
                                  && !cont.FlagDisable
                                  select cont.Guid).Any())
                                 //::basic condition
                                 && (OriginSite_Guid == null || loc_brinks.MasterSite_Guid != OriginSite_Guid)
                                 && !loc.FlagDisable
                                 && cust.FlagDisable == false
                                 && cust.FlagChkCustomer == true
                                 && loc_brinks.FlagDefaultBrinksSite
                                 // Service Hour
                                 && sv_hour.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                 && sv_hour.MasterLineOfBusiness_Guid == MasterLineOfBusiness_Guid
                                 && sv_hour.SystemDayOfWeek_Guid == SystemDayOfWeek_Guid
                                 //Location Type
                                 && locType.Any(o => DbContext.TblSystemCustomerLocationType.Any(c => c.Guid == loc.SystemCustomerLocationType_Guid && o == c.CustomerLocationTypeID))
                                 //Show All Location
                                 && (FlagShowAll || !(from head in DbContext.TblMasterRouteJobHeader
                                                      join leg in DbContext.TblMasterRouteJobServiceStopLegs on head.Guid equals leg.MasterRouteJobHeader_Guid
                                                      where head.MasterRoute_Guid == MasterRoute_Guid
                                                          && head.FlagDisable == false
                                                          && head.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                                          && leg.CustomerLocationAction_Guid == Action_Guid
                                                          && leg.MasterCustomerLocation_Guid == loc.Guid
                                                      select leg.MasterCustomerLocation_Guid).Any())
                           select new
                           {
                               SiteGuid = site.Guid,
                               SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               CustomerGuid = cust.Guid,
                               CustomerName = cust.CustomerFullName,
                               LocationGuid = loc.Guid,
                               LocationName = loc.BranchName,
                               FlagNonBillable = loc.FlagNonBillable
                           }).Distinct();

            #endregion

            #region #### SET Customer or Location
            if (FlagGetOnlyCustomer)
            {
                //SET customer
                result.CustomerList = allDest.GroupBy(g => new { g.CustomerGuid })
                                             .Select(g => g.FirstOrDefault())
                                             .Select(o => new DropdownCustomerView
                                             {
                                                 CustomerGuid = o.CustomerGuid,
                                                 CustomerName = o.CustomerName
                                             }).OrderBy(o=>o.CustomerName);
            }
            else
            {
                //SET location
                result.LocationList = allDest.GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid })
                                             .Select(g => g.FirstOrDefault())
                                             .Select(o => new DropdownLocationView
                                             {
                                                 SiteGuid = o.SiteGuid,
                                                 SiteName = o.SiteName,
                                                 CustomerGuid = o.CustomerGuid,
                                                 CustomerName = o.CustomerName,
                                                 LocationGuid = o.LocationGuid,
                                                 LocationName = o.LocationName,
                                                 FlagNonBillable = o.FlagNonBillable
                                             }).OrderBy(o=>o.LocationName);
            }

            #endregion

            return result;
        }

        private MultiBrDetailView GetTVmultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid, Guid? SystemSubServiceType_Guid, Guid SystemServiceJobType_Guid, Guid MasterLineOfBusiness_Guid, Guid SystemDayOfWeek_Guid, DateTime? WorkDate, bool FlagShowAll, Guid? MasterRoute_Guid)
        {

            MultiBrDetailView result = new MultiBrDetailView();
            var locType = new int[] { 1, 2, 3 };
            bool FlagJobWithoutContract = GetMasterRouteFlagJobWithoutContract(OriginSite_Guid);

            //Destination is Customer
            var allDest = (from locDest in DbContext.TblMasterCustomerLocation_LocationDestination
                           join loc in DbContext.TblMasterCustomerLocation on locDest.MasterCustomerLocationDes_Guid equals loc.Guid
                           join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                           join loc_brinks in DbContext.TblMasterCustomerLocation_BrinksSite on locDest.MasterCustomerLocationDes_Guid equals loc_brinks.MasterCustomerLocation_Guid
                           join site in DbContext.TblMasterSite on loc_brinks.MasterSite_Guid equals site.Guid
                           join sv_hour in DbContext.TblMasterCustomerLocation_ServiceHour on locDest.MasterCustomerLocationDes_Guid equals sv_hour.MasterCustomerLocation_Guid
                           where locDest.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                                 && loc_brinks.MasterSite_Guid != OriginSite_Guid
                                 && !loc.FlagDisable
                                 && cust.FlagDisable == false
                                 && cust.FlagChkCustomer == true
                                 && locDest.MasterSitePathHeader_Guid != null
                                 && loc_brinks.FlagDefaultBrinksSite
                                 // Service Hour
                                 && sv_hour.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                 && sv_hour.MasterLineOfBusiness_Guid == MasterLineOfBusiness_Guid
                                 && sv_hour.SystemDayOfWeek_Guid == SystemDayOfWeek_Guid
                                 //Location Type
                                 && locType.Any(o => DbContext.TblSystemCustomerLocationType.Any(c => c.Guid == loc.SystemCustomerLocationType_Guid && o == c.CustomerLocationTypeID))
                                 //Without Contract
                                 && (FlagJobWithoutContract || (from cont in DbContext.TblMasterCustomerContract
                                                                join cont_sv in DbContext.TblMasterCustomerContract_ServiceLocation on cont.Guid equals cont_sv.MasterCustomerContract_Guid
                                                                where cont_sv.MasterCustomerLocation_Guid == locDest.MasterCustomerLocationDes_Guid
                                                                && (cont.ContractExpiredDate >= WorkDate || cont.ContractExpiredDate == null) && !cont.FlagDisable
                                                                && cont_sv.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                                                && cont_sv.SystemLineOfBusiness_Guid == MasterLineOfBusiness_Guid
                                                                && cont_sv.SystemSubServiceType_Guid == SystemSubServiceType_Guid
                                                                && !cont.FlagDisable
                                                                select cont.Guid).Any())

                                //Show All Location
                                && (FlagShowAll || !(from head in DbContext.TblMasterRouteJobHeader
                                                     join leg in DbContext.TblMasterRouteJobServiceStopLegs on head.Guid equals leg.MasterRouteJobHeader_Guid
                                                     where head.MasterRoute_Guid == MasterRoute_Guid
                                                         && head.FlagDisable == false
                                                         && head.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                                                         && leg.FlagDestination
                                                         && leg.MasterCustomerLocation_Guid == locDest.MasterCustomerLocationDes_Guid
                                                     select leg.MasterCustomerLocation_Guid).Any())
                           select new
                           {
                               sites = new DropdownListSiteView
                               {
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               },
                               customers = new DropdownCustomerView
                               {
                                   SiteGuid = site.Guid,
                                   CustomerGuid = cust.Guid,
                                   CustomerName = cust.CustomerFullName
                               },
                               loactions = new DropdownLocationView
                               {
                                   SiteGuid = site.Guid,
                                   CustomerGuid = cust.Guid,
                                   LocationGuid = loc.Guid,
                                   LocationName = loc.BranchName,
                                   FlagNonBillable = loc.FlagNonBillable
                               },
                               sitepath = new DropdownSitePathView
                               {
                                   SiteGuid = site.Guid,
                                   CustomerGuid = cust.Guid,
                                   LocationGuid = loc.Guid,
                                   SitePath_Guid = locDest.MasterSitePathHeader_Guid,
                                   SitePathName = locDest.TblMasterSitePathHeader.SitePathName

                               }
                           }).Distinct();

            //SET site
            result.SiteList = allDest.Select(o => o.sites)
                                .GroupBy(g => new { g.SiteGuid })
                                .Select(g => g.FirstOrDefault());

            if (result.SiteList.Count() != 1)
            {
                result.SiteList = new List<DropdownListSiteView>();
            }

            //SET customer
            result.CustomerList = allDest.Select(o => o.customers)
                                         .GroupBy(g => new { g.SiteGuid, g.CustomerGuid })
                                         .Select(g => g.FirstOrDefault());
            //SET location
            result.LocationList = allDest.Select(o => o.loactions)
                                         .GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid })
                                         .Select(g => g.FirstOrDefault());

            //SET site path
            result.SitePathList = allDest.Select(o => o.sitepath)
                                .GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid, g.SitePath_Guid })
                                .Select(g => g.FirstOrDefault());

            return result;
        }

        private MultiBrDetailView GetPmultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid)
        {

            MultiBrDetailView result = new MultiBrDetailView();
            var locType = new int[] { 1, 2, 3 };
            //var Origin_Customer = DbContext.TblMasterCustomerLocation.FirstOrDefault(o => o.Guid == OriginCustomerLocation_Guid)?.MasterCustomer_Guid
            //bool FlagJobWithoutContract = GetMasterRouteFlagJobWithoutContract(OriginSite_Guid)

            //Destination is Customer
            var allDest = (from locDest in DbContext.TblMasterCustomerLocation_LocationDestination
                           join intDep in DbContext.TblMasterCustomerLocation_InternalDepartment on locDest.MasterCustomerLocation_InternalDepartment_Guid equals intDep.Guid
                           join loc in DbContext.TblMasterCustomerLocation on intDep.MasterCustomerLocation_Guid equals loc.Guid
                           join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                           join site in DbContext.TblMasterSite on loc.MasterSite_Guid equals site.Guid
                           where locDest.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                                 && loc.MasterSite_Guid != OriginSite_Guid
                                 && !loc.FlagDisable
                                 && cust.FlagDisable == false
                                 && cust.FlagChkCustomer == false
                                 && locDest.MasterSitePathHeader_Guid != null
                                 //Location Type
                                 && locType.Any(o => DbContext.TblSystemCustomerLocationType.Any(c => c.Guid == loc.SystemCustomerLocationType_Guid && o == c.CustomerLocationTypeID))
                           select new
                           {
                               sites = new DropdownListSiteView
                               {
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               },
                               internalDepartment = new InternalDepartmentView
                               {
                                   OnwardDestinationTypeID = 1,
                                   SiteGuid = site.Guid,
                                   InternalDepartment_Guid = intDep.Guid,
                                   InternalDepartmentName = intDep.InterDepartmentName
                               },
                               sitepath = new DropdownSitePathView
                               {
                                   SiteGuid = site.Guid,
                                   InternalDepartment_Guid = locDest.MasterCustomerLocation_InternalDepartment_Guid,
                                   SitePath_Guid = locDest.MasterSitePathHeader_Guid,
                                   SitePathName = locDest.TblMasterSitePathHeader.SitePathName
                               }
                           }).Distinct();

            //SET site
            result.SiteList = allDest.Select(o => o.sites)
                                            .GroupBy(g => new { g.SiteGuid })
                                            .Select(g => g.FirstOrDefault());

            if (result.SiteList.Count() != 1)
            {
                result.SiteList = new List<DropdownListSiteView>();
            }

            //SET internal department
            result.InternalDepartmentList = allDest.Select(o => o.internalDepartment)
                                         .GroupBy(g => new { g.SiteGuid, g.InternalDepartment_Guid })
                                         .Select(g => g.FirstOrDefault());

            //SET site path
            result.SitePathList = allDest.Select(o => o.sitepath)
                                .GroupBy(g => new { g.SiteGuid, g.InternalDepartment_Guid, g.SitePath_Guid })
                                .Select(g => g.FirstOrDefault());

            return result;
        }

        private MultiBrDetailView GetAdhocAllCustomerAndLocation(Guid Site_Guid, Guid? MasterCustomer_Guid, Guid? OriginSite_Guid)
        {
            #region #### Condition
            bool FlagGetAllCustomer = MasterCustomer_Guid == null;
            #endregion

            #region #### Variable
            MultiBrDetailView result = new MultiBrDetailView();
            //var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid
            #endregion

            #region #### LINQ MAIN QUERY

            //Destination is Customer
            //** ignore OriginSite_Guid
            var allDest = (
                           from loc in DbContext.TblMasterCustomerLocation
                           join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                           join loc_brinks in DbContext.TblMasterCustomerLocation_BrinksSite on loc.Guid equals loc_brinks.MasterCustomerLocation_Guid
                           join site in DbContext.TblMasterSite on loc_brinks.MasterSite_Guid equals site.Guid
                           where loc_brinks.MasterSite_Guid == Site_Guid
                                 && (OriginSite_Guid == null || (loc_brinks.MasterSite_Guid != OriginSite_Guid))
                                 && !loc.FlagDisable
                                 && cust.FlagDisable == false
                                 && cust.FlagChkCustomer == true
                                 && loc_brinks.FlagDefaultBrinksSite
                                 // By Customer
                                 && (MasterCustomer_Guid == null || (cust.Guid == MasterCustomer_Guid))
                           select new
                           {
                               SiteGuid = site.Guid,
                               SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               CustomerGuid = cust.Guid,
                               CustomerName = cust.CustomerFullName,
                               LocationGuid = loc.Guid,
                               LocationName = loc.BranchName,
                               FlagNonBillable = loc.FlagNonBillable
                           }).Distinct();

            #endregion

            #region #### SET Customer or Location
            if (FlagGetAllCustomer)
            {
                //SET customer
                result.CustomerList = allDest.GroupBy(g => new { g.SiteGuid, g.CustomerGuid })
                                             .Select(g => g.FirstOrDefault())
                                             .Select(o => new DropdownCustomerView
                                             {
                                                 CustomerGuid = o.CustomerGuid,
                                                 CustomerName = o.CustomerName
                                             });
            }
            else
            {
                //SET location
                result.LocationList = allDest.GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid })
                                             .Select(g => g.FirstOrDefault())
                                             .Select(o => new DropdownLocationView
                                             {
                                                 SiteGuid = o.SiteGuid,
                                                 SiteName = o.SiteName,
                                                 CustomerGuid = o.CustomerGuid,
                                                 CustomerName = o.CustomerName,
                                                 LocationGuid = o.LocationGuid,
                                                 LocationName = o.LocationName,
                                                 FlagNonBillable = o.FlagNonBillable
                                             });
            }

            #endregion

            return result;
        }
        #endregion

        #region ## Adhoc
        private MultiBrDetailView GetAdhocTVmultiBrDeliveryDetailByOriginLocation(Guid OriginCustomerLocation_Guid, Guid OriginSite_Guid)
        {

            MultiBrDetailView result = new MultiBrDetailView();
            //var allowJobType = new int[] { IntTypeJob.TV_MultiBr } //
            //var locType = new int[] { 1, 2, 3 } //
            //var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == OriginSite_Guid)?.MasterCountry_Guid

            //Destination is Customer
            var allDest = (from locDest in DbContext.TblMasterCustomerLocation_LocationDestination
                           join loc in DbContext.TblMasterCustomerLocation on locDest.MasterCustomerLocationDes_Guid equals loc.Guid
                           join cust in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cust.Guid
                           join loc_brinks in DbContext.TblMasterCustomerLocation_BrinksSite on locDest.MasterCustomerLocationDes_Guid equals loc_brinks.MasterCustomerLocation_Guid
                           join site in DbContext.TblMasterSite on loc_brinks.MasterSite_Guid equals site.Guid
                           where locDest.MasterCustomerLocation_Guid == OriginCustomerLocation_Guid
                                 && loc_brinks.MasterSite_Guid != OriginSite_Guid
                                 && !loc.FlagDisable
                                 && cust.FlagDisable == false
                                 && cust.FlagChkCustomer == true
                                 && loc_brinks.FlagDefaultBrinksSite
                           select new
                           {
                               sites = new DropdownListSiteView
                               {
                                   CompanyGuid = site.MasterCustomer_Guid,
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                               },
                               customers = new DropdownCustomerView
                               {
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                                   CustomerGuid = cust.Guid,
                                   CustomerName = cust.CustomerFullName
                               },
                               loactions = new DropdownLocationView
                               {
                                   SiteGuid = site.Guid,
                                   SiteName = (site.SiteCode ?? "") + " - " + (site.SiteName ?? ""),
                                   CustomerGuid = cust.Guid,
                                   CustomerName = cust.CustomerFullName,
                                   LocationGuid = loc.Guid,
                                   LocationName = loc.BranchName,
                                   FlagNonBillable = loc.FlagNonBillable
                               },
                               sitepath = new DropdownSitePathView
                               {
                                   SiteGuid = site.Guid,
                                   CustomerGuid = cust.Guid,
                                   LocationGuid = loc.Guid,
                                   SitePath_Guid = locDest.MasterSitePathHeader_Guid,
                                   SitePathName = locDest.TblMasterSitePathHeader.SitePathName

                               }
                           }).Distinct();

            //SET site
            result.SiteList = allDest.Select(o => o.sites)
                                .GroupBy(g => new { g.SiteGuid })
                                .Select(g => g.FirstOrDefault());

            if (result.SiteList.Count() != 1)
            {
                result.SiteList = new List<DropdownListSiteView>();
            }

            //SET customer
            result.CustomerList = allDest.Select(o => o.customers)
                                         .GroupBy(g => new { g.SiteGuid, g.CustomerGuid })
                                         .Select(g => g.FirstOrDefault());
            //SET location
            result.LocationList = allDest.Select(o => o.loactions)
                                         .GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid })
                                         .Select(g => g.FirstOrDefault());

            //SET site path
            result.SitePathList = allDest.Select(o => o.sitepath)
                                .GroupBy(g => new { g.SiteGuid, g.CustomerGuid, g.LocationGuid, g.SitePath_Guid })
                                .Select(g => g.FirstOrDefault());

            return result;
        }

        #endregion

        #endregion
    }

}





