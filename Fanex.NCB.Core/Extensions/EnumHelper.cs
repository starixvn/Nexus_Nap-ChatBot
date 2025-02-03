namespace Fanex.NCB.Core.Extensions;

using System;
using System.ComponentModel;
using System.Reflection;

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