using System;
using Newtonsoft.Json.Converters;

namespace Fanex.NCB.Bot.Skype.Models
{
    public class FixedIsoDateTimeOffsetConverter : IsoDateTimeConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
        }

        public FixedIsoDateTimeOffsetConverter() : base()
        {
            this.DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal;
        }
    }
}