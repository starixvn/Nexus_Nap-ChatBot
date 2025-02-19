namespace Fanex.NCB.Criteria.Common;

using Data;
using Data.Repository;

public class GetConversationChannelCriteria : CriteriaBase
{
    [SpParam(AllowNull = true)] public byte? Id { get; set; }

    [SpParam(AllowNull = true)] public string Name { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_Channel_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}