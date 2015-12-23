using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace EFunTech.Sms.Core
{
    public static class Converter
    {
        public const string yyyyMMddHHmm = "yyyyMMddHHmm";
        public const string Every8d_SentTime = "yyyy/MM/dd HH:mm:ss";
        // 2010/03/23 12:05:29

        public static DateTime? ToDateTime(string str, string format)
        {
            // string str = "201512312359";
            DateTime dt;
            if (DateTime.TryParseExact(str, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
            else
            {
                return null;
            }
        }

        public static string DebugString(DateTime time, string format = Every8d_SentTime)
        {
            return string.Format("({0}) {1}", time.Kind.ToString(), time.ToString(format));
        }

        private static TimeZoneInfo GetTimeZoneInfo(TimeSpan timezoneOffset)
        {
            return TimeZoneInfo.GetSystemTimeZones().Where(p => 
                p.BaseUtcOffset == timezoneOffset && 
                p.SupportsDaylightSavingTime == false).FirstOrDefault(); // 必須排除日光節約時間，否則誤差會有一個小時
        }

        public static DateTime ToLocalTime(DateTime utcTime, TimeSpan timezoneOffset)
        {
            TimeZoneInfo timeZoneInfo = GetTimeZoneInfo(timezoneOffset);

            var result = TimeZoneInfo.ConvertTime(utcTime, timeZoneInfo);

            result = DateTime.SpecifyKind(result, DateTimeKind.Local);

            return result;
        }

        public static string ToLocalTimeString(DateTime? utcTime, TimeSpan timezoneOffset, string format)
        {
            return utcTime.HasValue
                ? ToLocalTimeString(utcTime.Value, timezoneOffset, format)
                : string.Empty;
        }

        public static string ToLocalTimeString(DateTime utcTime, TimeSpan timezoneOffset, string format)
        {
            var result = ToLocalTime(utcTime, timezoneOffset);

            if (result == DateTime.MinValue) return string.Empty;

            return result.ToString(format);
        }

        public static List<DayOfWeek> ToLocalDayOfWeeks(DateTime utcTime, List<DayOfWeek> utcDayOfWeeks, TimeSpan timezoneOffset)
        {
            List<DateTime> matchs = new List<DateTime>();

            DateTime dt = utcTime;

            for (int i = 0; i < 7; i++)
            {
                if (utcDayOfWeeks.Contains(dt.DayOfWeek))
                {
                    matchs.Add(Converter.ToLocalTime(dt, timezoneOffset));
                }
                
                dt = dt.AddDays(1);
            }

            return matchs.Select(p => p.DayOfWeek).OrderBy(p => p).ToList();
        }

        public static DateTime ToUniversalTime(DateTime localTime, TimeSpan timezoneOffset)
        {
            TimeZoneInfo timeZoneInfo = GetTimeZoneInfo(timezoneOffset);

            var result = TimeZoneInfo.ConvertTime(localTime, timeZoneInfo).ToUniversalTime();

            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);

            return result;
        }

        public static DateTime? ToUniversalTime(string strLocalTime, string format, TimeSpan timezoneOffset)
        {
            DateTime? localTime = ToDateTime(strLocalTime, format);

            if(localTime.HasValue)
            {
                return ToUniversalTime(localTime.Value, timezoneOffset);
            }
            else{
                return null;
            }
        }

        public static DateTime GetDateBegin(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        public static DateTime GetDateEnd(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }

        /// <summary>
        /// SendTime to String
        /// </summary>
        public static string ToString(DateTime? dt, string format)
        {
            if (!dt.HasValue)
                return string.Empty;
            else
                return dt.Value.ToString(format);
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> query)
        {
            DataTable tbl = new DataTable();

            // 不論有沒有資料，都要建立 Table Columns
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                Type colType = pi.PropertyType;
                //針對Nullable<>特別處理
                if (colType.IsGenericType
                    && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    colType = colType.GetGenericArguments()[0];
                //建立欄位
                tbl.Columns.Add(pi.Name, colType);
            }

            foreach (T item in query)
            {
                DataRow row = tbl.NewRow();
                foreach (PropertyInfo pi in props)
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            return tbl;
        }
    }
}
