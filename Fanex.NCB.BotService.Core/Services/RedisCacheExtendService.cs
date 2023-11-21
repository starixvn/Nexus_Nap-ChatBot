namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RedisCacheExtendService> filelogger;

        public RedisCacheExtendService(IConnectionMultiplexer redisConnection,
            ICacheService cacheService,
            RedisConfiguration redisConfiguration,
            ILogger<RedisCacheExtendService> filelogger)
        {
            this.redisConnection = redisConnection;
            this.cacheService = cacheService;
            this.redisConfiguration = redisConfiguration;
            this.filelogger = filelogger;
        }

        public IEnumerable<T> GetValueByKeyContains<T>(string key)
        {
            filelogger.LogWarning("GetValueByKeyContains: 1");
            EndPoint endPoint = redisConnection.GetEndPoints().FirstOrDefault();
            filelogger.LogWarning("GetValueByKeyContains: 2");
            IServer server = redisConnection.GetServer(endPoint);
            filelogger.LogWarning("GetValueByKeyContains: 3");

            IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
 
            string[] keysArr = keys.Select(key => GetActualKey(key)).ToArray();
            filelogger.LogWarning("GetValueByKeyContains: 5");
            var result = GetValueByKey<T>(keysArr);
            filelogger.LogWarning("GetValueByKeyContains: 6");
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