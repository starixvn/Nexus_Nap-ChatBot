namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
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
            EndPoint endPoint = redisConnection.GetEndPoints().FirstOrDefault();
            IServer server = redisConnection.GetServer(endPoint);

            IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");

            string[] keysArr = keys.Select(k => GetActualKey(k)).ToArray();
            filelogger.LogWarning($"GetValueByKeyContains 1: {JsonConvert.SerializeObject(keysArr)}");
            var result = GetValueByKey<T>(keysArr);
            filelogger.LogWarning("GetValueByKeyContains: 7");
            return result;
        }

        private string GetActualKey(RedisKey key)
        {
            var keyString = key.ToString();
            filelogger.LogWarning($"Key string: {keyString}");
            return keyString.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
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