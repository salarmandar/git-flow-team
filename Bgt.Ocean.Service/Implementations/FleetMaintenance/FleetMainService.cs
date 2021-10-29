using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System;

namespace Bgt.Ocean.Service.Implementations.FleetMaintenance
{
    public interface IFleetMainService
    {
        FleetMainResponse GetRunResourcesBySite(FleetMainRequest req);
    }
    public class FleetMainService : RequestBase, IFleetMainService
    {
        public readonly IMasterRunResourceRepository _masterRunResourceRepository;
        public readonly ISystemMessageRepository _systemMessageRepository;
        public readonly ISystemService _systemService;

        public FleetMainService(IMasterRunResourceRepository masterRunResourceRepository
            , ISystemMessageRepository systemMessageRepository
            , ISystemService systemService)
        {
            _masterRunResourceRepository = masterRunResourceRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemService = systemService;
        }

        public FleetMainResponse GetRunResourcesBySite(FleetMainRequest req)
        {
            var result = new FleetMainResponse();
            try
            {
                result.RunResourceList = _masterRunResourceRepository.GetRunResourcesBySite(req.SiteGuid);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, this.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }
    }
}
