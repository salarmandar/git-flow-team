using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Nemo.NemoSync;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class NemoSyncMapper
    {
        public static SyncSystemServiceJobTypeRequest ConvertToNemoServiceJobTypeRequest(this TblSystemServiceJobType model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblSystemServiceJobType, SyncSystemServiceJobTypeRequest>(model);
        }

        public static List<SyncSystemServiceJobTypeRequest> ConvertToNemoServiceJobTypeRequest(this List<TblSystemServiceJobType> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<List<TblSystemServiceJobType>, List<SyncSystemServiceJobTypeRequest>>(model);
        }
    }
}
