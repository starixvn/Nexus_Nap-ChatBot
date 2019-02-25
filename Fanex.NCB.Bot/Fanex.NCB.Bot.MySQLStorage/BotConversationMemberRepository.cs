using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Fanex.NCB.Bot.MySQLStorage.UnitTests")]
namespace Fanex.NCB.Bot.MySQLStorage
{
    public class BotConversationMemberRepository : IBotConversationMemberRepository
    {
        private readonly BotServiceDBContext _botServiceDBContext;
        public BotConversationMemberRepository(BotServiceDBContext botServiceDBContext)
        {
            _botServiceDBContext = botServiceDBContext;
        }

        public IEnumerable<BotConversationMember> Fetch(Expression<Func<BotConversationMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BotConversationMember>> FetchAsync(Expression<Func<BotConversationMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public async Task<int> InsertAsync(BotConversationMember value)
        {
            await _botServiceDBContext.BotConversationMembers.AddAsync(value).ConfigureAwait(false);
            return await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<BotConversationMember> SingleAsync(Expression<Func<BotConversationMember, bool>> predicate)
        {
            return await Task.Run(() => _botServiceDBContext.BotConversationMembers.FirstOrDefault(predicate)).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAsync(BotConversationMember newValue)
        {
            if (await SingleAsync(x => x.MemberId == newValue.MemberId && x.ConversationID == newValue.ConversationID).ConfigureAwait(false) != null)
            {
                _botServiceDBContext.BotConversationMembers.Update(newValue);
                var result = await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
                return result > 0;
            }

            return false;
        }
    }
}
