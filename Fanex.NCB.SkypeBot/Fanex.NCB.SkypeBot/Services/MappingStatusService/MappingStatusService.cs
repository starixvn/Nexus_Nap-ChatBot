using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NCB.SkypeBot.Services.MappingStatusService
{
    public class MappingStatusService : IMappingStatusService
    {
        public int CalculateMappingStatus(bool isConversationMapped, BotConversationMember botConversationMember)
        {
            var isBotActive = botConversationMember != null && botConversationMember.IsActive;
            if (!isBotActive)
            {
                return (int)Common.Constant.MappingStatus.BotRemoved;
            }

            return isConversationMapped ? (int)Common.Constant.MappingStatus.Mapped : (int)Common.Constant.MappingStatus.NotMapped;
        }

        public List<MappingStatus> GetMappingStatus()
        {
            var data = new List<MappingStatus> {                
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.NotMapped, Name = "Not Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.Mapped, Name = "Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.BotRemoved, Name = "Bot Removed" }
            };

            return data;
        }

        public string GetMappingStatusName(int mappingStatusID)
        {
            return GetMappingStatus().First(x => x.Id == mappingStatusID).Name;
        }
    }
}
