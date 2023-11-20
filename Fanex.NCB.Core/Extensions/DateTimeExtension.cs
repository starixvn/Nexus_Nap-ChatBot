namespace Fanex.NCB.Core.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    public static class DateTimeExtension
    {
        public static DateTime ConvertToUTC(this DateTime dateTime)
        {
            TimeZoneInfo info = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");

            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), info);
        }

        public static DateTime ConvertFromUTC(this DateTime dateTime)
        {
            TimeZoneInfo info = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), info);
        }

        public static string ConvertToDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}