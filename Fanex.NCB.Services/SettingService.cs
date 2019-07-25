namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.Criteria.Setting;
    using Fanex.NCB.Models.Setting;
    using Fanex.NCB.Services.Interfaces;

    public class SettingService : ISettingService
    {
        private readonly IBaseService baseService;

        public SettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<BotName>> GetBotNameAsync()
        {
            var criteria = new GetBotNameCriteria();
            return await baseService.GetDataAsync<BotName>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ChatGroupType>> GetChatGroupTypeAsync()
        {
            var criteria = new GetChatGroupTypeCriteria();
            return await baseService.GetDataAsync<ChatGroupType>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BaseModel>> GetFilterRecipientTypesAsync()
        {
            var criteria = new GetFilterRecipientTypeCriteria();
            return await baseService.GetDataAsync<BaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BaseModel>> GetLicenseeStatusAsync()
        {
            var criteria = new GetLicenseeStatusCriteria();
            return await baseService.GetDataAsync<BaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync()
        {
            var criteria = new GetRecipientCriteria();
            return await baseService.GetDataAsync<Recipient>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BaseModel>> GetRecipientTypesAsync()
        {
            var criteria = new GetRecipientTypeCriteria();
            return await baseService.GetDataAsync<BaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Recipient>> SearchRecipientsAsync(string query)
        {
            var criteria = new SearchRecipientCriteria
            {
                Query = query
            };
            return await baseService.GetDataAsync<Recipient>(criteria).ConfigureAwait(false);
        }
    }
}