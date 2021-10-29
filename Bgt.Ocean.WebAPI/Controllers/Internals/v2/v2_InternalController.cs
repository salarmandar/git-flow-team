using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;

using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.Users;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_InternalController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISystemService _systemService;
        public v2_InternalController(
            IUserService userService,
            ISystemService systemService)
        {
            _userService = userService;
            _systemService = systemService;
        }

        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DataStorage BrinksLogin(UserAuthenRequest request)
        {
            var authenDetails = _userService.GetUserAuthenTypeDetail(request.UserName, request.Password);
            if (authenDetails != null)
            {
                var domainGuid = _userService.GetDomainAile(authenDetails.Domain);
                if (domainGuid != null)
                {
                    var serverLDAP = _userService.GetDomainServer(domainGuid.Value);
                    if (serverLDAP == null)
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Your domain was not found."));

                    var userData = _userService.GetAuthenLDAP(authenDetails, serverLDAP);
                    if (userData != null)
                    {
                        userData.UserEmail = _userService.GetUserEmail(userData.UserGuid);
                        _userService.UpdateLastLogin(request.UserName, request.ClientDateTime, request.ClientSessionId);
                        _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, "Login successful.", request.UserName, SystemHelper.CurrentIpAddress,
                            ApiSession.Application_Guid.GetValueOrDefault());

                        userData.NumberCurrencyCultureCode = _systemService.GetNumberCurrencyCultureCode(userData.FormatNumberCurrency);
                        return userData;
                    }
                }
                else
                {
                    if (_userService.IsUserDomain(request.UserName))
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This username is domain user."));

                    var userData = _userService.GetAuthenLocal(request.UserName, request.Password, request.ApplicationId).ConvertAuthenResponseToDataStorage();
                    if (userData != null && !userData.FlagLock.GetValueOrDefault())
                    {
                        userData.UserEmail = _userService.GetUserEmail(userData.UserGuid);
                        _userService.UpdateLastLogin(request.UserName, request.ClientDateTime, request.ClientSessionId);
                        _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, "Login successful.", request.UserName, SystemHelper.CurrentIpAddress,
                            ApiSession.Application_Guid.GetValueOrDefault());

                        userData.NumberCurrencyCultureCode = _systemService.GetNumberCurrencyCultureCode(userData.FormatNumberCurrency);
                        return userData;
                    }
                    else if (userData != null && userData.FlagLock.GetValueOrDefault())
                    {
                        _userService.UpdateCountInvalidLogOn(request.UserName, request.ClientSessionId, request.ClientDateTime);
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("User {0} has been locked.", request.UserName)));
                    }
                }
            }

            var message = _userService.UpdateCountInvalidLogOn(request.UserName, request.ClientSessionId, request.ClientDateTime);
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
        }
    }
}
