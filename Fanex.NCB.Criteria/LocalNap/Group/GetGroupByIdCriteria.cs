namespace Fanex.NCB.Criteria.LocalNap.Group
{
    using Fanex.Data.Repository;

    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override string GetSettingKey() => "NAP_Group_GetById";

        public override bool IsValid() => GroupId > 0;
    }
}