namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;

    public interface ICacheRedisService
    {
        IEnumerable<RedisKey> GetAllValuesContainKey(string key);
    }

    public class CacheRedisService : ICacheRedisService
    {
        private readonly IConfiguration configuration;

        public CacheRedisService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<RedisKey> GetAllValuesContainKey(string key)
        {
            ConnectionMultiplexer con = ConnectionMultiplexer.Connect(configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value);
            EndPoint endPoint = con.GetEndPoints().First();
            RedisKey[] keys = con.GetServer(endPoint).Keys(pattern: $"{key}*").ToArray();

            return keys;
        }
    }
}