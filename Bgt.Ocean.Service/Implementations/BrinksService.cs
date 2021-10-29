using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.InternalDepartments;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.BrinksService;
using Bgt.Ocean.Service.Messagings.MasterService;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.ModelViews.Masters;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Helpers.SystemHelper;

namespace Bgt.Ocean.Service.Implementations
{
    public interface IBrinksService
    {
        IEnumerable<CommodityView> GetAllCommodityBySite(Guid? siteGuid);
        IEnumerable<SystemJobHideScreenView> GetSystemJobHideScreenView();
        IEnumerable<BrinksCompanyResponse> GetCompanyByCountry(Guid? userGuid, int roleUser, Guid? countryGuid);
        IEnumerable<Messagings.BrinksService.CountryByUserResponse> GetCountryByUserHandle(string userName);
        IEnumerable<CustomerGeneralResponse> GetCustomer(Guid? siteGuid, Guid countryGuid, Guid? masterUserGuid, int roleType = 1);
        IEnumerable<CountryView> GetCountryListByUser(Guid userGuid, int roleId);
        IEnumerable<CountryView> GetCountryList();
        IEnumerable<CustomerGeneralResponse> GetCustomerInContract(Guid countryGuid, Guid? masterUserGuid, int roleType = 1);
        CustomerContractResponse GetCustomerContract(CustomerContractRequest request);
        CustomerContractRequest MapPropertySRForSearchContract(CustomerContractRequest request);
        IEnumerable<ProvinceStateView> GetProvinceStateByCountry(Guid countryGuid);
        IEnumerable<MasterDistrictResponse> GetDistrictCityByProviceState(Guid? provinceStateGuid, Guid? countryGuid = null, bool includeProvince = false);
        IEnumerable<DistrictView> GetDistrictCityByCountry(Guid countryGuid);
        CityView GetCityDetail(Guid countryGuid, Guid? cityGuid);
        IEnumerable<CurrencyView> GetCurrencyDependOnCountry(Guid countryGuid);
        IEnumerable<CurrencyView> GetCurrencyIncludeUSDByCountry(Guid countryGuid);
        IEnumerable<Nautilus_CurrencyView> Nautilus_GetCurrencyDependOnCountry(Guid countryGuid);
        BrinksSiteView GetBrinksSiteInfo(Guid masterSiteGuid);
        IEnumerable<CommodityView> GetAllCommodityByCountry(Guid countryGuid);
        CountryView GetCountryDetail(Guid countryGuid);
        SiteNetworkResponse GetDestinationSiteByOriginSiteOrLocation(SiteNetWorkRequest request);
        IEnumerable<BrinksSiteByUserResult> GetBrinksSiteByCompany(Guid? userGuid, int? roleTypeId, Guid? companyGuid = null, bool flagDestinationLocation = false, bool flagDestinationInternal = false, Guid? cusLocationPKGuid = null, Guid? siteGuid = null, bool isHubBrinksSite = false, bool isChangedHubBrinksite = false);
        IEnumerable<InternalDepartmentView> GetInternalDepartmentsInCompany(Guid company_Guid, int internalDepartmentTypeID);
        IEnumerable<SystemJobHideFieldView> GetSystemJobHideFieldView(Guid SystemJobHideScreen_Guid);
        IEnumerable<DisplayTextDropDownView> GetReasonType(Guid siteGuid);
        IEnumerable<CountryOptionView> FindListCountryOptionByAppKey(Guid? siteGuid, Guid? countryGuid, List<string> appkey);
        IEnumerable<CommodityView> GetCXandNonBarcodeBySite(Guid? siteGuid);
    }

    public class BrinksService : IBrinksService
    {
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCountryRepository _masterCountryRepository;
        private readonly IMasterCustomerContractRepository _masterCustomerContractRepository;
        private readonly IMasterCountry_StateRepository _masterCountry_StateRepository;
        private readonly IMasterCityRepository _masterCityRepository;
        private readonly IMasterDistrictRepository _masterDistrictRepository;
        private readonly IMasterCurrencyRepository _masterCurrencyRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly IMasterCommodityRepository _masterCommodityRepository;
        private readonly IMasterCommodityCountryRepository _masterCommodityCountryRepository;
        private readonly IMasterSiteNetworkRepository _masterSiteNetworkRepository;
        private readonly IMasterCustomerLocationInternalDepartmentRepository _masterCustomerLocationInternalDepartmentRepository;
        private readonly ISystemJobHideScreenRepository _systemJobHideScreenRepository;
        private readonly ISystemJobHideFieldRepository _systemJobHideFieldRepository;
        private readonly ISystemLineOfBusinessRepository _systemLineOfBusinessRepository;
        private readonly ISystemCustomerLocationTypeRepository _systemCustomerLocationTypeRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly IMasterReasonTypeRepository _masterReasonTypeRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;

