using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Service.ModelViews.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_CustomerLocationController : ApiControllerBase
    {
        private readonly IMasterCustomerLocationService _masterCustomerLocationService;
        public v1_CustomerLocationController(IMasterCustomerLocationService masterCustomerLocationService)
        {
            _masterCustomerLocationService = masterCustomerLocationService;

        }
        /// <summary>
        /// Get Customer location
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<AdhocCustomerLocationView> GetAdhocCustomerLocationByCustomer(AdhocCustomerLocationRequest request)
        {
            return _masterCustomerLocationService.GetAdhocCustomerLocation(request);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<LocationEmailActionView> GetLocationsEmail(LocationEmailRequest request)
        {
            if (request != null && request.CustomerGuid.Any())
            {
                var locationEmail = Task.Run(() => _masterCustomerLocationService.GetLocationsForEmailAction(request));
                var locationList = locationEmail.Result;
                return locationList;
            }
            return new List<LocationEmailActionView>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<LocationEmailActionView> GetLocationsEmailByCustomer(Guid customerGuid)
        {
            var locationEmail = Task.Run(() => _masterCustomerLocationService.GetLocationsForEmailActionByCustomerGuid(customerGuid));
            var locationList = locationEmail.Result;
            return locationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView SaveLocationEmailAction(IEnumerable<LocationEmailActionView> request)
        {
            return _masterCustomerLocationService.SaveLocationEmailAction(request);
        }
        /// <summary>
        /// Open popup change customer location
        /// </summary>
        /// <param name="locationGuid"></param>
        /// <returns></returns>
        public ChangeCustomerview GetChangeCustomer(Guid locationGuid)
        {
            return _masterCustomerLocationService.GetChangeCustomer(locationGuid);
        }
        /// <summary>
        /// SaveChangeCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView SaveChangeCustomer(SaveChangeCustomerRequest request)
        {
            var validateDuplicateLocation = _masterCustomerLocationService.ValidateBeForeSave(request.LocationGuid, request.CustomerGuid);
            if (validateDuplicateLocation != null)
            {
                return validateDuplicateLocation;
            }
            var result = _masterCustomerLocationService.SaveChangeCustomer(request.LocationGuid, request.CustomerGuid);
            return result;
        }
    }
}
