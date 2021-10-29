using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messaging.SystemConfigurationAdditional;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    /// <summary>
    /// interface of UserAccessGroupCountryEmail.
    /// </summary>
    public interface IMasterUserAccessGroupCountryEmailService
    {
        /// <summary>
        /// Get Pre Defined Emails Info.
        /// </summary>
        /// <returns>PreDefinedEmailsView List</returns>
        IEnumerable<PreDefinedEmailsView> GetPreDefinedEmailsInfo();

        /// <summary>
        /// Create Update Pre-defined Emails List
        /// </summary>
        /// <param name="_preDefinedEmailsView"></param>
        /// <returns></returns>
        SystemMessageView CreateUpdatePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails);

        /// <summary>
        /// Delete Pre-defined Emails List 
        /// </summary>
        /// <param name="_preDefinedEmailsView"></param>
        /// <returns></returns>
        SystemMessageView DeletePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails);
    }

    public class MasterUserAccessGroupCountryEmailService : IMasterUserAccessGroupCountryEmailService
    {
        private readonly IMasterUserAccessGroupCountryEmailRepository _userAccessGroupCountryEmailRepositoy;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IBrinksService _brinksService;
        private readonly ISystemConfigurationAuditLogRepository _systemConfigurationAuditLogRepository;

        public MasterUserAccessGroupCountryEmailService(IMasterUserAccessGroupCountryEmailRepository userAccessGroupCountryEmailRepositoy,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow,
            IBrinksService brinksService,
            ISystemConfigurationAuditLogRepository systemConfigurationAuditLogRepository)
        {
            _userAccessGroupCountryEmailRepositoy = userAccessGroupCountryEmailRepositoy;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
            _brinksService = brinksService;
            _systemConfigurationAuditLogRepository = systemConfigurationAuditLogRepository;
        }



        public IEnumerable<PreDefinedEmailsView> GetPreDefinedEmailsInfo()
        {
            List<PreDefinedEmailsView> response = _userAccessGroupCountryEmailRepositoy.GetListUserAccessGroupCountryEmail().ConvertToPreDefinedEmailsView().ToList();
            response.ForEach(x => x.Country = new Service.ModelViews.Masters.CountryView
            {
                MasterCountry_Guid = x.CountryGuid,
                MasterCountryName = _brinksService.GetCountryDetail(x.CountryGuid).MasterCountryName
            });
            return response;
        }

        public SystemMessageView CreateUpdatePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    if (preDefinedEmails != null)
                    {
                        if (preDefinedEmails.UserAccessGroupCountryEmailGuid == null || preDefinedEmails.UserAccessGroupCountryEmailGuid == Guid.Empty)
                        {
                            PreDefinedEmailsView newitem = new PreDefinedEmailsView();
                            newitem.UserAccessGroupCountryEmailGuid = Guid.NewGuid();
                            newitem.AllowedEmailList = preDefinedEmails.AllowedEmailList;
                            newitem.CountryGuid = preDefinedEmails.Country.MasterCountry_Guid;
                            _userAccessGroupCountryEmailRepositoy.Create(newitem.ConvertToTblMasterUserAccessGroupCountryEmail());
                            SystemPreDefinedEmailsView newSystemItem = newitem.ConvertToSystemPreDefinedEmailsView();
                            newSystemItem.CountryName = _brinksService.GetCountryDetail(preDefinedEmails.Country.MasterCountry_Guid).MasterCountryName;
                            _systemConfigurationAuditLogRepository.CreateLogPredefinedMail(newSystemItem, 
                                EnumRoute.State.RowState_Add, preDefinedEmails.UserData.UserName);
                        }
                        else
                        {
                            var updateItem = _userAccessGroupCountryEmailRepositoy.FindById(preDefinedEmails.UserAccessGroupCountryEmailGuid);
                            if (updateItem != null)
                            {
                                SystemPreDefinedEmailsView logItem = new SystemPreDefinedEmailsView()
                                {
                                    AllowedEmailList = updateItem.EmailList,
                                    CountryName = _brinksService.GetCountryDetail(updateItem.MasterCountry_Guid).MasterCountryName
                                };
                                updateItem.EmailList = preDefinedEmails.AllowedEmailList;
                                updateItem.MasterCountry_Guid = preDefinedEmails.Country.MasterCountry_Guid;
                                SystemPreDefinedEmailsView newItem = updateItem.ConvertToSystemPreDefinedEmailsView();
                                newItem.CountryName = _brinksService.GetCountryDetail(preDefinedEmails.Country.MasterCountry_Guid).MasterCountryName;
                                _systemConfigurationAuditLogRepository.CreateLogPredefinedMail(newItem,
                                    EnumRoute.State.RowState_Edit, preDefinedEmails.UserData.UserName, logItem);
                            }
                            _userAccessGroupCountryEmailRepositoy.Modify(updateItem);
                        }
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(704, LanguageGuid).ConvertToMessageView(true);
                    transection.Complete();
                    return responseMsg;
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                    return responseMsg;
                }
            }
        }

        public SystemMessageView DeletePreDefinedEmails(PreDefinedEmailsRequest preDefinedEmails)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    var deleteItem = _userAccessGroupCountryEmailRepositoy.FindById(preDefinedEmails.UserAccessGroupCountryEmailGuid);
                    if (deleteItem != null)
                    {
                        _userAccessGroupCountryEmailRepositoy.Remove(deleteItem);
                        SystemPreDefinedEmailsView logItem = deleteItem.ConvertToSystemPreDefinedEmailsView();
                        logItem.CountryName = _brinksService.GetCountryDetail(deleteItem.MasterCountry_Guid).MasterCountryName;
                        _systemConfigurationAuditLogRepository.CreateLogPredefinedMail(logItem,
                                    EnumRoute.State.RowState_Delete, preDefinedEmails.UserData.UserName);
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(750, LanguageGuid).ConvertToMessageView(true).
                        ReplaceTextContentStringFormatWithValue("Predefined List Email by Ocean Online");
                    transection.Complete();
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