        public BrinksService(
            IMasterCustomerRepository masterCustomerRepository,
            IMasterCountryRepository masterCountryRepository,
            IMasterCustomerContractRepository masterCustomerContractRepository,
            IMasterCountry_StateRepository masterCountry_StateRepository,
            IMasterCityRepository masterCityRepository,
            IMasterDistrictRepository masterDistrictRepository,
            IMasterCurrencyRepository masterCurrencyRepository,
            IMasterSiteRepository masterSiteRepository,
            IMasterCommodityRepository masterCommodityRepository,
            IMasterCommodityCountryRepository masterCommodityCountryRepository,
            IMasterSiteNetworkRepository masterSiteNetworkRepository,
            IMasterCustomerLocationInternalDepartmentRepository masterCustomerLocationInternalDepartmentRepository,
            ISystemJobHideScreenRepository systemJobHideScreenRepository,
            ISystemLineOfBusinessRepository systemLineOfBusinessRepository,
            ISystemCustomerLocationTypeRepository systemCustomerLocationTypeRepository,
            ISystemJobHideFieldRepository systemJobHideFieldRepository,
            ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository,
            IMasterReasonTypeRepository masterReasonTypeRepository,
            ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository
            )
        {
            _masterCustomerRepository = masterCustomerRepository;
            _masterCountryRepository = masterCountryRepository;
            _masterCustomerContractRepository = masterCustomerContractRepository;
            _masterCountry_StateRepository = masterCountry_StateRepository;
            _masterCityRepository = masterCityRepository;
            _masterDistrictRepository = masterDistrictRepository;
            _masterCurrencyRepository = masterCurrencyRepository;
            _masterSiteRepository = masterSiteRepository;
            _masterCommodityRepository = masterCommodityRepository;
            _masterCommodityCountryRepository = masterCommodityCountryRepository;
            _masterSiteNetworkRepository = masterSiteNetworkRepository;
            _masterCustomerLocationInternalDepartmentRepository = masterCustomerLocationInternalDepartmentRepository;
            _systemJobHideScreenRepository = systemJobHideScreenRepository;
            _systemLineOfBusinessRepository = systemLineOfBusinessRepository;
            _systemCustomerLocationTypeRepository = systemCustomerLocationTypeRepository;
            _systemJobHideFieldRepository = systemJobHideFieldRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _masterReasonTypeRepository = masterReasonTypeRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
        }

        public IEnumerable<SystemJobHideScreenView> GetSystemJobHideScreenView()
        {
            return _systemJobHideScreenRepository.FindAll(ApiSession.UserLanguage_Guid);
        }

        public IEnumerable<SystemJobHideFieldView> GetSystemJobHideFieldView(Guid SystemJobHideScreen_Guid)
        {
            return _systemJobHideFieldRepository.FindByHideScreen(ApiSession.UserLanguage_Guid, SystemJobHideScreen_Guid);
        }

        public SiteNetworkResponse GetDestinationSiteByOriginSiteOrLocation(SiteNetWorkRequest request)
        {
            SiteNetworkResponse result = new SiteNetworkResponse();
            result.PopupSiteNetworkData = _masterSiteNetworkRepository.GetDestinationSiteByOriginSiteOrLocation(request.SiteGuid, request.OriginCustomerLocation_Guid, request.ServiceJobTypeID);
            return result;
        }

        public IEnumerable<BrinksCompanyResponse> GetCompanyByCountry(Guid? userGuid, int roleUser, Guid? countryGuid)
        {
            var result = _masterCustomerRepository.Func_BrinksCompany_Get(userGuid, roleUser, null, countryGuid, null)
                .ConvertToBrinksCompanyResponse();
            return result.OrderBy(e => e.CustomerFullName);
        }

        public IEnumerable<Messagings.BrinksService.CountryByUserResponse> GetCountryByUserHandle(string userName)
        {
            return _masterCountryRepository.Func_System_CountryHandleByUser(userName).ConvertToCountryByUserResult();
        }

        /// <summary>
        /// Mandatory by country and role
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="countryGuid"></param>
        /// <param name="masterUserGuid"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public IEnumerable<CustomerGeneralResponse> GetCustomer(Guid? siteGuid, Guid countryGuid, Guid? masterUserGuid, int roleType = 1)
        {
            return _masterCustomerRepository.Func_Customer_General_Get(siteGuid, countryGuid, masterUserGuid, roleType).ConvertToCustomerGeneralResponse();
        }

