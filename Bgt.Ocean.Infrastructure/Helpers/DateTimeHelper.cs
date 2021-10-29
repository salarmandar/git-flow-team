using Bgt.Ocean.Infrastructure.Storages;
using System;
using System.Globalization;
using System.Linq;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class DateTimeHelper
    {
        public static string GetTime(this DateTime src, string format = "HH:mm")
            => src.ToString(format);

        /// <summary>
        /// QA pass
        /// </summary>
        public static DateTimeOffset UtcNowOffsetLocal
        {
            get
            {
                return ApiSession.ClientDatetime;
            }
        }        

        public static string ToDateFormat(this DateTimeOffset? date, string formatDate = "")
        {
            if (!date.HasValue)
                return "";
            if (string.IsNullOrEmpty(formatDate))
                return date.Value.ToString("yyyy/MM/dd");
            return date.Value.ToString(formatDate);
        }

        public static string ToDateFormat(this DateTimeOffset date, string formatDate = "")
        {
            if (string.IsNullOrEmpty(formatDate))
                return date.ToString("yyyy/MM/dd");
            return date.ToString(formatDate);
        }

        /// <summary>
        /// ** User Format dd-MM-yyyy
        /// ** DATE TIME to object date ex. 18/02/2020 08:20 => 18-02-2020 08:20
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ChangeFromDateTimeToUserFormatDateTime(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty : ChangeFromDateTimeToUserFormatDateTime(dateTime.GetValueOrDefault());
        }

        /// <summary>
        /// ** User Format dd-MM-yyyy
        /// ** DATE TIME to object date ex. 18/02/2020 08:20 => 18-02-2020 08:20
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ChangeFromDateTimeToUserFormatDateTime(this DateTime dateTime)
        {
            return dateTime.ChangeFromDateToString($"{ApiSession.UserFormatDate} HH:mm");
        }
        /// <summary>
        ///  ** DATE ONLY to object date ex. 18/02/2020 => 18/02/2020 00:00
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ChangeFromStrDateToDateTime(this string date)
        {
            return date.ChangeFromStringToDate(ApiSession.UserFormatDate).GetValueOrDefault();
        }


        public static DateTime ChangeStrToDateTime(this string date, string fixedFormatDate)
        {
            DateTime dt = default(DateTime);

            Action SetDefaultDateTime = () =>
            {
                DateTimeOffset now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                var offset = TimeSpan.FromMinutes(ApiSession.UtcOffset);
                dt = now.ToOffset(offset).DateTime;
            };

            var isValid = !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(fixedFormatDate);
            if (isValid)
            {
                var isNotPassed = !DateTime.TryParseExact(date, fixedFormatDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                if (isNotPassed)
                {
                    SetDefaultDateTime();
                }
            }
            else
            {
                SetDefaultDateTime();
            }
            return dt;
        }

        /// <summary>
        /// condition:
        /// <para />if date string is clientDateTime, that date must have time and isClientDateTime is true.
        /// <para />other date string (workDate, etc.) that date must have only date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="isClientDateTime"></param>
        /// <returns></returns>
        public static DateTime? ChangeFromStringToDate(this string date, string userFormatDate, bool isClientDateTime = false)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            DateTime dt;
            if (isClientDateTime)
            {
                // system date format is MM/dd/yyyy HH:mm:ss
                dt = DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                return dt;
            }

            if (string.IsNullOrEmpty(userFormatDate))
            {
                if (!DateTime.TryParseExact(date, AppSetting.AppFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return null;
                }
            }
            else
            {
                if (!DateTime.TryParseExact(date, AppSetting.AppFormat.FirstOrDefault(e => e == userFormatDate),
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return null;
                }
            }

            return dt;
        }

        public static DateTime? ChangeFromDateTimeOffsetToDate(this DateTimeOffset? offset)
        {
            //if null
            DateTime? d = offset.HasValue ? offset.Value.DateTime : DateTime.MaxValue;
            if (d == DateTime.MaxValue)
            {
                d = null;
            }
            return d;
        }

        public static DateTime? ChangeFromJsonStringToClientDate(this string dateTimeOffset)
        {
            DateTimeOffset dateTime;
            if (DateTimeOffset.TryParse(dateTimeOffset, out dateTime))
            {
                dateTime = dateTime.AddMinutes(ApiSession.UtcOffset);
                return dateTime.Date;
            }
            return null;
        }

        public static string ChangeDateTimeForDolphin(this DateTime? datetime)
        {
            return datetime != null ? datetime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        public static string ChangeFromDateToString(this DateTime? date, string format = "")
        {
            format = string.IsNullOrEmpty(format) ? "MM/dd/yyyy" : format;

            string dateTimeStr = "";
            if (date != null)
                dateTimeStr = date.Value.ToString(format);

            return dateTimeStr;
        }

        public static string ChangeFromDateToString(this DateTime date, string format = null)
        {
            format = format ?? "MM/dd/yyyy";

            string dateTimeStr = "";
            if (date != DateTime.MinValue)
                dateTimeStr = date.ToString(format, CultureInfo.InvariantCulture);

            return dateTimeStr;
        }

        public static string ChangeFromDateToStringSQLFormat(this DateTime date) => ChangeFromDateToString(date, "yyyy-MM-dd HH:mm:ss");

        public static string ChangFromDateToTimeString(this DateTime? date)
        {
            return date == null ? "00:00" : date.Value.ToString("HH:mm");
        }

        public static string ChangeFromDateToTimeString(this DateTime date)
        {
            return date == default(DateTime) ? "00:00" : date.ToString("HH:mm");
        }

        public static string ChangeFromDateToTimeString(this DateTime? date)
        {
            return date == default(DateTime) || date == null ? "00:00" : date.GetValueOrDefault().ToString("HH:mm");
        }

        public static DateTime ToMinDateTime(this DateTime? value)
        {
            string time = value.GetValueOrDefault().ToString("HH:mm");
            time = time == "12:00" ? "00:00" : time;
            string[] times = time.Split(':');

            // Top Edit : In the Ocean Old verstion use 1900 not use 1990 if you have a new value please tell me
            DateTime datetime = new DateTime(1900, 1, 1, int.Parse(times[0]), int.Parse(times[1]), 00);

            return datetime;
        }

        /// <summary>
        /// change string time to object DateTime 
        /// </summary>
        /// <param name="strTime">"08:02"</param>
        /// <returns>null,object DateTime</returns>
        public static DateTime? ChangeFromTimeToDateTime(this string strTime)
        {
            string time = string.IsNullOrWhiteSpace(strTime) ? "00:00" : strTime;
            DateTime? newDate = null;

            newDate = DateTime.ParseExact("1900-01-01 " + time, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            return newDate;
        }



        /// <summary>
        /// ** TIME ONLY to object date ex. 08:20 => 1900-01-01 08:20
        /// </summary>
        /// <param name="strTime">"08:02"</param>
        /// <returns>null,object DateTime</returns>
        public static DateTime ChangeFromStrTimeToDateTime(this string strTime) => ChangeFromTimeToDateTime(strTime).GetValueOrDefault();

        /// <summary>
        /// Combine Date and Time
        /// Example  srcTime 10/10/2017 10:00 ,srcDate 02/11/2017 00:00
        /// Result 02/11/2017 10:00
        /// </summary>
        /// <param name="srcTime">Source Time</param>
        /// <param name="srcDate">Source Date<</param>
        /// <returns></returns>
        public static DateTime FromDateCombineWithTime(DateTime srcDate, DateTime srcTime)
        {
            int d = srcDate.Day,
                m = srcDate.Month,
                y = srcDate.Year,
                H = srcTime.Hour,
                M = srcTime.Minute,
                Ms = srcTime.Millisecond;

            return new DateTime(y, m, d, H, M, Ms);
        }

        public static DateTime FromDateCombineWithTime(DateTime? srcDate, DateTime? srcTime) => FromDateCombineWithTime(srcDate.GetValueOrDefault(), srcTime.GetValueOrDefault());
        public static DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }
        public static DateTime LastDayOfWeek(DateTime date)
        {
            DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }
    }
}
