using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
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
    public interface ISystemEnvironmentService
    {
        IEnumerable<SystemEnvironmentView> GetSystemEnvironmentInfo();

        SystemMessageView CreateUpdateSystemEnvironment(SystemEnvironmentRequest systemEnvironmentGlobal);

        SystemMessageView DeleteSystemEnvironmentGlobal(SystemEnvironmentRequest systemEnvironmentGlobal);
        
    }
    public class SystemEnvironmentService : ISystemEnvironmentService
    {
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironmentRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemConfigurationAuditLogRepository _systemConfigurationAuditLogRepository;

        public SystemEnvironmentService(ISystemEnvironment_GlobalRepository systemEnvironmentRepository,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow,
            ISystemConfigurationAuditLogRepository systemConfigurationAuditLogRepository)
        {
            _systemEnvironmentRepository = systemEnvironmentRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
            _systemConfigurationAuditLogRepository = systemConfigurationAuditLogRepository;
        }

        public IEnumerable<SystemEnvironmentView> GetSystemEnvironmentInfo()
        {
            return _systemEnvironmentRepository.GetListSystemEnviroment().ConvertToSystemEnvironmentViewListView().ToList();
        }

        public SystemMessageView CreateUpdateSystemEnvironment(SystemEnvironmentRequest systemEnvironmentGlobal) {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    SystemEnvironmentView item = new SystemEnvironmentView() {
                        SystemEnvironmentGuid = Guid.NewGuid(),
                        SystemEnvironmentAppKey = systemEnvironmentGlobal.SystemEnvironmentAppKey,
                        SystemEnvironmentAppDescription = systemEnvironmentGlobal.SystemEnvironmentAppDescription,
                        SystemEnvironmentAppValue1 = systemEnvironmentGlobal.SystemEnvironmentAppValue1,
                        SystemEnvironmentAppValue2 = systemEnvironmentGlobal.SystemEnvironmentAppValue2,
                        SystemEnvironmentAppValue3 = systemEnvironmentGlobal.SystemEnvironmentAppValue3,
                        SystemEnvironmentAppValue4 = systemEnvironmentGlobal.SystemEnvironmentAppValue4,
                        SystemEnvironmentAppValue5 = systemEnvironmentGlobal.SystemEnvironmentAppValue5,
                        SystemEnvironmentAppValue6 = systemEnvironmentGlobal.SystemEnvironmentAppValue6,
                        SystemEnvironmentAppValue7 = systemEnvironmentGlobal.SystemEnvironmentAppValue7,
                        SystemEnvironmentAppValue8 = systemEnvironmentGlobal.SystemEnvironmentAppValue8,
                        SystemEnvironmentAppValue9 = systemEnvironmentGlobal.SystemEnvironmentAppValue9,
                    };
                    if (systemEnvironmentGlobal != null)
                    {
                        if (systemEnvironmentGlobal.SystemEnvironmentGuid == null || systemEnvironmentGlobal.SystemEnvironmentGuid == Guid.Empty)
                        {
                            _systemEnvironmentRepository.Create(item.ConvertToTblSystemEnvironment_Global());
                            _systemConfigurationAuditLogRepository.CreateLogSystemEnvironmentGlobal(item.ConvertToSystemEnvironmentGlobalView(),
                                EnumRoute.State.RowState_Add, systemEnvironmentGlobal.UserData.UserName);
                        }
                        else
                        {
                            var updateItem = _systemEnvironmentRepository.FindById(systemEnvironmentGlobal.SystemEnvironmentGuid);
                            if (updateItem != null)
                            {
                                _systemConfigurationAuditLogRepository.CreateLogSystemEnvironmentGlobal(item.ConvertToSystemEnvironmentGlobalView(),
                                    EnumRoute.State.RowState_Edit, systemEnvironmentGlobal.UserData.UserName, updateItem.ConvertToSystemEnvironmentGlobalView());
                                updateItem.AppKey = systemEnvironmentGlobal.SystemEnvironmentAppKey;
                                updateItem.AppDescription = systemEnvironmentGlobal.SystemEnvironmentAppDescription;
                                updateItem.AppValue1 = systemEnvironmentGlobal.SystemEnvironmentAppValue1;
                                updateItem.AppValue2 = systemEnvironmentGlobal.SystemEnvironmentAppValue2;
                                updateItem.AppValue3 = systemEnvironmentGlobal.SystemEnvironmentAppValue3;
                                updateItem.AppValue4 = systemEnvironmentGlobal.SystemEnvironmentAppValue4;
                                updateItem.AppValue5 = systemEnvironmentGlobal.SystemEnvironmentAppValue5;
                                updateItem.AppValue6 = systemEnvironmentGlobal.SystemEnvironmentAppValue6;
                                updateItem.AppValue7 = systemEnvironmentGlobal.SystemEnvironmentAppValue7;
                                updateItem.AppValue8 = systemEnvironmentGlobal.SystemEnvironmentAppValue8;
                                updateItem.AppValue9 = systemEnvironmentGlobal.SystemEnvironmentAppValue9;
                            }
                            _systemEnvironmentRepository.Modify(updateItem);
                        }
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(711, LanguageGuid).ConvertToMessageView(true);
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

        public SystemMessageView DeleteSystemEnvironmentGlobal(SystemEnvironmentRequest systemEnvironmentGlobal) {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    var deleteItem = _systemEnvironmentRepository.FindById(systemEnvironmentGlobal.SystemEnvironmentGuid);
                    if (deleteItem != null)
                    {
                        _systemConfigurationAuditLogRepository.CreateLogSystemEnvironmentGlobal(deleteItem.ConvertToSystemEnvironmentGlobalView(),
                                    EnumRoute.State.RowState_Delete, systemEnvironmentGlobal.UserData.UserName);
                        _systemEnvironmentRepository.Remove(deleteItem);
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(750, LanguageGuid).ConvertToMessageView(true).
                        ReplaceTextContentStringFormatWithValue("System Environment Global Value by Ocean Online");
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
