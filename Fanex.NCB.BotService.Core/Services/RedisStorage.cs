namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Fanex.NCB.BotService.Core.Models;
    using StackExchange.Redis;

    internal class RedisStorage : IDisposable
    {
        private volatile ConnectionMultiplexer _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly RedisConfiguration redisConfiguration;
        private IServer _server;
        private bool disposed;

        public RedisStorage(RedisConfiguration redisConfiguration)
        {
            this.redisConfiguration = redisConfiguration;
        }

        public IServer GetRedisServer()
        {
            Connect();
            return _server;
        }

        private void Connect()
        {
            if (_server != null)
            {
                return;
            }

            _connectionLock.Wait();

            try
            {
                if (_server == null)
                {
                    _connection = ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString);
                    EndPoint endPoint = _connection.GetEndPoints().FirstOrDefault();

                    _server = _connection.GetServer(endPoint);
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _connectionLock.Dispose();
                _connection?.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}