using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.NemoConfiguration;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    #region Interface

    public interface INemoConfigurationService
    {
        IEnumerable<GlobalUnitText> DistanceUnit_Get();
        NemoCountryValueView NemoCountryConfig_Get(Guid countryGuid);
        IEnumerable<SiteForApplyView> SiteForApply_Get(Guid countryGuid);
        IEnumerable<SiteForApplyView> SiteIsApply_Get(Guid countryGuid);
        NemoSiteValueView NemoSiteConfig_Get(Guid countryGuid, Guid siteGuid);
        IEnumerable<NemoTrafficFactorValueView> NemoTrafficConfig_Get(Guid countryGuid, Guid siteGuid);
        SystemMessageView NemoCountryConfig_AddUpdate(NemoConfigValueRequest nemoCountryConfigModel);
        SystemMessageView NemoCountryConfig_ApplyToSite(NemoApplyToSiteRequest nemoApplyToSiteReq);
        SystemMessageView NemoSiteAndTraffic_AddUpdate(NemoConfigValueRequest nemoConfigReq);
    }

    #endregion

    public class NemoConfigurationService : INemoConfigurationService
    {
        private readonly IMasterNemoCountryValueRepository _masterNemoCountryValueRepository;
        private readonly IMasterNemoSiteValueRepository _masterNemoSiteValueRepository;
        private readonly IMasterNemoTrafficFactorValueRepository _masterNemoTrafficFactorValueRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemDayOfWeekRepository _systemDayOfWeekRepository;
        private readonly ISystemService _systemService;
        public NemoConfigurationService(
            IMasterNemoCountryValueRepository masterNemoCountryValueRepository,
            IMasterNemoSiteValueRepository masterNemoSiteValueRepository,
            IMasterNemoTrafficFactorValueRepository masterNemoTrafficFactorValueRepository,
            ISystemMessageRepository systemMessageRepository,
            ISystemDayOfWeekRepository systemDayOfWeekRepository,
            ISystemService systemService
            )
        {
            _masterNemoCountryValueRepository = masterNemoCountryValueRepository;
            _masterNemoSiteValueRepository = masterNemoSiteValueRepository;
            _masterNemoTrafficFactorValueRepository = masterNemoTrafficFactorValueRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemDayOfWeekRepository = systemDayOfWeekRepository;
            _systemService = systemService;
        }


        #region ### GET ###

        #region Nemo country configuration
        public IEnumerable<GlobalUnitText> DistanceUnit_Get()
        {
            var languageGuid = ApiSession.UserLanguage_Guid.Value;
            IEnumerable<GlobalUnitText> result = null;
            try
            {
                result = _masterNemoCountryValueRepository.DistanceUnitGet(languageGuid);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public NemoCountryValueView NemoCountryConfig_Get(Guid countryGuid)
        {
            NemoCountryValueView result = null;
            try
            {
                result = _masterNemoCountryValueRepository.NemoCountryConfigGet(countryGuid).ConvertToNemoCountryValueView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public IEnumerable<SiteForApplyView> SiteForApply_Get(Guid countryGuid)
        {
            IEnumerable<SiteForApplyView> result = null;
            try
            {
                result = _masterNemoCountryValueRepository.SiteForApplyGet(countryGuid);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        public IEnumerable<SiteForApplyView> SiteIsApply_Get(Guid countryGuid)
        {
            IEnumerable<SiteForApplyView> result = null;
            try
            {
                result = _masterNemoCountryValueRepository.SiteForApplyGet(countryGuid).Where(o => o.FlagSiteApply);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        #endregion

        #region Nemo site configuration
        public NemoSiteValueView NemoSiteConfig_Get(Guid countryGuid, Guid siteGuid)
        {
            NemoSiteValueView result = null;
            try
            {
                result = _masterNemoSiteValueRepository.NemoSiteConfigGet(countryGuid, siteGuid).ConvertToNemoSiteValueView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        #endregion

        #region Nemo traffic factor configuration
        public IEnumerable<NemoTrafficFactorValueView> NemoTrafficConfig_Get(Guid countryGuid, Guid siteGuid)
        {
            IEnumerable<NemoTrafficFactorValueView> result = null;
            try
            {
                var dayOfWeek = _systemDayOfWeekRepository.FindAll(o => !o.FlagDisable);
                result = _masterNemoTrafficFactorValueRepository.NemoTrafficConfigGet(countryGuid, siteGuid)
                             .Join(dayOfWeek, traffic => traffic.DayofWeek_Guid, day => day.Guid,
                             (traffic, day) => new TblMasterNemoTrafficFactorValue
                             {
                                 Guid = traffic.Guid,
                                 MasterCountry_Guid = traffic.MasterCountry_Guid,
                                 MasterSite_Guid = traffic.MasterSite_Guid,
                                 DayofWeek_Guid = traffic.DayofWeek_Guid,
                                 DayOfWeekName = day.MasterDayOfWeek_Name,
                                 StartTime = traffic.StartTime,
                                 EndTime = traffic.EndTime,
                                 TrafficMultiplier = traffic.TrafficMultiplier,
                                 UserCreated = traffic.UserCreated,
                                 DatetimeCreated = traffic.DatetimeCreated,
                                 UniversalDatetimeCreated = traffic.UniversalDatetimeCreated,
                                 UserModifed = traffic.UserModifed,
                                 DatetimeModified = traffic.DatetimeModified,
                                 UniversalDatetimeModified = traffic.UniversalDatetimeModified,
                                 FlagTrafficCal = traffic.FlagTrafficCal
                             }).ConvertToNemoTrafficFactorValueView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        #endregion

        #endregion ### GET ###

        #region ### ADD & UPDATE ###

        #region Nemo coutry configuration
        public SystemMessageView NemoCountryConfig_AddUpdate(NemoConfigValueRequest nemoCountryConfigModel)
        {
            var nemoCountryConfig = nemoCountryConfigModel.CountryConfig;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            SystemMessageView responseMsg = null;
            
            try
            {
                var mapNemoCountryConfig = nemoCountryConfig.ConvertToTblMasterNemoCountryValue();
                mapNemoCountryConfig.UserCreated = nemoCountryConfigModel.UserName;
                mapNemoCountryConfig.DatetimeCreated = nemoCountryConfigModel.LocalClientDateTime;
                mapNemoCountryConfig.UniversalDatetimeCreated = nemoCountryConfigModel.UniversalDatetime;
                mapNemoCountryConfig.UserModifed = nemoCountryConfigModel.UserName;
                mapNemoCountryConfig.DatetimeModified = nemoCountryConfigModel.LocalClientDateTime;
                mapNemoCountryConfig.UniversalDatetimeModified = nemoCountryConfigModel.UniversalDatetime;
                _masterNemoCountryValueRepository.NemoCountryConfigAddUpdate(mapNemoCountryConfig);
                responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
            }
            return responseMsg;
        }

        public SystemMessageView NemoCountryConfig_ApplyToSite(NemoApplyToSiteRequest nemoApplyToSiteReq)
        {
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            SystemMessageView responseMsg = null;
            try
            {              
                var applyToSite = nemoApplyToSiteReq.ConvertToNemoApplyToSiteView();
                applyToSite.ClientDateTime = nemoApplyToSiteReq.LocalClientDateTime;
                applyToSite.UniversalDateTime = nemoApplyToSiteReq.UniversalDatetime;
                applyToSite.UserName = nemoApplyToSiteReq.UserName;              
                _masterNemoCountryValueRepository.NemoCountryApplyToSite(applyToSite);
                responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
            }
            return responseMsg;
        }
        #endregion

        #region Nemo site and traffic configuration
        public SystemMessageView NemoSiteAndTraffic_AddUpdate(NemoConfigValueRequest nemoConfigReq)
        {
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            SystemMessageView responseMsg = null;
            try
            {
                #region #For site config
                var mapNemoSiteConfig = nemoConfigReq.SiteConfig.ConvertToTblMasterNemoSiteValue();
                mapNemoSiteConfig.DatetimeCreated = nemoConfigReq.LocalClientDateTime;
                mapNemoSiteConfig.UserCreated = nemoConfigReq.UserName;
                mapNemoSiteConfig.UniversalDatetimeCreated = nemoConfigReq.UniversalDatetime;
                mapNemoSiteConfig.DatetimeModified = nemoConfigReq.LocalClientDateTime;
                mapNemoSiteConfig.UserModifed = nemoConfigReq.UserName;
                mapNemoSiteConfig.UniversalDatetimeModified = nemoConfigReq.UniversalDatetime;
                _masterNemoSiteValueRepository.NemoSiteConfigAddUpdate(mapNemoSiteConfig);
                #endregion

                #region #For traffic config
                var mapNemoTrafficConfig = nemoConfigReq.TrafficConfig.ConvertToTblMasterNemoTrafficValue();
                _masterNemoTrafficFactorValueRepository.NemoTrafficConfigAddUpdate(mapNemoTrafficConfig, nemoConfigReq.CountryGuid, nemoConfigReq.SiteGuid.GetValueOrDefault(), nemoConfigReq.UserName, nemoConfigReq.LocalClientDateTime, nemoConfigReq.UniversalDatetime);
                #endregion

                responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
            }
            return responseMsg;
        }
        #endregion      

        #endregion ### ADD & UPDATE ###

    }
}
