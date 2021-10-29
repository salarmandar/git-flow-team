using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_InternalController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISystemService _systemService;
        public v1_InternalController(
            IUserService userService,
            ISystemService systemService)
        {
            _userService = userService;
            _systemService = systemService;
        }

        /// <summary>
        /// body:
        /// {
        ///     "userName": "ChanakanG",
        ///     "Password": "Brinks0716*"
        /// }
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DataStorage BrinksLogin(UserAuthenRequest request)
        {
            DataStorage response = new DataStorage();
            Bgt.Ocean.Service.ModelViews.Users.AuthenDetails authenDetails = _userService.GetAuthenCredential(request.UserName, request.Password);
            if (authenDetails != null)
            {
                switch (authenDetails.AuthenType)
                {
                    case EnumUser.AuthenType.Domain:
                    case EnumUser.AuthenType.Email:
                        {
                            var domain = _userService.GetDomain(authenDetails.Domain);
                            if (domain != null)
                            {
                                var domainDC = _userService.GetDomainDC(domain.Guid);
                                response = _userService.GetAuthenLDAP(authenDetails, domainDC.LdapAuthPath, request.ApplicationId).ConvertAuthenResponseToDataStorage();
                            }
                            break;
                        }
                    case EnumUser.AuthenType.Local:
                        {
                            response = _userService.GetAuthenLocal(authenDetails.FullUsername, authenDetails.Password, SystemHelper.Ocean_ApplicationId).ConvertAuthenResponseToDataStorage();
                            break;
                        }
                }
            }

            // authentication failure
            if (response != null)
            {
                if (response.FlagLock.GetValueOrDefault())
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, string.Format("User {0} has been locked.", request.UserName)));
                }
            }
            else
            {
                var message = _userService.UpdateCountInvalidLogOn(request.UserName, request.ClientSessionId, request.ClientDateTime);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message));
            }

            _userService.UpdateLastLogin(request.UserName, request.ClientDateTime, request.ClientSessionId);
            _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, "Login successful.", request.UserName, SystemHelper.CurrentIpAddress,
                ApiSession.Application_Guid.GetValueOrDefault());
            return response;
        }

        /// <summary>
        /// Get menus,
        /// To get all menu set applicationID = null (by default)
        /// </summary>
        /// <param name="masterUser_Guid"></param>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<MasterMenuDetailResponse> GetMenuListInUser(Guid masterUser_Guid, int? applicationID = null)
        {
            var menuList = _userService.GetMenuListInUser(masterUser_Guid, applicationID);
            return menuList;
        }

        /// <summary>
        /// Get menu command Id by user
        /// </summary>
        /// <param name="masterUser_Guid"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<int> GetMenuCommandInUser(Guid masterUser_Guid, int appId)
        {
            var allCommandInUser = _userService.GetMenuCommandInUser(masterUser_Guid, appId);
            return allCommandInUser;
        }
    }
}
