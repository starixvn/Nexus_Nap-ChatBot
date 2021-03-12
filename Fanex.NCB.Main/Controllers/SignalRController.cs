namespace Fanex.NCB.Main.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SignalRController : ControllerBase
    {
        private readonly IOptionsSnapshot<AppSettings> appSettings;
        private readonly ISignalRService signalRService;
        private readonly INotificationService notificationService;
        private readonly string invalidUsernameOrPasswordText = "Username or password is incorrect";

        public SignalRController(IOptionsSnapshot<AppSettings> appSettings, ISignalRService signalRService, INotificationService notificationService)
        {
            this.appSettings = appSettings;
            this.signalRService = signalRService;
            this.notificationService = notificationService;
        }

        [HttpPost]
        [Route("api/signalr/auth")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public ObjectResult Authenticate(string username, string password)
        {
            if (IsValidCredential(username, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Value.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(sercurityToken);

                return Ok(new { AccessToken = token });
            }
            else
            {
                return BadRequest(invalidUsernameOrPasswordText);
            }
        }

        private bool IsValidCredential(string username, string password)
        {
            return !string.IsNullOrWhiteSpace(username)
                && !string.IsNullOrWhiteSpace(password)
                && username.Equals(appSettings.Value.AppCredentialId, StringComparison.InvariantCultureIgnoreCase)
                && password.Equals(appSettings.Value.AppCredentialPassword, StringComparison.InvariantCultureIgnoreCase);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("api/signalr/NewNotifications")]
        public async Task<ObjectResult> PushSignalOnCreateNotifications(string notificationIds)
        {
            await signalRService.InsertNewNotification(notificationIds);

            return Ok("Ok");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("api/signalr/UpdateNoti")]
        public async Task<ObjectResult> PushSignalUpdateNotificationStatus(string notificationIds)
        {
            await signalRService.UpdateNotificationStatus(notificationIds);

            return Ok("Ok");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("api/signalr/errNoti")]
        public async Task<ObjectResult> HasNewErrNotification()
        {
            var notifications = await notificationService.GetErrNotificationsAsync();
            await signalRService.HasNewErrNotifications(notifications);

            return Ok("Ok");
        }
    }
}