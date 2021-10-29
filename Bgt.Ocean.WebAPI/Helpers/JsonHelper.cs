using Bgt.Ocean.Infrastructure.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bgt.Ocean.WebAPI.Helpers
{
    public static class JsonHelper
    {
        public const string _dateFormat = "yyyy/MM/dd HH:mm:ss";

        public static string GetJsonString(this object obj)
        {
            if (obj != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                var output = JsonConvert.SerializeObject(obj, isoConvert);
                return output;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert date to JSDateformat (eg. Date(142303..))
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJsonStringJSDateFormat(this object obj)
        {
            if (obj != null)
            {
                var defaultConverter = new JavaScriptDateTimeConverter();

                var output = JsonConvert.SerializeObject(obj, defaultConverter);
                return output;
            }

            return null;
        }

        public static TObject GetObjectFromJsonString<TObject>(string jsonString)
        {
            TObject t;
            try
            {
                t = JsonConvert.DeserializeObject<TObject>(jsonString);
            }
            catch { t = default(TObject); }
            return t;
        }
        public static string GetJSONStringByArray<T>(params T[] collection)
            => collection.ToJSONString();
    }
}
