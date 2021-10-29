using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bgt.Ocean.WebAPI.Filters;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Messagings.MasterService;

namespace Bgt.Ocean.WebAPI.Controllers
{
    [ActionFilter]
    public class v2_AccountController : ApiController
    {
        private readonly IUserService _userService;
        public v2_AccountController(
            IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Register to use API by application username/password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [HttpPost]
        public HttpResponseMessage Register(UserAuthenRequest request)
        {
            var user = _userService.ValidateWebAPIUser(request.UserName, request.Password);
            if (user.UserGuid.HasValue)
            {
                if (!user.ApplicationGuids.ToList().Contains(ApiSession.Application_Guid.GetValueOrDefault()))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Application not match with user.");
                }
                else
                {
                    if (_userService.ExpiredTokenKey(user.Token.GetValueOrDefault()))
                    {
                        // delete old token
                        _userService.DeleteToken(user.UserGuid.Value);

                        // create new token
                        user.Token = _userService.CreateTokenKey(user.UserGuid.Value);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, "User not found.");
        }

        /// <summary>
        /// Verify active token 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        [HttpGet]
        public bool VerifyTokenKey(Guid tokenId)
        {
            return _userService.VerifyTokenKey(tokenId);
        }

        /// <summary>
        /// Check token expired date
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        [HttpGet]
        public bool ExpiredTokenKey(Guid tokenId)
        {
            return _userService.ExpiredTokenKey(tokenId);
        }

        /// <summary>
        /// Get token from app key
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        [HttpGet]
        public SystemApplicationResponse RetreiveAccessToken(Guid appKey)
        {
            var data = _userService.RetreiveAccessToken(appKey);
            return data;
        }

        /// <summary>
        /// Switch language and return new menu bar with new language.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<MasterMenuDetailResponse> ChangeLanguage(ChangeLanguageRequest request)
        {
            // change user language
            _userService.ChangeLanguage(request);

            // get new menu
            var menuList = _userService.GetMenuListInUser(request.MasterUser_Guid, request.ApplicationID);
            return menuList;
        }
    }
}
