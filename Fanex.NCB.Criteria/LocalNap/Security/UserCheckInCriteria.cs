namespace Fanex.NCB.Criteria.LocalNap.Security
{
    using System;
    using Fanex.Data.Repository;

    public class UserCheckInCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public string NetSessionId { get; set; }

        public string Ip { get; set; }

        public DateTime LoginDate { get; set; }

        public int OnUserId { get; set; }

        public override string GetSettingKey() => "NAP_User_CheckIn";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}