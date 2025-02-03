namespace Fanex.NCB.AutoJobs.Core.Helpers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[ExcludeFromCodeCoverage(Justification = "Helpers")]
public static class DateTimeExtension
{
    public static DateTime ConvertToUTC(this DateTime dateTime)
    {
        var info = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
            : TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");

        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), info);
    }

    public static DateTime ConvertFromUTC(this DateTime dateTime)
    {
        var info = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
            : TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), info);
    }

    public static string ConvertToDateTimeString(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}