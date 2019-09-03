
namespace Fanex.NCB.Criteria.LocalNap.Security
{
    using Fanex.Data.Repository;

    public class UserReCheckInUserCriteria : CriteriaBase
    {
        public int OnUserId { get; set; }

        public bool IsKickedOut { get; set; }

        public override bool IsValid() => OnUserId > 0;

        public override string GetSettingKey() => "NAP_User_ReCheckIn";
    }
}
