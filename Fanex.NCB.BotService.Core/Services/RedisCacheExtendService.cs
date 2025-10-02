namespace Fanex.NCB.BotService.Core.Services;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Caching;
using Models;
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
        var endPoint = redisConnection.GetEndPoints().FirstOrDefault();
        var server = redisConnection.GetServer(endPoint);

        var keys = server.Keys(redisConfiguration.Db, $"*{key}*");
        var result = Enumerable.Empty<T>();
        if (keys.Any())
        {
            var keyList = keys.Select(k => GetActualKey(k)).ToList();

            result = GetValueByKey<T>(keyList);
        }

        return result;
    }

    private string GetActualKey(string key)
    {
        return key.Replace($"{{{redisConfiguration.ApplicationName}}}:", string.Empty).Replace("{", string.Empty)
            .Replace("}", string.Empty);
    }

    private IEnumerable<T> GetValueByKey<T>(List<string> keyList)
    {
        var values = new List<T>();
        foreach (var key in keyList)
        {
            var value = cacheService.Get<T>(key);
            if (!Equals(value, default(T)))
            {
                values.Add(value);
            }
        }

        return values;
    }
}