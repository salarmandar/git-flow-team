using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Service.Messagings.MasterService;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class MasterMapper
    {
        public static IEnumerable<MasterDistrictResponse> ConvertToMasterDistrictResponse(this IEnumerable<TblMasterDistrict> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterDistrict>, IEnumerable<MasterDistrictResponse>>(model);

        public static MasterImageView ConvertToMasterImageView(this TblMasterImage model)
            => ServiceMapperBootstrapper.MapperService.Map<MasterImageView>(model);
       
    }
}
