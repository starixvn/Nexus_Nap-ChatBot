using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Fanex.NCB.Bot.Core.Controllers
{
    public abstract class BaseController : Controller
    {
        public static readonly JsonSerializer BotMessageSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ReadOnlyJsonContractResolver(),
            Converters = new List<JsonConverter> { new FixedIsoDateTimeOffsetConverter() },
        });

        public async Task PostAsync(string lb)
        {
            Validate(lb);
            var activity = default(Activity);

            using (var bodyReader = new JsonTextReader(new StreamReader(Request.Body, Encoding.UTF8)))
            {
                activity = BotMessageSerializer.Deserialize<Activity>(bodyReader);
            }

            var botFrameworkAdapter = CreateAdapter(lb);

            var invokeResponse = await botFrameworkAdapter.ProcessActivityAsync(
                Request.Headers["Authorization"],
                activity,
                OnTurnAsync,
                default(CancellationToken)).ConfigureAwait(false);

            if (invokeResponse == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.ContentType = "application/json";
                Response.StatusCode = invokeResponse.Status;

                using (var writer = new StreamWriter(Response.Body))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        BotMessageSerializer.Serialize(jsonWriter, invokeResponse.Body);
                    }
                }
            }
        }

        private static void Validate(string lb)
        {
            if (string.IsNullOrWhiteSpace(lb))
            {
                throw new ArgumentException("Invalid request!The bot name is null or empty");
            }
        }

        /// <summary>
        /// Override CreateAdapter to create your own custom adapter or set properties on a BotFrameworkAdapter instance.
        /// </summary>
        /// <returns>An adapter that will be used for the current request.</returns>
        protected virtual IAdapterIntegration CreateAdapter(string botName) => new BotFrameworkAdapter(new SimpleCredentialProvider());

        /// <summary>
        /// Override OnTurnAsync to implement the bot specific logic. This is exactly the same as implementing
        /// the IBot OnTurnAsync except here we can have multiple independent bots in the same project in addition
        /// to including arbitrary other MVC Controllers.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected abstract Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}
