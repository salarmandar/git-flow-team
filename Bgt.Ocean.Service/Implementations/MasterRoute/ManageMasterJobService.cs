
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using System;

namespace Bgt.Ocean.Service.Implementations.MasterRoute
{
    public interface IManageMasterJobService
    {
        GetMasterRouteDeliveryLegResponse GetMasterRouteDeliveryLeg(GetMasterRouteDeliveryLegRequest req);
    }
    public class ManageMasterJobService : IManageMasterJobService
    {
        public readonly IMasterRouteJobHeaderRepository _masterRouteJobHeaderRepository;
        public readonly ISystemMessageRepository _systemMessageRepository;
        public readonly ISystemService _systemService;
        public readonly IBaseRequest _req;
        public ManageMasterJobService(IMasterRouteJobHeaderRepository masterRouteJobHeaderRepository,
             ISystemMessageRepository systemMessageRepository,
             ISystemService systemService,
             IBaseRequest req
            )
        {
            _masterRouteJobHeaderRepository = masterRouteJobHeaderRepository;
            _systemService = systemService;
            _req = req;
        }

        public GetMasterRouteDeliveryLegResponse GetMasterRouteDeliveryLeg(GetMasterRouteDeliveryLegRequest req)
        {
            GetMasterRouteDeliveryLegResponse result = new GetMasterRouteDeliveryLegResponse();
            try
            {
                result.MasterRouteDeliveryLegList = _masterRouteJobHeaderRepository.GetMasterRouteDeliveryLegs(req.MasterJobGuids);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }
    }
}
