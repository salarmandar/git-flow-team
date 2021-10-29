
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_SystemController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISystemService _systemService;
        public v1_SystemController(
            IUserService userService,
            ISystemService systemService)
        {
            _userService = userService;
            _systemService = systemService;
        }

        [HttpGet]
        public HttpResponseMessage ChangeLanguage(Guid languageId, Guid userId)
        {
            try
            {
                _userService.ChangeLanguage(userId, languageId);
                return Request.CreateErrorResponse(HttpStatusCode.OK, "Change languate complete.");
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Change language failure.");
            }
        }

        [HttpGet]
        public IEnumerable<LanguageView> GetLanguageList()
        {
            var languageList = _systemService.GetLanguageList();
            return languageList;
        }

        [HttpGet]
        public IEnumerable<SystemGlobalUnitView> GetWeightUnitList()
        {
            var weightUnitList = _systemService.GetSystemWeigthUnitList();
            return weightUnitList;
        }
        [HttpGet]
        public IEnumerable<LineOfBusinessAndJobType> GetLineOfBusinessAndJobType()
        {
            var response = _systemService.GetLineOfBusinessAndJobType();
            return response;
        }


        [HttpGet]
        public IEnumerable<ServiceJobTypeView> GetSystemServiceJobTypeList(Guid lobGuid)
        {
            var serviceJobTypeData = _systemService.GetServiceJobTypes(null, lobGuid);
            return serviceJobTypeData;
        }


        [HttpGet]
        public IEnumerable<SystemDayOfWeekView> GetSystemDayOfWeekList()
        {
            return _systemService.GetSystemDayOfWeekList();
        }

        [HttpGet]
        public IEnumerable<ServiceJobTypeView> GetServiceJobTypeListOfLobList(List<String> lobGuids)
        {
            List<ServiceJobTypeView> allServiceJobTypeByLob;
            List<ServiceJobTypeView> allServiceJobTypeExistInBoth;
            List<ServiceJobTypeView> allServiceJobTypeResult = new List<ServiceJobTypeView>();

            if (lobGuids != null)
            {
                foreach (String lobGuid in lobGuids)
                {
                    allServiceJobTypeByLob = _systemService.GetServiceJobTypes(ApiSession.UserLanguage_Guid, new Guid(lobGuid)).ToList();

                    if (allServiceJobTypeResult.Count == 0 && allServiceJobTypeByLob != null && allServiceJobTypeByLob.Count > 0)
                    {
                        allServiceJobTypeResult = new List<ServiceJobTypeView>(allServiceJobTypeByLob);
                    }
                    else
                    {
                        allServiceJobTypeExistInBoth = allServiceJobTypeResult.Where(X => allServiceJobTypeByLob.Any(Y => X.Guid == Y.Guid)).ToList();
                        allServiceJobTypeResult = new List<ServiceJobTypeView>(allServiceJobTypeExistInBoth);
                    }
                }
            }

            return allServiceJobTypeResult;
        }

        [HttpGet]
        public SystemEnvironmentView GetAppKey(string appKey)
        {
            return _systemService.GetAppKey(appKey).ConvertToSystemEnvironmentView();
        }

        [HttpGet]
        public IEnumerable<SystemMaterRouteTypeOfWeekView> GetSystemMasterRouteTypeOfWeek()
        {
            return _systemService.GetSystemMasterRouteTypeOfWeek();
        }
    }
}
