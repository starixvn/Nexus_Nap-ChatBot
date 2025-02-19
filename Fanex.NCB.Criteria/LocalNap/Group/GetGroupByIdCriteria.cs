namespace Fanex.NCB.Criteria.LocalNap.Group;

using Data.Repository;

public class GetGroupByIdCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_GetById";
    }

    public override bool IsValid()
    {
        return GroupId > 0;
    }
}