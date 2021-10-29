using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Account;
using Bgt.Ocean.Models.Email;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Email;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.User;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Implementations.Email;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.MasterService;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static Bgt.Ocean.Infrastructure.Util.EnumUser;

namespace Bgt.Ocean.Service.Implementations
{
    public interface IUserService
    {
        UserView GetUserView(string userName);
        IEnumerable<UserView> GetSalePersons(Guid company_Guid);
        bool VerifyTokenKey(Guid tokenId);
        UserAuthenType GetUserAuthenTypeDetail(string userName, string password);
        UserApplicationView ValidateWebAPIUser(string userName, string password);
        bool ExpiredTokenKey(Guid tokenId);
        void DeleteToken(Guid userGuid);
        Guid CreateTokenKey(Guid userGuid);
        bool VerifyEmail(string email);

        /// <summary>
        /// Create api access history.
        /// </summary>
        void CreateAccessAPILogHistory(Guid? token, DateTime requestTime, string apiMethodName, string requestBody, DateTime responseTime, string responseBody);
        SystemApplicationResponse RetreiveAccessToken(Guid appKey);
        int ChangePassword(string userName, string oldPassword, string newPassword);
        void ChangeLanguage(ChangeLanguageRequest request);
        Guid? GetDomainAile(string ailesName);
        string GetDomainServer(Guid ailesGuid);
        DataStorage GetAuthenLDAP(UserAuthenType authenDetails, string serverLDAP, int applicationNumber = 1);
        AuthenLoginResponse GetAuthenLDAP(AuthenDetails inAuthenDetails, string inLDAP, int applicationNumber);
        AuthenLoginResponse GetAuthenLocal(string username, string password, int applicationId);
        IEnumerable<Messagings.UserService.MasterMenuDetailResponse> GetMenuListInUser(Guid masterUser_Guid, int? applicationID = null);
        string GetUserEmail(Guid userGuid);
        void UpdateLastLogin(string userName, DateTime? clientDateTime, string clientSessionId);
        bool IsUserDomain(string userName);
        string UpdateCountInvalidLogOn(string userName, string clientSessionId, DateTime? clientDateTime);
        void ChangeLanguage(Guid userId, Guid languageGuid);
        SystemDomainDCResponse GetDomainDC(Guid ailesGuid);
        SystemDomainAilesResponse GetDomain(string ailesName);
        AuthenDetails GetAuthenCredential(string inUsername, string inPassword);
        IEnumerable<int> GetMenuCommandInUser(Guid userGuid, int appId);
        IEnumerable<MasterGroupView> GetMasterGroupByCountry(Guid countryGuid);
        IEnumerable<UserView> GetUserDetailByRoleType(int? roleType);
        MasterUserGroupView GetUserGroup(Guid userGuid);
        IEnumerable<UserGroupView> FindUserCanSeeMenu(Guid masterMenuDetailGuid, Guid masterSiteGuid);
        IEnumerable<UserGroupView> FindUserByMenuCommand(Guid masterMenuDetailCommandGuid, Guid masterSiteGuid);
        IEnumerable<UserView> GetUsersInCompany(Guid company_Guid);
        IEnumerable<UserView> GetUsersInCountry(Guid country_Guid, string filterUserName, string filterFullName, string filterEmail, int? pageNumber, int? numberPerPage, string sortBy, string sortWith);
        UserAuthenType GetUserAuthenTypeDetail_v3(string userName, string password);
        bool CanAccessInThisTime(DateTime? clientDateTime, Guid userGuid);
        string GetEmployeeID(Guid userGuid);

        int ResetPassword(ForgotPasswordViewModel request);
        int ResetPassword(Guid userGuid);

        TblMasterGroup_MenuCommand GetMenuCommandInUserByCommandId(Guid userGuid, int appId, EnumMenuCommandId menuCommandId);
    }

    public class UserService : IUserService
    {
        #region Objects & Variables
        private const string EMAIL_RESETPASSWORD = "Ocean_Online_MVC_AlertMail_ResetPassword";
        private const string EMAIL_CANNOT_RESETPASSWORD = "Ocean_Online_MVC_AlertMail_Cannot_ResetPassword";
        private const string EMAIL_WELCOME = "Ocean_Online_MVC_AlertMail_Welcome";
        private const string EMAIL_REQUEST_APPROVE = "Ocean_Online_MVC_AlertMail_Approve";
        private const string EMAIL_CREATE_LOCALADMIN = "Ocean_Online_AlertMail_Create_Local_Admin";

