namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Fanex.NCB.BotService.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    public class AuthController : ControllerBase
    {
        private readonly AppSettings appSettings;
        private readonly BotConfiguration botConfig;
        private readonly string invalidUsernameOrPasswordText = "Username or password is incorrect";

        public AuthController(IOptions<AppSettings> appSettings, BotConfiguration botConfig)
        {
            this.appSettings = appSettings.Value;
            this.botConfig = botConfig;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/authenticate")]
        public ObjectResult Authenticate(string username, string password)
        {
            var token = string.Empty;
            var botId = GetValidatedCredential(username, password);
            if (!string.IsNullOrWhiteSpace(botId))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, botId)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                token = tokenHandler.WriteToken(sercurityToken);

                return Ok(new { AccessToken = token });
            }
            else
            {
                return BadRequest(new { message = invalidUsernameOrPasswordText });
            }
        }

        private string GetValidatedCredential(string username, string password)
        {
            var botId = string.Empty;
            var botSetting = botConfig.Services.FirstOrDefault(t => ((EndpointService)t).AppId.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                && ((EndpointService)t).AppPassword.Equals(password, StringComparison.InvariantCultureIgnoreCase)
            );

            if (botSetting != null)
            {
                botId = botSetting.Id;
            }

            return botId;
        }
    }
}