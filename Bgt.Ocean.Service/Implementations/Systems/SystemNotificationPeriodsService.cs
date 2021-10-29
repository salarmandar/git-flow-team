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
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    public interface ISystemNotificationPeriodsService
    {
        IEnumerable<NotificationConfigPeriodsView> GetNotificationPeriodsInfo(Guid countryGuid, bool flagDisable);
        SystemMessageView CreateUpdateNotificationPeriods(NotificationConfigPeriodsRequest notificationPeriods);
        NotificationConfigPeriodsView SearchGetNofiticacionConfigPeriodById(Guid Guid);
        SystemMessageView DisableOrEnableNotificationConfigPeriod(NotificationConfigPeriodsRequest notificationPeriods);
        IEnumerable<UserView> GetInfoGlobalAdmin();

    }
    public class SystemNotificationPeriodsService : ISystemNotificationPeriodsService
    {
        private readonly ISystemNotificationConfigPeriodsRepository _systemNotificationConfigPeriodsRepository;
        private readonly ISystemNotificationConfigPeriodsUsersRepository _systemNotificationConfigPeriodsUsersRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly ISystemEnvironmentMasterCountryValueRepository _systemEnvironmentMasterCountryValueRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IUserService _userService;
        private readonly ISystemConfigurationAuditLogRepository _systemConfigurationAuditLogRepository;

        public SystemNotificationPeriodsService(ISystemNotificationConfigPeriodsRepository systemNotificationConfigPeriods,
            ISystemNotificationConfigPeriodsUsersRepository systemNotificationConfigPeriodsUsersRepository,
            ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository,
            ISystemEnvironmentMasterCountryValueRepository systemEnvironmentMasterCountryValueRepository,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IUnitOfWork<OceanDbEntities> uow,
            IUserService userService,
            ISystemConfigurationAuditLogRepository systemConfigurationAuditLogRepository)
        {
            _systemNotificationConfigPeriodsRepository = systemNotificationConfigPeriods;
            _systemNotificationConfigPeriodsUsersRepository = systemNotificationConfigPeriodsUsersRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _systemEnvironmentMasterCountryValueRepository = systemEnvironmentMasterCountryValueRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
            _userService = userService;
            _systemConfigurationAuditLogRepository = systemConfigurationAuditLogRepository;
        }

        public IEnumerable<NotificationConfigPeriodsView> GetNotificationPeriodsInfo(Guid countryGuid, bool flagDisable)
        {
            return _systemNotificationConfigPeriodsRepository.GetListSystemNotificationConfigPeriod(countryGuid, flagDisable).ConvertToNotificationConfigPeriodsView().ToList();
        }

        public SystemMessageView CreateUpdateNotificationPeriods(NotificationConfigPeriodsRequest notificationPeriods)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var enviromentMasterCountryGuid = _systemEnvironmentMasterCountryRepository.FindAll(e => e.AppKey == "daysForSendAlert").FirstOrDefault()?.Guid;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    if (notificationPeriods != null)
                    {
                        if (notificationPeriods.NotificationConfigPeriodsGuid == null || notificationPeriods.NotificationConfigPeriodsGuid == Guid.Empty)
                        {
                            if (enviromentMasterCountryGuid != null)
                            {
                                Guid newNotificationConfigPeriod = Guid.NewGuid();
                                var insertSystemNotificationConfigPeriod = new TblSystemNotificationConfigPeriods()
                                {
                                    Guid = newNotificationConfigPeriod,
                                    PeriodTitle = notificationPeriods.PeriodTitle,
                                    StartDate = notificationPeriods.InitialDate,
                                    EndDate = notificationPeriods.FinalDate,
                                    SystemEnvironmentMasterCountry_Guid = enviromentMasterCountryGuid.GetValueOrDefault(),
                                    MasterCountry_Guid = notificationPeriods.Country,
                                    DatetimeCreated = notificationPeriods.UserData.LocalClientDateTime,
                                    UniversalDatetimeCreated = date,
                                    UserCreated = notificationPeriods.UserData.UserName
                                };

                                _systemNotificationConfigPeriodsRepository.Create(insertSystemNotificationConfigPeriod);
                                // Insert Detail Users
                                if (notificationPeriods.NotificationPeriodDetail.Any())
                                {
                                    InsertNotificationConfigPeriodUsers(notificationPeriods.NotificationPeriodDetail, newNotificationConfigPeriod);
                                }
                                var environmentMasterCountryValueData = _systemEnvironmentMasterCountryValueRepository.
                                                FindAll(e => e.MasterCountry_Guid == notificationPeriods.Country &&
                                                e.SystemEnvironmentMasterCountry_Guid == enviromentMasterCountryGuid).FirstOrDefault();
                                if (environmentMasterCountryValueData != null)
                                {
                                    environmentMasterCountryValueData.AppValue1 = (notificationPeriods.NotifyBeforeDueDate).ToString();
                                    environmentMasterCountryValueData.AppValue2 = (notificationPeriods.DaysBeforeDueDate).ToString();
                                    _systemEnvironmentMasterCountryValueRepository.Modify(environmentMasterCountryValueData);
                                }
                                else
                                {
                                    environmentMasterCountryValueData = new TblSystemEnvironmentMasterCountryValue
                                    {
                                        Guid = Guid.NewGuid(),
                                        AppValue1 = (notificationPeriods.NotifyBeforeDueDate).ToString(),
                                        AppValue2 = (notificationPeriods.DaysBeforeDueDate).ToString(),
                                        MasterCountry_Guid = notificationPeriods.Country,
                                        SystemEnvironmentMasterCountry_Guid = enviromentMasterCountryGuid.Value
                                    };
                                    _systemEnvironmentMasterCountryValueRepository.Create(environmentMasterCountryValueData);
                                }
                                SystemNotificationConfigPeriodsView systemNotificationConfigPeriodsView = new SystemNotificationConfigPeriodsView()
                                {
                                    NotificationConfigPeriodsGuid = newNotificationConfigPeriod,
                                    PeriodTitle = notificationPeriods.PeriodTitle,
                                    InitialDate = notificationPeriods.InitialDate,
                                    FinalDate = notificationPeriods.FinalDate,
                                    DaysBeforeDueDate = notificationPeriods.DaysBeforeDueDate,
                                    NotifyBeforeDueDate = notificationPeriods.NotifyBeforeDueDate
                                };
                                foreach (var emailUser in notificationPeriods.NotificationPeriodDetail)
                                {
                                    systemNotificationConfigPeriodsView.textMailList += string.Format("{0} ({1}), ", emailUser.Email, emailUser.UserName);
                                }
                                if(systemNotificationConfigPeriodsView.textMailList != "")
                                {
                                    systemNotificationConfigPeriodsView.textMailList = systemNotificationConfigPeriodsView.textMailList.Substring(0, systemNotificationConfigPeriodsView.textMailList.Length - 2);
                                }
                                _systemConfigurationAuditLogRepository.CreateLogConfigNotificationPeriods(systemNotificationConfigPeriodsView, EnumRoute.State.RowState_Add,
                                    notificationPeriods.UserData.UserName);
                            }
                            else
                            {
                                responseMsg = _systemMessageRepository.FindByMsgId(-2066, LanguageGuid).ConvertToMessageView();
                                return responseMsg;
                            }
                        }
                        else
                        {
                            //UPDATE data
                            var updateItem = _systemNotificationConfigPeriodsRepository.FindById(notificationPeriods.NotificationConfigPeriodsGuid);
                            SystemNotificationConfigPeriodsView oldSystemNotificationConfigPeriodsView = new SystemNotificationConfigPeriodsView()
                            {
                                NotificationConfigPeriodsGuid = updateItem.Guid,
                                PeriodTitle = updateItem.PeriodTitle,
                                InitialDate = updateItem.StartDate,
                                FinalDate = updateItem.EndDate
                            };
                            updateItem.PeriodTitle = notificationPeriods.PeriodTitle;
                            updateItem.StartDate = notificationPeriods.InitialDate;
                            updateItem.EndDate = notificationPeriods.FinalDate;
                            updateItem.DatetimeModified = notificationPeriods.DatetimeCreated;
                            updateItem.UniversalDatetimeModified = date;
                            updateItem.UserModifed = notificationPeriods.UserData.UserName;
                            _systemNotificationConfigPeriodsRepository.Modify(updateItem);

                            var environmentMasterCountryValueData = _systemEnvironmentMasterCountryValueRepository.
                                                FindAll(e => e.MasterCountry_Guid == notificationPeriods.Country && e.SystemEnvironmentMasterCountry_Guid == enviromentMasterCountryGuid).FirstOrDefault();
                            if (environmentMasterCountryValueData != null)
                            {
                                oldSystemNotificationConfigPeriodsView.NotifyBeforeDueDate = Convert.ToInt32(environmentMasterCountryValueData.AppValue1);
                                oldSystemNotificationConfigPeriodsView.DaysBeforeDueDate = Convert.ToInt32(environmentMasterCountryValueData.AppValue2);
                                environmentMasterCountryValueData.AppValue1 = (notificationPeriods.NotifyBeforeDueDate).ToString();
                                environmentMasterCountryValueData.AppValue2 = (notificationPeriods.DaysBeforeDueDate).ToString();
                                _systemEnvironmentMasterCountryValueRepository.Modify(environmentMasterCountryValueData);
                            }
                            // Remove and Insert Details
                            var removeDetailUserNotification = _systemNotificationConfigPeriodsUsersRepository.FindAll(e => e.SystemNotificationConfigPeriods_Guid == notificationPeriods.NotificationConfigPeriodsGuid);
                            foreach (var emailUser in removeDetailUserNotification)
                            {
                                oldSystemNotificationConfigPeriodsView.textMailList += string.Format("{0} ({1}), ", emailUser.EscalationEmail, emailUser.UserName);
                            }
                            _systemNotificationConfigPeriodsUsersRepository.RemoveRange(removeDetailUserNotification);
                            if (notificationPeriods.NotificationPeriodDetail != null && notificationPeriods.NotificationPeriodDetail.Any())
                            {
                                InsertNotificationConfigPeriodUsers(notificationPeriods.NotificationPeriodDetail, notificationPeriods.NotificationConfigPeriodsGuid);
                            }
                            SystemNotificationConfigPeriodsView newSystemNotificationConfigPeriodsView = new SystemNotificationConfigPeriodsView()
                            {
                                PeriodTitle = updateItem.PeriodTitle,
                                InitialDate = updateItem.StartDate,
                                FinalDate = updateItem.EndDate,
                                NotifyBeforeDueDate = Convert.ToInt32(environmentMasterCountryValueData?.AppValue1),
                                DaysBeforeDueDate = Convert.ToInt32(environmentMasterCountryValueData?.AppValue2)
                            };
                            foreach (var emailUser in notificationPeriods.NotificationPeriodDetail)
                            {
                                newSystemNotificationConfigPeriodsView.textMailList += string.Format("{0} ({1}), ", emailUser.Email, emailUser.UserName);
                            }
                            if (oldSystemNotificationConfigPeriodsView.textMailList != "")
                            {
                                oldSystemNotificationConfigPeriodsView.textMailList = oldSystemNotificationConfigPeriodsView.textMailList.Substring(0, oldSystemNotificationConfigPeriodsView.textMailList.Length - 2);
                            }
                            if (newSystemNotificationConfigPeriodsView.textMailList != "")
                            {
                                newSystemNotificationConfigPeriodsView.textMailList = newSystemNotificationConfigPeriodsView.textMailList.Substring(0, newSystemNotificationConfigPeriodsView.textMailList.Length - 2);
                            }
                            _systemConfigurationAuditLogRepository.CreateLogConfigNotificationPeriods(newSystemNotificationConfigPeriodsView, EnumRoute.State.RowState_Edit,
                                notificationPeriods.UserData.UserName, oldSystemNotificationConfigPeriodsView);
                        }
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(2052, LanguageGuid).ConvertToMessageView(true);
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

        private void InsertNotificationConfigPeriodUsers(List<NotificationConfigPeriodsUsers> DetailUsersNotification,
            Guid notificationConfigGuid)
        {
            foreach (var rowDetail in DetailUsersNotification)
            {
                var itemDetail = new TblSystemNotificationConfigPeriodsUsers
                {
                    Guid = Guid.NewGuid(),
                    SystemNotificationConfigPeriods_Guid = notificationConfigGuid,
                    EscalationEmail = rowDetail.Email,
                    UserName = rowDetail.UserName,
                    ExternalUser = rowDetail.IsExternalGlobalAdmin,
                };
                _systemNotificationConfigPeriodsUsersRepository.Create(itemDetail);
            }
        }

        public NotificationConfigPeriodsView SearchGetNofiticacionConfigPeriodById(Guid Guid)
        {
            NotificationConfigPeriodsView result = _systemNotificationConfigPeriodsRepository.GetSystemNotificationConfigPeriodById(Guid).ConvertToNotificationConfigPeriodsView().FirstOrDefault();
            result.NotificationPeriodDetail = new List<NotificationConfigPeriodsUsers>();
            var lstDetail = _systemNotificationConfigPeriodsUsersRepository.FindAll(e => e.SystemNotificationConfigPeriods_Guid == Guid);
            foreach (var item in lstDetail)
            {
                NotificationConfigPeriodsUsers ListUsers = new NotificationConfigPeriodsUsers();
                ListUsers.GlobalUserGuid = item.Guid;
                ListUsers.SystemNotificationConfigPeriodsGuid = item.SystemNotificationConfigPeriods_Guid;
                ListUsers.Email = item.EscalationEmail;
                ListUsers.UserName = String.IsNullOrEmpty(item.UserName) ? string.Empty : item.UserName;
                ListUsers.IsExternalGlobalAdmin = item.ExternalUser ?? false;
                result.NotificationPeriodDetail.Add(ListUsers);
            }
            return result;
        }

        public SystemMessageView DisableOrEnableNotificationConfigPeriod(NotificationConfigPeriodsRequest notificationPeriods)
        {
            SystemMessageView responseMsg = null;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            using (var transaction = _uow.BeginTransaction())
            {
                try
                {
                    NotificationConfigPeriodsView ncpToEnableOrDisable = this.SearchGetNofiticacionConfigPeriodById(notificationPeriods.NotificationConfigPeriodsGuid);
                    if (ncpToEnableOrDisable != null)
                    {
                        var updateItem = _systemNotificationConfigPeriodsRepository.FindById(ncpToEnableOrDisable.NotificationConfigPeriodsGuid);
                        _systemConfigurationAuditLogRepository.CreateLogConfigNotificationPeriods(new SystemNotificationConfigPeriodsView() { PeriodTitle = ncpToEnableOrDisable.PeriodTitle, FlagDisable = ncpToEnableOrDisable.FlagDisable },
                            (notificationPeriods.FlagDisable ? "Disable" : "Enable"), notificationPeriods.UserData.UserName, new SystemNotificationConfigPeriodsView() { PeriodTitle = ncpToEnableOrDisable.PeriodTitle, FlagDisable = notificationPeriods.FlagDisable });
                        updateItem.FlagDisable = notificationPeriods.FlagDisable;
                        updateItem.SystemEnvironmentMasterCountry_Guid = ncpToEnableOrDisable.SystemEnvironmentMasterCountry_Guid;
                        updateItem.UserModifed = notificationPeriods.UserData.UserName;
                        updateItem.DatetimeModified = notificationPeriods.UserData.LocalClientDateTime;
                        updateItem.UniversalDatetimeModified = date;
                        _systemNotificationConfigPeriodsRepository.Modify(updateItem);
                        
                    }
                    _uow.Commit();
                    responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView(true);
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-998, LanguageGuid).ConvertToMessageView();
                }
            }
            return responseMsg;
        }

        public IEnumerable<UserView> GetInfoGlobalAdmin()
        {
            return _userService.GetUserDetailByRoleType(EnumUser.RoleType.GlobalAdmin);
        }
    }
}
