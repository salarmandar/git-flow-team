using System;
using System.Web;

namespace Bgt.Ocean.Infrastructure.Storages
{
    public static class ApiSession
    {
        private const string LANGUAGE_KEY = "LANGUAGE_KEY";
        public static Guid? UserLanguage_Guid
        {
            get
            {
                return HttpContext.Current.Items[LANGUAGE_KEY] == null ? null : (Guid?)HttpContext.Current.Items[LANGUAGE_KEY];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(LANGUAGE_KEY))
                    HttpContext.Current.Items.Add(LANGUAGE_KEY, value);
                else
                    HttpContext.Current.Items[LANGUAGE_KEY] = value;
            }
        }

        private const string AUTH_KEY = "AUTH_KEY";
        public static string Authentication_Key
        {
            get
            {
                return HttpContext.Current.Items[AUTH_KEY] == null ? null : (string)HttpContext.Current.Items[AUTH_KEY];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(AUTH_KEY))
                    HttpContext.Current.Items.Add(AUTH_KEY, value);
                else
                    HttpContext.Current.Items[AUTH_KEY] = value;
            }
        }

        private const string APP_KEY = "APP_KEY";
        public static string Application_Token
        {
            get
            {
                return HttpContext.Current.Items[APP_KEY] == null ? null : (string)HttpContext.Current.Items[APP_KEY];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(APP_KEY))
                    HttpContext.Current.Items.Add(APP_KEY, value);
                else
                    HttpContext.Current.Items[APP_KEY] = value;
            }
        }

        private const string DATE_OFFSET = "DATE_OFFSET";
        public static int UtcOffset
        {
            get
            {
                return HttpContext.Current.Items[DATE_OFFSET] == null ? 0 : (0 - (int)HttpContext.Current.Items[DATE_OFFSET]);
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(DATE_OFFSET))
                    HttpContext.Current.Items.Add(DATE_OFFSET, value);
                else
                    HttpContext.Current.Items[DATE_OFFSET] = value;
            }
        }

        private const string SESSION_API_APP_NUMBER_ID = "SESSION_API_NUMBER_USER";
        public static int? ApplicationNumber
        {
            get
            {
                return HttpContext.Current.Items[SESSION_API_APP_NUMBER_ID] == null ? null : (int?)HttpContext.Current.Items[SESSION_API_APP_NUMBER_ID];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(SESSION_API_APP_NUMBER_ID))
                    HttpContext.Current.Items.Add(SESSION_API_APP_NUMBER_ID, value);
                else
                    HttpContext.Current.Items[SESSION_API_APP_NUMBER_ID] = value;
            }
        }

        private const string SESSION_API_APPLICATION_GUID = "SESSION_API_APPLICATION_GUID_USER";
        public static Guid? Application_Guid
        {
            get
            {
                return HttpContext.Current.Items[SESSION_API_APPLICATION_GUID] == null ? null : (Guid?)HttpContext.Current.Items[SESSION_API_APPLICATION_GUID];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(SESSION_API_APPLICATION_GUID))
                    HttpContext.Current.Items.Add(SESSION_API_APPLICATION_GUID, value);
                else
                    HttpContext.Current.Items[SESSION_API_APPLICATION_GUID] = value;
            }
        }

        private const string USER_GUID = "USER_GUID";
        public static Guid? UserGuid
        {
            get
            {
                return HttpContext.Current.Items[USER_GUID] == null ? null : (Guid?)HttpContext.Current.Items[USER_GUID];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(USER_GUID))
                    HttpContext.Current.Items.Add(USER_GUID, value);
                else
                    HttpContext.Current.Items[USER_GUID] = value;
            }
        }

        private const string USER_NAME = "USER_NAME";
        public static string UserName
        {
            get
            {
                return HttpContext.Current.Items[USER_NAME] == null ? null : (string)HttpContext.Current.Items[USER_NAME];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(USER_NAME))
                    HttpContext.Current.Items.Add(USER_NAME, value);
                else
                    HttpContext.Current.Items[USER_NAME] = value;
            }
        }

        private const string USER_FORMATDATE = "USER_FORMATDATE";
        public static string UserFormatDate
        {
            get
            {
                return HttpContext.Current.Items[USER_FORMATDATE] == null ? null : (string)HttpContext.Current.Items[USER_FORMATDATE];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(USER_FORMATDATE))
                    HttpContext.Current.Items.Add(USER_FORMATDATE, value);
                else
                    HttpContext.Current.Items[USER_FORMATDATE] = value;
            }
        }

        private const string CLIENT_DATETIME = "CLIENT_DATETIME";
        public static DateTimeOffset ClientDatetime
        {
            get
            {
                return HttpContext.Current.Items[CLIENT_DATETIME] == null ? default(DateTimeOffset) : (DateTimeOffset)HttpContext.Current.Items[CLIENT_DATETIME];
            }
            set
            {
                if (!HttpContext.Current.Items.Contains(CLIENT_DATETIME))
                    HttpContext.Current.Items.Add(CLIENT_DATETIME, value);
                else
                    HttpContext.Current.Items[CLIENT_DATETIME] = value;
            }
        }

    }
}
