using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.SiteNetwork;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface ISiteNetworkService
    {
        IEnumerable<SiteNetworkViewResponse> GetSiteNetworkListInfo(Guid MasterCountry_Guid, bool? FlagDisable);
        SiteNetworkViewResponse CreateUpdateSiteNetwork(SiteNetworkViewRequest SiteNetwork);
        SiteNetworkViewResponse GetSiteNetworkInfo(Guid SiteNetworkGuid);
        SystemMessageView DisableOrEnableSiteNetwork(SiteNetworkViewRequest SiteNetwork);
    }
    public class SiteNetworkService : ISiteNetworkService
    {
        private readonly IMasterSiteNetworkRepository _masterSiteNetworkRepository;
        private readonly IMasterSiteNetworkMemberRepository _masterSiteNetworkMemberRepository;
        private readonly ISiteNetworkAuditoLogService _siteNetworkAuditLogService;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;

        public SiteNetworkService(IMasterSiteNetworkRepository masterSiteNetworkRepository,
            IMasterSiteNetworkMemberRepository masterSiteNetworkMemberRepository,
            ISiteNetworkAuditoLogService siteNetworkAuditLogService,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow)
        {
            _masterSiteNetworkRepository = masterSiteNetworkRepository;
            _masterSiteNetworkMemberRepository = masterSiteNetworkMemberRepository;
            _siteNetworkAuditLogService = siteNetworkAuditLogService;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
        }

        public IEnumerable<SiteNetworkViewResponse> GetSiteNetworkListInfo(Guid MasterCountry_Guid, bool? FlagDisable) {
            return _masterSiteNetworkRepository.GetSiteNetworkInfoList(MasterCountry_Guid, FlagDisable).ConvertToSiteNetworkViewResponseList().ToList();
        }

        public SiteNetworkViewResponse GetSiteNetworkInfo(Guid SiteNetworkGuid) {
            return _masterSiteNetworkRepository.GetSiteNetworkInfoById(SiteNetworkGuid).ConvertToSiteNetworkViewResponseView();
        }

        public SiteNetworkViewResponse CreateUpdateSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            SiteNetworkViewResponse result = new SiteNetworkViewResponse();
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            string messageText = "created";
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    Guid newSiteGuid = Guid.NewGuid();
                    if (SiteNetwork != null)
                    {
                        var siteNameList = _masterSiteNetworkRepository.FindAll(x => 
                            x.SiteNetworkName == SiteNetwork.SiteName 
                            && x.MasterCountry_Guid == SiteNetwork.MasterCountryGuid
                            && x.Guid != SiteNetwork.SiteGuid
                            ).FirstOrDefault();
                        if (siteNameList == null)
                        {
                            if (SiteNetwork.SiteGuid == Guid.Empty)
                            {
                                if (SiteNetwork.BrinksSitelist.Any())
                                {
                                    SiteNetwork.SiteGuid = newSiteGuid;
                                    TblMasterSiteNetwork tblSiteNetwork = new TblMasterSiteNetwork()
                                    {
                                        Guid = newSiteGuid,
                                        SiteNetworkName = SiteNetwork.SiteName,
                                        FlagDisable = false,
                                        UserCreated = SiteNetwork.UserData.UserName,
                                        DatetimeCreated = SiteNetwork.UserData.LocalClientDateTime,
                                        UniversalDatetimeCreated = SiteNetwork.UserData.UniversalDatetime,
                                        MasterCountry_Guid = SiteNetwork.MasterCountryGuid
                                    };
                                    _masterSiteNetworkRepository.Create(tblSiteNetwork);
                                    foreach (BrinksSiteBySiteNetwork brinkSite in SiteNetwork.BrinksSitelist)
                                    {
                                        TblMasterSiteNetworkMember siteNetworkMember = new TblMasterSiteNetworkMember()
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterSiteNetwork_Guid = newSiteGuid,
                                            MasterSite_Guid = brinkSite.SiteGuid,
                                            UserCreated = SiteNetwork.UserData.UserName,
                                            DatetimeCreated = SiteNetwork.UserData.LocalClientDateTime,
                                            UniversalDatetimeCreated = SiteNetwork.UserData.UniversalDatetime
                                        };
                                        _masterSiteNetworkMemberRepository.Create(siteNetworkMember);
                                    }
                                    _siteNetworkAuditLogService.NewSiteNetwork(SiteNetwork);
                                }
                                else
                                {
                                    result.Message = _systemMessageRepository.FindByMsgId(-319, LanguageGuid).ConvertToMessageView();
                                    return result;
                                }
                            }
                            else
                            {
                                var updateItem = _masterSiteNetworkRepository.GetSiteNetworkInfoById(SiteNetwork.SiteGuid);
                                if (updateItem != null)
                                {
                                    TblMasterSiteNetwork siteNetwork = _masterSiteNetworkRepository.FindById(SiteNetwork.SiteGuid);
                                    newSiteGuid = updateItem.SiteGuid;
                                    siteNetwork.SiteNetworkName = SiteNetwork.SiteName;
                                    siteNetwork.UserModified = SiteNetwork.UserData.UserName;
                                    siteNetwork.DatetimeModified = SiteNetwork.UserData.LocalClientDateTime;
                                    siteNetwork.UniversalDatetimeModified = SiteNetwork.UserData.UniversalDatetime;
                                    _masterSiteNetworkRepository.Modify(siteNetwork);
                                    var brinkSiteList = _masterSiteNetworkMemberRepository.FindAll(x => x.MasterSiteNetwork_Guid == SiteNetwork.SiteGuid);
                                    foreach (TblMasterSiteNetworkMember brinkSite in brinkSiteList)
                                    {
                                        var item = SiteNetwork.BrinksSitelist.FirstOrDefault(x => x.SiteGuid == brinkSite.MasterSite_Guid && x.Guid != null && x.Guid != Guid.Empty);
                                        if (item == null)
                                        {
                                            //Delete
                                            _masterSiteNetworkMemberRepository.Remove(brinkSite);
                                        }
                                        //Can not update because the data of Brinks Sites do not change
                                    }
                                    //Create
                                    SiteNetwork.BrinksSitelist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x =>
                                    {
                                        TblMasterSiteNetworkMember siteNetworkMember = new TblMasterSiteNetworkMember()
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterSiteNetwork_Guid = SiteNetwork.SiteGuid,
                                            MasterSite_Guid = x.SiteGuid,
                                            UserCreated = SiteNetwork.UserData.UserName,
                                            DatetimeCreated = SiteNetwork.UserData.LocalClientDateTime,
                                            UniversalDatetimeCreated = SiteNetwork.UserData.UniversalDatetime
                                        };
                                        _masterSiteNetworkMemberRepository.Create(siteNetworkMember);
                                    });
                                    messageText = "updated";
                                    _siteNetworkAuditLogService.ModifiedSiteNetwork(SiteNetwork, updateItem.ConvertToSiteNetworkViewResponseView());
                                }
                                else
                                {
                                    result.Message = _systemMessageRepository.FindByMsgId(-319, LanguageGuid).ConvertToMessageView();
                                    return result;
                                }
                            }
                        }
                        else
                        {
                            result.Message = _systemMessageRepository.FindByMsgId(-377, LanguageGuid).ConvertToMessageView();
                            return result;
                        }
                    }
                    _uow.Commit();
                    result = _masterSiteNetworkRepository.GetSiteNetworkInfoById(newSiteGuid).ConvertToSiteNetworkViewResponseView();
                    result.Message = _systemMessageRepository.FindByMsgId(829, LanguageGuid).ConvertToMessageView(true)
                        .ReplaceTextContentStringFormatWithValue(result.SiteName, messageText);
                    transection.Complete();
                    return result;
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    result.Message = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                    return result;
                }
            }
        }

        public SystemMessageView DisableOrEnableSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            string siteName = "";
            using (var transaction = _uow.BeginTransaction())
            {
                try
                {
                    if (SiteNetwork != null)
                    {
                        var updateItem = _masterSiteNetworkRepository.FindById(SiteNetwork.SiteGuid);
                        updateItem.FlagDisable = SiteNetwork.FlagDisable;
                        updateItem.UserModified = SiteNetwork.UserData.UserName;
                        updateItem.DatetimeModified = SiteNetwork.UserData.LocalClientDateTime;
                        updateItem.UniversalDatetimeModified = SiteNetwork.UserData.UniversalDatetime;
                        _masterSiteNetworkRepository.Modify(updateItem);
                        siteName = updateItem.SiteNetworkName;
                        _siteNetworkAuditLogService.DisableEnableSiteNetwork(SiteNetwork);

                        _uow.Commit();
                        responseMsg = _systemMessageRepository.FindByMsgId(829, LanguageGuid).ConvertToMessageView(true)
                            .ReplaceTextContentStringFormatWithValue(siteName, (SiteNetwork.FlagDisable ? "disabled" : "enabled"));
                        transaction.Complete();
                    }
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                }
            }
            return responseMsg;
        }
    }
}
