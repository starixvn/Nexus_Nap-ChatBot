namespace Fanex.NCB.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetUsersCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_User_Sel";

        public override bool IsValid() => true;
    }
}