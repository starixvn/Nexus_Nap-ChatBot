namespace Fanex.NCB.AutoJobs.Core.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetServiceSettingCriteria : CriteriaBase
    {
        public string InServiceName { get; set; }

        public override string GetSettingKey() => "NCB_ChatBot_Service_SettingSel";

        public override bool IsValid() => true;
    }
}
