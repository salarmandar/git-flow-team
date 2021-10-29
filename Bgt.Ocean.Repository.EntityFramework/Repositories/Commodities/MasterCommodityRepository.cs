using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{

    public interface IMasterCommodityRepository : IRepository<TblMasterCommodity>
    {
        IEnumerable<CommodityView> GetCCBySite(Guid? siteGuid, bool onlyCX = false, bool onlyMC = false, bool onlyNonBarcodeAndGroupCX = false, bool onlyNonBarcode = false);
        IEnumerable<CommodityView> GetAllCommodityBySite(Guid? siteGuid, Guid? countryGuid = null, bool flagIncludeDisable = false);
    }

    public class MasterCommodityRepository : Repository<OceanDbEntities, TblMasterCommodity>, IMasterCommodityRepository
    {
        public MasterCommodityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        /// <summary>
        /// onlyCX : Only Canada Golden Rule
        /// onlyMC : Without Canada Golden Rule
        /// onlyNonBarcode : Not CX and Not Con
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="onlyCX"></param>
        /// <param name="onlyMC"></param>
        /// <param name="onlyNonBarcodeAndGroupCX"></param>
        /// <param name="onlyNonBarcode"></param>
        /// <returns></returns>
        public IEnumerable<CommodityView> GetCCBySite(Guid? siteGuid, bool onlyCX = false, bool onlyMC = false, bool onlyNonBarcodeAndGroupCX = false, bool onlyNonBarcode = false)
        {
            List<CommodityView> result = null;

            //CX
            if (onlyCX)
            {
                result = GetCommodityOnlyCXBySite(siteGuid).ToList();
            }
            //MC
            else if (onlyMC)
            {
                result = GetCommodityOnlyMCBySite(siteGuid).ToList();
            }
            //NonBarcode
            else if (onlyNonBarcode)
            {
                result = GetAllCommodityBySite(siteGuid).Where(c => !c.FlagReqDetail && c.FlagRequireSeal == false).ToList();
            }
            //CXGroup+NonBarcode
            else if (onlyNonBarcodeAndGroupCX)
            {
                result = GetAllCommodityBySite(siteGuid).Where(c => !c.FlagReqDetail && c.FlagRequireSeal == false).ToList();
                var cx = from ccgroup in DbContext.TblMasterCommodityGroup
                         where !ccgroup.FlagDisable && ccgroup.FlagReqDetail
                         select new CommodityView
                         {
                             MasterCommodityGroup_Guid = ccgroup.Guid,
                             CCGuid = ccgroup.Guid,
                             CommodityGroupName = ccgroup.CommodityGroupName,
                             CommodityCode = ccgroup.CommodityGroupName,
                             FlagReqDetail = ccgroup.FlagReqDetail,
                             CommodityName = ccgroup.CommodityGroupName //Wait requirement from BA,SA
                         };

                result.InsertRange(0, cx); //<-- CXGroup 
            }
            //CXGroup+MC+NonBarcode
            else
            {
                result = GetAllCommodityBySite(siteGuid).Where(c => !c.FlagReqDetail).ToList();
                var cx = from ccgroup in DbContext.TblMasterCommodityGroup
                         where !ccgroup.FlagDisable && ccgroup.FlagReqDetail
                         select new CommodityView
                         {
                             MasterCommodityGroup_Guid = ccgroup.Guid,
                             CCGuid = ccgroup.Guid,
                             CommodityGroupName = ccgroup.CommodityGroupName,
                             CommodityCode = ccgroup.CommodityGroupName,
                             FlagReqDetail = ccgroup.FlagReqDetail,
                         };

                result.InsertRange(0, cx);  //<-- CXGroup 
            }

            return result;
        }

        public IEnumerable<CommodityView> GetCommodityOnlyCXBySite(Guid? siteGuid)
        {
            var result = GetAllCommodityBySite(siteGuid);
            return result.Where(o => o.FlagRequireSeal == false && o.FlagReqDetail);
        }

        public IEnumerable<CommodityView> GetCommodityOnlyMCBySite(Guid? siteGuid)
        {
            var result = GetAllCommodityBySite(siteGuid);
            return result.Where(o => o.FlagRequireSeal == true && !o.FlagReqDetail);
        }


        public IEnumerable<CommodityView> GetAllCommodityBySite(Guid? siteGuid, Guid? countryGuid = null, bool flagIncludeDisable = false)
        {
            return DbContext.GetAllCommodityBySite(siteGuid, countryGuid, flagIncludeDisable);
        }


    }


}
