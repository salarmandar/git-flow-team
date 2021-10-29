using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.PreVault;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class PreVaultMapper
    {
        public static IEnumerable<CheckOutDeptInternalDeptModelView> ConvertToInternalDeptModelView(this IEnumerable<CustomerLocationInternalDepartmentModel> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CustomerLocationInternalDepartmentModel>, IEnumerable<CheckOutDeptInternalDeptModelView>>(model);
        }
    }
}
