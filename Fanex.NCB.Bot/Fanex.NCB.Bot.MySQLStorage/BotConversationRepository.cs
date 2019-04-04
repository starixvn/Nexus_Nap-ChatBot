using System;
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
    public class BotConversationRepository : IBotConversationRepository
    {
        private readonly BotServiceDBContext _botServiceDBContext;
        public BotConversationRepository(BotServiceDBContext botServiceDBContext)
        {
            _botServiceDBContext = botServiceDBContext;
        }

        public async Task<int> InsertAsync(BotConversation value)
        {

            await _botServiceDBContext.BotConversations.AddAsync(value).ConfigureAwait(false);
            await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
            return value.ID;
        }

        public Task<BotConversation> SingleAsync(Expression<Func<BotConversation, bool>> predicate)
        {
            return Task.Run(() => _botServiceDBContext.BotConversations.FirstOrDefault(predicate));
        }

        public async Task<bool> UpdateAsync(BotConversation newValue)
        {
            if (await SingleAsync(x => x.ID == newValue.ID && x.ConversationID == newValue.ConversationID).ConfigureAwait(false) != null)
            {
                _botServiceDBContext.BotConversations.Update(newValue);
                var result = await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
                return result > 0;
            }

            return false;
        }
    }
}
