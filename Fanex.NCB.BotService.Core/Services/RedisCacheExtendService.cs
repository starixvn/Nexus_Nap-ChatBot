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
            filelogger.LogWarning($"GetValueByKeyContains Key: {key}");
            EndPoint endPoint = redisConnection.GetEndPoints().FirstOrDefault();
            IServer server = redisConnection.GetServer(endPoint);

            IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
            filelogger.LogWarning($"GetValueByKeyContains: End Get all keys");
            var keyArray = keys.ToArray();
            filelogger.LogWarning($"GetValueByKeyContains: End Get all keys to Array");

            //var stringKeys = keys.Select(x => (string)x);
            var stringKeys = new List<string>();
            foreach (var k in keys)
            {
                stringKeys.Add((string)k);
            }
            filelogger.LogWarning($"GetValueByKeyContains: convert RedisKey to string");

            string[] keysArr = stringKeys.Select(k => GetActualKey(k)).ToArray();
            filelogger.LogWarning("GetValueByKeyContains: Get actual key");
            var result = GetValueByKey<T>(keysArr);
            filelogger.LogWarning("GetValueByKeyContains: 7");
            return result;
        }

        private string GetActualKey(string key)
        {
            filelogger.LogWarning($"GetActualKey: Keys: {key}");

            var value = key.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
            filelogger.LogWarning($"GetActualKey: Value: {value}");

            return value;
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