namespace Fanex.NCB.AutoJobs.Core.Helpers;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[ExcludeFromCodeCoverage(Justification = "Helpers")]
public static class EnumHelper
{
    public static string GetDescription(Enum enumType)
    {
        var description = string.Empty;

        if (enumType != null)
        {
            var fi = enumType.GetType().GetField(enumType.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            description = attributes.Length > 0 ? attributes[0].Description : enumType.ToString();
        }

        return description;
    }
}