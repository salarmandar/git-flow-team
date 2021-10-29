using Bgt.Ocean.Models;
using Bgt.Ocean.Service.ModelViews.Machine;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class MachineMapper
    {
        public static MachineModel ConvertToMachineModel(this Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result src)
            => ServiceMapperBootstrapper.MapperService.Map<Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result, MachineModel>(src);
    }
}
