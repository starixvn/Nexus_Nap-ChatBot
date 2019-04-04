using Newtonsoft.Json;

namespace Fanex.NCB.Bot.SkypeClient
{
    public class Token
    {
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }
}
