namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using StackExchange.Redis;

    public interface IRedisCacheExtendService
    {
        Task<IEnumerable<T>> GetValueByKeyContains<T>(string key);
    }

    public class RedisCacheExtendService : IRedisCacheExtendService
    {
        private readonly RedisConfiguration redisConfiguration;
        private readonly ICacheService cacheService;

        public RedisCacheExtendService(ICacheService cacheService, RedisConfiguration redisConfiguration)
        {
            this.cacheService = cacheService;
            this.redisConfiguration = redisConfiguration;
        }

        public async Task<IEnumerable<T>> GetValueByKeyContains<T>(string key)
        {
            using (var connection = await ConnectionMultiplexer.ConnectAsync(redisConfiguration.ConnectionString))
            {
                EndPoint endPoint = connection.GetEndPoints().FirstOrDefault();
                IServer server = connection.GetServer(endPoint);

                IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
                string[] keysArr = keys.Select(key => GetActualKey(key)).ToArray();
                var result = await GetValueByKey<T>(keysArr);
                return result;
            }
        }

        private string GetActualKey(string key)
        {
            return key.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
        }

        private async Task<IEnumerable<T>> GetValueByKey<T>(string[] keysArr)
        {
            var values = new List<T>();
            for (int i = 0; i < keysArr.Length; i++)
            {
                T value = await cacheService.GetAsync<T>(keysArr[i]);
                if (!Equals(value, default(T)))
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }
}