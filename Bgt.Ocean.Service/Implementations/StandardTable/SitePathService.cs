using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.SitePath;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface ISitePathService
    {
        IEnumerable<SitePathViewResponse> GetSitePathListInfo(Guid MasterCountry_Guid, bool FlagDisable);
        SitePathViewResponse GetSitePathInfo(Guid SitePathGuid);
        SitePathViewResponse CreateUpdateSitePath(SitePathViewRequest SitePath);
        SystemMessageView DisableOrEnablePath(SitePathViewRequest SitePath);
        IEnumerable<SitePathAuditLogView> GetLogListInfoBySitePath(Guid SitePathGuid);


        IEnumerable<BrinksSiteBySitePath> getShortInfoSitePathDetail(Guid? SitePathHeader_Guid);
        IEnumerable<SitePathDropdownViewResponse> getAllSitePathName(SitePathDropdownViewRequest model);
    }
    public class SitePathService : ISitePathService
    {
        private readonly IMasterSitePathRepository _masterSitePathRepository;
        private readonly IMasterSitePathDetailRepository _masterSitePathDetailRepository;
        private readonly IMasterSitePathDestinationRepository _masterSitePathDestionationRepository;
        private readonly IMasterSitePathAuditLogRepository _masterSitePathAuditLogRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        public SitePathService(IMasterSitePathRepository masterSitePathRepository,
            IMasterSitePathDetailRepository masterSitePathDetail,
            IMasterSitePathDestinationRepository masterSitePathDestionationRepository,
            IMasterSitePathAuditLogRepository masterSitePathAuditLogRepository,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow)
        {
            _masterSitePathRepository = masterSitePathRepository;
            _masterSitePathDetailRepository = masterSitePathDetail;
            _masterSitePathDestionationRepository = masterSitePathDestionationRepository;
            _masterSitePathAuditLogRepository = masterSitePathAuditLogRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
        }

        public IEnumerable<SitePathViewResponse> GetSitePathListInfo(Guid MasterCountry_Guid, bool FlagDisable) {
            return _masterSitePathRepository.GetSitePathInfoList(MasterCountry_Guid, FlagDisable).ConvertToSitePathViewResponseList().ToList();
        }

        public SitePathViewResponse GetSitePathInfo(Guid SitePathGuid) {
            return _masterSitePathRepository.GetSitePathInfoById(SitePathGuid).ConvertToSitePathViewResponse();
        }
        public SitePathViewResponse CreateUpdateSitePath(SitePathViewRequest SitePath) {
            SitePathViewResponse result = new SitePathViewResponse();
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            string changeType = "created";
            int error = 0;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    Guid newSiteGuid = Guid.NewGuid();
                    if (SitePath != null)
                    {
                        bool dulicateElement = _masterSitePathRepository.ValidateDuplicateSitePath(SitePath.ConvertToSitePathView(), out error);
                        if (dulicateElement)
                        {
                            if (SitePath.SitePathGuid == Guid.Empty)
                            {
                                if (SitePath.BrinksSitelist.Any())
                                {
                                    SitePath.SitePathGuid = newSiteGuid;
                                    TblMasterSitePathHeader tblSitePath = new TblMasterSitePathHeader()
                                    {
                                        Guid = newSiteGuid,
                                        SitePathName = SitePath.SitePathName,
                                        MasterSiteNetwork_Guid = SitePath.SiteNetworkGuid,
                                        FlagDisble = false,
                                        UserCreated = SitePath.UserData.UserName,
                                        DatetimeCreted = SitePath.UserData.LocalClientDateTime,
                                        UniversalDatetimeCreated = SitePath.UserData.UniversalDatetime,
                                    };
                                    _masterSitePathRepository.Create(tblSitePath);
                                    int index = 1;
                                    foreach (BrinksSiteBySitePath brinkSite in SitePath.BrinksSitelist)
                                    {
                                        TblMasterSitePathDetail sitePathMember = new TblMasterSitePathDetail()
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterSitePathHeader_Guid = newSiteGuid,
                                            MasterSite_Guid = brinkSite.SiteGuid,
                                            SequenceIndex = index,
                                            FlagDestination = (index == SitePath.BrinksSitelist.Count()? true : false),
                                            UserCreated = SitePath.UserData.UserName,
                                            DatetimeCreted = SitePath.UserData.LocalClientDateTime,
                                            UniversalDatetimeCreated = SitePath.UserData.UniversalDatetime
                                        };
                                        _masterSitePathDetailRepository.Create(sitePathMember);
                                        index++;
                                    }
                                    //Log New One
                                    NewAuditLogSitePath(SitePath);
                                }
                                else
                                {
                                    result.Message = _systemMessageRepository.FindByMsgId(-319, LanguageGuid).ConvertToMessageView();
                                    return result;
                                }
                            }
                            else
                            {
                                var updateItem = _masterSitePathRepository.GetSitePathInfoById(SitePath.SitePathGuid);
                                if (updateItem != null)
                                {
                                    TblMasterSitePathHeader tblSitePath = _masterSitePathRepository.FindById(SitePath.SitePathGuid);
                                    newSiteGuid = updateItem.SitePathGuid;
                                    tblSitePath.SitePathName = SitePath.SitePathName;
                                    tblSitePath.UserModified = SitePath.UserData.UserName;
                                    tblSitePath.DatetimeModified = SitePath.UserData.LocalClientDateTime;
                                    tblSitePath.UniversalDatetimeModified = SitePath.UserData.UniversalDatetime;
                                    _masterSitePathRepository.Modify(tblSitePath);
                                    //Modified Site List
                                    var brinkSiteList = _masterSitePathDetailRepository.FindAll(x => x.MasterSitePathHeader_Guid == SitePath.SitePathGuid);
                                    foreach (TblMasterSitePathDetail brinkSite in brinkSiteList)
                                    {
                                        var item = SitePath.BrinksSitelist.FirstOrDefault(x => x.SiteGuid == brinkSite.MasterSite_Guid && x.Guid != null && x.Guid != Guid.Empty);
                                        if (item == null)
                                        {
                                            //Remove brink's site
                                            _masterSitePathDetailRepository.Remove(brinkSite);
                                        }
                                        else
                                        {
                                            //Update brink's site
                                            brinkSite.SequenceIndex = item.SecuenceIndex;
                                            brinkSite.UserModified = SitePath.UserData.UserName;
                                            brinkSite.DatetimeModified = SitePath.UserData.LocalClientDateTime;
                                            brinkSite.UniversalDatetimeModified = SitePath.UserData.UniversalDatetime;
                                            _masterSitePathDetailRepository.Modify(brinkSite);
                                        }
                                    }
                                    //Create
                                    SitePath.BrinksSitelist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x =>
                                    {
                                        TblMasterSitePathDetail sitePathMember = new TblMasterSitePathDetail()
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterSitePathHeader_Guid = SitePath.SitePathGuid,
                                            MasterSite_Guid = x.SiteGuid,
                                            SequenceIndex = x.SecuenceIndex,
                                            UserCreated = SitePath.UserData.UserName,
                                            DatetimeCreted = SitePath.UserData.LocalClientDateTime,
                                            UniversalDatetimeCreated = SitePath.UserData.UniversalDatetime
                                        };
                                        _masterSitePathDetailRepository.Create(sitePathMember);
                                    });

                                    //Modified Location List of Destination Site
                                    var customerLocationList = _masterSitePathDestionationRepository.FindAll(x => x.MasterSitePathHeader_Guid == SitePath.SitePathGuid);
                                    var destinationSite = _masterSitePathDetailRepository.FindAll(x => x.MasterSitePathHeader_Guid == SitePath.SitePathGuid && x.FlagDestination == true).FirstOrDefault();
                                    foreach (TblMasterSitePathDestination customerLocation in customerLocationList)
                                    {
                                        var item = SitePath.CustomerLocationlist.FirstOrDefault(x => x.LocationGuid == customerLocation.MasterCustomerLocation_Guid && x.Guid != null && x.Guid != Guid.Empty);
                                        if (item == null)
                                        {
                                            //Remove Customer Location
                                            _masterSitePathDestionationRepository.Remove(customerLocation);
                                        }
                                    }
                                    //Create
                                    SitePath.CustomerLocationlist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x =>
                                    {
                                        TblMasterSitePathDestination sitePathDestinationMember = new TblMasterSitePathDestination()
                                        {
                                            Guid = Guid.NewGuid(),
                                            MasterSitePathHeader_Guid = SitePath.SitePathGuid,
                                            MasterSite_Guid = destinationSite.MasterSite_Guid,
                                            MasterCustomerLocation_Guid = x.LocationGuid,
                                            UserCreated = SitePath.UserData.UserName,
                                            DatetimeCreted = SitePath.UserData.LocalClientDateTime,
                                            UniversalDatetimeCreated = SitePath.UserData.UniversalDatetime
                                        };
                                        _masterSitePathDestionationRepository.Create(sitePathDestinationMember);
                                    });
                                    changeType = "updated";
                                    //Updated log
                                    ModifiedAuditLogSitePath(SitePath, updateItem.ConvertToSitePathViewResponse());
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
                            switch (error)
                            {
                                case 1:
                                    result.Message = _systemMessageRepository.FindByMsgId(-380, LanguageGuid).ConvertToMessageView();
                                    return result;
                                    break;
                                case 2:
                                    result.Message = _systemMessageRepository.FindByMsgId(-379, LanguageGuid).ConvertToMessageView();
                                    return result;
                                    break;
                            }
                        }
                    }
                    _uow.Commit();
                    result = _masterSitePathRepository.GetSitePathInfoById(newSiteGuid).ConvertToSitePathViewResponse();
                    result.Message = _systemMessageRepository.FindByMsgId(838, LanguageGuid).ConvertToMessageView(true)
                        .ReplaceTextContentStringFormatWithValue(result.SitePathName, changeType);
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
        public SystemMessageView DisableOrEnablePath(SitePathViewRequest SitePath) {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            string sitePathName = "";
            using (var transaction = _uow.BeginTransaction())
            {
                try
                {
                    if (SitePath != null)
                    {
                        var updateItem = _masterSitePathRepository.FindById(SitePath.SitePathGuid);
                        updateItem.FlagDisble = SitePath.FlagDisable;
                        updateItem.UserModified = SitePath.UserData.UserName;
                        updateItem.DatetimeModified = SitePath.UserData.LocalClientDateTime;
                        updateItem.UniversalDatetimeModified = SitePath.UserData.UniversalDatetime;
                        _masterSitePathRepository.Modify(updateItem);
                        sitePathName = updateItem.SitePathName;
                        DisableEnableAuditLogSitePath(SitePath);

                        _uow.Commit();
                        responseMsg = _systemMessageRepository.FindByMsgId(838, LanguageGuid).ConvertToMessageView(true)
                            .ReplaceTextContentStringFormatWithValue(sitePathName, (SitePath.FlagDisable ? "disabled" : "enabled"));
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
        public IEnumerable<SitePathAuditLogView> GetLogListInfoBySitePath(Guid SitePathGuid)
        {
            return _masterSitePathAuditLogRepository.FindAll(x => x.MasterSitePathHeader_Guid == SitePathGuid).OrderByDescending(x => x.DatetimeCreted).ConvertToSitePathAuditLogViewList().ToList();
        }

        private void NewAuditLogSitePath(SitePathViewRequest SitePath)
        {
            CreateSitePathLogConfiguration(1, string.Format("Site Path {0} has been created.", SitePath.SitePathName),
                SitePath.SitePathGuid, SitePath.UserData.UserName);
            CreateSitePathLogConfiguration(1, string.Format("Site Path {0} has been created. Site Network {1} was added.", SitePath.SitePathName, SitePath.SiteNetworkName),
                    SitePath.SitePathGuid, SitePath.UserData.UserName);
            foreach (BrinksSiteBySitePath brinkSite in SitePath.BrinksSitelist)
            {
                CreateSitePathLogConfiguration(1, string.Format("Site Path {0} has been created. Brink's Site {1} was added. Sequence Index {2} of {3}", 
                    SitePath.SitePathName, brinkSite.SiteCodeName, brinkSite.SecuenceIndex, SitePath.BrinksSitelist.Count()), SitePath.SitePathGuid, SitePath.UserData.UserName);
            }
        }

        private void ModifiedAuditLogSitePath(SitePathViewRequest NewSitePath, SitePathViewResponse OldSitePath)
        {
            if (NewSitePath.SitePathName != OldSitePath.SitePathName)
            {
                CreateSitePathLogConfiguration(2, string.Format("Site Path {0} has been changed Site Path Name from {1} to {0}.", NewSitePath.SitePathName, OldSitePath.SitePathName),
                    NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
            }
            foreach (BrinksSiteBySitePath brinkSite in OldSitePath.BrinksSitelist)
            {
                var item = NewSitePath.BrinksSitelist.FirstOrDefault(x => x.SiteGuid == brinkSite.SiteGuid && x.Guid != null && x.Guid != Guid.Empty);
                if (item == null)
                {
                    CreateSitePathLogConfiguration(4, string.Format("Site Path {0} has been changed. Brink's Site {1} was deleted.",
                        NewSitePath.SitePathName, brinkSite.SiteName), NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
                }
                else
                {
                    if (brinkSite.SecuenceIndex != item.SecuenceIndex)
                    {
                        CreateSitePathLogConfiguration(5, string.Format("Site Path {0} has been changed. Brink's Site {1} has been changed Sequence Index from {2} to {3}.",
                            NewSitePath.SitePathName, brinkSite.SiteName, brinkSite.SecuenceIndex, item.SecuenceIndex), NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
                    }
                }
            }
            NewSitePath.BrinksSitelist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x => {
                CreateSitePathLogConfiguration(3, string.Format("Site Path {0} has been changed. Brink's Site {1} was added.", NewSitePath.SitePathName, x.SiteCodeName),
                    NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
            });
            foreach (LocationViewBySitePath customerLocation in OldSitePath.CustomerLocationlist)
            {
                var item = NewSitePath.CustomerLocationlist.FirstOrDefault(x => x.LocationGuid == customerLocation.LocationGuid && x.Guid != null && x.Guid != Guid.Empty);
                if (item == null)
                {
                    CreateSitePathLogConfiguration(7, string.Format("Site Path {0} has been changed. Customer Location {1} was deleted.",
                        NewSitePath.SitePathName, customerLocation.CustomerName + " - " + customerLocation.LocationName), NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
                }
            }
            NewSitePath.CustomerLocationlist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x => {
                CreateSitePathLogConfiguration(6, string.Format("Site Path {0} has been changed. Customer Location {1} was added.", 
                    NewSitePath.SitePathName, x.CustomerName + " - " + x.LocationName), NewSitePath.SitePathGuid, NewSitePath.UserData.UserName);
            });
        }

        private void DisableEnableAuditLogSitePath(SitePathViewRequest SitePath)
        {
            CreateSitePathLogConfiguration((SitePath.FlagDisable ? 8 : 9), string.Format("Site Path {0} has been {1}.", SitePath.SitePathName, (SitePath.FlagDisable ? "Disabled" : "Enabled")),
                SitePath.SitePathGuid, SitePath.UserData.UserName);
        }

        private void CreateSitePathLogConfiguration(int msgId, string msgParameter, Guid sitePathGuid, string user)
        {
            TblMasterSitePathAuditLog newLog = new TblMasterSitePathAuditLog()
            {
                Guid = Guid.NewGuid(),
                MasterSitePathHeader_Guid = sitePathGuid,
                //1 = Add value in new Site Path
                //2 = Edit Site Path name
                //3 = Add Brink's Site
                //4 = Delete Brink's Site
                //5 = Edit Brink's Site
                //6 = Add Customer Location Site
                //7 = Delete Customer Location Site
                //8 = Disable Site Network
                //9 = Enable Site Network
                MsgID = msgId,
                MsgParameter = msgParameter,
                DatetimeCreted = DateTime.Now,
                UniversalDatetimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UserCreated = user
            };
            _masterSitePathAuditLogRepository.Create(newLog);
        }

        
        public IEnumerable<BrinksSiteBySitePath> getShortInfoSitePathDetail(Guid? SitePathHeader_Guid)
        {
            return _masterSitePathRepository.getShortInfoSitePathDetail(SitePathHeader_Guid);
        }

        public IEnumerable<SitePathDropdownViewResponse> getAllSitePathName(SitePathDropdownViewRequest model)
        {
            if (model.pickupSiteGuid.IsNullOrEmpty() || model.deliverySiteGuid.IsNullOrEmpty()) 
            {
                return new List<SitePathDropdownViewResponse>(); //Site Guid of both P, D Legs are a must
            }
            else if (model.jobType == 14 && model.pickupLocationGuid.IsNullOrEmpty()) 
            {
                return new List<SitePathDropdownViewResponse>(); //Job Type P Multi must have Location_Guid of Leg P
            }
            else if (model.jobType == 15 && (model.pickupLocationGuid.IsNullOrEmpty() || model.deliveryLocationGuid.IsNullOrEmpty()))
            {
                return new List<SitePathDropdownViewResponse>(); //Job Type TV Multi must have Location_Guid of Leg P and Leg D
            }
            var result = _masterSitePathRepository.getAllSitePathNameFromCustomerLocation(model.jobType, model.pickupLocationGuid, model.deliveryLocationGuid, model.pickupSiteGuid, model.deliverySiteGuid)
                .Distinct().OrderBy(o => o.SitePathName).Select(
                o => new SitePathDropdownViewResponse
                {
                    SitePath_Guid = o.Guid,
                    SitePath_Name = o.SitePathName
                }
                );

            return result;
        }
    }
}
