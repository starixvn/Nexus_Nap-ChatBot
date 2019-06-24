using Fanex.NCB.Core.Models.DTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Fanex.NCB.Core.Services
{
    public class SessionService : ISessionService
    {
        private const string AppSessionKey = "__Fanex.NCB.AppSession";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext Context => _httpContextAccessor.HttpContext;

        public ApplicationSession AppSession
        {
            get => JsonConvert.DeserializeObject<ApplicationSession>(Context.Session.GetString(AppSessionKey));
            set
            {
                AppSession = value;
                Context.Session.SetString(AppSessionKey, JsonConvert.SerializeObject(value));
            }
        }
    }
}