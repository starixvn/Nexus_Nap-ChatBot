using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Fanex.NCB.Bot.MySQLStorage
{
    public class BaseRepository
    {
        private readonly IConfiguration _configuration;
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected string ChatBotConnection => _configuration.GetConnectionString("ChatBotConnection");
    }
}
