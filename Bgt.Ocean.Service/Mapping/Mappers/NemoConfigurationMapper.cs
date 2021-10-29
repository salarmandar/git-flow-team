using Bgt.Ocean.Models;
using Bgt.Ocean.Models.NemoConfiguration;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class NemoConfigurationMapper
    {
        public static NemoCountryValueView ConvertToNemoCountryValueView(this TblMasterNemoCountryValue model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblMasterNemoCountryValue, NemoCountryValueView>(model);
        }

        public static NemoSiteValueView ConvertToNemoSiteValueView(this TblMasterNemoSiteValue model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblMasterNemoSiteValue, NemoSiteValueView>(model);
        }

        public static IEnumerable<NemoTrafficFactorValueView> ConvertToNemoTrafficFactorValueView(this IEnumerable<TblMasterNemoTrafficFactorValue> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterNemoTrafficFactorValue>, IEnumerable<NemoTrafficFactorValueView>>(model);
        }

        public static TblMasterNemoCountryValue ConvertToTblMasterNemoCountryValue(this NemoCountryValueView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<NemoCountryValueView, TblMasterNemoCountryValue>(model);
        }

        public static TblMasterNemoSiteValue ConvertToTblMasterNemoSiteValue(this NemoSiteValueView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<NemoSiteValueView, TblMasterNemoSiteValue>(model);
        }

        public static IEnumerable<TblMasterNemoTrafficFactorValue> ConvertToTblMasterNemoTrafficValue(this IEnumerable<NemoTrafficFactorValueView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<NemoTrafficFactorValueView>, IEnumerable<TblMasterNemoTrafficFactorValue>>(model);
        }

        public static NemoApplyToSiteView ConvertToNemoApplyToSiteView(this NemoApplyToSiteRequest model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<NemoApplyToSiteRequest, NemoApplyToSiteView>(model);
        }
    }
}
