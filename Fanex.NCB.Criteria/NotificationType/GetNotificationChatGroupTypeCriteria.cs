using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Criteria.NotificationType
{
    public class GetNotificationChatGroupTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_ChatGroupType_Sel";

        public override bool IsValid() => true;
    }
}
