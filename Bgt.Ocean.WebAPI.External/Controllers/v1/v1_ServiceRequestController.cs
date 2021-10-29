using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.SFO;
using Bgt.Ocean.Service.Mapping;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/servicerequests")]
    public class v1_ServiceRequestController : ApiControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ISystemService _systemService;

        public v1_ServiceRequestController(IServiceRequestService serviceRequestService, ISystemService systemService)
        {
            _serviceRequestService = serviceRequestService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Service Request data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryServiceRequest Query(RequestQueryServiceRequest request)
        {
            try
            {
                var result = new ResponseQueryServiceRequest();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _serviceRequestService.GetServiceRequestList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryServiceRequest
                {
                    responseCode = "-1",
                    responseMessage = "Error => Please contact administrator",
                    rows = 0
                };

                _systemService.CreateHistoryError(ex);

                return result;
            }
        }

        /// <summary>
        /// API to create new Service Request in Ocean Online
        /// </summary>
        [HttpPost]
        [Route("create")]
        public ResponseCreateServiceRequest Create(RequestCreateServiceRequest request)
        {
            try
            {
                var result = new ResponseCreateServiceRequest();
                var requestData = _serviceRequestService.GetCreateServiceRequestData(request);
                var requestCreate = new SRCreateRequestFLM();

                // Validate FLM(Main) request
                var requestCreateValidated = ValidateCreateRequestFLM(request, requestData);

                if (requestCreateValidated.isSuccess == true)
                {
                    requestCreate = requestCreateValidated.request;
                }
                else
                {
                    result.responseCode = "0";
                    result.responseMessage = requestCreateValidated.message;

                    return result;
                }

                var resultCreate = new SRCreateResponse();

                // Validate service job type abb
                switch (request.serviceRequestDetail.serviceJobTypeAbb)
                {
                    case "FLM":
                        resultCreate = _serviceRequestService.CreateServiceRequestFLM(requestCreate);
                        break;

                    case "F/SLM":
                        var requestCreateSlm = MapperService.Map<SRCreateRequestFLM, SRCreateRequestSLM>(requestCreate);
                        resultCreate = _serviceRequestService.CreateServiceRequestSLM(requestCreateSlm);
                        break;

                    case "ECASH":
                        var requestCreateEcash = MapperService.Map<SRCreateRequestFLM, SRCreateRequestECash>(requestCreate);
                        var requestCreateEcashValidated = ValidateCreateRequestEcash(request, requestCreateEcash);

                        if (requestCreateEcashValidated.isSuccess == true)
                        {
                            requestCreateEcash = requestCreateEcashValidated.request;
                            resultCreate = _serviceRequestService.CreateServiceRequestECash(requestCreateEcash);
                        }
                        else
                        {
                            result.responseCode = "0";
                            result.responseMessage = requestCreateEcashValidated.message;

                            return result;
                        }

                        break;

                    case "TM":
                        var requestCreateTechMeet = MapperService.Map<SRCreateRequestFLM, SRCreateRequestTechMeet>(requestCreate);

                        requestCreateTechMeet.TechMeetCompanyName = request.techMeetDetail.techMeetCompanyName;
                        requestCreateTechMeet.TechMeetName = request.techMeetDetail.techMeetName;
                        requestCreateTechMeet.TechMeetPhone = request.techMeetDetail.techMeetPhone;
                        requestCreateTechMeet.TechMeetReason = request.techMeetDetail.techMeetReason;
                        requestCreateTechMeet.TechMeetSecurityRequired = request.techMeetDetail.techMeetSecurityRequired;

                        resultCreate = _serviceRequestService.CreateServiceRequestTechMeet(requestCreateTechMeet);

                        break;

                    default:
                        result.responseCode = "0";
                        result.responseMessage = "Warning => Service job type is invalid";

                        return result;
                }

                if (resultCreate.IsSuccess == true)
                {
                    result.rows = 1;
                    result.result.ticketGuid = resultCreate.TicketGuid.ToString();
                    result.result.ticketNumber = resultCreate.TicketNumber;
                    result.result.jobHeaderGuid = resultCreate.JobHeaderGuid.ToString();
                }
                else
                {
                    result.responseCode = "0";
                    result.responseMessage = string.Concat("Warning => ", resultCreate.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseCreateServiceRequest
                {
                    responseCode = "-1",
                    responseMessage = "Error => Please contact administrator",
                    rows = 0
                };

                _systemService.CreateHistoryError(ex);

                return result;
            }
        }

        private ValidateCreateRequestFLMModel ValidateCreateRequestFLM(RequestCreateServiceRequest request, ResquestCreateServiceRequestData requestData)
        {
            var returnRequest = new ValidateCreateRequestFLMModel();

            // Validate country abb
            if (requestData.masterCountryGuid == null)
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Please check country abb";
                goto ReturnState;
            }
            else
            {
                returnRequest.request.MasterCountryGuid = requestData.masterCountryGuid.ToGuid();
            }

            // Validate machine id
            if (requestData.atmGuid == null)
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Please check machine id";
                goto ReturnState;
            }
            else
            {
                returnRequest.request.MachineGuid = requestData.atmGuid.ToGuid();
            }

            // Validate open source id
            if (requestData.openSourceGuid == null)
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Please check open source id";
                goto ReturnState;
            }
            else
            {
                returnRequest.request.OpenSourceGuid = requestData.openSourceGuid.ToGuid();
            }

            // Validate machine service type name
            if (requestData.machineServiceTypeGuid == null)
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Please check machine service type name";
                goto ReturnState;
            }
            else
            {
                returnRequest.request.MachineServiceTypeGuid = requestData.machineServiceTypeGuid.ToGuid();
            }

            // Validate problem id
            if (requestData.problemGuid == null)
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Please check problem id";
                goto ReturnState;
            }
            else
            {
                returnRequest.request.ProblemGuid = requestData.problemGuid.ToGuid();
            }

            // Validate downed datetime
            if (string.IsNullOrEmpty(request.datetimeDetail.downedDateTime))
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Downed datetime is required";
                goto ReturnState;
            }
            else
            {
                // Validate downed datetime format
                try
                {
                    returnRequest.request.DateTimeDown = DateTime.ParseExact(request.datetimeDetail.downedDateTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Downed datetime is invalid format (Required yyyy-MM-dd HH:mm)";
                    goto ReturnState;
                }
            }

            // Validate notified datetime
            if (string.IsNullOrEmpty(request.datetimeDetail.notifiedDatetime))
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Notified datetime is required";
                goto ReturnState;
            }
            else
            {
                // Validate notified datetime format
                try
                {
                    returnRequest.request.DateTimeNotified = DateTime.ParseExact(request.datetimeDetail.notifiedDatetime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Notified datetime is invalid format (Required yyyy-MM-dd HH:mm)";
                    goto ReturnState;
                }
            }

            // Validate serviced datetime
            if (string.IsNullOrEmpty(request.datetimeDetail.servicedDatetime))
            {
                returnRequest.isSuccess = false;
                returnRequest.message = "Warning => Serviced datetime is required";
                goto ReturnState;
            }
            else
            {
                // Validate serviced datetime format
                try
                {
                    returnRequest.request.DateTimeServiceDate = DateTime.ParseExact(request.datetimeDetail.servicedDatetime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Serviced datetime is invalid format (Required yyyy-MM-dd HH:mm)";
                    goto ReturnState;
                }
            }

            // Optional fields
            returnRequest.request.ContactName = request.serviceRequestDetail.contactName;
            returnRequest.request.ContactPhone = request.serviceRequestDetail.contactPhone;
            returnRequest.request.CustomerReferenceNumber = request.serviceRequestDetail.customerReferenceNumber;
            returnRequest.request.ReportedIncidentDescription = request.serviceRequestDetail.reportedIncidentDescription;

            // Return
            ReturnState:
            {
                return returnRequest;
            };
        }

        private ValidateCreateRequestEcashModel ValidateCreateRequestEcash(RequestCreateServiceRequest request, SRCreateRequestECash requestData)
        {
            var returnRequest = new ValidateCreateRequestEcashModel();
            var addEcashList = new List<ECashAmount>();

            foreach (var item in request.eCashDetail)
            {
                var addEcash = new ECashAmount();

                // Validate currency
                if (string.IsNullOrEmpty(item.currency))
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Currency is required";
                    goto ReturnState;
                }
                else
                {
                    // Get currency guid
                    addEcash.Currency = item.currency;
                }

                // Validate denomination
                if (string.IsNullOrEmpty(item.denomination))
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Denomination is required";
                    goto ReturnState;
                }
                else
                {
                    // Get denomination guid
                    addEcash.Denomination = item.denomination;
                }

                // Validate amount
                if (string.IsNullOrEmpty(item.amount))
                {
                    returnRequest.isSuccess = false;
                    returnRequest.message = "Warning => Amount is required";
                    goto ReturnState;
                }
                else
                {
                    // Validate amount format
                    try
                    {
                        addEcash.Amount = Int32.Parse(item.amount);
                    }
                    catch
                    {
                        returnRequest.isSuccess = false;
                        returnRequest.message = "Warning => Amount is invalid format (Required numeric)";
                        goto ReturnState;
                    }
                }

                addEcashList.Add(addEcash);
            }

            requestData.ECashViewList = addEcashList;
            returnRequest.request = requestData;

            goto ReturnState;

            // Return
            ReturnState:
            {
                return returnRequest;
            }
        }

        private class ValidateCreateRequestFLMModel
        {
            public bool isSuccess { get; set; } = true;
            public string message { get; set; }
            public SRCreateRequestFLM request { get; set; } = new SRCreateRequestFLM();
        }

        private class ValidateCreateRequestEcashModel
        {
            public bool isSuccess { get; set; } = true;
            public string message { get; set; }
            public SRCreateRequestECash request { get; set; } = new SRCreateRequestECash();
        }
    }
}
