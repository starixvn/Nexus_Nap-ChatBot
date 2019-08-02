namespace Fanex.NCB.Core.Models
{
    using System.Collections.Generic;

    public class BotSettings
    {
        public string BotDomain { get; set; }

        public string BotServiceUrl { get; set; }

        public string TokenUrl { get; set; }

        public List<BotInfo> Bots { get; set; }
    }
}