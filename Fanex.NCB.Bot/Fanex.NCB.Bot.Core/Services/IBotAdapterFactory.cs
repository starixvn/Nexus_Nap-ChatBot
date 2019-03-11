using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;

namespace Fanex.NCB.Bot.Core.Services
{
    public interface IBotAdapterFactory
    {
        IAdapterIntegration Create(BotConfiguration botConfig, string name);
    }
}