        public IEnumerable<CountryView> GetCountryListByUser(Guid userGuid, int roleId)
        {
            var data = _masterCountryRepository.FindCountryFromBrinksHandle(userGuid, roleId, null);
            return data.Select(e => new CountryView()
            {
                MasterCountry_Guid = e.MasterCountry_Guid.GetValueOrDefault(),
                MasterCountryName = e.MasterCountryName,
                FlagHaveState = e.FlagHaveState,
                FlagInputCityManual = e.FlagInputCityManual
            }).OrderBy(e => e.MasterCountryName);
        }

        public IEnumerable<CountryView> GetCountryList()
        {
            return _masterCountryRepository.FindAll(e => !e.FlagDisable).OrderBy(e => e.MasterCountryName)
                .Select(e => new CountryView
                {
                    MasterCountry_Guid = e.Guid,
                    MasterCountryName = e.MasterCountryName,
                    MasterCountryAbbreviation = e.MasterCountryAbbreviation,
                    FlagHaveState = e.FlagHaveState,
                    FlagInputCityManual = e.FlagInputCityManual
                }).OrderBy(e => e.MasterCountryName);
        }

        public IEnumerable<CustomerGeneralResponse> GetCustomerInContract(Guid countryGuid, Guid? masterUserGuid, int roleType = 1)
        {
            var customers = _masterCustomerRepository.Func_Customer_General_Get(null, countryGuid, masterUserGuid, roleType);
            var contractSign = _masterCustomerContractRepository.FindAll(e => e.FlagSigned).Select(e => e.MasterCustomer_Guid);
            var contractCustomer = customers.Where(e => contractSign.Contains(e.Guid)).ConvertToCustomerGeneralResponse();
            return contractCustomer;
        }
        public CustomerContractResponse GetCustomerContract(CustomerContractRequest request)
        {
            CustomerContractResponse result = new CustomerContractResponse();
            result.ContractList = _masterCustomerContractRepository.FindCustomerContract(request.locationPKGuids, request.locationDLGuids, request.lobGuid, request.servicetypeGuid, request.subservicetypeGuid, request.workdatePK, request.workdateDL);
            result.IsSuccess = true;
            return result;
        }

        public CustomerContractRequest MapPropertySRForSearchContract(CustomerContractRequest request)
        {
            Guid lobGuid = Guid.Empty;
            int? ExternalID = _systemCustomerLocationTypeRepository.FindAll().Where(e => e.Guid == request.lobGuid).Select(s => s.CustomerLocationTypeID).FirstOrDefault();
            switch (ExternalID)
            {
                case 1: // ATM
                    lobGuid = _systemLineOfBusinessRepository.FindAll().Where(e => e.InternalID == 2).Select(s => s.Guid).FirstOrDefault();
                    break;
                case 3: // ComputeSafe
                    lobGuid = _systemLineOfBusinessRepository.FindAll().Where(e => e.InternalID == 4).Select(s => s.Guid).FirstOrDefault();
                    break;
            }
            request.lobGuid = lobGuid;
            return request;
        }

        public IEnumerable<ProvinceStateView> GetProvinceStateByCountry(Guid countryGuid)
        {
            var tblCountry = _masterCountryRepository.FindById(countryGuid);
            if (tblCountry.FlagHaveState == true)
            {
                var response = _masterCountry_StateRepository.FindByCountry(countryGuid)
                             .Select(x => new ProvinceStateView
                             {
                                 ProvinceStateGuid = x.Guid,
                                 ProvinceStateName = x.MasterStateName,
                                 ProvinceStateNameAbb = x.MasterStateAbbreviation,
                                 FlagHaveState = tblCountry.FlagHaveState
                             });
                return response;
            }
            else
            {
                var response = _masterCityRepository.FineByCountry(countryGuid)
                                .Select(x => new ProvinceStateView
                                {
                                    ProvinceStateGuid = x.Guid,
                                    ProvinceStateName = x.MasterCityName,
                                    ProvinceStateNameAbb = x.MasterCityAbbreviation,
                                    FlagHaveState = tblCountry.FlagHaveState
                                });
                return response;
            }
        }

