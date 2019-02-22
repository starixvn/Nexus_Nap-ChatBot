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
    public class BotConversationRepository : IBotConversationRepository
    {
        private readonly BotServiceDBContext _botServiceDBContext;
        public BotConversationRepository(BotServiceDBContext botServiceDBContext)
        {
            _botServiceDBContext = botServiceDBContext;
        }

        public IEnumerable<BotConversation> Fetch(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BotConversation>> FetchAsync(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public async Task<int> InsertAsync(BotConversation value)
        {

            await _botServiceDBContext.BotConversations.AddAsync(value).ConfigureAwait(false);
            await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
            return value.GroupID;
        }

        public async Task<BotConversation> SingleAsync(Expression<Func<BotConversation, bool>> predicate)
        {
            return await Task.Run(() => _botServiceDBContext.BotConversations.FirstOrDefault(predicate)).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAsync(BotConversation newValue)
        {
            _botServiceDBContext.BotConversations.Update(newValue);
            var result = await _botServiceDBContext.SaveChangesAsync().ConfigureAwait(false);
            return result > 0;
        }
    }
}
