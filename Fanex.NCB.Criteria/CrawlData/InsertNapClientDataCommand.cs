namespace Fanex.NCB.Criteria.CrawlData
{
    using Fanex.NCB.Shared.Models;

    public class InsertNapClientDataCommand : BaseCommand
    {
        public string Data { get; set; }

        public override string GetSettingKey() => "NCB_NapClientCrawlData_Ins";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Data);
    }
}