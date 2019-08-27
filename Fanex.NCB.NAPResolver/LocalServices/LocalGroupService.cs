namespace Fanex.NCB.NAPResolver.LocalServices
{
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using System;
    using System.Threading.Tasks;

    public class LocalGroupService : IGroupService
    {
        public Group Get(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<Group> GetAsync(int groupId)
        {
            throw new NotImplementedException();
        }
    }
}