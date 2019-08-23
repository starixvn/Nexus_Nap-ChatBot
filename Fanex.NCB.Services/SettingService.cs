namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.Setting;
    using Fanex.NCB.Models.Setting;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class SettingService : ISettingService
    {
        private readonly IBaseService baseService;

        public SettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<BaseModel> GetAllMappingStatus()
        {
            var status = Enum.GetValues(typeof(MappingStatus)).Cast<MappingStatus>().Select(v => new BaseModel
            {
                Id = (int)v,
                Name = EnumHelper.GetDescription(v)
            });
            return status;
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

        public async Task<IEnumerable<BaseModel>> SearchChatGroupAsync(string query)
        {
            var criteria = new SearchChatGroupNameCriteria
            {
                Keyword = query
            };
            return await baseService.GetDataAsync<BaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Recipient>> SearchRecipientsAsync(string query)
        {
            var criteria = new SearchRecipientCriteria
            {
                Keyword = query
            };
            return await baseService.GetDataAsync<Recipient>(criteria).ConfigureAwait(false);
        }
    }
}