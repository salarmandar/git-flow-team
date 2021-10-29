using Bgt.Ocean.Models.NemoConfiguration;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_NemoConfigurationController : ApiControllerBase
    {
        private readonly INemoConfigurationService _nemoConfigurationService;
        private readonly ISystemService _systemService;

        public v1_NemoConfigurationController(INemoConfigurationService nemoConfigurationService, ISystemService systemService)
        {
            _nemoConfigurationService = nemoConfigurationService;
            _systemService = systemService;

        }

        #region GET

        #region Day of week
        /// <summary>
        /// GET : api/v1/in/NemoConfiguration/GetDayOfWeek
        /// </summary>
        /// <param name="languageGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SystemDayOfWeekView> GetDayOfWeek()
        {
            return _systemService.GetSystemDayOfWeekList();
        }
        #endregion

        #region Nemo country configuration

        /// <summary>
        /// GET : api/v1/in/NemoConfiguration/DistanceUnitListGet
        /// </summary>
        /// <param name="languageGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<GlobalUnitText> DistanceUnitListGet()
        {
            return _nemoConfigurationService.DistanceUnit_Get();
        }

        /// <summary>
        /// GET : api/v1/in/NemoConfiguration/NemoCountryConfigGet
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public NemoConfigValueView NemoCountryConfigGet(Guid countryGuid)
        {
            var result = new NemoConfigValueView();
            result.CountryConfig = _nemoConfigurationService.NemoCountryConfig_Get(countryGuid);
            return result;
        }

        #endregion       

        #region Nemo site and traffic configuration
        /// <summary>
        /// GET : api/v1/in/NemoConfiguration/SiteAndTrafficConfigGet
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public NemoConfigValueView SiteAndTrafficConfigGet(Guid countryGuid, Guid siteGuid)
        {
            var result = new NemoConfigValueView();
            result.SiteConfig = _nemoConfigurationService.NemoSiteConfig_Get(countryGuid, siteGuid);
            result.TrafficConfig = _nemoConfigurationService.NemoTrafficConfig_Get(countryGuid, siteGuid);           
            return result;
        }
        #endregion

        #region Site
        /// <summary>
        /// For grid apply to site.
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SiteForApplyView> SiteForApplyGet(Guid countryGuid)
        {            
            return _nemoConfigurationService.SiteForApply_Get(countryGuid);
        }

        /// <summary>
        /// For dropdown list site is apply.
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SiteForApplyView> SiteIsApplyGet(Guid countryGuid)
        {
            return _nemoConfigurationService.SiteIsApply_Get(countryGuid);
        }
        #endregion

        #endregion

        #region ADD & UPDATE

        #region Nemo country configuration    

        /// <summary>
        /// Add and update nemo country configuration
        /// </summary>
        /// <param name="nemoCountryConfigReq"></param>  
        /// <param name="languageGuid"></param>                  
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView NemoCountryConfigAddUpdate(NemoConfigValueRequest nemoCountryConfigReq) 
        {
            return _nemoConfigurationService.NemoCountryConfig_AddUpdate(nemoCountryConfigReq);
        }

        [HttpPost]
        public SystemMessageView NemoCountryApplyToSite(NemoApplyToSiteRequest nemoApplyToSiteReq)
        {
            return _nemoConfigurationService.NemoCountryConfig_ApplyToSite(nemoApplyToSiteReq);
        }
        #endregion

        #region Nemo site and traffic factor configuration
        [HttpPost]
        public SystemMessageView NemoSiteAndTrafficConfigAddUpdate(NemoConfigValueRequest nemoConfigReq)
        {            
            return _nemoConfigurationService.NemoSiteAndTraffic_AddUpdate(nemoConfigReq);
        }
        #endregion       

        #endregion

    }
}