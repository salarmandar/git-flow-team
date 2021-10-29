
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.InternalDepartments;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Messagings.BrinksService;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.ModelViews.Masters;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.WebAPI.Models.Brinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_BrinksController : ApiControllerBase
    {
        private readonly IBrinksService _brinksService;
        private readonly ISystemService _systemService;
        private readonly IMasterDenominationRepository _masterDenominationRepository;
        public v1_BrinksController(
            BrinksService brinksService,
            ISystemService systemService,
            IMasterDenominationRepository masterDenominationRepository)
        {
            _brinksService = brinksService;
            _systemService = systemService;
            _masterDenominationRepository = masterDenominationRepository;
        }

        /// <summary>
        /// Get brink's company list by country
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <param name="userGuid"></param>
        /// <param name="userRoleType"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<BrinksCompanyResponse> GetBrinksCompanyByCountry(Guid countryGuid, Guid userGuid, int userRoleType)
        {
            var brinksCompanyResult = _brinksService.GetCompanyByCountry(userGuid, userRoleType, countryGuid);
            return brinksCompanyResult;
        }

        [HttpGet]
        public SystemMessageView GetMessageByMsgID(int msgId, Guid? languageGuid = null)
        {
            if (languageGuid != null)
                return _systemService.GetMessage(msgId, languageGuid.GetValueOrDefault());

            return _systemService.GetMessageByMsgId(msgId);
        }

        /// <summary>
        /// Max row search is use on query maximum rows by this value
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public int GetDefaultMaxRow(Guid countryGuid)
        {
            var maxRow = _systemService.GetDefultMaxRow(null, countryGuid);
            return maxRow;
        }

        #region Country & City & District
        /// <summary>
        /// Get country list by user handle
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCountryByUserHandle(string userName)
        {
            var status = _brinksService.GetCountryByUserHandle(userName);
            return Request.CreateResponse(status);
        }

        /// <summary>
        /// Get country of brinks by user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CountryView> GetCountryListByUser(Guid userGuid, int roleId)
        {
            return _brinksService.GetCountryListByUser(userGuid, roleId);
        }

        /// <summary>
        /// Use in popup select city
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CountryView> GetCountryList()
        {
            var countryList = _brinksService.GetCountryList();
            return countryList;
        }

        /// <summary>
        /// Get customer list in country
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <param name="userGuid"></param>
        /// <param name="roleId"></param>
        /// <param name="haveContractOnly"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CustomerGeneralResponse> GetCustomerList(Guid countryGuid, Guid userGuid, int roleId, bool haveContractOnly = false)
        {
            if (haveContractOnly) // contract signed only
                return _brinksService.GetCustomerInContract(countryGuid, userGuid, roleId);
            else
                return _brinksService.GetCustomer(null, countryGuid, userGuid, roleId);
        }
        [HttpPost]
        public CustomerContractResponse GetCustomerContract(CustomerContractRequest request)
        {
            return _brinksService.GetCustomerContract(request);
        }

        [HttpPost]
        public CustomerContractResponse GetCustomerServiceRequestContract(CustomerContractRequest request)
        {
            // Convert DateTime from UTC to Client DateTime
            request.workdateDL = request.workdateDL.GetValueOrDefault().AddHours(request.ClientDateTime.Offset.Hours);
            request = _brinksService.MapPropertySRForSearchContract(request);
            return _brinksService.GetCustomerContract(request);
        }

        [HttpGet]
        public IEnumerable<ProvinceStateView> GetProvinceStateListByCountry(Guid countryGuid)
        {
            var provinceList = _brinksService.GetProvinceStateByCountry(countryGuid);
            return provinceList;
        }

        [HttpGet]
        public IEnumerable<DistrictView> GetDistrictCityListByProviceState(Guid? provinceGuid)
        {
            var districtList = _brinksService.GetDistrictCityByProviceState(provinceGuid, null).Select(x => new DistrictView()
            {
                Guid = x.Guid,
                MasterDistrictName = x.MasterDistrictName
            });
            return districtList;
        }

        [HttpGet]
        public IEnumerable<DistrictView> GetDistrictCityInCountry(Guid countryGuid)
        {
            var districtList = _brinksService.GetDistrictCityByCountry(countryGuid);
            return districtList;
        }

        [HttpGet]
        public CityView GetCityDetail(Guid countryGuid, Guid? cityGuid)
        {
            var cityData = _brinksService.GetCityDetail(countryGuid, cityGuid);
            return cityData;
        }
        #endregion

        [HttpGet]
        public IEnumerable<ServiceJobTypeView> GetSystemServiceJobTypeList(Guid languageGuid)
        {
            var serviceJobTypeData = _systemService.GetServiceJobTypes(languageGuid);
            return serviceJobTypeData;
        }

        /// <summary>
        /// Get currency by country
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CurrencyView> GetCurrencyListByCountry(Guid countryGuid)
        {
            return _brinksService.GetCurrencyDependOnCountry(countryGuid);
        }

        [HttpGet]
        public IEnumerable<CurrencyView> GetCurrencyIncludeUSDByCountry(Guid countryGuid)
        {
            return _brinksService.GetCurrencyIncludeUSDByCountry(countryGuid);
        }

        /// <summary>
        /// Get all commodity by country
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CommodityView> GetCommodityListByCountry(Guid countryGuid)
        {
            return _brinksService.GetAllCommodityByCountry(countryGuid);
        }

        [HttpGet]
        public IEnumerable<CommodityView> GetAllCommodityBySite(Guid siteGuid)
        {
            return _brinksService.GetAllCommodityBySite(siteGuid);
        }

        [HttpGet]
        public IEnumerable<CommodityView> GetCXandNonBarcodeBySite(Guid? siteGuid)
        {
            return _brinksService.GetCXandNonBarcodeBySite(siteGuid.Value);
        }

        [HttpPost]
        public SiteNetworkResponse GetDestinationSiteByOriginSiteOrLocation(SiteNetWorkRequest request)
        {
            return _brinksService.GetDestinationSiteByOriginSiteOrLocation(request);
        }

        /// <summary>
        /// Get Brink's Site List by Brink's Company
        /// </summary>
        /// <param name="CompanyGuid"></param>
        /// <param name="UserGuid"></param>
        /// <param name="RoleTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<BrinksSiteByUserResult> GetBrinksSiteByCompany(Guid CompanyGuid, Guid UserGuid, int RoleTypeId)
        {
            return _brinksService.GetBrinksSiteByCompany(UserGuid, RoleTypeId, CompanyGuid);
        }

        #region Vault

        /// <summary>
        /// Internal Department Type
        /// 1 Vault
        /// 2 Pre-Vault
        /// 3 Room
        /// </summary>
        /// <param name="company_Guid"></param>
        /// <param name="internalDepartmentTypeID"></param>
        public IEnumerable<InternalDepartmentView> GetInternalDepartmentsInCompany(Guid company_Guid, int internalDepartmentTypeID)
        {
            return _brinksService.GetInternalDepartmentsInCompany(company_Guid, internalDepartmentTypeID);
        }
        #endregion

        /// <summary>
        /// Get all colocation
        /// </summary>
        /// <param name="Currency_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDenomenationByCurrency(Guid Currency_Guid)
        {
            var result = _masterDenominationRepository.GetDenoByCurrency(Currency_Guid);
            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

        }

        [HttpGet]
        public IEnumerable<SystemJobHideScreenView> GetSystemJobHideScreenView()
        {
            return _brinksService.GetSystemJobHideScreenView();
        }


        [HttpGet]
        public IEnumerable<SystemJobHideFieldView> GetSystemJobHideFieldView(Guid? SystemJobHideScreen_Guid)
        {
            return _brinksService.GetSystemJobHideFieldView(SystemJobHideScreen_Guid.GetValueOrDefault());
        }

        /// <summary>
        /// Get reason type for summary popup
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DisplayTextDropDownView> GetReasonType(Guid siteGuid)
        {
            return _brinksService.GetReasonType(siteGuid);
        }

        [HttpPost]
        public HttpResponseMessage FindListCountryOptionByAppKey(CountryOptionRequestModel request)
        {
            return Request.CreateResponse(_brinksService.FindListCountryOptionByAppKey(request.SiteGuid, request.countryGuid, request.Appkey));
        }


    }
}