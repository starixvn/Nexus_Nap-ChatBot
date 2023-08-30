namespace Fanex.NCB.Services
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.TicketInfo;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class RefNoTicketService : IRefNoTicketService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogService logService;

        public RefNoTicketService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogService logService)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.logService = logService;
        }

        public async Task<TicketInfo> GetTicketByRefNoAsync(long refno)
        {
            var napUrl = configuration.GetSection("NapSettings:Url").Value;
            var apiKey = configuration.GetSection("NapSettings:APIKey").Value;

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = GenerateHeaderAuthorization(apiKey);

            var url = new Uri(napUrl + "BetListApi/GetBetListByRefNo?refno=" + refno);
            using var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return !string.IsNullOrEmpty(jsonString) ? JsonConvert.DeserializeObject<TicketInfo>(jsonString) : await Task.FromResult<TicketInfo>(null);
            }
            return default;
        }

        private static AuthenticationHeaderValue GenerateHeaderAuthorization(string apiKey)
        {
            string nonce = Guid.NewGuid().ToString("N");
            string signatureRawData = apiKey + ":" + nonce;
            using HMACSHA256 hmac = new(Convert.FromBase64String(apiKey));
            string requestSignatureBase64String = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureRawData)));
            return new AuthenticationHeaderValue("amx", string.Format("{0}:{1}", requestSignatureBase64String, nonce));
        }

        public async Task<(long, string)> GetTransInfoAsync(long refno)
        {
            var tickInfo = await GetTicketByRefNoAsync(refno);
            if (tickInfo is null)
            {
                return (refno, string.Empty);
            }
            await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(tickInfo));
            string content = GenerateTransInfoContent(tickInfo);
            return (refno, content);
        }

        private static string GenerateTransInfoContent(TicketInfo tickInfo)
        {
            var content = new StringBuilder();
            if (tickInfo is not null && tickInfo.ErrorCode == "0")
            {
                content.Append(GenerateTickInfoContent(tickInfo.Description));
                if (!string.IsNullOrEmpty(tickInfo.Odds))
                {
                    content.Append(Environment.NewLine);
                    content.Append($"Odds: {GenerateOdds(tickInfo.Odds)}");
                }
            }
            return content.ToString();
        }

        private static string GenerateOdds(string oddHtml)
        {
            string spaceStringReplaced = "ODDSPACE";
            string contentSpace = oddHtml.Replace(" ", spaceStringReplaced)
                .Replace(Environment.NewLine, spaceStringReplaced)
                .Replace("<br>", spaceStringReplaced);
            var rawText = TextHelper.HtmlToPlainText(contentSpace);
            string result = rawText.Replace(spaceStringReplaced, " ");
            return result;
        }

        private static string GenerateTickInfoContent(string html)
        {
            string removedRefNoAndTimeHtml = html.Split("</br>").LastOrDefault();
            string content = removedRefNoAndTimeHtml.Replace("</br>", "")
                                    .Replace("> <", "><")
                                    .Replace("<div", "<br><div")
                                    .Replace("class=\"bettype\"> ", "class=\"bettype\">");

            return TextHelper.HtmlToPlainText(content);
        }
    }
}