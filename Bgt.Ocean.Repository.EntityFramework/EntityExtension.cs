using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework
{
    public static class EntityExtension
    {
        public static IEnumerable<CommodityView> GetAllCommodityBySite(this OceanDbEntities dbContext, Guid? siteGuid, Guid? countryGuid = null, bool flagIncludeDisable = false)
        {
            countryGuid = countryGuid == null || countryGuid == Guid.Empty ? dbContext.TblMasterSite.Where(s => s.Guid == siteGuid).FirstOrDefault()?.MasterCountry_Guid : countryGuid;

            var global = from commodity in dbContext.TblMasterCommodity
                         join ccgroup in dbContext.TblMasterCommodityGroup on commodity.MasterCommodityGroup_Guid equals ccgroup.Guid
                         where commodity.FlagCommodityGlobal &&
                               ((!ccgroup.FlagDisable && !commodity.FlagDisable && !flagIncludeDisable) ||
                               (flagIncludeDisable))
                         select new CommodityView
                         {
                             MasterCommodity_Guid = commodity.Guid,
                             CCGuid = commodity.Guid,
                             MasterCommodityGroup_Guid = ccgroup.Guid,
                             CommodityGroupName = ccgroup.CommodityGroupName,
                             CommodityName = commodity.CommodityName,
                             DenoText = string.IsNullOrEmpty(commodity.DenoText) ? commodity.CommodityName : commodity.DenoText,
                             ColumnInReport = commodity.ColumnInReport ?? string.Empty,
                             CommodityValue = commodity.CommodityValue ?? 0,
                             CommodityAmount = commodity.CommodityAmount ?? 0,
                             CommodityCode = string.IsNullOrEmpty(commodity.CommodityCode) ? commodity.CommodityName : commodity.CommodityCode,
                             FlagCommodityGlobal = commodity.FlagCommodityGlobal,
                             FlagReqDetail = ccgroup.FlagReqDetail,
                             FlagRequireSeal = commodity.FlagRequireSeal
                         };

            var local = from commodity in dbContext.TblMasterCommodity
                        join ccgroup in dbContext.TblMasterCommodityGroup on commodity.MasterCommodityGroup_Guid equals ccgroup.Guid
                        join cccountry in dbContext.TblMasterCommodityCountry on commodity.Guid equals cccountry.MasterCommodity_Guid
                        where !commodity.FlagCommodityGlobal &&
                              cccountry.MasterCountry_Guid == countryGuid &&
                               ((!ccgroup.FlagDisable && !commodity.FlagDisable && !flagIncludeDisable) ||
                               (flagIncludeDisable))
                        select new CommodityView
                        {
                            MasterCommodity_Guid = commodity.Guid,
                            CCGuid = commodity.Guid,
                            MasterCommodityGroup_Guid = ccgroup.Guid,
                            CommodityGroupName = ccgroup.CommodityGroupName,
                            CommodityName = string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal,
                            DenoText = string.IsNullOrEmpty(cccountry.DenoText) ?
                                       string.IsNullOrEmpty(commodity.DenoText) ?
                                       string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal : commodity.DenoText : cccountry.DenoText,
                            ColumnInReport = string.IsNullOrEmpty(cccountry.ColumnInReport) ? commodity.ColumnInReport : cccountry.ColumnInReport,
                            CommodityValue = (cccountry.CommodityValue ?? commodity.CommodityValue) ?? 0,
                            CommodityAmount = (cccountry.CommodityAmount ?? commodity.CommodityAmount) ?? 0,
                            CommodityCode = string.IsNullOrEmpty(cccountry.CommodityCode) ?
                                            string.IsNullOrEmpty(commodity.CommodityCode) ?
                                            string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal : commodity.CommodityCode : cccountry.CommodityCode,
                            FlagCommodityGlobal = commodity.FlagCommodityGlobal,
                            FlagReqDetail = ccgroup.FlagReqDetail,
                            FlagRequireSeal = commodity.FlagRequireSeal
                        };


            return global.Union(local).OrderBy(o => o.CommodityGroupName.Contains("CX") ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.CommodityName).ToList();
        }


        public static IEnumerable<RoadNetCommodityView> GetRoadNetAllCommodityBySite(this OceanDbEntities dbContext, Guid? siteGuid, Guid? countryGuid = null)
        {
            countryGuid = countryGuid == null || countryGuid == Guid.Empty ? dbContext.TblMasterSite.Where(s => s.Guid == siteGuid).FirstOrDefault()?.MasterCountry_Guid : countryGuid;

            var global = from commodity in dbContext.TblMasterCommodity
                         join ccgroup in dbContext.TblMasterCommodityGroup on commodity.MasterCommodityGroup_Guid equals ccgroup.Guid
                         where commodity.FlagCommodityGlobal &&
                               !ccgroup.FlagDisable &&
                               !commodity.FlagDisable
                         select new RoadNetCommodityView
                         {
                             MasterCommodity_Guid = commodity.Guid,
                             CCGuid = commodity.Guid,
                             MasterCommodityGroup_Guid = ccgroup.Guid,
                             CommodityGroupName = ccgroup.CommodityGroupName,
                             CommodityName = commodity.CommodityName,
                             DenoText = string.IsNullOrEmpty(commodity.DenoText) ? commodity.CommodityName : commodity.DenoText,
                             ColumnInReport = commodity.ColumnInReport ?? string.Empty,
                             CommodityValue = commodity.CommodityValue ?? 0,
                             CommodityAmount = commodity.CommodityAmount ?? 0,
                             CommodityCode = string.IsNullOrEmpty(commodity.CommodityCode) ? commodity.CommodityName : commodity.CommodityCode,
                             FlagCommodityGlobal = commodity.FlagCommodityGlobal,
                             FlagReqDetail = ccgroup.FlagReqDetail,
                             FlagRequireSeal = commodity.FlagRequireSeal,
                             SiteGuid = siteGuid,
                             CountryGuid = countryGuid
                         };

            var local = from commodity in dbContext.TblMasterCommodity
                        join ccgroup in dbContext.TblMasterCommodityGroup on commodity.MasterCommodityGroup_Guid equals ccgroup.Guid
                        join cccountry in dbContext.TblMasterCommodityCountry on commodity.Guid equals cccountry.MasterCommodity_Guid
                        where !commodity.FlagCommodityGlobal &&
                              cccountry.MasterCountry_Guid == countryGuid &&
                              !ccgroup.FlagDisable &&
                              !commodity.FlagDisable
                        select new RoadNetCommodityView
                        {
                            MasterCommodity_Guid = commodity.Guid,
                            CCGuid = commodity.Guid,
                            MasterCommodityGroup_Guid = ccgroup.Guid,
                            CommodityGroupName = ccgroup.CommodityGroupName,
                            CommodityName = string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal,
                            DenoText = string.IsNullOrEmpty(cccountry.DenoText) ?
                                       string.IsNullOrEmpty(commodity.DenoText) ?
                                       string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal : commodity.DenoText : cccountry.DenoText,
                            ColumnInReport = string.IsNullOrEmpty(cccountry.ColumnInReport) ? commodity.ColumnInReport : cccountry.ColumnInReport,
                            CommodityValue = (cccountry.CommodityValue ?? commodity.CommodityValue) ?? 0,
                            CommodityAmount = (cccountry.CommodityAmount ?? commodity.CommodityAmount) ?? 0,
                            CommodityCode = string.IsNullOrEmpty(cccountry.CommodityCode) ?
                                            string.IsNullOrEmpty(commodity.CommodityCode) ?
                                            string.IsNullOrEmpty(cccountry.CommodityNameLocal) ? commodity.CommodityName : cccountry.CommodityNameLocal : commodity.CommodityCode : cccountry.CommodityCode,
                            FlagCommodityGlobal = commodity.FlagCommodityGlobal,
                            FlagReqDetail = ccgroup.FlagReqDetail,
                            FlagRequireSeal = commodity.FlagRequireSeal,
                            SiteGuid = siteGuid,
                            CountryGuid = countryGuid
                        };


            return global.Union(local).OrderBy(o => o.CommodityGroupName.Contains("CX") ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.CommodityName).ToList();
        }
        public static IQueryable<SealBagView> GetSealBagView(this IQueryable<TblMasterActualJobMCSItemSeal> source, Guid? jobGuid, SealTypeID sealType, Capability cpability)
        {

            return source.Where(o => o.MasterActualJobHeader_Guid == jobGuid
                                                                 && o.TblSystemSealType.SealTypeID == (int)sealType
                                                                 && o.SFOTblSystemMachineCapability.CapabilityID == (int)cpability)
                                                        .Select(o => new SealBagView { SealNo = o.SealNo });
        }




        public static IQueryable<TblMasterActualJobServiceStopLegs> TblMasterActualJobServiceStopLegs_ExCustomer(this OceanDbEntities dbContext, IEnumerable<Guid> jobGuids)
        {
            var query = from leg in dbContext.TblMasterActualJobServiceStopLegs
                        join loc in dbContext.TblMasterCustomerLocation on leg.MasterCustomerLocation_Guid equals loc.Guid
                        join cus in dbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                        where cus.FlagChkCustomer == true && jobGuids.Any(o => o == leg.MasterActualJobHeader_Guid)
                        select leg;
            return query;
        }

        public static IQueryable<TblMasterActualJobServiceStopLegs> TblMasterActualJobServiceStopLegs_ExBrinks(this OceanDbEntities dbContext, IEnumerable<Guid> jobGuids)
        {

            var query = from leg in dbContext.TblMasterActualJobServiceStopLegs
                        join loc in dbContext.TblMasterCustomerLocation on leg.MasterCustomerLocation_Guid equals loc.Guid
                        join cus in dbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                        where cus.FlagChkCustomer == false && jobGuids.Any(o => o == leg.MasterActualJobHeader_Guid)
                        select leg;
            return query;
        }
    }
}
