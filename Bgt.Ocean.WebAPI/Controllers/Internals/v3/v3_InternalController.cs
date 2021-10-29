using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Security;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.Account;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v3
{
    public class v3_InternalController : ApiControllerBase
    {
        #region Objects and Variables
        private readonly IUserService _userService;
        private readonly ISystemService _systemService;

        private readonly string MSG_SUCCESS = "Login successful.";
        private readonly string MSG_ERROR_APPLICATION_NOT_EXIST = "Your application does not exist.";
        private readonly string MSG_ERROR_LOCKED_USER = "User {0} has been locked.";
        private readonly string MSG_ERROR_USERNAME_PASSWORD = "Username or password is incorrect.";
        private readonly string MSG_ERROR_PASSWORD_EXPIRE = "Password is Expired.";
        private readonly string MSG_ERROR_CANNOT_ACCESS_THIS_TIME = "Your account can't access system in this time.";

        public static string CurrentIpAddress
        {
            get
            {
                try
                {
                    bool GetLan = false;
                    string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (String.IsNullOrEmpty(visitorIPAddress))
                        visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (string.IsNullOrEmpty(visitorIPAddress))
                        visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

                    if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
                    {
                        GetLan = true;
                        visitorIPAddress = string.Empty;
                    }

                    if (GetLan && string.IsNullOrEmpty(visitorIPAddress))
                    {
                        //This is for Local(LAN) Connected ID Address
                        string stringHostName = System.Net.Dns.GetHostName();

                        visitorIPAddress = Dns.GetHostByName(stringHostName).AddressList[0].ToString();
                    }

                    return visitorIPAddress;
                }
                catch { return "IP address is undefind."; }
            }
        }

        #endregion

        #region Constuctor
        public v3_InternalController(
            IUserService userService,
            ISystemService systemService)
        {
            _userService = userService;
            _systemService = systemService;
        }
        #endregion

        #region Authentication
        /// <summary>
        /// User Authentication (With Encrypted Username and Password)
        /// </summary>
        /// <param name="request">User Authen Data</param>
        /// <returns></returns>
        [HttpPost]
        public DataStorage BrinksLogin(UserAuthenRequest request)
        {
            var application = _systemService.GetSystemApplication(request.ApplicationId);
            if (application == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MSG_ERROR_APPLICATION_NOT_EXIST));

            var txtUsername = request.UserName.AES_Decrypt(application.Guid.ToString().ToLower());
            var txtPassword = request.Password.AES_Decrypt(application.Guid.ToString().ToLower());
            if (txtUsername.IsSuccess && txtPassword.IsSuccess && request.ClientDateTime.HasValue)
            {
                var authenDetails = _userService.GetUserAuthenTypeDetail_v3(txtUsername.Message, txtPassword.Message);
                if (authenDetails != null)
                {
                    var domainGuid = _userService.GetDomainAile(authenDetails.Domain);
                    if (domainGuid != null)
                    {
                        var serverLDAP = _userService.GetDomainServer(domainGuid.Value);
                        if (serverLDAP == null)
                            return setMessageWarning(401, MSG_ERROR_USERNAME_PASSWORD, application.Guid); // Username or password incorrect

                        var userData = _userService.GetAuthenLDAP(authenDetails, serverLDAP);
                        if (userData != null)
                        {
                            userData.EmployeeID = _userService.GetEmployeeID(userData.UserGuid);
                            userData.UserEmail = _userService.GetUserEmail(userData.UserGuid);
                            _userService.UpdateLastLogin(txtUsername.Message, request.ClientDateTime, request.ClientSessionId);
                            _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, MSG_SUCCESS, txtUsername.Message, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());

                            userData.NumberCurrencyCultureCode = _systemService.GetNumberCurrencyCultureCode(userData.FormatNumberCurrency);
                            userData.Password = null;
                            userData.PasswordDecrypted = null;
                            userData.IsSuccess = true;
                            userData.IsCheckDomain = true;
                            return userData;
                        }
                    }
                    else
                    {
                        if (_userService.IsUserDomain(txtUsername.Message))
                            return setMessageWarning(401, MSG_ERROR_USERNAME_PASSWORD, application.Guid); // Username or password incorrect

                        var userData = _userService.GetAuthenLocal(txtUsername.Message, txtPassword.Message, request.ApplicationId).ConvertAuthenResponseToDataStorage();
                        if (userData == null)
                        {
                            // update invalid logon and return result
                            var userView = _userService.GetUserView(txtUsername.Message);

                            if (userView != null)
                            {
                                _userService.UpdateCountInvalidLogOn(txtUsername.Message, request.ClientSessionId, request.ClientDateTime.GetValueOrDefault());

                                return userView.FlagLock
                                    ? setMessageWarning(402, string.Format(MSG_ERROR_LOCKED_USER, txtUsername.Message), application.Guid)
                                    : setMessageWarning(401, MSG_ERROR_USERNAME_PASSWORD, application.Guid);
                                
                            }
                            return setMessageWarning(401, MSG_ERROR_USERNAME_PASSWORD, application.Guid);
                        }

                        if (!userData.FlagLock.GetValueOrDefault())
                        {
                            if (userData.ExpiryDate.GetValueOrDefault().Date <= request.ClientDateTime.GetValueOrDefault())
                            {
                                _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, "Login failed, Password is Expired.", userData.UserName, CurrentIpAddress, application.Guid);
                                return setMessageWarning(406, MSG_ERROR_PASSWORD_EXPIRE, application.Guid); // Password Expire
                            }
                            else if (userData.FlagChangePWDNextLogIn.GetValueOrDefault())
                            {
                                return setMessageWarning(407, "", application.Guid); // CHANGE PASSWORD NEXT LOGIN
                            }
                            else if (userData.FlagLimitedTimeAccess.GetValueOrDefault() && !_userService.CanAccessInThisTime(request.ClientDateTime.GetValueOrDefault(), userData.UserGuid))
                            {
                                return setMessageWarning(408, MSG_ERROR_CANNOT_ACCESS_THIS_TIME, application.Guid); // CANNOT ACCESS THIS TIME
                            }
                            else
                            {
                                userData.EmployeeID = _userService.GetEmployeeID(userData.UserGuid);
                                userData.UserEmail = _userService.GetUserEmail(userData.UserGuid);
                                _userService.UpdateLastLogin(txtUsername.Message, request.ClientDateTime.GetValueOrDefault(), request.ClientSessionId);
                                _systemService.CreateLogActivity(SystemActivityLog.AuthenticationLogin, MSG_SUCCESS, txtUsername.Message, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());

                                userData.NumberCurrencyCultureCode = _systemService.GetNumberCurrencyCultureCode(userData.FormatNumberCurrency);
                                userData.Password = null;
                                userData.PasswordDecrypted = null;
                                userData.IsSuccess = true;
                                return userData;
                            }
                        }
                        else if (userData.FlagLock.GetValueOrDefault())
                        {
                            _userService.UpdateCountInvalidLogOn(txtUsername.Message, request.ClientSessionId, request.ClientDateTime.GetValueOrDefault());
                            return setMessageWarning(402, string.Format(MSG_ERROR_LOCKED_USER, txtUsername.Message), application.Guid); // User Lock
                        }
                    }
                }
            }
            return setMessageWarning(401, MSG_ERROR_USERNAME_PASSWORD, application.Guid);// Username or password incorrect
        }

        private static DataStorage setMessageWarning(int id, string textContent, Guid applicationGuid)
        {
            return new DataStorage()
            {
                IsSuccess = false,
                MsgID = id,
                Message = textContent,
                Title = "Warning",
                ApplicationGuid = applicationGuid
            };
        }

        [HttpGet]
        public Guid GetApplicationGuid(int appID)
        {
            var applicationGuid = _systemService.GetSystemApplication(appID);
            return applicationGuid.Guid;
        }
        #endregion

        #region Password
        [HttpPost]
        public int ChangePassword(ChangePasswordViewModel request)
        {
            try
            {
                var application = _systemService.GetSystemApplication(request.ApplicationID);
                if (application == null)
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MSG_ERROR_APPLICATION_NOT_EXIST));

                var txtUsername = request.Username.AES_Decrypt(application.Guid.ToString().ToLower()).Message;
                var txtOldPassword = string.IsNullOrEmpty(request.VerifyKey) ? string.Empty : request.Password.AES_Decrypt(application.Guid.ToString().ToLower()).Message;
                var txtNewPassword = request.NewPassword.AES_Decrypt(application.Guid.ToString().ToLower()).Message;

                return _userService.ChangePassword(txtUsername, txtOldPassword, txtNewPassword);
            }
            catch(Exception)
            {
                return -213;
            }
        }
        
        [HttpPost]
        public int ForgetPassword(ForgotPasswordViewModel request)
        {
            try
            {
                return _userService.ResetPassword(request);
            }
            catch (Exception)
            {
                return -184;
            }
        }

        [HttpPost]
        public int ResetPassword(ResetPasswordViewModel request)
        {
            try
            {                
                return _userService.ResetPassword(request.UserGuid);
            }
            catch (Exception)
            {
                return -184;
            }
        }
        #endregion
    }
}