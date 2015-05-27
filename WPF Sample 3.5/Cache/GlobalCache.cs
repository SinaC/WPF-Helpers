using System.Collections.Generic;
using System.Linq;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.Cache
{
    public class GlobalCache : IGlobalCache, ICacheAdmin
    {
        // Simplified dictionary
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        #region IGlobalCache

        public T Get<T>(GlobalCacheKey cacheKey) where T : class
        {
            string key = cacheKey.ToString();
            bool found;
            object item;
            lock (_cache)
            {
                found = _cache.TryGetValue(key, out item);
            }
            if (found)
                Logger.Log(LogTypes.Info, "Found in global cache: {0}", key);
            return item as T;
        }

        public void Set<T>(GlobalCacheKey cacheKey, T item) where T : class
        {
            string key = cacheKey.ToString();
            lock (_cache)
            {
                _cache[key] = item;
            }
            Logger.Log(LogTypes.Info, "Set in global cache: {0}", key);
        }

        public void Clear(GlobalCacheKey cacheKey)
        {
            string key = cacheKey.ToString();
            lock (_cache)
            {
                _cache.Remove(key);
            }
            Logger.Log(LogTypes.Info, "Remove from global cache: {0}", key);
        }

        public void Clear()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
            Logger.Log(LogTypes.Info, "Clear all global cache entries");
        }

        public bool Contains(GlobalCacheKey cacheKey)
        {
            string key = cacheKey.ToString();
            lock (_cache)
                return _cache.ContainsKey(key);
        }

        #endregion

        #region ICacheAdmin

        public List<string> GetKeys()
        {
            lock (_cache)
                return _cache.Keys.ToList();
        }

        public object GetItem(string key)
        {
            lock (_cache)
            {
                object item;
                _cache.TryGetValue(key, out item);
                return item;
            }
        }

        #endregion
    }
}
