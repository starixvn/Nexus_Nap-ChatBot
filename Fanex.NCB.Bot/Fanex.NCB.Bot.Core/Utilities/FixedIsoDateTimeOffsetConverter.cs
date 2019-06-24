using Newtonsoft.Json.Converters;
using System;

namespace Fanex.NCB.Bot.Core.Utilities
{
    public class FixedIsoDateTimeOffsetConverter : IsoDateTimeConverter
    {
        public FixedIsoDateTimeOffsetConverter()
        {
            this.DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
        }
    }
}