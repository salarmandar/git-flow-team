using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Implementations.Systems;
using Bgt.Ocean.Service.Messagings.CustomerService;
using Bgt.Ocean.Service.ModelViews.Customer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_CustomerController : ApiController
    {
        private readonly IMasterCustomerService _masterCustomerService;
        private readonly ISystemEmailActionService _systemEmailActionService;
        public v1_CustomerController(
            IMasterCustomerService masterCustomerService,
            ISystemEmailActionService systemEmailActionService)
        {
            _masterCustomerService = masterCustomerService;
            _systemEmailActionService = systemEmailActionService;
        }

        /// <summary>
        /// GetCustomer
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public IEnumerable<AdhocCustomerView> GetAdhocCustomer(RequestAdhocCustomer request)
        {
            var response = _masterCustomerService.GetAdhocCustomer(request);
            return response;
        }

        /// <summary>
        /// Get all account in customer
        /// </summary>
        /// <param name="masterCustomer_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<AccountView> GetAccountsInCustomer(Guid masterCustomer_Guid)
        {
            return _masterCustomerService.GetAccountsInCustomer(masterCustomer_Guid);
        }

        /// <summary>
        /// Get all location in customer
        /// </summary>
        /// <param name="masterCustomer_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<LocationView> GetLocationsInCustomer(Guid masterCustomer_Guid)
        {
            return _masterCustomerService.GetLocationsInCustomer(masterCustomer_Guid);
        }

        [HttpGet]
        public IEnumerable<CustomerView> GetCustomerBySiteGuid(Guid siteGuid)
        {
            var customer = Task.Run(() => _masterCustomerService.GetCustomerBySiteGuid(siteGuid));
            return customer.Result;
        }


        [HttpGet]
        public IEnumerable<CustomerEmailActionModel> GetEmailActions(Guid languageGuid)
        {
            return _systemEmailActionService.GetEmailAction(languageGuid);
        }
    }
}
