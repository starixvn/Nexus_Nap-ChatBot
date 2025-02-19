namespace Fanex.NCB.Criteria.ChatGroupMapping;

using Data;
using Data.Repository;

public class GetChatGroupMappingCriteria : CriteriaBase
{
    [SpParam(AllowNull = true)] public string ChannelIds { get; set; }

    [SpParam(AllowNull = true)] public string BotIds { get; set; }

    [SpParam(AllowNull = true)] public string RecipientTypeIDs { get; set; }

    [SpParam(AllowNull = true)] public string RecipientName { get; set; }

    [SpParam(AllowNull = true)] public string GroupName { get; set; }

    [SpParam(AllowNull = true)] public string MappingStatusIds { get; set; }

    [SpParam(AllowNull = true)] public string LicenseeStatusIds { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public long TotalItems { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ChatGroupMapping_Sel";
    }

    public override bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0;
    }
}