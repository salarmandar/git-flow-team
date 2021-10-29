using Bgt.Ocean.Service.Messagings.StandardTable;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.WebAPI.External.Extends;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers
{
    public class ApiControllerBase : ApiController
    {
        protected HttpResponseMessage GetResponseMessageView(SystemMessageView msg, object data = null)
        {
            var response = new HttpSystemMessage(msg);
            response.StatusCode = HttpStatusCode.OK;

            response.Content = new StringContent(data != null ? data?.GetJsonString() : msg.GetJsonString());

            return response;
        }

        /// <summary>
        /// This method will log the exception and returns HttpResponseMessgae with content as its err
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        protected HttpResponseMessage GetErrorResponse(Exception err)
        {
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, err);
        }

        protected HttpResponseMessage GetErrorResponse(HttpStatusCode errorCode, string messageText)
        {
            return Request.CreateErrorResponse(errorCode, messageText);
        }

        protected HttpResponseMessage GetErrorResponse(string messageText)
        {
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, messageText);
        }

        protected BaseResponse ValidateBaseQueryRequest(BaseRequestQuery request)
        {
            var result = new BaseResponse();

            if (request == null)
            {
                result.responseCode = "0";
                result.responseMessage = "Warning => Please check request body";

                return result;
            }

            if (string.IsNullOrEmpty(request.countryAbb))
            {
                result.responseCode = "0";
                result.responseMessage = "Warning => Country abb is required";

                return result;
            }

            if (!ValidateDatetimeFormat(request.createdDatetimeFrom))
            {
                result.responseCode = "0";
                result.responseMessage = "Warning => Created datetime from is invalid format (Required yyyy-MM-dd HH:mm)";

                return result;
            }

            if (!ValidateDatetimeFormat(request.createdDatetimeTo))
            {
                result.responseCode = "0";
                result.responseMessage = "Warning => Created datetime to is invalid format (Required yyyy-MM-dd HH:mm)";

                return result;
            }

            return result;
        }

        protected bool ValidateDatetimeFormat(string dateTime)
        {
            try
            {
                if (dateTime != null)
                {
                    //DateTime.Parse(dateTime);
                    DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
