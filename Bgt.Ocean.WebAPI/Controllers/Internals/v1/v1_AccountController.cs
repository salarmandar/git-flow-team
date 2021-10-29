using Bgt.Ocean.WebAPI.Filters;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Messagings.UserService;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers
{
    [ActionFilter]
    public class v1_AccountController : ApiController
    {
        private readonly IUserService _userService;
        public v1_AccountController(
            IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Verify user email to check is any in system
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public bool VerifyEmail(string email)
        {
            return _userService.VerifyEmail(email);
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

                    return Request.CreateResponse(HttpStatusCode.OK, user.Token);
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, "User not found.");
        }

        /// <summary>
        /// api/v1/account/changepassword
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        [TokenFilter]
        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            Contract.Requires(changePasswordRequest != null);
            HttpResponseMessage response; ;
            int result = _userService.ChangePassword(changePasswordRequest.UserName, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (result.Equals(1))
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Change password successful.");
            }
            else
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Change password failures.");
            }
            return response;
        }

        /// <summary>
        /// Hook for test request get "Hook!!"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetHook()
        {
            return "Hook!!";
        }
    }
}
