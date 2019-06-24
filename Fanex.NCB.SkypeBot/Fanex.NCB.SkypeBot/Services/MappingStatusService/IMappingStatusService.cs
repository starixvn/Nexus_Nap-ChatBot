using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System.Collections.Generic;

namespace Fanex.NCB.SkypeBot.Services.MappingStatusService
{
    public interface IMappingStatusService
    {
        int CalculateMappingStatus(bool isConversationMapped, BotConversationMember botConversationMember);

        List<MappingStatus> GetMappingStatus();

        string GetMappingStatusName(int mappingStatusID);
    }
}