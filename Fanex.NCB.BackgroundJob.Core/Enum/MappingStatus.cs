namespace Fanex.NCB.AutoJobs.Core.Enum
{
    using System.ComponentModel;

    public enum MappingStatus
    {
        [Description("Bot Removed")]
        BotRemoved = 0,

        [Description("Not Mapped")]
        NotMapped = 1,

        [Description("Mapped")]
        Mapped = 2
    }
}