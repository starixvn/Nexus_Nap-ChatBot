using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;

namespace Fanex.NCB.Bot.MySQLStorage
{
    public class BotConversationRepository : IBotConversationRepository
    {
        public IEnumerable<BotConversation> Fetch(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Insert(BotConversation value)
        {
            throw new NotImplementedException();
        }

        public BotConversation Single(Expression<Func<BotConversation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Update(BotConversation newValue)
        {
            throw new NotImplementedException();
        }
    }
}