        private readonly IWebAPIUser_TokenRepository _webAPIUser_TokenRepository;
        private readonly IWebAPIUserRepository _webAPIUserRepository;
        private readonly IMasterMenuDetailRepository _masterMenuDetailRepository;
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly IMasterLogVerifyKeyRepository _masterLogVerifyKeyRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemLog_AttemptToLoginRepository _systemLog_AttemptToLoginRepository;
        private readonly ISystemDomainsRepository _systemDomainsRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IMasterUserLimitedTimeAccessRepository _masterUserLimitedTimeAccessRepository;
        private readonly IGenericLogService _genericLogService;
        private readonly IMasterEmailTemplateRepository _masterEmailTemplateRepository;
        private readonly IEmailService _emailService;
        #endregion

        public UserService(
            IWebAPIUser_TokenRepository webAPIUser_TokenRepository,
            IWebAPIUserRepository webAPIUserRepository,
            IMasterMenuDetailRepository masterMenuDetailRepository,
            IMasterUserRepository masterUserRepository,
            ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository,
            IMasterLogVerifyKeyRepository masterLogVerifyKeyRepository,
            ISystemService systemService,
            ISystemLog_AttemptToLoginRepository systemLog_AttemptToLoginRepository,
            ISystemDomainsRepository systemDomainsRepository,
            IUnitOfWork<OceanDbEntities> uow,
            IMasterUserLimitedTimeAccessRepository masterUserLimitedTimeAccessRepository,
            IGenericLogService genericLogService,
            IMasterEmailTemplateRepository masterEmailTemplateRepository,
            IEmailService emailService
            )
        {
            _webAPIUser_TokenRepository = webAPIUser_TokenRepository;
            _webAPIUserRepository = webAPIUserRepository;
            _masterMenuDetailRepository = masterMenuDetailRepository;
            _masterUserRepository = masterUserRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _masterLogVerifyKeyRepository = masterLogVerifyKeyRepository;
            _systemService = systemService;
            _systemLog_AttemptToLoginRepository = systemLog_AttemptToLoginRepository;
            _systemDomainsRepository = systemDomainsRepository;
            _uow = uow;
            _masterUserLimitedTimeAccessRepository = masterUserLimitedTimeAccessRepository;
            _genericLogService = genericLogService;
            _masterEmailTemplateRepository = masterEmailTemplateRepository;
            _emailService = emailService;
        }

        public IEnumerable<UserView> GetSalePersons(Guid company_Guid)
        {
            var saleUser = _masterUserRepository.FindAll(e => e.MasterCustomer_Guid == company_Guid && !e.FlagDisable && e.FlagLock == false && e.FlagSalePerson);
            return saleUser.Select(e => new UserView
            {
                FullName = $"{e.FirstName} {e.MiddleName} {e.LastName}",
                UserName = e.UserName,
                Email = e.Email,
                Guid = e.Guid
            }).Distinct((x, y) => x.Guid == y.Guid).OrderBy(e => e.FullName);
        }

        public Guid CreateTokenKey(Guid userGuid)
        {
            var expiredTime = DateTime.Now.AddMinutes(5);
            Guid tokenNew = Guid.NewGuid();
            TblWebAPIUser_Token token = new TblWebAPIUser_Token();
            token.TokenID = tokenNew;
            token.Guid = Guid.NewGuid();
            token.TokenExpireDateTime = expiredTime;
            token.WebAPIUser_Guid = userGuid;
            _webAPIUser_TokenRepository.Create(token);
            _uow.Commit();
            return tokenNew;
        }

        public void DeleteToken(Guid userGuid)
        {
            var tokenAsUser = _webAPIUser_TokenRepository.FindAll(e => e.WebAPIUser_Guid == userGuid);
            _webAPIUser_TokenRepository.RemoveRange(tokenAsUser);
            _uow.Commit();
        }

        public bool ExpiredTokenKey(Guid tokenId)
        {
            var token = _webAPIUser_TokenRepository.FindByTokenId(tokenId);
            return token.TokenExpireDateTime < DateTime.Now;
        }

        public IEnumerable<Messagings.UserService.MasterMenuDetailResponse> GetMenuListInUser(Guid masterUser_Guid, int? applicationID = default(int?))
        {
            var menusFromOcean = _masterMenuDetailRepository.Func_Menu_Get(masterUser_Guid.ToString(), null, applicationID)
                .ConvertToMasterMenuDetailResult();
            return menusFromOcean;
        }

