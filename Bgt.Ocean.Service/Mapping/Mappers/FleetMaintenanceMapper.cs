using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class FleetMaintenanceMapper
    {
        public static TblMasterRunResource_GasolineExpense ConvertToTblMasterRunResourceGasoline(this FleetGasolineDataRequest model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<FleetGasolineDataRequest, TblMasterRunResource_GasolineExpense>(model);
        }
    }
}
