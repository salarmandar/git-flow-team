using Bgt.Ocean.Infrastructure.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class SystemHelper
    {
        #region Objects & Variables
        private const int LENGTH_MINIMUN = 8;
        private const int LENGTH_UPPERCASE = 1;
        private const int LENGTH_LOWERCASE = 1;
        private const int LENGTH_NUMERIC = 1;
        private const int LENGTH_NONALPHA = 1;
        #endregion

        public static Guid Ocean_ApplicationGuid { get { return Guid.Parse("57def301-431d-4aae-880c-e9f9a8eaa897"); } }
        public static Guid SFO_ApplicationGuid { get { return Guid.Parse("4a7318d2-e41e-43f9-ae5a-e81d21766a6f"); } }

        public static string CurrentIpAddress
        {
            get
            {
                try
                {
                    bool GetLan = false;
                    string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (string.IsNullOrEmpty(visitorIPAddress))
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

                        visitorIPAddress = Dns.GetHostEntry(stringHostName).AddressList[1].ToString();

                    }

                    return visitorIPAddress;
                }
                catch { return "IP address is undefind."; }
            }
        }

        public static int Ocean_ApplicationId { get { return 1; } }

        public static string CurrentPageUri
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.Url.AbsoluteUri;
                }
                catch { return "Page is undefind."; }
            }
        }

        public static IEnumerable<Guid> ToGuids(this IEnumerable<Guid?> source)
        {
            List<Guid> n = source.Select(e => e.GetValueOrDefault()).ToList();
            return n;
        }

        public static IEnumerable<Guid?> ToGuidsNullable(this IEnumerable<Guid> source)
        {
            List<Guid?> n = source.Select(e => e == Guid.Empty ? (Guid?)null : e).ToList();
            return n;
        }

        public static Guid ToGuid(this string s)
        {
            Guid guid;
            if (Guid.TryParse(s, out guid))
                return guid;
            return Guid.Empty;
        }

        public static decimal ToDecimal(this string s)
        {
            decimal dec;
            if (decimal.TryParse(s, out dec))
                return Convert.ToDecimal(s);
            return decimal.Zero;
        }

        public static bool ToBoolean(this string s)
        {
            bool b;
            if (bool.TryParse(s, out b))
                return Convert.ToBoolean(s);
            return false;
        }

        public static int ToInt(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return Convert.ToInt32(s);
        }

        public static bool VerifyNullHeaderKey(IEnumerable<string> val)
        {
            if (val == null || !val.Any() || string.IsNullOrWhiteSpace(val.FirstOrDefault()))
            {
                return true;
            }

            return false;
        }

        public static bool IsNullOrEmpty(this Guid? guid)
        {
            return !guid.HasValue || guid.GetValueOrDefault() == Guid.Empty;
        }

        public static bool IsDifferent<T>(this IEnumerable<T> self, IEnumerable<T> to)
        {
            return self.Except(to).Any() || to.Except(self).Any();
        }

        public static Stream ConvertToStream(this byte[] byteArray)
        {
            return new MemoryStream(byteArray);
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, Func<T, T, bool> comparer)
        {
            return enumerable.Distinct(new LambdaComparer<T>(comparer));
        }

        public static int GetNumber(this string id)
        {
            var digits = id.Where(c => Char.IsDigit(c)).ToArray();
            return new string(digits).ToInt();
        }

        public static bool IsNumeric(this string txt)
        {
            try
            {
                decimal num;
                return Decimal.TryParse(txt, out num);
            }
            catch
            { return false; }
        }

        private class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _lambdaComparer;
            private readonly Func<T, int> _lambdaHash;

            public LambdaComparer(Func<T, T, bool> lambdaComparer) :
                this(lambdaComparer, o => 0)
            {
            }

            public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
            {
                if (lambdaComparer == null)
                    throw new ArgumentNullException("lambdaComparer");
                if (lambdaHash == null)
                    throw new ArgumentNullException("lambdaHash");

                _lambdaComparer = lambdaComparer;
                _lambdaHash = lambdaHash;
            }

            public bool Equals(T x, T y)
            {
                return _lambdaComparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _lambdaHash(obj);
            }
        }

        public static int?[] ConvertToInts(this string[] str_array)
        {
            int[] ints = Array.ConvertAll(str_array, s => int.Parse(s));
            return ints.Cast<int?>().ToArray();
        }

        public static Dictionary<string, decimal> GetDecimalToCompare<T>(this T o)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            var type = typeof(T);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(e => e.Name))
            {
                var val = type.GetProperty(prop.Name).GetValue(o, null)?.ToString() ?? string.Empty;
                if (val.IsMoney())
                {
                    decimal d = 0;
                    decimal.TryParse(val.ToString(), out d);
                    result.Add(prop.Name, d);
                }
            }
            return result;
        }

        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (typeof(T).Name == typeof(char).Name && enumerable != null)
            {
                string str = string.Join("", enumerable);

                return string.IsNullOrWhiteSpace(str);
            }

            return enumerable == null || !enumerable.Any();
        }

        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ClientHostName
        {
            get
            {
                var hostName = ConfigurationManager.AppSettings["HostName"];
                string result = string.Empty;
                if (hostName != null)
                {
                    result = Dns.GetHostEntry(hostName)?.HostName;
                }

                return result;
            }
        }

        public static DateTime ToTimeDateTime(this string value)
        {
            if (value == null)
                value = string.Empty;

            value = value.Replace('_', '0');
            value = string.IsNullOrEmpty(value) ? "00:00" : value;
            string[] time = value.Split(':');

            string hr = "00", min = "00";   //Ping Edit -> Fix bug Index out of bound because input value doesn't have ":"
            if (time.Length > 0)
                hr = time[0];
            if (time.Length > 1)
                min = time[1];

            // Top Edit : In the Ocean Old verstion use 1900 not use 1990 if you have a new value please tell me
            DateTime datetime = new DateTime(1900, 1, 1, int.Parse(hr), int.Parse(min), 00);

            return datetime;
        }

        public static string ToJSONString<T>(this ICollection<T> historyParams)
        {
            var result = string.Empty;
            try
            {
                //New Format JSON
                result = JsonConvert.SerializeObject(historyParams);
            }
            catch (Exception)
            {
                //Old Format
                result = string.Join(",", historyParams);
            }

            return result;
        }

        public static string SeparateWordByUppercase(this string str, string separator = " ")
        {
            if (str.IsEmpty()) return string.Empty;

            var data = System.Text.RegularExpressions.Regex.Split(str, @"(?<!^)(?=[A-Z])");

            return string.Join(
                    separator,
                    data.Select(s => $"{s.Replace("_", "").First().ToString().ToUpper()}{s.Replace("_", "").Substring(1)}")

                );
        }

        public static string GetJSONStringByArray<T>(params T[] collection)
            => collection.ToJSONString();

        public static string Truncate(this string str, int length)
        {
            try
            {
                if (str?.Length > length)
                    return str.Substring(0, length);

                return str;
            }
            catch
            {
                return str;
            }
        }

        public static string ToPascalString(this string value)
        {
            // Creates a TextInfo based on the "en-US" culture. 
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            // Changes a string to titlecase. Must turn to lowercase fist
            return textInfo.ToTitleCase(value.ToLower());
        }

        #region Password
        public static String GetGeneratedPassword
        {
            get
            {
                string newPassword;
                do
                {
                    newPassword = GeneratePassword(EnumUser.PasswordLength.Manual, 8);
                }
                while (ValidatePasswordPolicy(newPassword).Equals(false));
                return newPassword;
            }
        }

        private static Boolean ValidatePasswordPolicy(string Password)
        {
            if ((Password.Length < LENGTH_MINIMUN)
                || (CountUpperCase(Password) < LENGTH_UPPERCASE)
                || (CountLowerCase(Password) < LENGTH_LOWERCASE)
                || (CountNumeric(Password) < LENGTH_NUMERIC)
                || (CountNonAlpha(Password) < LENGTH_NONALPHA))
            {
                return false;
            }
            return true;
        }

        private static String GeneratePassword(EnumUser.PasswordLength inGenerateType, Int32 inLenght)
        {
            String result = String.Empty;
            switch (inGenerateType)
            {
                case EnumUser.PasswordLength.AutoGenerate:
                    Random lengthRandom = new Random();
                    result = GenerateProcess(lengthRandom.Next(8, 16));
                    break;
                case EnumUser.PasswordLength.Manual:
                    result = GenerateProcess(inLenght);
                    break;
            }
            return result;
        }

        private static string GenerateProcess(int inLenght)
        {
            StringBuilder password = new StringBuilder();
            Random randomNum = new Random();

            String strLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz=+-_@";
            String strNums = "0123456789";

            while (password.Length < inLenght)
            {
                if (randomNum.Next(100) > 70)
                {
                    password.Append(strLetters[randomNum.Next(strLetters.Length)]);
                }
                else
                {
                    password.Append(strNums[randomNum.Next(strNums.Length)]);
                }
            }
            return password.ToString();
        }

        public static string GetGeneratedVerifyKey(int inLenght = 20)
        {
            StringBuilder verifyKey = new StringBuilder();
            Random randomIndex = new Random();

            String strAll = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghojklmnopqrstuvwxyz";

            for (Int32 i = 1; i <= inLenght; i++)
            {
                verifyKey.Append(strAll[randomIndex.Next(strAll.Length)]);
            }
            return verifyKey.ToString();
        }
        #endregion

        #region Regex Count
        private static Int32 CountUpperCase(String inText)
        {
            return Regex.Matches(inText, "[A-Z]").Count;
        }

        private static Int32 CountLowerCase(String inText)
        {
            return Regex.Matches(inText, "[a-z]").Count;
        }

        private static Int32 CountNumeric(String inText)
        {
            return Regex.Matches(inText, "[0-9]").Count;
        }

        private static Int32 CountNonAlpha(String inText)
        {
            return Regex.Matches(inText, "[^0-9a-zA-Z\\._]").Count;
        }
        #endregion

        #region Country Option
        public static class EnumAppKey
        {
            /// <summary>
            /// [EnumDescription("Flag request approve cancel job")]
            /// </summary>
            public const string FlagRequestCancelApprove = "FlagRequestCancelApprove";
            /// <summary>
            /// [EnumDescription("Calculate premise time per job on update job in progress (AR : arrival time , AC : actual time , DP : departure time)")]
            /// </summary>
            public const string CalculatePremiseTimePerJob = "CalculatePremiseTimePerJob";
            /// <summary>
            /// [EnumDescription("True,False")]
            /// </summary>
            public const string FlagReOrderWorkSeqWhenDispatch = "FlagReOrderWorkSeqWhenDispatch";
            /// <summary>
            /// [EnumDescription("Flag used BGS Pickup HK stlye")]
            /// </summary>
            public const string FlagUsedBgsPuHKStlye = "FlagUsedBgsPuHKStlye";
            /// <summary>
            /// [EnumDescription("Minimum seal digi")]
            /// </summary>
            public const string MinmumSealDigi = "MinmumSealDigi";

            /// <summary>
            /// [EnumDescription("Max Liability digi (STC)")]
            /// </summary>
            public const string MaxLiabilityDigit = "MaxLiabilityDigit";

            /// <summary>
            /// [EnumDescription("")]
            /// </summary>
            public const string RoleIDInRunResourceDailyDuplicate = "RoleIDInRunResourceDailyDuplicate";
            /// <summary>
            /// [EnumDescription("Default Home Currency")]
            /// </summary>
            public const string HomeCurrency = "HomeCurrency";
            /// <summary>
            /// [EnumDescription("1 : Time Inputed , 2 : In Vault")]
            /// </summary>
            public const string CloseRunForPUJobRule = "CloseRunForPUJobRule";
            /// <summary>
            /// [EnumDescription("Requite register employee return equipment")]
            /// </summary>
            public const string FlagRequiteRegisterEmployeeReturnEquipment = "FlagRequiteRegisterEmployeeReturnEquipment";
            /// <summary>
            /// [EnumDescription("Requite register employee return equipment")]
            /// </summary>
            public const string DefaultGenerateDailyJob = "DefaultGenerateDailyJob";
            /// <summary>
            /// [EnumDescription("Alert periodic maintenance when closed run")]
            /// </summary>
            public const string FlagAlertPMWhenClosedRun = "FlagAlertPMWhenClosedRun";
            /// <summary>
            /// [EnumDescription("Check role on run used mobile")]
            /// </summary>
            public const string RoleIDUsedMobile = "RoleIDUsedMobile";
            /// <summary>
            /// [EnumDescription("Dispatch run automatic when create job from WS")]
            /// </summary>
            public const string FlagDispatchAutoWhenCreateJobFromWS = "FlagDispatchAutoWhenCreateJobFromWS";
            /// <summary>
            /// [EnumDescription("Validate time sort on customize sort")]
            /// </summary>
            public const string FlagValidateTimeSortByCustomize = "FlagValidateTimeSortByCustomize";
            /// <summary>
            /// [EnumDescription("Show location name  D leg for T,TV")]
            /// </summary>
            public const string FlagShowLocationDleg = "FlagShowLocationDleg";
            /// <summary>
            /// [EnumDescription("Equipment use RFID code")]
            /// </summary>
            public const string FlagEquipmetUseRFID = "FlagEquipmetUseRFID";
            /// <summary>
            /// [EnumDescription("Input time 24 hrs or 12 hrs.")]
            /// </summary>
            public const string FlagInputTime24Hrs = "FlagInputTime24Hrs";
            /// <summary>
            /// [EnumDescription("Allow create job without contract")]
            /// </summary>
            public const string FlagAllowCreateJobWithoutContract = "FlagAllowCreateJobWithoutContract";
            /// <summary>
            /// [EnumDescription("Associated contract No. to job")]
            /// </summary>
            public const string FlagAssociatedContractNoToJob = "FlagAssociatedContractNoToJob";
            /// <summary>
            /// [EnumDescription("Flag request approve change schedule time")]
            /// </summary>
            public const string FlagRequestChangeScheduleTimeApprove = "FlagRequestChangeScheduleTimeApprove";
            /// <summary>
            /// [EnumDescription("It's Number Follow ( RoleInRunResoureID )")]
            /// </summary>
            public const string RoleAllowSendToPreVault = "RoleAllowSendToPreVault";
            /// <summary>
            /// [EnumDescription("It's Number Follow ( RoleInRunResoureID )")]
            /// </summary>
            public const string FlagSkipCheckInFromMP = "FlagSkipCheckInFromMP";
            /// <summary>
            /// [EnumDescription("LOB allow duplicate seal number")]
            /// </summary>
            public const string LobIdAllowDuplicateSeal = "FlagLobIdAllowDuplicateSeal";
            /// <summary>
            /// [EnumDescription("1 : Search only employee permission 2 : Search only employee have working on that day 3 : Search only employee have job with cusomter search.")]
            /// </summary>
            public const string CustomerSearchCrew = "CustomerSearchCrew";
            /// <summary>
            /// [EnumDescription("True : INJOb , False : IN SITE")]
            /// </summary>
            public const string FlagAllowDispatchWithoutRoleCheckIn = "FlagAllowDispatchWithoutRoleCheckIn";
            /// <summary>
            /// [EnumDescription("Validate duplicate seal per Country ON : Not allow duplicate seal in same country OFF : Not allow duplicate seal in same job")]
            /// </summary>
            public const string SealDuplicate = "SealDuplicate";
            /// <summary>
            /// [EnumDescription("Trip indicator required")]
            /// </summary>
            public const string FlagRequiredTripIndicator = "FlagRequiredTripIndicator";
            /// <summary>
            /// [EnumDescription("Days for Alert Password Before Expire")]
            /// </summary>
            public const string DaysAlertPasswordBeforeExpire = "DaysAlertPasswordBeforeExpire";
            /// <summary>
            /// [EnumDescription("Not Use PWD Within Time")]
            /// </summary>
            public const string NotUsePWDWithInTime = "NotUsePWDWithInTime";
            /// <summary>
            /// [EnumDescription("Key Expire")]
            /// </summary>
            public const string KeyExpire = "KeyExpire";
            /// <summary>
            /// [EnumDescription("Job duplicate validation: True = can't aprove for job duplicate data , False = aprove for job duplicate data")]
            /// </summary>
            public const string FlagJobDupValidate = "FlagJobDupValidate";
            /// <summary>
            /// [EnumDescription("Number of times input verify key invalid.")]
            /// </summary>
            public const string InvalidAttempKey = "InvalidAttempKey";
            /// <summary>
            /// [EnumDescription("Authen type by Active Directory Only.")]
            /// </summary>
            public const string FlagAuthenByAdOnly = "FlagAuthenByAdOnly";
            /// <summary>
            /// [EnumDescription("Start date and end date validate with contract.")]
            /// </summary>
            public const string FlagValidateStartEndDateWithContract = "FlagValidateStartEndDateWithContract";
            /// <summary>
            /// [EnumDescription("Allow withdraw equipment without crew on the run")]
            /// </summary>
            public const string FlagAllowWithdrawEquipmentWithoutRun = "FlagAllowWithdrawEquipmentWithoutRun";
            /// <summary>
            /// [EnumDescription("L2C check exceed with this percent")]
            /// </summary>
            public const string ExceedPercentage = "ExceedPercentage";
            /// <summary>
            /// [EnumDescription("Maximum row search")]
            /// </summary>
            public const string MaxDefaultRowSearch = "MaxDefaultRowSearch";
            /// <summary>
            /// FlagRequiredLocationCode : true = request
            /// </summary>
            public const string FlagRequiredLocationCode = "FlagRequiredLocationCode";
            /// <summary>
            /// [EnumDescription("Maximum Ditgit Location Code")]
            /// </summary>
            public const string MaximumDigitLocationCodeRef = "MaximumDigisLocationCodeRef";
            /// <summary>
            /// [EnumDescription("Numeric Only")]
            /// </summary>
            public const string FlagLocationCodeNumericOnly = "FlagLocationCodeNumericOnly"; //added: 2018/05/04
            /// <summary>
            /// [EnumDescription("All Capital Letter")]
            /// </summary>
            public const string FlagLocationCodeAllCapitalLetter = "FlagLocationCodeAllCapitalLetter"; //added: 2018/05/04
            /// <summary>
            /// [EnumDescription("Require Exact Length")]
            /// </summary>
            public const string FlagLocationCodeRequireExactLength = "FlagLocationCodeRequireExactLength"; //added: 2018/05/04
            /// <summary>
            /// [EnumDescription("Show/hide select time for MX")]
            /// </summary>
            public const string InputTimeOnCrewManagement = "InputTimeOnCrewManagement";
            /// <summary>
            /// [EnumDescription("Allow edit job detail for delivery job"]
            /// </summary>
            public const string FlagAllowEditJobDetailForDLeg = "FlagAllowEditJobDetailForDLeg";

            public const string DaysForSendAlertAccessUserGroup = "daysForSendAlert";
            /// <summary>
            /// FlagAllowDuplicateCommercialRegId : true = allow
            /// </summary>
            public const string FlagAllowDuplicateCommercialRegId = "FlagAllowDuplicateCommercialRegId";
            /// <summary>
            /// [EnumDescription("Faster Data Entry Barcode allow continuous without validate STC Value")]
            /// </summary>
            public const string FlagFasterDataEntryNonBarcodeValidateSTC = "FlagFasterDataEntryNonBarcodeValidateSTC";
            /// <summary>
            /// [EnumDescription("Automatic consolidate delivery location when execute pickup")]
            /// </summary>
            public const string FlagAutoConsolidateDeliveryLocation = "FlagAutoConsolidateDeliveryLocation";
            /// <summary>
            /// [EnumDescription("Determines if in the country is used DolphinVault")]
            /// </summary>
            public const string FlagUsedDolhinVault = "UseDolphinVault";
            /// <summary>
            /// [EnumDescription("Machine Assigned Branch Is Mandatory")]
            /// </summary>
            public const string FlagAssignedBranchIsMandatory = "FlagAssignedBranchIsMandatory";

            public const string FlagAutoGenOTCBase = "FlagAutoGenOTCBase";

            public const string FlagAutoCreateSLMfromOtherTicket = "FlagAutoCreateSLMfromOtherTicket";

            public const string OO_WEB_HOSTNAME = "OO_WEB_HOSTNAME";
            public const string DefaultCurrency = "DefaultCurrency";

            public const string ISAWebServicePath = "ISAWebServicePath";
            public const string ISASecretKey = "ISASecretKey";
            public const string FlagEnableISA = "FlagEnableISA";

            public const string RouteOptimizationDirectoryPath = "RouteOptimizationDirectoryPath";
            /// <summary>
            /// System Config (Hide Identity Fields)
            /// </summary>
            public const string ShowOrHideIdentifyFields = "ShowOrHideIdentifyFields";
        }

        public static class Application
        {
            public const int Ocean = 1;
            public const int Aquarium = 2;
            public const int L2C = 3;
            public const int StarFish = 4;
            public const int HKEx = 5;
        }

        public static class FixStringHelper
        {
            /// <summary>
            /// EnumAppKey Global
            /// </summary>
            public const string MaxRowSearch = "MaxRowSearch";
        }
        #endregion
    }

    public static class SystemActivityLog
    {
        public const int AuthenticationLogin = 1;
        public const int ChangePassword = 2;
        public const int CreateUser = 3;
        public const int LockedUser = 4;
        public const int UpdateUser = 5;
        public const int EnableUser = 6;
        public const int UnlockUser = 7;
        public const int DisableUser = 8;
        public const int GenericActivity = 9;
        public const int APIActivity = 10;
        public const int SendEmail = 11;
        public const int DPOOAPIActivity = 17;
        public const int FluentScheduler = 18;
    }


}
