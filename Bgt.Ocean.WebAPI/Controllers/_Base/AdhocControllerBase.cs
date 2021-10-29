using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Messagings.AdhocService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.WebAPI.Controllers._Base
{
    public class AdhocControllerBase : ApiControllerBase
    {
        protected readonly IAdhocService _adhocService;
        private readonly IMachineService _machineService;

        public AdhocControllerBase(
                IAdhocService adhocService,
                IMachineService machineService
            )
        {
            _adhocService = adhocService;
            _machineService = machineService;
        }

        protected CreateJobAdHocResponse ValidateAndProcessRequest(CreateJobAdHocRequest request, Func<Guid[]> fnGetRun, Func<CreateJobAdHocResponse> processRequest)
        {
            var run = fnGetRun();
            var isDailyRunUnderAlarm = _adhocService.CheckDailyRunResourceUnderAlarm(-817, request.LanguagueGuid, run);

            if (isDailyRunUnderAlarm != null)
            {
                return isDailyRunUnderAlarm;
            }

            try
            {
                var deliveryLocations = request.ServiceStopLegDelivery?.LocationByLegGuid ?? new List<Guid?>();
                var pickupLocations = request.ServiceStopLegPickup?.LocationByLegGuid ?? new List<Guid?>();

                var allLocation = deliveryLocations.Union(pickupLocations).Select(e => e.GetValueOrDefault());
                var isHasCrypto = _machineService.IsMachineHasCryptoLock(allLocation.ToArray());

                if (isHasCrypto)
                {
                    return _adhocService.CreateResponseFromID(-17373, request.LanguagueGuid);
                }
            }
            catch { /* not handle */ }


            return processRequest();
        }
    }
}