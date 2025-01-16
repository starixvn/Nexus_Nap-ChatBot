namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using Data;
    using Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Shared.Extensions;

    public class GetMissingMessageInfoCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public List<CurrentMaxMessageInfo> Messages { get; set; } = new List<CurrentMaxMessageInfo>();

        [SpParam(Ignored = true)]
        public List<MessageTypeInfo> MsgContentTypes { get; set; } = new List<MessageTypeInfo>();

        [SpParam(Ignored = true)]
        public List<MessageRequestId> Requests { get; set; } = new List<MessageRequestId>();

        [SpParam(Ignored = true)]
        public List<int> MessageIds { get; set; } = new List<int>();

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                return XmlHelper<CurrentMaxMessageInfo>.Serialize(Messages);
            }
        }

        [SpParam(SqlParamName = "MsgContentsTypeList")]
        public string MsgContentsTypeListXml
        {
            get
            {
                return XmlHelper<MessageTypeInfo>.Serialize(MsgContentTypes);
            }
        }

        [SpParam(SqlParamName = "VoidingCollectionRequestList")]
        public string VoidingCollectionRequestListXml
        {
            get
            {
                return XmlHelper<MessageRequestId>.Serialize(Requests);
            }
        }

        [SpParam(SqlParamName = "VoidingHistoryRequestList")]
        public string VoidingHistoryRequestListXml
        {
            get
            {
                return XmlHelper<int>.Serialize(MessageIds);
            }
        }

        public string SiteAsJson { get; set; }
        public string DepCustSAsJson { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMesssageInfo_Sel";

        public override bool IsValid() => true;
    }
}