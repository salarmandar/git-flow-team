using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.ModelViews.SitePath;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_SitePathController : ApiControllerBase
    {
        private readonly ISitePathService _sitePathService;

        public v1_SitePathController(ISitePathService sitePathService)
        {
            _sitePathService = sitePathService;
        }

        /// <summary>
        /// Get Brink's Site Path List by Country
        /// </summary>
        /// <param name="MasterCountry_Guid"></param>
        /// <param name="FlagDisable"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SitePathViewResponse> GetSitePathListInfo(Guid MasterCountry_Guid, bool FlagDisable)
        {
            return _sitePathService.GetSitePathListInfo(MasterCountry_Guid, FlagDisable);
        }

        /// <summary>
        /// Get Brink's Site Path by Guid
        /// </summary>
        /// <param name="SitePathGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public SitePathViewResponse GetSitePathInfo(Guid SitePathGuid)
        {
            return _sitePathService.GetSitePathInfo(SitePathGuid);
        }

        /// <summary>
        /// Update or Create Brink's Site Path
        /// </summary>
        /// <param name="SitePath"></param>
        /// <returns></returns>
        [HttpPost]
        public SitePathViewResponse CreateUpdateSitePath(SitePathViewRequest SitePath)
        {
            return _sitePathService.CreateUpdateSitePath(SitePath);
        }

        /// <summary>
        /// Disable or Enable Brink's Site Path
        /// </summary>
        /// <param name="SitePath"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView DisableOrEnablePath(SitePathViewRequest SitePath)
        {
            return _sitePathService.DisableOrEnablePath(SitePath);
        }

        /// <summary>
        /// Get Brink's Site Path Log By Guid
        /// </summary>
        /// <param name="SitePathGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SitePathAuditLogView> GetLogListInfoBySitePath(Guid SitePathGuid)
        {
            return _sitePathService.GetLogListInfoBySitePath(SitePathGuid);
        }

        [HttpGet]
        public IEnumerable<BrinksSiteBySitePath> getShortInfoSitePathDetail(Guid? SitePathHeader_Guid)
        {
            return _sitePathService.getShortInfoSitePathDetail(SitePathHeader_Guid);
        }


        [HttpPost]
        public IEnumerable<SitePathDropdownViewResponse> AllSitePathName(SitePathDropdownViewRequest model)
        {
            return _sitePathService.getAllSitePathName(model);
        }

        [HttpGet]
        public IEnumerable<SitePathDropdownViewResponse> GetSitePathList(int? jobType, Guid? pickupLocationGuid= null, Guid? deliveryLocationGuid =null, Guid? pickupSiteGuid = null, Guid? deliverySiteGuid= null)
        {
            SitePathDropdownViewRequest model = new SitePathDropdownViewRequest();
            model.pickupLocationGuid = pickupLocationGuid;
            model.deliveryLocationGuid = deliveryLocationGuid;
            model.pickupSiteGuid = pickupSiteGuid;
            model.deliverySiteGuid = deliverySiteGuid;
            model.jobType = jobType;
            return _sitePathService.getAllSitePathName(model);
        }
    }
}