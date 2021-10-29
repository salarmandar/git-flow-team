using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using Bgt.Ocean.Service.ModelViews.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FleetMainController : ApiControllerBase
    {
        private readonly IFleetMainService _fleetMainService;
        private readonly IBrinksService _brinksService;
        public v1_FleetMainController(IFleetMainService fleetMainService
            , IBrinksService brinksService)
        {
            _fleetMainService = fleetMainService;
            _brinksService = brinksService;
        }

        [HttpPost]
        public FleetMainResponse GetRunResourcesBySite(FleetMainRequest req)
        {
            var result = _fleetMainService.GetRunResourcesBySite(req);
            return result;
        }

        [HttpGet]
        public IEnumerable<DropdownViewModel<CurrencyView>> GetCurrencyByCountryGuid(Guid? countryGuid)
        {

            var result = _brinksService.GetCurrencyDependOnCountry(countryGuid.GetValueOrDefault()).Select(o => new DropdownViewModel<CurrencyView>
            {
                //Obj = o,
                Text = o.MasterCurrencyAbbreviation,
                Value = o.MasterCurrency_Guid.ToString()
            });

            return result;
        }

    }
}