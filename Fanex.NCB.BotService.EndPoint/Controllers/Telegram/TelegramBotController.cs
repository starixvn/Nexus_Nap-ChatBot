namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    public class TelegramBotController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;

        public TelegramBotController(
            IOptions<AppSettings> appSettings,
            IOptions<List<TelegramBotConfigurations>> botConfigurations)
        {
            this.appSettings = appSettings.Value;
            this.botConfigurations = botConfigurations.Value;
        }

        [AllowAnonymous]
        [Route("api/telegram/bot/setWebhook")]
        public async Task<IActionResult> SetWebhook()
        {
            try
            {
                foreach (var bot in botConfigurations)
                {
                    var webhook = $"{appSettings.Domain}/api/telegram/webhook/{bot.BotId}/update";
                    await bot.BotClient.DeleteWebhookAsync();
                    await bot.BotClient.SetWebhookAsync(webhook);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok("Successfully!");
        }

        [AllowAnonymous]
        [Route("api/telegram/bot/healthcheck")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                foreach (var bot in botConfigurations)
                {
                    await bot.BotClient.GetMeAsync();
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

            return Ok("Bots are healthy!");
        }
    }
}