        public UserAuthenType GetUserAuthenTypeDetail(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName).Equals(false) && string.IsNullOrEmpty(password).Equals(false))
            {
                UserAuthenType authenDetails = new UserAuthenType();
                authenDetails.FullUsername = userName;
                authenDetails.Password = password;
                if (userName.IndexOf('/') > 0)
                {
                    authenDetails.AuthenType = AuthenType.Domain;
                    string[] authen = userName.Split('/');
                    authenDetails.UserName = authen[1];
                    authenDetails.Domain = authen[0];
                }
                else if (userName.IndexOf('@') > 0)
                {
                    authenDetails.AuthenType = AuthenType.Email;
                    var user = _masterUserRepository.FindByEmail(userName);
                    string[] authen = userName.Split('@');
                    authenDetails.UserName = user?.UserName;
                    authenDetails.Domain = authen[1];
                }
                else
                {
                    authenDetails.AuthenType = AuthenType.Local;
                    authenDetails.UserName = userName;
                    authenDetails.Domain = string.Empty;
                }
                return authenDetails;
            }
            return null;
        }

        public UserApplicationView ValidateWebAPIUser(string userName, string password)
        {
            var user = _webAPIUserRepository.FindAll(e => e.FlagDisable == false && e.UserName.ToLower() == userName.ToLower() && e.Password == password).FirstOrDefault();
            var tokenUser = _webAPIUser_TokenRepository.FindAll(e => e.WebAPIUser_Guid == user.Guid).FirstOrDefault();
            UserApplicationView userView = new UserApplicationView();
            userView.UserGuid = user?.Guid;
            userView.ApplicationGuids = user?.TblWebAPIUserApplication.Select(e => e.SystemApplication_Guid);
            userView.Token = tokenUser?.TokenID;
            userView.ExpiryDate = tokenUser?.TokenExpireDateTime;
            return userView;
        }

        public bool VerifyTokenKey(Guid tokenId)
        {
            return _webAPIUser_TokenRepository.FindByTokenId(tokenId) != null;
        }

        public bool VerifyEmail(string email)
        {
            return _masterUserRepository.FindAll(e => e.Email.ToLower() == email.ToLower()).Any();
        }

        public void CreateAccessAPILogHistory(Guid? token, DateTime requestTime, string apiMethodName, string requestBody, DateTime responseTime, string responseBody)
        {
            TblSystemApiHistory history = new TblSystemApiHistory();
            history.Guid = Guid.NewGuid();
            history.TokenID = token;
            history.DatetimeStop = responseTime;
            history.DatetimeStart = requestTime;
            history.ApiMethodName = apiMethodName;
            history.ResponseBody = responseBody;
            history.RequestBody = requestBody;
            _webAPIUserRepository.CreateHistory(history);
            _uow.Commit();
        }

        public SystemApplicationResponse RetreiveAccessToken(Guid appKey)
        {
            return _webAPIUser_TokenRepository.FindApplicationByAppKey(appKey).ConvertToSystemApplicationResponse();
        }

        public void ChangeLanguage(ChangeLanguageRequest request)
        {
            TblMasterUser userTarget = _masterUserRepository.FindById(request.MasterUser_Guid);
            userTarget.SystemLanguage_Guid = request.SystemLanguage_Guid;
            _masterUserRepository.Modify(userTarget);
            _uow.Commit();
        }

        public Guid? GetDomainAile(string ailesName)
        {
            var result = _masterUserRepository.FindDomainAilesByName(ailesName);
            return result?.Guid;
        }

        public string GetDomainServer(Guid ailesGuid)
        {
            var result = _masterUserRepository.FindDomainDC(ailesGuid);
            return result?.LdapAuthPath;
        }

        public DataStorage GetAuthenLDAP(UserAuthenType authenDetails, string serverLDAP, int applicationNumber = 1)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, serverLDAP))
            {
                if (pc.ValidateCredentials(authenDetails.UserName, authenDetails.Password))
                {
                    DataStorage authenLoginResult = GetAuthenLocal(authenDetails.UserName, authenDetails.Password, applicationNumber).ConvertAuthenResponseToDataStorage();
                    return authenLoginResult;
                }
            }
            return null;
        }

        public AuthenLoginResponse GetAuthenLDAP(AuthenDetails inAuthenDetails, string inLDAP, int applicationNumber)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, inLDAP))
            {
                if (pc.ValidateCredentials(inAuthenDetails.Username, inAuthenDetails.Password))
                {
                    var authenLoginResult = GetAuthenLocal(inAuthenDetails.Username, inAuthenDetails.Password, applicationNumber);
                    return authenLoginResult;
                }
            }
            return null;
        }

        public AuthenLoginResponse GetAuthenLocal(string username, string password, int applicationId)
        {
            var result = _masterUserRepository.Func_AuthenLogin_Get(username, password, applicationId);
            return result.ConvertToAuthenLoginResponse();
        }

        public string GetUserEmail(Guid userGuid)
        {
            return _masterUserRepository.FindById(userGuid)?.Email;
        }

        public void UpdateLastLogin(string userName, DateTime? clientDateTime, string clientSessionId)
        {
            TblMasterUser user;
            if (userName.IndexOf('/') > 0)
            {
                string[] authen = userName.Split('/');
                user = _masterUserRepository.FindByUserName(authen[1]);
            }
            else if (userName.IndexOf('\\') > 0)
            {
                string[] authen = userName.Split('\\');
                user = _masterUserRepository.FindByUserName(authen[1]);
            }
            else if (userName.IndexOf('@') > 0)
            {
                user = _masterUserRepository.FindByEmail(userName);
            }
            else
            {
                user = _masterUserRepository.FindByUserName(userName);
            }

            // login success
            user.CountInvalidLogOn = 0;
            user.LastDateTimeLogin = clientDateTime;
            user.CurrentSession = clientSessionId;
            _masterUserRepository.Modify(user);
            _uow.Commit();
        }

        public string UpdateCountInvalidLogOn(string userName, string clientSessionId, DateTime? clientDateTime)
        {
            string message = "Username or password is incorrect.";
            int activity = SystemActivityLog.AuthenticationLogin;
            string sender = userName;
            string existText = "not exist";

            // login fail
            var user = _masterUserRepository.FindByUserName(userName);
            bool userExist = user != null;

            #region Attempt to Login fail settings
            // register to base
            TblSystemLog_AttemptToLogin attemptToLogin = new TblSystemLog_AttemptToLogin();
            attemptToLogin.Guid = Guid.NewGuid();
            attemptToLogin.ClientSession_ID = clientSessionId;
            attemptToLogin.Client_IP = SystemHelper.CurrentIpAddress;
            attemptToLogin.Details = string.Format("Attempting to log in to {0}.", "Ocean Online (Web API)");
            attemptToLogin.LoginDate = clientDateTime.GetValueOrDefault();
            attemptToLogin.UserName = userName;
            attemptToLogin.FlagExistUser = userExist;
            _systemLog_AttemptToLoginRepository.Create(attemptToLogin);
            _uow.Commit();

            // get count of session registered
            List<TblSystemLog_AttemptToLogin> sessionRecords = null;
            if (userExist)
            {
                user.CountInvalidLogOn = user.CountInvalidLogOn == null ? 1 : user.CountInvalidLogOn + 1;
                sessionRecords = _systemLog_AttemptToLoginRepository
                    .FindAllAsQueryable(e => e.FlagExistUser == userExist && e.UserName == userName)
                    .OrderByDescending(e => e.LoginDate)
                    .Take(user.CountInvalidLogOn.GetValueOrDefault())
                    .ToList();
                existText = "exist";
            }
            else
            {
                sessionRecords = _systemLog_AttemptToLoginRepository
                    .FindAllAsQueryable(e => e.ClientSession_ID == clientSessionId && e.FlagExistUser == userExist)
                    .OrderByDescending(e => e.LoginDate)
                    .ToList();
            }

            int countRecord = sessionRecords.Count;

            // set body message
            string subBodyMessage = string.Empty;
            sessionRecords.ForEach(e => subBodyMessage += $"User: {e.UserName} ({existText}) {e.LoginDate}<br />");
            string bodyMessage = string.Format("To whom it may concern, <br /><br />Client IP/ NAT IP: {0}<br />Details: Attempting to log in to {1}<br /><br />",
                "API", "Ocean Online (Web API)");

            bodyMessage += subBodyMessage;
            bodyMessage += string.Format("<br />Attempt to log in over: {0} times.", countRecord);
            #endregion

            int systemLockCount = _systemEnvironment_GlobalRepository.FindSystemLockCount();
            if (userExist)
            {
                #region User is Exist
                if (user.CountInvalidLogOn >= systemLockCount || user.FlagLock == true)
                {
                    #region Lock user and notify user by email
                    // lock user
                    user.FlagLock = true;

                    // log activity lock user
                    message = string.Format("User {0} has been locked.", userName);
                    sender = "System";

                    #endregion

                    #region send e-mail
                    // send email
                    List<string> emails = new List<string>();
                    var userDetails = _masterUserRepository.Func_UserDetailCountryAndRole_Get();
                    var userCurrent = userDetails.FirstOrDefault(e => e.UserName.ToLowerInvariant() == userName.ToLowerInvariant());
                    if (userCurrent != null)
                    {
                        if (userCurrent?.RoleNumber == EnumUser.RoleType.GlobalAdmin)
                        {
                            // send to global
                            emails = _masterUserRepository.FindGroup(EnumUser.RoleType.GlobalAdmin).SelectMany(e => e.Group.Emails).ToList();
                        }
                        else if (userCurrent?.RoleNumber == EnumUser.RoleType.LocalAdmin)
                        {

                            // userName is Local Admin or Brink's user
                            // get other local admin email in country handle
                            emails = _masterUserRepository.FindGroupByUser(userCurrent.UserGuid.GetValueOrDefault()).SelectMany(e => e.Group.Emails).ToList();
                        }
                        else
                        {
                            // brink's user
                            Guid? countryGuidGroupHandle = _masterUserRepository.FindGroupByUser(userCurrent.UserGuid.GetValueOrDefault()).FirstOrDefault()?.Group.MasterCountry_Guid;
                            emails = _masterUserRepository.FindGroup(EnumUser.RoleType.LocalAdmin).Where(e => e.Group.MasterCountry_Guid == countryGuidGroupHandle).SelectMany(e => e.Group.Emails).ToList();
                        }
                    }
                    AsyncSendEmail(bodyMessage, emails);
                    #endregion
                }
                #endregion

                if (user.CountInvalidLogOn == systemLockCount && user.FlagLock.GetValueOrDefault())
                {
                    _genericLogService.InsertTransactionGenericLog(new ModelViews.GenericLog.TransactionGenericLogModel
                    {
                        DateTimeCreated = DateTime.UtcNow,
                        JSONValue = SystemHelper.GetJSONStringByArray("lblUserMngt_UserAccount", "lblUserMngt_ActionLocked"),
                        LabelIndex = SystemHelper.GetJSONStringByArray(0, 1),
                        SystemLogCategory_Guid = SFOLogCategoryHelper.ADM_USER_MGR_USER_ACCOUNT.ToGuid(),
                        SystemLogProcess_Guid = SFOProcessHelper.ADM_USER_MGR.ToGuid(),
                        SystemMsgID = "881",
                        UserCreated = user.UserName,
                        ReferenceValue = user.Guid.ToString()
                    });
                }

                // update user
                _masterUserRepository.Modify(user);
            }
            else
            {
                // send email when out of quota
                if (countRecord >= systemLockCount)
                {
                    // get all global email
                    var globalAdminEmails = _masterUserRepository.FindGroup(EnumUser.RoleType.GlobalAdmin).SelectMany(e => e.Group.Emails).ToList();
                    AsyncSendEmail(bodyMessage, globalAdminEmails);
                }
            }
            _uow.Commit();

            // log activity user login incorrect password
            _systemService.CreateLogActivity(activity, message, sender, SystemHelper.CurrentIpAddress, SystemHelper.Ocean_ApplicationGuid);

            return message;
        }

        public bool IsUserDomain(string userName)
        {
            return _masterUserRepository.Any(o => o.UserName == userName && o.SystemDomain_Guid.HasValue);
        }

        private Task AsyncSendEmail(string bodyMessage, IEnumerable<string> emails)
        {
            string signature = $@"<br /><br /><p><b>Sent from</b> API</p>";
            bodyMessage += signature;
            var mails = emails.Where(e => !string.IsNullOrEmpty(e)).ToList();
            var taskSendMail = Task.Run(() =>
            {
                //_mailService.SendEmail("Attempting to log in to Ocean Online.", bodyMessage, mails);
            });
            return taskSendMail;
        }

        public void ChangeLanguage(Guid userId, Guid languageGuid)
        {
            TblMasterUser userTarget = _masterUserRepository.FindById(userId);
            userTarget.SystemLanguage_Guid = languageGuid;
            _masterUserRepository.Modify(userTarget);
            _uow.Commit();
        }

        public SystemDomainDCResponse GetDomainDC(Guid ailesGuid)
        {
            var result = _systemDomainsRepository.FindDomainDCByAiles(ailesGuid)?.ConvertToSystemDomainDCResponse();
            return result;
        }

        public SystemDomainAilesResponse GetDomain(string ailesName)
        {
            var result = _systemDomainsRepository.FindAilesByName(ailesName)?.ConvertToSystemDoaminAilesResponse();
            return result;
        }

        public AuthenDetails GetAuthenCredential(string inUsername, string inPassword)
        {
            if (string.IsNullOrEmpty(inUsername).Equals(false) && string.IsNullOrEmpty(inPassword).Equals(false))
            {
                AuthenDetails authenDetails = new AuthenDetails();
                authenDetails.FullUsername = inUsername;
                authenDetails.Password = inPassword;

                if (inUsername.IndexOf('/') > 0)
                {
                    authenDetails.AuthenType = Bgt.Ocean.Infrastructure.Util.EnumUser.AuthenType.Domain;

                    String[] strAuthentication = inUsername.Split('/');
                    authenDetails.Username = strAuthentication[1];
                    authenDetails.Domain = strAuthentication[0];
                }
                else if (inUsername.IndexOf('@') > 0)
                {
                    authenDetails.AuthenType = Bgt.Ocean.Infrastructure.Util.EnumUser.AuthenType.Email;

                    var user = _masterUserRepository.FindByEmail(inUsername);
                    String[] strAuthentication = inUsername.Split('@');
                    authenDetails.Username = user?.UserName;
                    authenDetails.Domain = strAuthentication[1];
                }
                else
                {
                    authenDetails.AuthenType = Bgt.Ocean.Infrastructure.Util.EnumUser.AuthenType.Local;

                    authenDetails.Username = inUsername;
                    authenDetails.Domain = string.Empty;
                }
                return authenDetails;
            }
            return null;
        }

        public IEnumerable<int> GetMenuCommandInUser(Guid userGuid, int appId)
        {
            var allUserGroups = _masterUserRepository.FindGroupByUser(userGuid);
            var allGroupGuids = allUserGroups.Select(e => e.MasterGroup_Guid).ToGuidsNullable().ToList();
            var commandIds = _masterUserRepository.FindMenuCommandByGroups(allGroupGuids)
                .Where(e => e.TblMasterMenuDetailCommand.TblMasterMenuDetail.ApplicationID == appId)
                .Select(e => e.TblMasterMenuDetailCommand.CommandID).ToList();
            return commandIds.Any() ? commandIds : null;
        }

        public TblMasterGroup_MenuCommand GetMenuCommandInUserByCommandId(Guid userGuid, int appId, EnumMenuCommandId menuCommandId)
        {
            var allUserGroups = _masterUserRepository.FindGroupByUser(userGuid);
            var allGroupGuids = allUserGroups.Select(e => e.MasterGroup_Guid).ToGuidsNullable().ToList();
            var commandIds = _masterUserRepository.FindMenuCommandByGroups(allGroupGuids)
                .FirstOrDefault(e => e.TblMasterMenuDetailCommand.TblMasterMenuDetail.ApplicationID == appId &&
                                e.TblMasterMenuDetailCommand.CommandID == (int)menuCommandId);
            return commandIds;
        }

        public IEnumerable<MasterGroupView> GetMasterGroupByCountry(Guid countryGuid)
        {
            var result = _masterUserRepository.GetMasterGroupByCountry(countryGuid);
            var masterGroupViewList = result.Select(u => new MasterGroupView()
            {
                Guid = u.Guid,
                GroupName = u.GroupName,
                FlagDisable = u.FlagDisable,
                MasterCountry_Guid = u.MasterCountry_Guid.GetValueOrDefault(),
                RoleType = u.RoleType.GetValueOrDefault(),
                SystemRoleGroupType_Guid = u.SystemRoleGroupType_Guid.GetValueOrDefault()
            });

            return masterGroupViewList;
        }

        public IEnumerable<UserView> GetUserDetailByRoleType(int? roleType)
        {

            var UserDetail = _masterUserRepository.Func_UserDetailCountryAndRole_Get(null, roleType, true, false);
            return UserDetail.Select(e => new UserView
            {
                UserName = e.UserName,
                Email = e.Email,
                Guid = (Guid)e.UserGuid
            });
        }

        public MasterUserGroupView GetUserGroup(Guid userGuid)
        {
            var result = _masterUserRepository.GetUserGroup(userGuid);
            MasterUserGroupView userGroupResult = new MasterUserGroupView()
            {
                MasterUser_Guid = result.MasterUser_Guid,
                UserGroupList = result.UserGroupList.Select(c => new MasterGroupView()
                {
                    Guid = c.Guid,
                    FlagDisable = c.FlagDisable,
                    GroupName = c.GroupName,
                    MasterCountry_Guid = c.MasterCountry_Guid.GetValueOrDefault(),
                    RoleType = c.RoleType.GetValueOrDefault(),
                    SystemRoleGroupType_Guid = c.SystemRoleGroupType_Guid.GetValueOrDefault(),
                    MasterSiteGuidList = c.MasterSiteGuidList
                })
            };

            return userGroupResult;
        }

        public IEnumerable<UserGroupView> FindUserCanSeeMenu(Guid masterMenuDetailGuid, Guid masterSiteGuid)
        {
            var result = _masterUserRepository.FindUserCanSeeMenu(masterMenuDetailGuid, masterSiteGuid);
            var userGroupList = result.Select(c => new UserGroupView()
            {
                Group = new Models.Masters.GroupView()
                {
                    Guid = c.Group.Guid,
                    GroupName = c.Group.GroupName,
                    RoleType = c.Group.RoleType
                },
                User = new UserView()
                {
                    Guid = c.User.Guid,
                    UserName = c.User.UserName
                }
            });

            return userGroupList;
        }

        public IEnumerable<UserGroupView> FindUserByMenuCommand(Guid masterMenuDetailCommandGuid, Guid masterSiteGuid)
        {
            var result = _masterUserRepository.FindUserByMenuCommand(masterMenuDetailCommandGuid, masterSiteGuid);
            var userGroupList = result.Select(c => new UserGroupView()
            {
                User = new UserView()
                {
                    Guid = c.User.Guid,
                    UserName = c.User.UserName
                }
            });

            return userGroupList;
        }

        public IEnumerable<UserView> GetUsersInCompany(Guid company_Guid)
        {
            var result = _masterUserRepository.GetUsersInCompany(company_Guid);
            var userList = result.Select(c => new UserView
            {
                Guid = c.Guid,
                Email = c.Email,
                UserName = c.UserName,
                FlagDisable = c.FlagDisable,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                FullName = c.FirstName + " " + c.LastName,
                UserModify = c.UserModify,
                DateTimeModify = c.DateTimeModify
            });
            return userList;
        }

        public IEnumerable<UserView> GetUsersInCountry(Guid country_Guid, string filterUserName, string filterFullName, string filterEmail, int? pageNumber, int? numberPerPage, string sortBy, string sortWith)
        {
            var result = _masterUserRepository.GetUsersInCountry(country_Guid, filterUserName, filterFullName, filterEmail, pageNumber, numberPerPage, sortBy, sortWith);
            var userList = result.Select(c => new UserView
            {
                Guid = c.Guid,
                Email = c.Email,
                UserName = c.UserName,
                FlagDisable = c.FlagDisable,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                FullName = c.FirstName + " " + c.LastName,
                UserModify = c.UserModify,
                DateTimeModify = c.DateTimeModify,
                TotalRecord = c.TotalRecord
            });
            return userList;
        }

        #region Authentication V3
        public UserAuthenType GetUserAuthenTypeDetail_v3(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                UserAuthenType authenDetails = new UserAuthenType();
                authenDetails.FullUsername = userName;
                authenDetails.Password = password;
                if (userName.IndexOf('\\') > 0)
                {
                    authenDetails.AuthenType = AuthenType.Domain;
                    string[] authen = userName.Split('\\');
                    authenDetails.UserName = authen[1];
                    authenDetails.Domain = authen[0];
                }
                else if (userName.IndexOf('@') > 0)
                {
                    authenDetails.AuthenType = AuthenType.Email;
                    var user = _masterUserRepository.FindByEmail(userName);
                    string[] authen = userName.Split('@');
                    authenDetails.UserName = user?.UserName;
                    authenDetails.Domain = authen[1];
                }
                else
                {
                    authenDetails.AuthenType = AuthenType.Local;
                    authenDetails.UserName = userName;
                    authenDetails.Domain = string.Empty;
                }
                return authenDetails;
            }
            return null;
        }

        public bool CanAccessInThisTime(DateTime? clientDateTime, Guid userGuid)
        {
            var limitedTimeAccess = _masterUserLimitedTimeAccessRepository.FindAll(e => e.MasterUser_Guid == userGuid);
            int clientDayOfWeek = (int)clientDateTime.GetValueOrDefault(DateTime.Now).DayOfWeek;
            var clientTimeOfDay = clientDateTime.GetValueOrDefault(DateTime.Now).TimeOfDay;

            bool dayCanAccess = limitedTimeAccess.Any(
                e => e.TblSystemDayOfWeek.MasterDayOfWeek_Sequence.GetValueOrDefault() == (clientDayOfWeek + 1) &&
                clientTimeOfDay > e.AccessStart.GetValueOrDefault().TimeOfDay &&
                clientTimeOfDay < e.AccessEnd.GetValueOrDefault().TimeOfDay);

            return dayCanAccess;
        }

        public string GetEmployeeID(Guid userGuid)
        {
            return _masterUserRepository.FindById(userGuid).StaffID;
        }

        public UserView GetUserView(string userName)
        {
            var user = _masterUserRepository.FindByUserName(userName);
            return user.ConvertToUserView();
        }
        #endregion

        #region Password
        public int ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var user = _masterUserRepository.FindByUserName(userName);

            // Check password with lastest xx
            var countPasswordStore = _systemEnvironment_GlobalRepository.FindByAppKey(EnumAppKey.NotUsePWDWithInTime.ToString()).AppValue1;
            var stories = _masterUserRepository.FindLastLogin(user.Guid, countPasswordStore.ToInt());
            foreach (var item in stories.Where(o => o.SaltKey.HasValue))
            {
                var encryptPassword = _masterUserRepository.EncryptPassword(newPassword, item.SaltKey.Value);
                if (encryptPassword == item.PasswordLastTime)
                    return -308;
            }

            ClearOldVerifyKey(user.Guid);
            var result = _masterUserRepository.Func_ChangePassword_Set(userName, string.IsNullOrEmpty(oldPassword) ? null : oldPassword, newPassword);
            if (result > 0)
            {
                _systemService.CreateLogActivity(SystemActivityLog.ChangePassword, "Changed password successful.", userName, HttpContext.Current.Request.UserHostAddress, ApplicationKey.OceanOnline);

                /* Insert Audit Log */
                var genericLogList = new TransactionGenericLogModel()
                {
                    DateTimeCreated = DateTime.UtcNow,
                    JSONValue = SystemHelper.GetJSONStringByArray("lblUserMngt_LogPassword", "lblUserMngt_ActionChanged"),
                    LabelIndex = SystemHelper.GetJSONStringByArray(0, 1),
                    ReferenceValue = user.Guid.ToString(),
                    SystemLogCategory_Guid = SFOLogCategoryHelper.ADM_USER_MGR_PASSWORD.ToGuid(),
                    SystemLogProcess_Guid = SFOProcessHelper.ADM_USER_MGR.ToGuid(),
                    SystemMsgID = "881",
                    UserCreated = user.UserName
                };
                _genericLogService.InsertTransactionGenericLog(genericLogList);
            }
            return result;
        }

        public int ResetPassword(ForgotPasswordViewModel request)
        {
            using (var transaction = _uow.BeginTransaction())
            {
                var user = _masterUserRepository.FindAll(o => !o.FlagDisable && !string.IsNullOrEmpty(o.Email) && o.Email.ToLower() == request.Email.ToLower()).FirstOrDefault();
                if (user != null)
                {
                    ClearOldVerifyKey(user.Guid);

                    if (user.SystemDomain_Guid == null)
                    {
                        var newVerifyKey = SystemHelper.GetGeneratedVerifyKey();

                        if (ResetUserPassword(user, SystemHelper.GetGeneratedPassword))
                        {
                            // create verify key
                            SaveVerifyKey(user.Guid, newVerifyKey);

                            /* Insert Audit Log */
                            var genericLogList = new TransactionGenericLogModel()
                            {
                                DateTimeCreated = DateTime.UtcNow,
                                JSONValue = SystemHelper.GetJSONStringByArray("lblUserMngt_LogPassword", "lblUserMngt_ActionReset"),
                                LabelIndex = SystemHelper.GetJSONStringByArray(0, 1),
                                ReferenceValue = user.Guid.ToString(),
                                SystemLogCategory_Guid = SFOLogCategoryHelper.ADM_USER_MGR_PASSWORD.ToGuid(),
                                SystemLogProcess_Guid = SFOProcessHelper.ADM_USER_MGR.ToGuid(),
                                SystemMsgID = "881",
                                UserCreated = user.UserName
                            };
                            _genericLogService.InsertTransactionGenericLog(genericLogList);

                            var sendEmail = SendEmail_ForgetPassword(newVerifyKey, request.Email, user.FlagLock.GetValueOrDefault(), user.FlagDisable);

                            _uow.Commit();
                            transaction.Complete();

                            if (sendEmail) return 884;
                        }
                    }
                    else
                    {
                        return 885;
                    }
                }
            }
            return -184;
        }

        public int ResetPassword(Guid userGuid)
        {
            var user = _masterUserRepository.FindById(userGuid);
            return ResetPassword(new ForgotPasswordViewModel() { Email = user.Email });
        }

        private void ClearOldVerifyKey(Guid userId)
        {
            var data = _masterLogVerifyKeyRepository.FindAll(o => o.MasterUser_Guid == userId && o.Action).ToList();
            if (data.Any())
            {
                data.ForEach(o =>
                {
                    o.Action = false;
                    _masterLogVerifyKeyRepository.Modify(o);
                });

                _uow.Commit();
            }
        }

        private void SaveVerifyKey(Guid userGuid, string verifyKey)
        {
            var systemKeyExpire = _systemEnvironment_GlobalRepository.FindByAppKey(SystemHelper.EnumAppKey.KeyExpire.ToString()).AppValue1;
            var insertVerifyKey = new TblMasterLogVerifyKey()
            {
                Guid = Guid.NewGuid(),
                Action = true,
                Verify_key = verifyKey,
                MasterUser_Guid = userGuid,
                KeyExpire = DateTime.UtcNow.AddHours(Convert.ToInt32(systemKeyExpire)),
            };
            _masterLogVerifyKeyRepository.Create(insertVerifyKey);
        }

        private bool ResetUserPassword(TblMasterUser inUser, string inNewPassword)
        {
            int result = _masterUserRepository.Func_ResetPassword_Set(inUser.UserName, inNewPassword);
            return result == 1;
        }
        #endregion

        #region Email
        private bool SendEmail_ForgetPassword(string verifyKey, string userEmail, bool flagLock = false, bool flagDisable = false)
        {
            try
            {
                var domain = _systemEnvironment_GlobalRepository.FindByAppKey(SystemHelper.EnumAppKey.OO_WEB_HOSTNAME).AppValue1;
                var template = _masterEmailTemplateRepository.FindByName((flagLock || flagDisable) ? EMAIL_CANNOT_RESETPASSWORD : EMAIL_RESETPASSWORD);
                if (domain != null && template != null)
                {
                    string[] parameters = new string[] { domain + "ChangePassword", verifyKey };
                    List<string> mails = userEmail.Split(',').ToList();
                    var email = new EmailModel()
                    {
                        EmailTo = mails.Select(o => new EmailAddress() { Email = o }).ToList(),
                        Subject = template.EmailSubject,
                        Body = string.Format(template.EmailBody, parameters)
                    };
                    return _emailService.SendEmail(email);
                }
                return false;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryErrorAsync(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, SystemHelper.ClientHostName);
            }
            return false;
        }
        #endregion
    }

}

