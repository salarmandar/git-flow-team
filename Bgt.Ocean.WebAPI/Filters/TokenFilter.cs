using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Bgt.Ocean.WebAPI.Filters
{
    public sealed class TokenFilter : System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var userService = (IUserService)actionContext.Request.GetDependencyScope().GetService(typeof(IUserService));

            TokenBox tokenInHeaders = GetTokenFromHeaders(actionContext);
            if (tokenInHeaders == null)
                return false;
            else
            {
                if (!tokenInHeaders.Auth_key.HasValue || !tokenInHeaders.App_key.HasValue)
                {
                    return false;
                }

                #region Verify Token Key
                if (tokenInHeaders.Auth_key.HasValue && !userService.VerifyTokenKey(tokenInHeaders.Auth_key.Value))
                {
                    return false;
                }

                // check expired key
                if (userService.ExpiredTokenKey(tokenInHeaders.Auth_key.Value))
                {
                    HttpResponseMessage response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Token key has expired.");
                    throw new System.Web.Http.HttpResponseException(response);
                }
                #endregion

                #region Verify App Key
                // check from app key
                var app = userService.RetreiveAccessToken(tokenInHeaders.App_key.Value);
                if (app == null)
                    return false;
                #endregion

                ApiSession.ApplicationNumber = app.ApplicationID;
                ApiSession.Application_Token = app.TokenID.ToString();
                ApiSession.Application_Guid = app.Guid;
                return true;
            }
        }

        private TokenBox GetTokenFromHeaders(HttpActionContext actionContext)
        {
            TokenBox tokenBox = new TokenBox();
            IEnumerable<string> tokenValues;
            IEnumerable<string> languageValues;
            IEnumerable<string> appValues;
            IEnumerable<string> utcOffset;
            IEnumerable<string> userGuid;
            IEnumerable<string> userName;
            IEnumerable<string> userFormatDate;
            IEnumerable<string> clientDatetime;

            actionContext.Request.Headers.TryGetValues("auth_key", out tokenValues);
            actionContext.Request.Headers.TryGetValues("app_key", out appValues);
            actionContext.Request.Headers.TryGetValues("language", out languageValues);
            actionContext.Request.Headers.TryGetValues("utcOffset", out utcOffset);
            actionContext.Request.Headers.TryGetValues("userGuid", out userGuid);
            actionContext.Request.Headers.TryGetValues("userName", out userName);
            actionContext.Request.Headers.TryGetValues("userFormatDate", out userFormatDate);
            actionContext.Request.Headers.TryGetValues("clientdatetime", out clientDatetime);

            if (!SystemHelper.VerifyNullHeaderKey(appValues))
                tokenBox.App_key = new Guid(appValues.First());

            // verify authen key must not be null
            if (SystemHelper.VerifyNullHeaderKey(tokenValues))
                return null;

            // get language (if)
            if (!SystemHelper.VerifyNullHeaderKey(languageValues))
                ApiSession.UserLanguage_Guid = Guid.Parse(languageValues.First());

            // get client date time (if)
            if (!SystemHelper.VerifyNullHeaderKey(utcOffset))
                ApiSession.UtcOffset = utcOffset.First().ToInt();

            // get userGuid (if)
            if (!SystemHelper.VerifyNullHeaderKey(userGuid))
                ApiSession.UserGuid = Guid.Parse(userGuid.First());

            // get userName (if)
            if (!SystemHelper.VerifyNullHeaderKey(userName))
                ApiSession.UserName = userName.First().ToString();

            // get userFormatDate (if)
            if (!SystemHelper.VerifyNullHeaderKey(userFormatDate))
                ApiSession.UserFormatDate = userFormatDate.First().ToString();

            // get clientDatetime (if) ** Always Set
            ApiSession.ClientDatetime = SetUpClientDatetime(clientDatetime);


            tokenBox.Auth_key = new Guid(tokenValues.First());
            ApiSession.Authentication_Key = tokenBox.Auth_key.ToString();

            return tokenBox;
        }

        private static DateTimeOffset SetUpClientDatetime(IEnumerable<string> clientDatetime)
        {
            var formatdate = "yyyy/MM/dd HH:mm:ss";
            var _strclientDateTime = clientDatetime != null && clientDatetime.Any() ? clientDatetime.First().ToString() : string.Empty;
            var _clientDateTime = _strclientDateTime.ChangeStrToDateTime(formatdate);
            DateTimeOffset _clientDateTimeoffSet = DateTime.SpecifyKind(_clientDateTime, DateTimeKind.Unspecified);
            return _clientDateTimeoffSet;
        }

        private class TokenBox
        {
            public Guid? Auth_key { get; set; }
            public Guid? App_key { get; set; }
        }
    }
}
