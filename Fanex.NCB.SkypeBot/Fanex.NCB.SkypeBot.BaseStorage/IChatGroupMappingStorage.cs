using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface IChatGroupMappingStorage
    {
        Task<List<BotName>> GetBotNameAsync();
    }
}
