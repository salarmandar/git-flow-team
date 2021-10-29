using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ServiceRequest;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    public interface IRuleEngineService
    {
        void StartServiceRequestRule(Guid serviceRequestGuid);
    }

    public class RuleEngineService : IRuleEngineService
    {
        private readonly IUnitOfWork<OceanDbEntities> _unitOfWork;
        private readonly ISystemService _systemService;
        private readonly ISFOMasterMachineRepository _machineRepository;
        private readonly ISFOTransactionServiceRequestRepository _serviceRequestRepository;
        private readonly IMasterSiteRepository _siteRepository;
        private readonly ISFOSystemEscalationRuleHeaderRepository _ruleHeaderRepository;
        private readonly ISFOSystemEscalationRuleDetailRepository _ruleDetailRepository;
        private readonly ISFOSystemEscalationRuleHeader_DetailRepository _ruleHeaderDetailRepository;
        private readonly ISFOSystemServiceRequestStateRepository _serviceRequestStateRepository;
        private readonly ISFOMasterEscalationHeaderRepository _escHeaderRepository;
        private readonly ISFOMasterEscalationDetailRepository _escDetailRepository;
        private readonly ISFOMasterEscalationDetail_PositionRepository _escDetailPositionRepository;
        private readonly ISFOMasterEscalationDetail_EmailRepository _escDetailEmailRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvRepository;
        private readonly IUserEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPositionRepository _positionRepository;

        public RuleEngineService
        (
            IUnitOfWork<OceanDbEntities> unitOfWork,
            ISystemService systemService,
            ISFOMasterMachineRepository machineRepository,
            ISFOTransactionServiceRequestRepository serviceRequestRepository,
            IMasterSiteRepository siteRepository,
            ISFOSystemEscalationRuleHeaderRepository ruleHeaderRepository,
            ISFOSystemEscalationRuleDetailRepository ruleDetailRepository,
            ISFOSystemEscalationRuleHeader_DetailRepository ruleHeaderDetailRepository,
            ISFOSystemServiceRequestStateRepository serviceRequestStateRepository,
            ISFOMasterEscalationHeaderRepository escHeaderRepository,
            ISFOMasterEscalationDetailRepository escDetailRepository,
            ISFOMasterEscalationDetail_PositionRepository escDetailPositionRepository,
            ISFOMasterEscalationDetail_EmailRepository escDetailEmailRepository,
            ISystemEnvironment_GlobalRepository systemEnvRepository,
            IUserEmailTemplateRepository emailTemplateRepository,
            IEmployeeRepository employeeRepository,
            IPositionRepository positionRepository
        )
        {
            _unitOfWork = unitOfWork;
            _systemService = systemService;
            _machineRepository = machineRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _siteRepository = siteRepository;
            _ruleHeaderRepository = ruleHeaderRepository;
            _ruleDetailRepository = ruleDetailRepository;
            _ruleHeaderDetailRepository = ruleHeaderDetailRepository;
            _serviceRequestStateRepository = serviceRequestStateRepository;
            _escHeaderRepository = escHeaderRepository;
            _escDetailRepository = escDetailRepository;
            _escDetailPositionRepository = escDetailPositionRepository;
            _escDetailEmailRepository = escDetailEmailRepository;
            _systemEnvRepository = systemEnvRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _employeeRepository = employeeRepository;
            _positionRepository = positionRepository;
        }

        public async void StartServiceRequestRule(Guid serviceRequestGuid)
        {
            try
            {
                var lstTask = new List<Task>();
                var serviceRequestDetail = GetServiceRequestByGuidEscalation(serviceRequestGuid);
                var currentStateResult = GetServiceRequestState(serviceRequestDetail);

                if (currentStateResult != null) // State found
                {
                    var currentStateModel = _serviceRequestStateRepository.FindById(currentStateResult.SystemServiceRequestState_Guid);

                    // Escalation
                    lstTask.Add(Task.Run(() => SendEscalation(currentStateResult, serviceRequestDetail)));

                    //// Customer ECI
                    //lstTask.Add(Task.Run(() => SendCustomerECI(serviceRequestDetail, currentStateModel)));

                    //// Vendor ECI
                    //lstTask.Add(Task.Run(() => SendVendorECI(serviceRequestDetail, currentStateModel)));

                    //// Customer Email
                    //lstTask.Add(Task.Run(() => SendCustomerEmail(serviceRequestDetail, currentStateModel)));
                }

                await Task.WhenAll(lstTask);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex);
            }

        }

        //public void StartServiceRequestRule(Guid serviceRequestGuid)
        //{
        //    try
        //    {
        //        var serviceRequestDetail = GetServiceRequestByGuidEscalation(serviceRequestGuid);
        //        var currentStateResult = GetServiceRequestState(serviceRequestDetail);

        //        if (currentStateResult != null) // State found
        //        {
        //            var currentStateModel = _serviceRequestStateRepository.FindById(currentStateResult.SystemServiceRequestState_Guid);

        //            // Escalation
        //            SendEscalation(currentStateResult, serviceRequestDetail);

        //            //// Customer ECI
        //            //SendCustomerECI(serviceRequestDetail, currentStateModel);

        //            //// Vendor ECI
        //            //SendVendorECI(serviceRequestDetail, currentStateModel);

        //            //// Customer Email
        //            //SendCustomerEmail(serviceRequestDetail, currentStateModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _systemService.CreateHistoryError(ex);
        //    }

        //}

        private Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result GetServiceRequestByGuidEscalation(Guid serviceRequestGuid)
        {
            var result = _serviceRequestRepository.Func_SFO_GetServiceRequestByGuidEscalation(serviceRequestGuid);
            return result;
        }

        private RuleEngineView GetServiceRequestState(Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result serviceRequestDetail)
        {
            var lstRuleHeader = GetRule(serviceRequestDetail.MasterSite_Guid.Value, serviceRequestDetail.TicketStatus_Guid);

            if (lstRuleHeader.Count > 0)
            {
                for (int i = 0; i < lstRuleHeader.Count; i++)
                {
                    var lstRuleDetail = lstRuleHeader[i].RuleDetail;

                    if (lstRuleDetail.Count > 0)
                    {
                        bool chkRule = false;

                        for (int j = 0; j < lstRuleDetail.Count; j++)
                        {
                            chkRule = Operation(serviceRequestDetail, lstRuleDetail[j].TargetField, lstRuleDetail[j].Operator, lstRuleDetail[j].TargetValue);
                            if (chkRule == false)
                            {
                                break;
                            }
                        }
                        if (chkRule == true)
                        {
                            var ServiceRequestModify = _serviceRequestRepository.FindById(serviceRequestDetail.ServiceRequestGuid);

                            ServiceRequestModify.PreviousServiceRequestState_Guid = lstRuleHeader[i].SystemServiceRequestState_Guid;
                            _serviceRequestRepository.Modify(ServiceRequestModify);

                            _unitOfWork.Commit();

                            return lstRuleHeader[i];
                        }
                    }
                }

                return null; // No state matched
            }

            return null; // Status not set
        }

        private List<RuleEngineView> GetRule(Guid siteGuid, Guid statusGuid)
        {
            var lstState = new List<RuleEngineView>();
            var countryGuid = _siteRepository.FindById(siteGuid).MasterCountry_Guid;

            // Get all possible state in this status
            var lstHeader = _ruleHeaderRepository.FindAll(e => e.MasterCountry_Guid == countryGuid && e.SystemJobStatus_Guid == statusGuid).ToList();

            // Loop in possible state
            foreach (var itemHeader in lstHeader)
            {
                var itemState = new RuleEngineView();

                itemState.MasterCountry_Guid = itemHeader.MasterCountry_Guid;
                itemState.SystemJobStatus_Guid = itemHeader.SystemJobStatus_Guid;
                itemState.SystemServiceRequestState_Guid = itemHeader.SFOSystemServiceRequestState_Guid;
                itemState.SystemServiceRequestState_Name = itemHeader.SFOTblSystemServiceRequestState.ServiceRequestStateName;

                // Find mapping rule detail guid for each state
                var lstDetail = new List<RuleDetail>();
                var lstMapping = _ruleHeaderDetailRepository.FindAll(e => e.SFOSystemEscalationRuleHeader_Guid == itemHeader.Guid).Select(e => e.SFOSystemEscalationRuleDetail_Guid).ToList();

                // Find rule detail
                foreach (var itemDetail in lstMapping)
                {
                    var detail = new RuleDetail();
                    var rule = _ruleDetailRepository.FindById(itemDetail);

                    detail.TargetField = rule.TargetField;
                    detail.TargetValue = rule.TargetValue;
                    detail.Operator = rule.Operator;

                    lstDetail.Add(detail);
                }

                itemState.RuleDetail = lstDetail;
                lstState.Add(itemState);
            }

            return lstState;
        }

        private bool Operation(dynamic objTargetObject, string strTargetProperty, string strOperator, string strTargetValue)
        {
            try
            {
                bool chkRule = false;
                dynamic valTargetObject = objTargetObject.GetType().GetProperty(strTargetProperty).GetValue(objTargetObject, null);
                string strTargetObject;

                if (strTargetValue == null) // For null value
                {
                    switch (strOperator)
                    {
                        case "=":
                            if (valTargetObject == null)
                                chkRule = true;
                            break;
                        case "<>":
                        case "!=":
                            if (valTargetObject != null)
                                chkRule = true;
                            break;
                    }
                }
                else
                {
                    strTargetObject = valTargetObject.ToString();
                    strTargetObject = strTargetObject.ToUpper();
                    strTargetValue = strTargetValue.ToUpper();

                    switch (strOperator)
                    {
                        case "=":
                            if (strTargetObject == strTargetValue)
                                chkRule = true;
                            break;
                        case "<>":
                        case "!=":
                            if (strTargetObject != strTargetValue)
                                chkRule = true;
                            break;
                    }
                }

                return chkRule;
            }
            catch
            {
                return false;
            }
        }

        private void SendEscalation(RuleEngineView rule, Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result serviceRequestDetail)
        {
            var escalation = GetEscalationToServiceRequest(serviceRequestDetail, rule.SystemServiceRequestState_Guid.Value, serviceRequestDetail.ServiceRequestGuid);
            var result = new EscalationData();

            result.TicketGuid = serviceRequestDetail.ServiceRequestGuid;
            result.TicketNumber = serviceRequestDetail.TicketNumber;
            result.TicketStatus = rule.SystemServiceRequestState_Name;
            result.StatusTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            result.FormatDatetime = "yyyy-MM-dd HH:mm:ss";
            result.OffSet = 0;

            if (escalation.Count() > 0)
            {
                string strEscalation = JsonConvert.SerializeObject(escalation);
                result.MailDetail = strEscalation;

                PushEscalation(result);
            }
            else
            {
                string strEscalation = "";
                result.MailDetail = strEscalation;

                PushEscalation(result);
            }
        }

        private IEnumerable<MailDetailInfo> GetEscalationToServiceRequest(Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result input, Guid stateGuid, Guid serviceRequestGuid)
        {
            var machineTypeGuid = _machineRepository.FindById(input.MachineGuid).SystemMachineType_Guid;
            var countryGuid = _siteRepository.FindById(input.MasterSite_Guid).MasterCountry_Guid;
            var model = _escHeaderRepository.FindAll(e => e.MasterCountry_Guid == countryGuid && e.SFOSystemServiceRequestState_Guid == stateGuid && e.SystemCustomerLocationType_Guid == machineTypeGuid && e.FlagDisable == false).FirstOrDefault();

            if (model == null)
            {
                return new List<MailDetailInfo>();
            }

            var escModel = new EscalationView();

            escModel.Guid = model.Guid;
            escModel.MasterCountry_Guid = model.MasterCountry_Guid.GetValueOrDefault();
            escModel.MasterSite_Guid = model.MasterSite_Guid.GetValueOrDefault();
            escModel.MachineType_Guid = model.SystemCustomerLocationType_Guid.GetValueOrDefault();
            escModel.ServiceRequestState_Guid = model.SFOSystemServiceRequestState_Guid.GetValueOrDefault();
            escModel.EscalationDetailLevel = new List<EscalationDetail>();

            var lstEscDetail = _escDetailRepository.FindAll(e => e.SFOMasterEscalationHeader_Guid == escModel.Guid && e.FlagDisable == false).OrderBy(e => e.EscalationLevel);

            foreach (var itemDetail in lstEscDetail)
            {
                var addDetail = new EscalationDetail();

                addDetail.Guid = itemDetail.Guid;
                addDetail.SFOMasterEscalationHeader_Guid = itemDetail.SFOMasterEscalationHeader_Guid;
                addDetail.EmailTemplate_Guid = itemDetail.MasterEmailTemplate_Guid.GetValueOrDefault();
                addDetail.EmailTemplate_Name = _emailTemplateRepository.FindById(itemDetail.MasterEmailTemplate_Guid).EmailTemplateName;
                addDetail.EscalationLevel = itemDetail.EscalationLevel;
                addDetail.NotifyFromMinute = itemDetail.NotifyFromMinute.GetValueOrDefault();
                addDetail.NotifyToMinute = itemDetail.NotifyToMinute.GetValueOrDefault();
                addDetail.FlagNotifyToResponder = itemDetail.FlagNotifyToResponder;
                addDetail.AdditionalEmail = getEmailNameToSend(addDetail.Guid);
                addDetail.PersonName = getPositionName(addDetail.Guid, addDetail.FlagNotifyToResponder.GetValueOrDefault());
                addDetail.EscalationDetailEmail = new List<EscalationAdditionalEmail>();
                addDetail.EscalationDetailPosition = new List<EscalationPosition>();

                var lstPosition = _escDetailPositionRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == addDetail.Guid);
                var lstEmail = _escDetailEmailRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == addDetail.Guid).OrderBy(e => e.DatetimeCreated);

                foreach (var itemPosition in lstPosition)
                {
                    var addPosition = new EscalationPosition();

                    addPosition.Guid = itemPosition.Guid;
                    addPosition.MasterPosition_Guid = itemPosition.MasterPosition_Guid;
                    addPosition.SFOMasterEscalationDetail_Guid = itemPosition.SFOMasterEscalationDetail_Guid;

                    addDetail.EscalationDetailPosition.Add(addPosition);
                }

                foreach (var itemEmail in lstEmail)
                {
                    var  addEmail = new EscalationAdditionalEmail();

                    addEmail.Guid = itemEmail.Guid;
                    addEmail.SFOMasterEscalationDetail_Guid = itemEmail.SFOMasterEscalationDetail_Guid;
                    addEmail.Email = itemEmail.Email;
                    addEmail.FullName = itemEmail.FullName;
                    addEmail.Position = itemEmail.Position;

                    addDetail.EscalationDetailEmail.Add(addEmail);
                }

                escModel.EscalationDetailLevel.Add(addDetail);
            }

            List<MailDetailInfo> result = new List<MailDetailInfo>();

            foreach (var item in escModel.EscalationDetailLevel)
            {
                var emailtemplate = _emailTemplateRepository.FindById(item.EmailTemplate_Guid);
                var value = new MailDetailInfo();

                value.From = _systemEnvRepository.FindByAppKey("SFOEscalationEmailFrom").AppValue1;
                value.Cc = "";
                value.Bcc = "";
                value.Level = item.EscalationLevel.GetValueOrDefault();
                value.Subject = emailtemplate.EmailSubject;
                value.NotifyFrom = item.NotifyFromMinute;
                value.NotifyTo = item.NotifyToMinute;

                var emailTo = getEmailByPosition(item.EscalationDetailPosition.Select(e => e.MasterPosition_Guid).ToList()) + item.AdditionalEmail;

                // ###### Get responder email ##### //
                if (item.FlagNotifyToResponder == true && input.MasterDailyRunResource_Guid != new Guid?())
                {
                    var ResponderEmail = _serviceRequestRepository.FindAll().Where(e => e.Guid == serviceRequestGuid).FirstOrDefault().ResponderEmail;

                    if (ResponderEmail != null)
                    {
                        emailTo += ResponderEmail;
                    }
                }

                if (string.IsNullOrEmpty(emailTo))
                {
                    emailTo = _systemEnvRepository.FindByAppKey("SFOTempEmailTo").AppValue1;
                }

                while (emailTo[emailTo.Length - 1] == ';')
                {
                    emailTo = emailTo.Substring(0, emailTo.Length - 1);
                }

                value.To = emailTo;
                value.Content = getEmailContent(emailtemplate.EmailBody, input, item.Guid, item.FlagNotifyToResponder.GetValueOrDefault());

                result.Add(value);
            }

            return result;
        }

        public void PushEscalation(dynamic escalation)
        {
            try
            {
                //string output = JsonConvert.SerializeObject(escalation);
                //var urlSFOIntegration = _systemEnvRepository.FindByAppKey("UrlSFOIntegration").AppValue1;
                //var escalationUrl = _systemEnvRepository.FindByAppKey("EscalationUrl").AppValue1;
                //byte[] data = Encoding.UTF8.GetBytes(output);
                //var client = new RestClient(urlSFOIntegration);
                //RestRequest request = new RestRequest(escalationUrl, Method.POST);
                //request.SetAuthHeader();
                //request.AddParameter("text/json", output, ParameterType.RequestBody);

                //_systemService.CreateLogActivityAsync(SystemActivityLog.APIActivity, $"Request to {urlSFOIntegration}{escalationUrl} with: {output}", SessionAuthen.Data.UserName, ConstantHelper.CurrentIpAddress, ConstantHelper.SFO_ApplicationGuid);

                //var response = client.ExecuteWithRetry(request);
                //var r = response.Content;

                //_systemService.CreateLogActivityAsync(SystemActivityLog.APIActivity, $"Response from {urlSFOIntegration}{escalationUrl} is: {r}", SessionAuthen.Data.UserName, ConstantHelper.CurrentIpAddress, ConstantHelper.SFO_ApplicationGuid);

            }
            catch (Exception ex)
            {
                // OO error logger
                //_systemService.CreateHistoryErrorAsync(ex);
            }

        }

        private string getEmailNameToSend(Guid Guid)
        {
            string result = "";
            var email = _escDetailEmailRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == Guid).OrderBy(e => e.DatetimeCreated).Select(e => e.Email);

            foreach (var item in email)
            {
                var emailname = item;
                result += emailname + ";";
            }

            return result;
        }

        private string getEmailByPosition(List<Guid> listposition)
        {
            string emailto = "";
            foreach (var item in listposition)
            {
                var emaillist = _employeeRepository.FindAll(e => e.MasterPosition_Guid == item).Select(e => e.EmailAddress);
                foreach (var item2 in emaillist)
                {
                    if (string.IsNullOrEmpty(item2))
                        continue;
                    emailto += item2 + ";";
                }
            }

            return emailto;
        }

        private string getPositionName(Guid Guid, bool flagNotifyToResponder)
        {
            string result = "";
            var position = _escDetailPositionRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == Guid).Select(e => e.MasterPosition_Guid);

            foreach (var item in position)
            {
                var positionname = _positionRepository.FindAll(e => e.Guid == item).FirstOrDefault().Position_Name;
                result += positionname + ", ";
            }

            if (flagNotifyToResponder)
            {
                result = String.Format("{0}, " + result, "Responder");
            }
            if (result == "")
                return "";
            result = result.Remove(result.Length - 2, 2);
            return result;
        }

        private string getEmailContent(string content, Up_OceanOnlineMVC_SFO_GetServiceRequestByGuidEscalation_Result data, Guid EscalationDetailGuid, bool FlagResponder)
        {
            data.AdditionalFullName = getAdditionalEmailFullName(EscalationDetailGuid);
            data.AdditionalPosition = getAdditionalPosition(EscalationDetailGuid);
            data.AdditionalFullNamePosition = getAdditionalEmailFullNameAndPosition(EscalationDetailGuid);
            var field = data.GetType().GetProperties().Select(e => e.Name);
            string template = content;
            //replace variable
            for (int i = 1; i < template.Length; i++)
            {
                int start = i;
                int end = i;
                if (template[i] == '@' && template[i - 1] == '[')
                {
                    start = i - 1;

                    while (true)
                    {
                        if (template[i] == ']')
                        {
                            end = i;
                            if (field.Contains(template.Substring(start + 2, end - start - 2)) && data.GetType().GetProperty(template.Substring(start + 2, end - start - 2)).GetValue(data, null) != null)
                            {
                                template = template.Replace(template.Substring(start, end + 1 - start), data.GetType().GetProperty(template.Substring(start + 2, end - start - 2)).GetValue(data, null).ToString());
                                i = start + 1;
                            }

                            break;
                        }
                        i++;
                    }
                }
            }
            //replace path src image
            for (int i = 4; i < template.Length; i++)
            {
                int start = i;
                int end = i;
                if (template[i] == '"' && template[i - 4] == 's' && template[i - 3] == 'r' && template[i - 2] == 'c' && template[i - 1] == '=')
                {
                    i++;
                    start = i;
                    if (template[start] != '/' || template[start] != '.')
                        continue;
                    while (true)
                    {
                        if (template[i] == '"')
                        {
                            end = i;
                            string temp = template.Substring(start, end - start);
                            template = template.Replace(template.Substring(start, end - start), "http://" + HttpContext.Current.Request.Url.Authority + temp);
                            i = start + 1;
                            break;
                        }
                        i++;
                    }
                }
            }
            return template;
        }

        private string getAdditionalEmailFullName(Guid Guid)
        {
            string result = "";
            var name = _escDetailEmailRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == Guid).OrderBy(e => e.DatetimeCreated).Select(e => e.FullName);

            foreach (var item in name)
            {
                var fullname = item;
                result += fullname + ", ";
            }

            if (result == "")
                return "";
            result = result.Remove(result.Length - 2, 2);
            return result;
        }

        private string getAdditionalPosition(Guid Guid)
        {
            string result = "";
            var name = _escDetailEmailRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == Guid).OrderBy(e => e.DatetimeCreated).Select(e => e.Position);

            foreach (var item in name)
            {
                var position = item;
                result += position + ", ";
            }

            if (result == "")
                return "";
            result = result.Remove(result.Length - 2, 2);
            return result;
        }

        private string getAdditionalEmailFullNameAndPosition(Guid Guid)
        {
            string result = "";
            var email = _escDetailEmailRepository.FindAll(e => e.SFOMasterEscalationDetail_Guid == Guid).OrderBy(e => e.DatetimeCreated);

            foreach (var item in email)
            {
                var fullname = item.FullName;
                var positionname = item.Position;
                result += fullname + "(" + positionname + ")" + ", ";
            }

            if (result == "")
                return "";
            result = result.Remove(result.Length - 2, 2);
            return result;
        }

        #region Model class

        private class EscalationData
        {
            public Guid? TicketGuid { get; set; }
            public string TicketStatus { get; set; }
            public string MailDetail { get; set; }
            public string StatusTime { get; set; }
            public int? OffSet { get; set; }
            public string TicketNumber { get; set; }
            public string FormatDatetime { get; set; }
        }

        private class MailDetailInfo
        {
            public int Level { get; set; }
            public string To { get; set; }
            public string Cc { get; set; }
            public string Bcc { get; set; }
            public string From { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
            public int NotifyFrom { get; set; }
            public int NotifyTo { get; set; }

        }

        private class EscalationView
        {
            public Guid Guid { get; set; }
            public Guid MasterCountry_Guid { get; set; }
            public Guid MasterSite_Guid { get; set; }
            public Guid MachineType_Guid { get; set; }
            public Guid ServiceRequestState_Guid { get; set; }
            public List<EscalationDetail> EscalationDetailLevel { get; set; }

        }
        private class EscalationDetail
        {
            public Guid Guid { get; set; }
            public Guid SFOMasterEscalationHeader_Guid { get; set; }
            public Guid EmailTemplate_Guid { get; set; }
            public string EmailTemplate_Name { get; set; }
            public int? EscalationLevel { get; set; }
            public int NotifyFromMinute { get; set; }
            public int NotifyToMinute { get; set; }
            public string PersonName { get; set; }
            public bool? FlagNotifyToResponder { get; set; }
            public string AdditionalEmail { get; set; }
            public List<EscalationPosition> EscalationDetailPosition { get; set; }
            public List<EscalationAdditionalEmail> EscalationDetailEmail { get; set; }
        }

        private class EscalationPosition
        {
            public Guid Guid { get; set; }
            public Guid SFOMasterEscalationDetail_Guid { get; set; }
            public Guid MasterPosition_Guid { get; set; }
        }

        private class EscalationAdditionalEmail
        {
            public Guid Guid { get; set; }
            public Guid SFOMasterEscalationDetail_Guid { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Position { get; set; }
        }

        #endregion Model class
    }
}
