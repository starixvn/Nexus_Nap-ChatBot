using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public Task<HttpStatusCode> ReplyAsync(IMessageActivity activity, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> SendAsyn(string message, string conversationId)
        {
            //get token
            //send message to skype with azure service
            throw new NotImplementedException();
        }

        public Task<HttpStatusCode> SendASync(string message, string conversationId)
        {
            throw new NotImplementedException();
        }

        private string GetToken()
        {
            return string.Empty;
        }
    }
}
