using System;
using System.Configuration;

namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumGlobalEnvironment
    {
        public static class appkey
        {
            public const string Connection_Report_Config = "ConnectionReportConfig";
            public const string MaxRow_RequestBCO = "MaxRowRequestBCO";
            public const string Url_Mobile_PIN = "UrlMobilePIN";
            public const string Dolphin_Jobs = "DolphinJobs";
            public const string Dolphin_Call = "DolphinCall";
            public const string OceanAPI_Config = "OceanAPIConfig";
            public const string Ocean_Close_Seal_Config = "OceanCloseSealConfig";
            public const string SFOAPI_Config = "SFOAPIConfig";
            public const string NemoAPI_Config = "NemoAPIConfig";
            public const string Session_Timeout_Config = "SessionTimeoutConfig";
            public const string Poseidon_Config = "POSEIDON_API";
            public const string DTS_Config = "DTSConfig";
            public const string Nemo_Config = "NemoConfig";
            public const string Nemo_Integration = "NemoIntegration";
            public const string DolphinVault = "DolphinVaultApi";

            public const string BulkSaveComit = "BulkSaveComit";

            public const string FlagEnableISA = "FlagEnableISA";

            #region SFO

            public const string OTC_GEN_URL = "OTC_GEN_URL";

            #endregion

        }

        public static bool IsStaging() => ConfigurationManager.AppSettings["EnvSTG"].ToBoolean();

        public static string IsStaging(this string appkey)
        {
            bool isStaging = IsStaging();
            appkey = isStaging ? $"{appkey}{"_STG"}" : appkey;
            return appkey;
        }

        public static bool ToBoolean(this string s)
        {
            bool b;
            if (bool.TryParse(s, out b))
                return Convert.ToBoolean(s);
            return false;
        }

        public static class EnvironmentSetting
        {
            public const string HostName = "127.0.0.1";
            public const string SystemFormatDate = "MM/dd/yyyy|dd/MM/yyyy|yyyy.MM.dd|dd.MM.yyyy|dd-MM-yyyy|dd/MM/yyyy|MM-dd-yyyy|MMM dd, yyyy|yyyy/MM/dd|dd MMM yyyy";
        }
      
    }
}
