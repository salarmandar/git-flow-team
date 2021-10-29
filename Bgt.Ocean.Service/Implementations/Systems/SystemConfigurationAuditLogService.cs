using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    public interface ISystemConfigurationAuditLogService
    {

        IEnumerable<ConfigurationAuditLogView> GetSystemLogInfo();

    }
    public class SystemConfigurationAuditLogService : ISystemConfigurationAuditLogService
    {
        private readonly ISystemConfigurationAuditLogRepository _systemConfigurationAuditLogRepository;

        public SystemConfigurationAuditLogService(ISystemConfigurationAuditLogRepository systemConfigurationAuditLogRepository)
        {
            _systemConfigurationAuditLogRepository = systemConfigurationAuditLogRepository;
        }

        public IEnumerable<ConfigurationAuditLogView> GetSystemLogInfo() {
            return _systemConfigurationAuditLogRepository.GetSystemLogList().ConvertToConfigurationAuditLogListView().ToList();
        }
    }
}
