using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface ISettingStorage
    {
        Task<List<RecipientType>> GetRecipientTypesAsync();
        Task<List<Recipient>> GetRecipientsAsync();
        Task<List<BotName>> GetBotNameAsync();
        Task<List<ChatGroupType>> GetChatGroupTypeAsync();
    }
}