        public IEnumerable<MasterDistrictResponse> GetDistrictCityByProviceState(Guid? provinceStateGuid, Guid? countryGuid = null, bool includeProvince = false)
        {
            Debug.Assert(provinceStateGuid.HasValue != countryGuid.HasValue, "If provinceStateGuid has value, countryGuid is not need.");

            List<TblMasterDistrict> allDistrictInProvince = new List<TblMasterDistrict>();
            if (countryGuid != null && provinceStateGuid == null)
            {
                var allProvince = GetProvinceStateByCountry(countryGuid.Value);

                allDistrictInProvince = _masterDistrictRepository.FindAll(e => !e.FlagDisable).Join(allProvince,
                    d => d.MasterCity_Guid.HasValue ? d.MasterCity_Guid : d.MasterCountry_State_Guid,
                    p => p.ProvinceStateGuid,
                    (d, p) => new { d, p }).Select(e => e.d).ToList();
            }
            else if (provinceStateGuid != null)
            {
                allDistrictInProvince = _masterDistrictRepository.FindByProvinceState(provinceStateGuid.Value).ToList();
            }

            if (includeProvince)
            {
                allDistrictInProvince.ForEach(
                    e => e.ProvinceStateName =
                        e.MasterCity_Guid.HasValue ?
                        _masterCityRepository.FindById(e.MasterCity_Guid)?.MasterCityName :
                        _masterCountry_StateRepository.FindById(e.MasterCountry_State_Guid)?.MasterStateName
                );
            }

            return allDistrictInProvince.ConvertToMasterDistrictResponse();
        }

        public IEnumerable<DistrictView> GetDistrictCityByCountry(Guid countryGuid)
        {
            var allProvince = GetProvinceStateByCountry(countryGuid);
            var allDistrictInProvince = _masterDistrictRepository.FindAll(e => !e.FlagDisable).Join(allProvince,
                d => d.MasterCity_Guid.HasValue ? d.MasterCity_Guid : d.MasterCountry_State_Guid,
                p => p.ProvinceStateGuid,
                (d, p) => new { d, p }).Select(e => e.d).ToList();

            allDistrictInProvince.ForEach(
                e => e.ProvinceStateName = e.MasterCity_Guid.HasValue ?
                _masterCityRepository.FindById(e.MasterCity_Guid)?.MasterCityName :
                _masterCountry_StateRepository.FindById(e.MasterCountry_State_Guid)?.MasterStateName);

            return allDistrictInProvince.ConvertToDistrictView();
        }

        public CityView GetCityDetail(Guid countryGuid, Guid? cityGuid)
        {
            CityView city = new CityView();
            if (cityGuid != null)
            {
                var districtCity = _masterDistrictRepository.FindById(cityGuid);
                var provinceGuid = Guid.Empty;
                if (districtCity.MasterCity_Guid.HasValue)
                    provinceGuid = districtCity.MasterCity_Guid.Value;
                else if (districtCity.MasterCountry_State_Guid.HasValue)
                    provinceGuid = districtCity.MasterCountry_State_Guid.Value;

                if (provinceGuid != Guid.Empty)
                {
                    city.MasterProvince_Guid = provinceGuid;
                    var searchCountryInCity = _masterCityRepository.FindById(provinceGuid);
                    if (searchCountryInCity != null)
                        city.MasterCountry_Guid = searchCountryInCity.MasterCountry_Guid;
                    else
                    {
                        var searchCountryInCountryState = _masterCountry_StateRepository.FindById(provinceGuid);
                        if (searchCountryInCountryState != null)
                            city.MasterCountry_Guid = searchCountryInCountryState.MasterCountry_Guid;
                    }
                }
                city.Guid = cityGuid.Value;
            }
            else
            {
                city.MasterCountry_Guid = countryGuid;
            }

            return city;
        }

        public IEnumerable<CurrencyView> GetCurrencyDependOnCountry(Guid countryGuid)
        {
            return _masterCurrencyRepository.FindByCountryGuid(countryGuid).ConvertToCurrencyView();
        }

        public IEnumerable<CurrencyView> GetCurrencyIncludeUSDByCountry(Guid countryGuid)
        {

            var usdGuid = (_systemEnvironment_GlobalRepository.FindByAppKey(EnumAppKey.DefaultCurrency).AppValue1 ?? string.Empty).ToGuid();
            var result = _masterCurrencyRepository.FindByCountryGuid(countryGuid).ConvertToCurrencyView();

            if (!result.Any(e => e.Guid == usdGuid))
            {
                var usd = _masterCurrencyRepository.FindAll(e => e.Guid == usdGuid).ConvertToCurrencyView();
                result = result.Union(usd);
            }

            return result.OrderBy(e => e.MasterCurrencyAbbreviation);
        }

        public IEnumerable<Nautilus_CurrencyView> Nautilus_GetCurrencyDependOnCountry(Guid countryGuid)
        {
            return _masterCurrencyRepository.FindByCountryGuid(countryGuid).ConvertToNautilus_CurrencyView();
        }


