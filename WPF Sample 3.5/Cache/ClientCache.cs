using System;
using System.Collections.Generic;
using System.Linq;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.Cache
{
    // First Key: ClientId
    // Second Key: ClientCacheKey
    // Third Key: string [optional]
    public class ClientCache : IClientCache, ICacheAdmin
    {
        // TODO: use this dictionary instead
        //private readonly Dictionary<string, Dictionary<ClientCacheKey, Dictionary<string, object>>> _cache = new Dictionary<string, Dictionary<ClientCacheKey, Dictionary<string, object>>>();

        // Simplified dictionary
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        #region IClientCache

        public T Get<T>(string clientId, ClientCacheKey cacheKey, string subKey) where T : class
        {
            string compositeKey = ComputeCompositeKey(clientId, cacheKey, subKey);
            bool found;
            object item;
            lock (_cache)
            {
                found = _cache.TryGetValue(compositeKey, out item);
            }
            if (found)
                Logger.Log(LogTypes.Info, "Found in client cache: {0}", compositeKey);
            return item as T;
        }

        public T Get<T>(string clientId, ClientCacheKey cacheKey) where T : class
        {
            return Get<T>(clientId, cacheKey, null);
        }

        public void Set<T>(string clientId, ClientCacheKey cacheKey, string subKey, T item) where T : class
        {
            string compositeKey = ComputeCompositeKey(clientId, cacheKey, subKey);
            lock (_cache)
            {
                _cache[compositeKey] = item;
            }
            Logger.Log(LogTypes.Info, "Set in client cache: {0}", compositeKey);
        }

        public void Set<T>(string clientId, ClientCacheKey cacheKey, T item) where T : class
        {
            Set(clientId, cacheKey, null, item);
        }

        public void Clear(string clientId, ClientCacheKey cacheKey, string subKey)
        {
            string compositeKey = ComputeCompositeKey(clientId, cacheKey, subKey);
            lock (_cache)
            {
                _cache.Remove(compositeKey);
            }
            Logger.Log(LogTypes.Info, "Remove from client cache: {0}", compositeKey);
        }

        public void Clear(string clientId, ClientCacheKey cacheKey)
        {
            string compositePattern = ComputeCompositePattern(clientId, cacheKey);
            List<string> matchingKeys;
            lock(_cache)
            {
                matchingKeys = _cache.Keys.Where(key => key.StartsWith(compositePattern)).ToList();
                foreach (string key in matchingKeys)
                    _cache.Remove(key);
            }
            foreach(string matchingKey in matchingKeys)
                Logger.Log(LogTypes.Info, "Remove from client cache: {0}", matchingKey);
        }

        public void Clear(string clientId)
        {
            string compositePattern = ComputeCompositePattern(clientId);
            List<string> matchingKeys;
            lock (_cache)
            {
                matchingKeys = _cache.Keys.Where(key => key.StartsWith(compositePattern)).ToList();
                foreach (string key in matchingKeys)
                    _cache.Remove(key);
            }
            foreach (string matchingKey in matchingKeys)
                Logger.Log(LogTypes.Info, "Remove from client cache: {0}", matchingKey);
        }

        public void Clear()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
            Logger.Log(LogTypes.Info, "Clear all client cache entries");
        }

        public bool Contains(string clientId, ClientCacheKey cacheKey, string subKey)
        {
            string compositeKey = ComputeCompositeKey(clientId, cacheKey, subKey);
            lock (_cache)
            {
                return _cache.ContainsKey(compositeKey);
            }
        }

        public bool Contains(string clientId, ClientCacheKey cacheKey)
        {
            return Contains(clientId, cacheKey, null);
        }

        #endregion

        #region ICacheAdmin

        public List<string> GetKeys()
        {
            lock (_cache)
            {
                return _cache.Keys.ToList();
            }
        }

        public object GetItem(string key)
        {
            lock(_cache)
            {
                object item;
                _cache.TryGetValue(key, out item);
                return item;
            }
        }

        #endregion

        private static string ComputeCompositeKey(string clientId, ClientCacheKey cacheKey, string subKey)
        {
            return ComputeCompositeKey(clientId, cacheKey.ToString(), subKey);
        }

        private static string ComputeCompositePattern(string clientId, ClientCacheKey cacheKey)
        {
            return ComputeCompositeKey(clientId, cacheKey.ToString(), null);
        }

        private static string ComputeCompositePattern(string clientId)
        {
            return ComputeCompositeKey(clientId, null, null);
        }

        private static string ComputeCompositeKey(string clientId, string cacheKey, string subKey)
        {
            return (clientId ?? String.Empty) + "-" + (cacheKey ?? String.Empty) + "-" + (subKey ?? String.Empty);
        }
    }
}
