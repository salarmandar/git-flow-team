using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Service.Messagings.BrinksService;
using Bgt.Ocean.Service.Messagings.CustomerService;
using Bgt.Ocean.Service.ModelViews.Customer;
using Bgt.Ocean.Service.ModelViews.CustomerLocation;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class CustomerMapper
    {
        public static IEnumerable<CustomerGeneralResponse> ConvertToCustomerGeneralResponse(this IEnumerable<CustomerGeneralResult> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CustomerGeneralResult>, IEnumerable<CustomerGeneralResponse>>(model);

        public static IEnumerable<AdhocCustomerView> ConvertToAdhocCustomerView(this IEnumerable<AdhocCustomerResult> model) => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<AdhocCustomerResult>, IEnumerable<AdhocCustomerView>>(model);
        public static AdhocCustomerRequest ConvertToAdhocCustomerRequest(this RequestAdhocCustomer mode)
            => ServiceMapperBootstrapper.MapperService.Map<RequestAdhocCustomer, AdhocCustomerRequest>(mode);

        public static IEnumerable<AdhocCustomerLocationView> ConvertToAdhocCustomerLocationView(this IEnumerable<AdhocLocationByCustomerResult> model) => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<AdhocLocationByCustomerResult>, IEnumerable<AdhocCustomerLocationView>>(model);

    }
}
