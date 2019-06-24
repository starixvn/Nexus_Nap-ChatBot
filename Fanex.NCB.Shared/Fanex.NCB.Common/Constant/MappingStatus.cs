using System.Collections.Generic;

namespace Fanex.NCB.Common.Constant
{
    public class MappingStatusData
    {
        public enum MappingStatus
        {
            BotRemoved = 0,
            NotMapped = 1,
            Mapped = 2
        }

        public static readonly IReadOnlyList<string> MappingStatusNames = new []
        {
            "Bot Removed",
            "Not Mapped",
            "Mapped"
        };
    } 
}