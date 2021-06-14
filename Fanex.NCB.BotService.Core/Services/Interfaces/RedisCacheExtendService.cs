namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using StackExchange.Redis;

    public interface IRedisCacheExtendService
    {
        IEnumerable<T> GetValueByKeyContains<T>(string key);
    }

    public class RedisCacheExtendService : IRedisCacheExtendService
    {
        private readonly RedisConfiguration redisConfiguration;
        private readonly ICacheService cacheService;

        internal RedisStorage RedisStorage { get; private set; }

        public RedisCacheExtendService(ICacheService cacheService, RedisConfiguration redisConfiguration)
        {
            this.cacheService = cacheService;
            this.redisConfiguration = redisConfiguration;
            RedisStorage = new RedisStorage(redisConfiguration);
        }

        public IEnumerable<T> GetValueByKeyContains<T>(string key)
        {
            IServer server = RedisStorage.GetRedisServer();

            IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
            string[] keysArr = keys.Select(key => GetActualKey(key)).ToArray();
            var result = GetValueByKey<T>(keysArr);
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