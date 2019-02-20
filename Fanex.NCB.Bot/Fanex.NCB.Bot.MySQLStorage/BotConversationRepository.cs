using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.Extensions.Configuration;
using System.Linq;
namespace Fanex.NCB.Bot.MySQLStorage
{
    public class BotConversationRepository : BaseRepository, IBotConversationRepository
    {
        public BotConversationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public IEnumerable<BotConversation> Fetch(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BotConversation>> FetchAsync(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertAsync(BotConversation value)
        {
            using (var context = new BotServiceDBContext(ChatBotConnection))
            {
                await context.BotConversations.AddAsync(value);
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
        }

        public async Task<BotConversation> SingleAsync(Expression<Func<BotConversation, bool>> predicate)
        {
            using (var context = new BotServiceDBContext(ChatBotConnection))
            {
                return await Task.Run(() => context.BotConversations.FirstOrDefault(predicate));
            }
        }

        public async Task<bool> UpdateAsync(BotConversation newValue)
        {
            using (var context = new BotServiceDBContext(ChatBotConnection))
            {
                context.BotConversations.Update(newValue);
                var result = await context.SaveChangesAsync();
                return result > 0;
            }

        }
    }
}
