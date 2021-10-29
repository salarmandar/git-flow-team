using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class VaultBalanceMapper
    {
        public static IEnumerable<VaultBalanceSealModelView> ConvertToVaultBalanceSealView(this IEnumerable<VaultBalanceSealModel> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<VaultBalanceSealModel>, IEnumerable<VaultBalanceSealModelView>>(model);
        }
    }
}
