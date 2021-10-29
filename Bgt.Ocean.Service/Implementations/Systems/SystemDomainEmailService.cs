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
    #region Interface

    public interface ISystemDomainEmailService
    {

        IEnumerable<EmailDomainsView> GetEmailDomainsInfo();

        SystemMessageView CreateUpdateEmailDomains(EmailDomainsRequest EmailDomain);

        SystemMessageView DeleteEmailDomain(EmailDomainsRequest EmailDomain);

    }

    #endregion

    public class SystemDomainEmailService : ISystemDomainEmailService
    {
        public readonly ISystemEmailDomainsRepository _systemEmailDomainsRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemConfigurationAuditLogRepository _systemConfigurationAuditLogRepositoryRepository;

        public SystemDomainEmailService(ISystemEmailDomainsRepository systemEmailDomainsRepository,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow,
            ISystemConfigurationAuditLogRepository systemConfigurationAuditLogRepositoryRepository)
        {
            _systemEmailDomainsRepository = systemEmailDomainsRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
            _systemConfigurationAuditLogRepositoryRepository = systemConfigurationAuditLogRepositoryRepository;
        }

        public IEnumerable<EmailDomainsView> GetEmailDomainsInfo()
        {
            return _systemEmailDomainsRepository.GetListSystemDomainEmailWhiteList().ConvertToSystemDomainEmailWhiteListView().ToList();
        }

        public SystemMessageView CreateUpdateEmailDomains(EmailDomainsRequest EmailDomain)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    if (EmailDomain != null)
                    {
                        EmailDomainsView updateDetail = new EmailDomainsView();

                        if (EmailDomain.AllowedDomain_Guid == null || EmailDomain.AllowedDomain_Guid == Guid.Empty)
                        {
                            updateDetail.AllowedDomain_Guid = Guid.NewGuid();
                            updateDetail.AllowedDomain = EmailDomain.AllowedDomain;
                            _systemEmailDomainsRepository.Create(updateDetail.ConvertToTblSystemDomainEmailWhite());
                            _systemConfigurationAuditLogRepositoryRepository.CreateLogDomainMail(updateDetail.ConvertToSystemDomainEmailView(), 
                                EnumRoute.State.RowState_Add, EmailDomain.UserData.UserName);
                        }
                        else
                        {
                            var updateItem = _systemEmailDomainsRepository.FindById(EmailDomain.AllowedDomain_Guid);
                            SystemEmailDomainsView updatelog = new SystemEmailDomainsView() {
                                AllowedDomain = EmailDomain.AllowedDomain
                            };
                            _systemConfigurationAuditLogRepositoryRepository.CreateLogDomainMail(updateItem.ConvertToSystemDomainEmailView(),
                                EnumRoute.State.RowState_Edit, EmailDomain.UserData.UserName, updatelog);
                            if (updateItem != null)
                            {
                                updateItem.DomainEmail = EmailDomain.AllowedDomain;
                            }
                            _systemEmailDomainsRepository.Modify(updateItem);
                        }
                    }
                    _uow.Commit();
                    
                    responseMsg = _systemMessageRepository.FindByMsgId(703, LanguageGuid).ConvertToMessageView(true);
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

        public SystemMessageView DeleteEmailDomain(EmailDomainsRequest EmailDomain)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    var deleteItem = _systemEmailDomainsRepository.FindById(EmailDomain.AllowedDomain_Guid);
                    _systemEmailDomainsRepository.Remove(deleteItem);
                    _systemConfigurationAuditLogRepositoryRepository.CreateLogDomainMail(deleteItem.ConvertToSystemDomainEmailView(),
                                EnumRoute.State.RowState_Delete, EmailDomain.UserData.UserName);
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(750, LanguageGuid).ConvertToMessageView(true).
                        ReplaceTextContentStringFormatWithValue("E-mail Domain by Ocean Online");
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
