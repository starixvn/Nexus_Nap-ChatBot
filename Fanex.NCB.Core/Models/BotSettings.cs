namespace Fanex.NCB.Core.Models;

using System.Collections.Generic;

public class BotSettings
{
    public string BotDomain { get; set; }
    public string TokenUrl { get; set; }
    public string AuthenticateUrl { get; set; }
    public string AppCredentialId { get; set; }
    public string AppCredentialPassword { get; set; }
    public IList<BotServiceEndPointConfigs> ApiConfigs { get; set; } = new List<BotServiceEndPointConfigs>();
}

public class BotServiceEndPointConfigs
{
    public byte ChannelId { get; set; }
    public string SendMessageUrl { get; set; }
    public string SendMultiMessageUrl { get; set; }
}