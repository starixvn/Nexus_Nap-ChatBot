using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Fanex.NCB.Bot.SkypeClient.UnitTests")]
namespace Fanex.NCB.Bot.SkypeClient
{
    public class BotClient : IBotClient
    {
        private readonly string _tokenUrl = "https://login.microsoftonline.com/botframework.com/oauth2/v2.0/token";
        private readonly string _clientId;
        private readonly string _password;
        public BotClient(string clientId, string password)
        {
            _clientId = clientId;
            _password = password;
        }
        public BotClient(string clientId, string password, string botTokenUrl, string serviceUrl)
        {
            _clientId = clientId;
            _password = password;
            if (!string.IsNullOrEmpty(botTokenUrl))
            {
                _tokenUrl = botTokenUrl;
            }
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                this.ServiceUrl = serviceUrl;
            }
        }
        public string ServiceUrl { get; set; } = "https://smba.trafficmanager.net/apis/";
        public async Task<HttpStatusCode> SendAsync(string message, string conversationId)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(conversationId))
            {
                return HttpStatusCode.NoContent;
            }

            var messageInfo = CreateMessageActivity(message, conversationId);

            return await SendAsync(messageInfo).ConfigureAwait(false);
        }

        private static IMessageActivity CreateMessageActivity(string message, string conversationId)
        {
            var item = Activity.CreateMessageActivity();
            item.Conversation = new ConversationAccount(id: conversationId);
            item.Text = message;
            return item;
        }

        internal virtual async Task<IConnectorClient> SetupClientConnection()
        {
            var account = new MicrosoftAppCredentials(_clientId, _password);
            var jwtToken = await account.GetTokenAsync(true).ConfigureAwait(false);
            return new ConnectorClient(new Uri(ServiceUrl), account, handlers: new BotDelegatingHandler(jwtToken));
        }

        internal virtual async Task<Token> GetToken()
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
                var response = await client.PostAsync(_tokenUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<Token>().ConfigureAwait(false);

            }
        }

        public Task<HttpStatusCode> ReplyAsync(IMessageActivity activity, string message)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<HttpStatusCode> SendAsync(IMessageActivity messageActivity)
        {
            var token = await GetToken().ConfigureAwait(false);
            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                return HttpStatusCode.Unauthorized;
            }
            var connector = await SetupClientConnection().ConfigureAwait(false);
            var result = await connector.Conversations.SendToConversationAsync((Activity)messageActivity).ConfigureAwait(false);
            return HttpStatusCode.OK;
        }

    }
}
