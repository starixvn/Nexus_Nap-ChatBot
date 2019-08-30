namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Criteria.LocalNap.Group;

    public class LocalGroupService : IGroupService
    {
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

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = await objectDb.QueryEntityAsync<Group, User, User, Group>(
                    (group, creator, modifier) =>
                    {
                        group.Creator = creator;
                        group.Modifier = modifier;
                        return group;
                    },
                    "CreatorId, ModifierId",
                    criteria).ConfigureAwait(false);

                return result;
            }
        }
    }
}