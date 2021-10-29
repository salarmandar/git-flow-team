using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.PricingRules;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_SalesController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISaleService _saleService;
        public v2_SalesController(
            IUserService userService,
            ISaleService saleService)
        {
            _userService = userService;
            _saleService = saleService;
        }

        /// <summary>
        /// Get sales list from Ocean API
        /// </summary>
        /// <param name="company_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserView> GetSalesInCompany(Guid company_Guid)
        {
            var allSale = _userService.GetSalePersons(company_Guid);
            return allSale;
        }

        [HttpGet]
        public IEnumerable<CustomerGeneralResult> GetCustomersInTeamUser(Guid user_Guid, Guid country_Guid)
        {
            var customers = _saleService.GetCustomersInTeamUser(user_Guid, country_Guid);
            return customers;
        }
    }
}
