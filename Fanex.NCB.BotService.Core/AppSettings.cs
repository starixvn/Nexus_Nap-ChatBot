using System.Collections.Generic;
using System.Linq;

namespace Fanex.NCB.BotService.Core;

public class AppSettings
{
    public string Domain { get; set; }
    public string AppCredentialId { get; set; }

    public string AppCredentialPassword { get; set; }

    public string Secret { get; set; }

    public string AllowedHosts { get; set; }

    public string BotFilePath { get; set; }

    public string BotFileSecret { get; set; }

    public string TokenUrl { get; set; }

    public string SkypeServiceUrl { get; set; }

    public string HealthCheckUrl { get; set; }
    public string EnableChannelIds { get; set; }

    public IEnumerable<byte> EnableChannels => !string.IsNullOrWhiteSpace(EnableChannelIds)
        ? EnableChannelIds.Split(',').Select(x => byte.Parse(x))
        : new List<byte>();
}