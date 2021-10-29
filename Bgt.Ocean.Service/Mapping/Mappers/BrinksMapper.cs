using Bgt.Ocean.Models;
using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Service.Messagings.BrinksService;
using Bgt.Ocean.Service.ModelViews.Masters;
using Bgt.Ocean.Service.ModelViews.Users;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class BrinksMapper
    {
        public static IEnumerable<DistrictView> ConvertToDistrictView(this IEnumerable<TblMasterDistrict> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterDistrict>, IEnumerable<DistrictView>>(model);
        }

        public static IEnumerable<CurrencyView> ConvertToCurrencyView(this IEnumerable<TblMasterCurrency> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterCurrency>, IEnumerable<CurrencyView>>(model);
        }
        public static IEnumerable<Nautilus_CurrencyView> ConvertToNautilus_CurrencyView(this IEnumerable<TblMasterCurrency> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterCurrency>, IEnumerable<Nautilus_CurrencyView>>(model);
        }

        public static DataStorage ConvertToDataStorage(this AuthenLoginResult authen)
        {
            return ServiceMapperBootstrapper.MapperService.Map<AuthenLoginResult, DataStorage>(authen);
        }

        public static BrinksSiteView ConvertToBrinksSiteView(this TblMasterSite model)
            => ServiceMapperBootstrapper.MapperService.Map<TblMasterSite, BrinksSiteView>(model);

        public static IEnumerable<BrinksCompanyResponse> ConvertToBrinksCompanyResponse(this IEnumerable<BrinksCompanyResult> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<BrinksCompanyResult>, IEnumerable<BrinksCompanyResponse>>(model);

        public static IEnumerable<CountryByUserResponse> ConvertToCountryByUserResult(this IEnumerable<CountryByUserResult> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CountryByUserResult>, IEnumerable<CountryByUserResponse>>(model);

        public static IEnumerable<CommodityView> ConvertToCommodityView(this IEnumerable<TblMasterCommodity> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterCommodity>, IEnumerable<CommodityView>>(model);
        public static IEnumerable<CommodityView> ConvertToCommodityView(this IEnumerable<TblMasterCommodityCountry> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterCommodityCountry>, IEnumerable<CommodityView>>(model);
        public static CountryView ConvertToCountryView(this TblMasterCountry model)
            => ServiceMapperBootstrapper.MapperService.Map<TblMasterCountry, CountryView>(model);
        public static IEnumerable<LobView> ConvertToLOBView(this IEnumerable<TblSystemLineOfBusiness> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemLineOfBusiness>, IEnumerable<LobView>>(model);
        
    }
}
