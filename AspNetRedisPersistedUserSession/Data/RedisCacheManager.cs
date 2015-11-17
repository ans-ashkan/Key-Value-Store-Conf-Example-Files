using System.Threading.Tasks;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace AspNetRedisPersistedUserSession.Data
{
    public sealed class RedisCacheManager
    {
        private static readonly RedisCacheManager instance = new RedisCacheManager();
        private readonly ConnectionMultiplexer _redis;
        private readonly StackExchangeRedisCacheClient _cacheClient;
        private readonly NewtonsoftSerializer newtonsoftSerializer;

        static RedisCacheManager()
        {
        }

        private RedisCacheManager()
        {
            //            object serializer = new NewtonsoftSerializer();
            _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions()
            {
                ClientName = "AspnetRedisPersitedUserSession",
                EndPoints = { { "localhost" } },
                AbortOnConnectFail = false,
                KeepAlive = 1
            });
            newtonsoftSerializer = new NewtonsoftSerializer();
            _cacheClient = new StackExchangeRedisCacheClient(_redis, newtonsoftSerializer);
        }

        public static RedisCacheManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void Set<T>(string key, T obj) where T : class
        {
            _cacheClient.Add(key, obj);
        }
        public T Get<T>(string key) where T : class
        {
            return _cacheClient.Get<T>(key);
        }
        public void Remove(string key)
        {
            _cacheClient.Remove(key);
        }
        public Task SetAsync<T>(string key, T obj) where T : class
        {
            return _cacheClient.AddAsync(key, obj);
        }
        public Task<T> GetAsync<T>(string key) where T : class
        {
            return _cacheClient.GetAsync<T>(key);
        }

        public Task RemoveAsync(string key)
        {
            return _cacheClient.RemoveAsync(key);
        }
    }
}