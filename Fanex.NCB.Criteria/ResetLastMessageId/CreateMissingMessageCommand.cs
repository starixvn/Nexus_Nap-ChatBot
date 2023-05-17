namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;

    public class CreateMissingMessageCommand : BaseCommand
    {
        [SpParam(Ignored = true)]
        public List<MissingMesssageInfo> Messages { get; set; } = new List<MissingMesssageInfo>();

        public string SiteAsJson { get; set; }
        public string DepCustSAsJson { get; set; }
        public string Environment { get; set; }

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (MissingMesssageInfo message in Messages)
                {
                    xml.AppendFormat("<r JobIdentify='{0}' MessageId='{1}'/>",
                        message.JobIdentify,
                        message.MessageId);
                }
                xml.Append("</root>");
                return xml.ToString();
            }
        }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMesssage_Ins";

        public override bool IsValid() => true;
    }
}