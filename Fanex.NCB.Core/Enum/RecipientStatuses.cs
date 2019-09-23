namespace Fanex.NCB.Core.Enum
{
    using System.ComponentModel;

    public enum RecipientStatuses
    {
        [Description("Ready to send message")]
        ReadyToSend = 0,

        [Description("Recipient is not mapped")]
        RecipientNotMapped = 1,

        [Description("Chatbot has been removed from the group")]
        ChatbotRemoved = 2,

        [Description("Recipient is changed group")]
        RecipientChangedGroup = 3,

        [Description("Error occured on chatbot service.")]
        BotServiceError = 4,

        [Description("Group type is changed.")]
        GroupTypeChanged = 5
    }
}