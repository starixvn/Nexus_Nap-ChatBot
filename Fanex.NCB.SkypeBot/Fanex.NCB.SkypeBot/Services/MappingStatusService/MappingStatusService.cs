using Fanex.NCB.Common.Constant;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NCB.SkypeBot.Services.MappingStatusService
{
    public class MappingStatusService : IMappingStatusService
    {
        public int CalculateMappingStatus(bool isConversationMapped, BotConversationMember botConversationMember)
        {
            var isBotActive = botConversationMember != null && botConversationMember.IsActive;
            if (!isBotActive)
            {
                return (int)MappingStatusData.MappingStatus.BotRemoved;
            }

            return isConversationMapped ? (int)MappingStatusData.MappingStatus.Mapped : (int)MappingStatusData.MappingStatus.NotMapped;
        }

        public List<MappingStatus> GetMappingStatus()
        {
            var data = new List<MappingStatus>
            {
                new MappingStatus
                {
                    Id = (int)MappingStatusData.MappingStatus.NotMapped,
                    Name =  MappingStatusData.MappingStatusNames[(int)MappingStatusData.MappingStatus.NotMapped]
                },
                new MappingStatus
                {
                    Id = (int)MappingStatusData.MappingStatus.Mapped,
                    Name =  MappingStatusData.MappingStatusNames[(int)MappingStatusData.MappingStatus.Mapped]
                },
                new MappingStatus
                {
                    Id = (int)MappingStatusData.MappingStatus.BotRemoved,
                    Name =  MappingStatusData.MappingStatusNames[(int)MappingStatusData.MappingStatus.BotRemoved]
                }
            };

            return data;
        }

        public string GetMappingStatusName(int mappingStatusID)
        {
            return GetMappingStatus().First(x => x.Id == mappingStatusID).Name;
        }
    }
}