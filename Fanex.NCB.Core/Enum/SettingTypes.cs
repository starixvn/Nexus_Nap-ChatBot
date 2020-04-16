namespace Fanex.NCB.Core.Enum
{
    using System.ComponentModel;

    public enum SettingTypes
    {
        [Description("Voiding Ticket for Licensee")]
        VoidingTicket = 1,

        [Description("Abnormal Bet")]
        AbnormalBet = 2,

        [Description("Site Message")]
        SiteMessage = 3
    }
}