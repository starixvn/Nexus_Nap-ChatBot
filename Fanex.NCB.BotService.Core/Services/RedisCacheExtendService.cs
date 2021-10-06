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
            string[] keysArr = keys.Select(key => GetActualKey(key)).ToArray();
            var result = await GetValueByKey<T>(keysArr);
            return result;
        }

        private string GetActualKey(string key)
        {
            return key.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
        }

        private IEnumerable<T> GetValueByKey<T>(string[] keysArr)
        {
            var values = new List<T>();
            for (int i = 0; i < keysArr.Length; i++)
            {
                T value = cacheService.Get<T>(keysArr[i]);
                if (!Equals(value, default(T)))
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }
}