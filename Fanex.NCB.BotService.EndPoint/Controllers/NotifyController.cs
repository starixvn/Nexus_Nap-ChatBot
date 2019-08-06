namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly BotConfiguration botConfig;
        private readonly IConversationService conversationService;

        public NotifyController(IBotFrameworkHttpAdapter adapter, BotConfiguration botConfig, IConversationService conversationService)
        {
            this.adapter = adapter;
            this.botConfig = botConfig;
            this.conversationService = conversationService;
        }

        [Route("api/notify")]
        public async Task<ObjectResult> Send(string botName, string conversationId, string message)
        {
            var appId = GetAppId(botName);
            var conversationReference = await GetConversationReference(conversationId, botName);

            var result = await SendMessage(appId, conversationReference, message);  
            return Ok(new List<ProactiveMessageError> { result });
        }

        [Route("api/notifymulti")]
        public async Task<ObjectResult> SendMulti([FromBody]IList<ProactiveMessage> proactiveMessages)
        {
            string appId;
            ProactiveMessageError result;
            var results = new List<ProactiveMessageError>();
            ConversationReference conversationReference;

            foreach (var proactiveMessage in proactiveMessages)
            {
                conversationReference = GetConversationReference(proactiveMessage);
                appId = GetAppId(proactiveMessage.BotName);
                result = await SendMessage(appId, conversationReference, proactiveMessage.Message);
                results.Add(result);
            }

            return Ok(results);
        }

        private async Task<ProactiveMessageError> SendMessage(string appId, ConversationReference conversationReference, string message)
        {
            var result = new ProactiveMessageError
            {
                ConversationId = conversationReference.Conversation.Id
            };

            try
            {
                await ((BotFrameworkAdapter)adapter).ContinueConversationAsync(appId, conversationReference,
                    async (context, token) =>
                    {
                        MicrosoftAppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
                        await context.SendActivityAsync(message);
                    }, 
                    new CancellationToken());
            }
            catch (Exception ex)
            {
                result.ErrorCode = 1;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<ConversationReference> GetConversationReference(string conversationId, string botName)
        {
            var conversationGroup = await conversationService.GetGroupConversation(conversationId, botName);
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = conversationGroup.BotConversationId, Name = conversationGroup.BotName, Role = "bot" },
                Conversation = new ConversationAccount() { Id = conversationId },
                ServiceUrl = conversationGroup.ServiceUrl,
                ChannelId = conversationGroup.ChannelId
            };
            
            return coversation;
        }

        private ConversationReference GetConversationReference(ProactiveMessage proactiveMessage)
        {
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = proactiveMessage.BotConversationId, Name = proactiveMessage.BotName, Role = "bot" },
                Conversation = new ConversationAccount() { Id = proactiveMessage.ConversationId },
                ServiceUrl = proactiveMessage.ServiceUrl,
                ChannelId = proactiveMessage.ChannelId
            };

            return coversation;
        }

        private string GetAppId(string botName)
        {
            var appId = ((EndpointService)botConfig.Services.FirstOrDefault(t => t.Name.Equals(botName, StringComparison.InvariantCultureIgnoreCase))).AppId;
            if (string.IsNullOrEmpty(appId))
            {
                appId = Guid.NewGuid().ToString();
            }

            return appId;
        }
    }
}
