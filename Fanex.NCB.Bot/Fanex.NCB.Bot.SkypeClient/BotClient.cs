using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.SkypeClient
{
    public class BotClient : IBotClient
    {
        private readonly string _tokenUrl = "https://login.microsoftonline.com/botframework.com/oauth2/v2.0/token";
        private readonly string _serviceUrl = "https://smba.trafficmanager.net/apis/";
        private readonly string _clientId;
        private readonly string _password;
        public BotClient(string clientId, string password)
        {
            _clientId = clientId;
            _password = password;
        }
        public BotClient(string clientId, string password, string botTokenUrl, string skypeServiceUrl)
        {
            _clientId = clientId;
            _password = password;
            if (!string.IsNullOrEmpty(botTokenUrl))
            {
                _tokenUrl = botTokenUrl;
            }
            if (!string.IsNullOrEmpty(skypeServiceUrl))
            {
                _serviceUrl = skypeServiceUrl;
            }
        }

        public async Task<HttpStatusCode> SendAsync(string message, string conversationId)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(conversationId))
            {
                return HttpStatusCode.NoContent;
            }

            var messageInfo = CreateMessageActivity(message, conversationId);

            return await SendAsync(messageInfo);
        }

        private static IMessageActivity CreateMessageActivity(string message, string conversationId)
        {
            var item = Activity.CreateMessageActivity();
            item.Conversation = new ConversationAccount(id: conversationId);
            item.Text = message;
            return item;
        }

        private async Task<ConnectorClient> SetupClientConnection()
        {
            var account = new MicrosoftAppCredentials(_clientId, _password);
            var jwtToken = await account.GetTokenAsync(true);
            return new ConnectorClient(new Uri(_serviceUrl), account, handlers: new BotDelegatingHandler(jwtToken));
        }

        private async Task<Token> GetToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new
                    MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _password),
                    new KeyValuePair<string, string>("scope", $"{_clientId}/.default"),


                });
                var response = await client.PostAsync(_tokenUrl, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<Token>();
            }
        }

        public Task<HttpStatusCode> ReplyAsync(IMessageActivity activity, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> SendAsync(IMessageActivity messageActivity)
        {
            var token = await GetToken();
            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                return HttpStatusCode.Unauthorized;
            }
            var connector = await SetupClientConnection();
            var result = await connector.Conversations.SendToConversationAsync((Activity)messageActivity);
            return HttpStatusCode.OK;
        }
    }
}
