namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using StackExchange.Redis;

    public interface IRedisCacheExtendService
    {
        IEnumerable<T> GetValueByKeyContains<T>(string key);
    }

    public class RedisCacheExtendService : IRedisCacheExtendService
    {
        private readonly IConnectionMultiplexer redisConnection;
        private readonly ICacheService cacheService;
        private readonly RedisConfiguration redisConfiguration;

        public RedisCacheExtendService(IConnectionMultiplexer redisConnection,
            ICacheService cacheService,
            RedisConfiguration redisConfiguration)
        {
            this.redisConnection = redisConnection;
            this.cacheService = cacheService;
            this.redisConfiguration = redisConfiguration;
        }

        public IEnumerable<T> GetValueByKeyContains<T>(string key)
        {
            EndPoint endPoint = redisConnection.GetEndPoints().FirstOrDefault();
            IServer server = redisConnection.GetServer(endPoint);

            IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
            var result = Enumerable.Empty<T>();
            if (keys.Any())
            {
                List<string> keyList = keys.Select(k => GetActualKey(k)).ToList();

                result = GetValueByKey<T>(keyList);
            }

            return result;
        }

        private string GetActualKey(string key) =>
                key.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);

        private IEnumerable<T> GetValueByKey<T>(List<string> keyList)
        {
            var values = new List<T>();
            foreach (var key in keyList)
            {
                T value = cacheService.Get<T>(key);
                if (!Equals(value, default(T)))
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }
}