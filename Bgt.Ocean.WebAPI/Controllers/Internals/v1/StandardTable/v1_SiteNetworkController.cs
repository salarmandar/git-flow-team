using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Bgt.Ocean.Service.ModelViews.Systems;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_SiteNetworkController : ApiControllerBase
    {
        private readonly ISiteNetworkService _siteNetworkService;
        private readonly ISiteNetworkAuditoLogService _siteNetworkAuditoLogService;
        public v1_SiteNetworkController(ISiteNetworkService siteNetworkService,
            ISiteNetworkAuditoLogService siteNetworkAuditoLogService)
        {
            _siteNetworkService = siteNetworkService;
            _siteNetworkAuditoLogService = siteNetworkAuditoLogService;
        }

        /// <summary>
        /// Get Site Network List by Country
        /// </summary>
        /// <param name="MasterCountry_Guid"></param>
        /// <param name="FlagDisable"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SiteNetworkViewResponse> GetSiteNetworkListInfo(Guid MasterCountry_Guid, bool FlagDisable)
        {
            return _siteNetworkService.GetSiteNetworkListInfo(MasterCountry_Guid, FlagDisable);
        }

        /// <summary>
        /// Get Site Network by Site Guid
        /// </summary>
        /// <param name="SiteNetworkGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public SiteNetworkViewResponse GetSiteNetworkInfo(Guid SiteNetworkGuid)
        {
            return _siteNetworkService.GetSiteNetworkInfo(SiteNetworkGuid);
        }

        /// <summary>
        /// Update or Create Site Network
        /// </summary>
        /// <param name="SiteNetwork"></param>
        /// <returns></returns>
        [HttpPost]
        public SiteNetworkViewResponse CreateUpdateSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            return _siteNetworkService.CreateUpdateSiteNetwork(SiteNetwork);
        }

        /// <summary>
        /// Disable or Enable Site Network
        /// </summary>
        /// <param name="SiteNetwork"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView DisableOrEnableSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            return _siteNetworkService.DisableOrEnableSiteNetwork(SiteNetwork);
        }

        /// <summary>
        /// Get Site Network Log By Site Network Guid
        /// </summary>
        /// <param name="SiteNetworkGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SiteNetworkAuditLogView> GetLogListInfoBySiteNetwork(Guid SiteNetworkGuid)
        {
            return _siteNetworkAuditoLogService.GetLogListInfoBySiteNetwork(SiteNetworkGuid);
        }
    }
}