        public BrinksSiteView GetBrinksSiteInfo(Guid masterSiteGuid)
        {
            try
            {
                var brinksSite = _masterSiteRepository.FindById(masterSiteGuid);

                return brinksSite.ConvertToBrinksSiteView();
            }
            catch (Exception err)
            {
                throw new ArgumentException($"MasterSiteGuid: {masterSiteGuid} not found", err);
            }


        }
        public IEnumerable<CommodityView> GetAllCommodityBySite(Guid? siteGuid)
        {
            var result = _masterCommodityRepository.GetAllCommodityBySite(siteGuid);

            return result.Select(o => new CommodityView
            {
                Guid = o.MasterCommodity_Guid.GetValueOrDefault(),
                CommodityName = o.CommodityName,
                CommodityValue = o.CommodityValue,
                CommodityAmount = o.CommodityAmount
            });
        }


        public IEnumerable<CommodityView> GetAllCommodityByCountry(Guid countryGuid)
        {
            var commodities = _masterCommodityRepository.FindAll(x => !x.FlagDisable);
            var commoditiesGlobal = commodities.Where(e => e.FlagCommodityGlobal).ConvertToCommodityView();
            var commoditiesGuids = commodities.Where(e => !e.FlagCommodityGlobal).Select(x => x.Guid);
            var commoditiesCountry = _masterCommodityCountryRepository.FindAll(e => e.MasterCountry_Guid == countryGuid && commoditiesGuids.Contains(e.MasterCommodity_Guid)).ConvertToCommodityView();
            var result = commoditiesGlobal.Union(commoditiesCountry).OrderBy(o => o.CommodityGroupName.Contains("CX") ? 0 : 1).ThenBy(o => o.ColumnInReport).ThenBy(o => o.CommodityName);
            return result;
        }

        public CountryView GetCountryDetail(Guid countryGuid)
        {
            var country = _masterCountryRepository.FindById(countryGuid).ConvertToCountryView();
            return country;
        }

        public IEnumerable<BrinksSiteByUserResult> GetBrinksSiteByCompany(Guid? userGuid, int? roleTypeId, Guid? companyGuid = null, bool flagDestinationLocation = false, bool flagDestinationInternal = false, Guid? cusLocationPKGuid = null, Guid? siteGuid = null, bool isHubBrinksSite = false, bool isChangedHubBrinksite = false)
        {
            var allBrinksSite = _masterSiteRepository.Func_BrinksSite_Get(userGuid, roleTypeId, companyGuid, flagDestinationLocation, flagDestinationInternal, cusLocationPKGuid, isHubBrinksSite, siteGuid, isChangedHubBrinksite).ToList();

            if (companyGuid != null && companyGuid != Guid.Empty)
            {
                allBrinksSite = allBrinksSite.Where(e => e.CustomerGuid == companyGuid).ToList();
            }

            return allBrinksSite;
        }

        public IEnumerable<InternalDepartmentView> GetInternalDepartmentsInCompany(Guid company_Guid, int internalDepartmentTypeID)
        {
            return _masterCustomerLocationInternalDepartmentRepository.GetInternalDepartmentsInCompany(company_Guid, internalDepartmentTypeID);
        }

        public IEnumerable<DisplayTextDropDownView> GetReasonType(Guid siteGuid)
        {
            return _masterReasonTypeRepository.FindBySiteGuid(siteGuid)
                .Select(o => new DisplayTextDropDownView
                {
                    Guid = o.Guid,
                    DisplayText = o.ReasonTypeName
                });
        }

        public IEnumerable<CountryOptionView> FindListCountryOptionByAppKey(Guid? siteGuid, Guid? countryGuid, List<string> appkey)
        {
            return _systemEnvironmentMasterCountryRepository.FindListCountryOptionByAppKey(siteGuid, countryGuid, appkey);
        }

        public IEnumerable<CommodityView> GetCXandNonBarcodeBySite(Guid? siteGuid)
        {
            var result = _masterCommodityRepository.GetAllCommodityBySite(siteGuid).Where(
                o => (!o.FlagReqDetail && o.FlagRequireSeal == false) || (o.FlagRequireSeal == false && o.FlagReqDetail)
            );

            return result.Select(o => new CommodityView
            {
                Guid = o.MasterCommodity_Guid.GetValueOrDefault(),
                CommodityName = o.CommodityName,
                CommodityValue = o.CommodityValue,
                CommodityAmount = o.CommodityAmount
            });
        }

        
    }
}
