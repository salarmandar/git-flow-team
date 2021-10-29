using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.Users;
using System;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    /// <summary>
    /// This service provide method for get User of Type DataStorage from SFOTblSystemDataConfiguration_Country
    /// </summary>
    public interface ISFOApiUserService
    {
        DataStorage GetUserByConfiguration(Guid countryGuid, string userAppKey = SFOSystemDataConfigurationDataKeyHelper.SFO_API_USER);
    }

    public class SFOApiUserService : ISFOApiUserService
    {
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly ISFOSystemDataConfigurationRepository _sfoSystemDataConfigurationRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLogHistoryError;

        public SFOApiUserService(
                ISystemLog_HistoryErrorRepository systemLogHistoryError,
                ISFOSystemDataConfigurationRepository sfoSystemDataConfigurationRepository,
                IMasterUserRepository masterUserRepository
            )
        {
            _systemLogHistoryError = systemLogHistoryError;
            _sfoSystemDataConfigurationRepository = sfoSystemDataConfigurationRepository;
            _masterUserRepository = masterUserRepository;
        }

        public DataStorage GetUserByConfiguration(Guid countryGuid, string userAppKey = SFOSystemDataConfigurationDataKeyHelper.SFO_API_USER)
        {
            try
            {
                var userInfo = GetAuthenLogin(countryGuid, userAppKey)
                    .ConvertToDataStorage();

                return userInfo;
            }
            catch (Exception ex)
            {
                _systemLogHistoryError.CreateHistoryError(ex);
                throw ex;
            }
        }

        private AuthenLoginResult GetAuthenLogin(Guid countryGuid, string userAppKey = SFOSystemDataConfigurationDataKeyHelper.SFO_API_USER)
        {
            var systemConfig = _sfoSystemDataConfigurationRepository.GetByDataKey(userAppKey, countryGuid);


            if (systemConfig == null || systemConfig.DataValue1.IsEmpty() || systemConfig.DataValue2.IsEmpty())
                throw new Exception($"Configuration not found, please check SFOSystemDataConfiguration.DataKey == {userAppKey}");

            var userData = _masterUserRepository.Func_AuthenLogin_Get(systemConfig.DataValue1, systemConfig.DataValue2, SystemHelper.Ocean_ApplicationId);

            if (userData == null)
                throw new Exception($"Username not found, please check SFOSystemDataConfiguration_Country.DataKey == {userAppKey} and CountryGuid == {countryGuid}");

            return userData;
        }
    }
}
