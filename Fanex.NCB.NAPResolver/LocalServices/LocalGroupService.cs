namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Criteria.LocalNap.Group;
    using Fanex.NCB.NAPResolver.Model;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class LocalGroupService : IGroupService
    {
        private readonly IBaseService baseService;

        public LocalGroupService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public Group Get(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<Group> GetAsync(int groupId)
        {
            var criteria = new GetGroupByIdCriteria
            {
                GroupId = groupId
            };

            var flatGroups = await baseService.GetDataEntityAsync<FlatGroup>(criteria);

            return flatGroups is not null ? flatGroups.ConvertToNestedModel() : null;
        }
    }
}