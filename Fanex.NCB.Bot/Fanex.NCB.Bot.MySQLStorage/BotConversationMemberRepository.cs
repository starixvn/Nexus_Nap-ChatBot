using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;

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

        public async Task<int> InsertAsync(BotConversationMember value)
        {
            await _botServiceDBContext.BotConversationMembers.AddAsync(value).ConfigureAwait(false);
            return await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<BotConversationMember> SingleAsync(Expression<Func<BotConversationMember, bool>> predicate)
        {
            return Task.Run(() => _botServiceDBContext.BotConversationMembers.FirstOrDefault(predicate));
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
