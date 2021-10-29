using static Bgt.Ocean.Infrastructure.Util.EnumGlobalEnvironment;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;

namespace Bgt.Ocean.Service.Implementations.Configuration
{
    public interface ISettingsService
    {
        #region Sync jobs to dolphin
        string DolphinUser { get; }
        string DolphinPwd { get; }
        string DolphinUrl { get; }
        string DolphinPushDisalarm { get; }
        string DolphinHostName { get; }
        #endregion
     
    }
    public class SettingsService : ISettingsService
    {
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironmentGlobalRepository;
        public SettingsService(ISystemEnvironment_GlobalRepository systemEnvironmentGlobalRepository)
        {
            _systemEnvironmentGlobalRepository = systemEnvironmentGlobalRepository;
        }

        #region Sync jobs to dolphin
        public string DolphinUser
        {
            get { return _systemEnvironmentGlobalRepository.FindByAppKey(appkey.Dolphin_Jobs.IsStaging()).AppValue2; }
        }

        public string DolphinPwd
        {
            get { return _systemEnvironmentGlobalRepository.FindByAppKey(appkey.Dolphin_Jobs.IsStaging()).AppValue3; }
        }

        public string DolphinUrl
        {
            get { return _systemEnvironmentGlobalRepository.FindByAppKey(appkey.Dolphin_Jobs.IsStaging()).AppValue4; }
        }

        public string DolphinPushDisalarm
        {
            get { return _systemEnvironmentGlobalRepository.FindByAppKey(appkey.Dolphin_Jobs.IsStaging()).AppValue5; }
        }

        public string DolphinHostName
        {
            get { return _systemEnvironmentGlobalRepository.FindByAppKey(appkey.Dolphin_Jobs.IsStaging()).AppValue6; }
        }
        #endregion
    }
}
