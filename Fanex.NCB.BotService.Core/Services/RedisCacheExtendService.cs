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
            try
            {
                filelogger.LogWarning($"GetValueByKeyContains Key: {key}");
                EndPoint endPoint = redisConnection.GetEndPoints().FirstOrDefault();
                IServer server = redisConnection.GetServer(endPoint);

                IEnumerable<RedisKey> keys = server.Keys(redisConfiguration.Db, pattern: $"*{key}*");
                filelogger.LogWarning($"GetValueByKeyContains: End Get all keys");
                var lstKeys = keys.ToList();

                var stringKeys = new List<string>();
                filelogger.LogWarning($"GetValueByKeyContains: start Add key to list");
                stringKeys.Add((string)keys.FirstOrDefault());
                filelogger.LogWarning($"GetValueByKeyContains: end Add key to list");

                List<string> keyList = stringKeys.Select(k => GetActualKey(k)).ToList();
                filelogger.LogWarning("GetValueByKeyContains: Get actual key");
                var result = GetValueByKey<T>(keyList);
                filelogger.LogWarning("GetValueByKeyContains: 7");
                return result;
            }
            catch (System.Exception ex)
            {
                filelogger.LogError(ex.ToString());
                throw;
            }
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