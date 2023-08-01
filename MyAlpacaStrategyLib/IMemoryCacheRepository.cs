using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
namespace MyAlpacaStrategyLib
{
    public interface IMemoryCacheRepository
    {
        bool Create<T>(string key, T value) where T : class;
        bool Update<T>(string key, T value) where T : class;
        void Set<T> (string key , T value ) where T:class;

        /// <summary>
        /// 若key不存在，什麼事都沒發生。
        /// </summary>
        void Remove(string key);


        /// <returns>key不存在 回傳 null</returns>
        T Get<T> (string key) where T : class;

    }
    public class MemoryCacheRepository : IMemoryCacheRepository
    {
        private readonly IDistributedCache _cache;
        public MemoryCacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool Create<T>(string key, T value) where T : class
        {
            // 錯誤：已存在
            if (_cache.Get(key) != null) return false;

            Set(key, value);
            return true;
        }
        public bool Update<T>(string key, T value) where T : class
        {
            // 錯誤：不存在
            if (_cache.Get(key) == null ) return false;

            Set(key, value);
            return true;
        }
        public void Set<T>(string key, T value) where T : class
        {
            _cache.Set(key, ObjectToByteArray(value),
                new DistributedCacheEntryOptions {}
            );
            //_cache.Refresh
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Get<T>(string key) where T : class
        {
            return ByteArrayToObject<T>(_cache.Get(key));
        }



        private static byte[] ObjectToByteArray(object obj) //沒有泛型 用obj也行
        {
            //using System.Text.Json;
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        private static T ByteArrayToObject<T>(byte[] bytes) where T : class
        {
            return bytes is null ? null : JsonSerializer.Deserialize<T>(bytes);
        }

    }
}
