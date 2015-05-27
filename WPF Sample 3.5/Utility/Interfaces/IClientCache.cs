namespace SampleWPF.Utility.Interfaces
{
    public enum ClientCacheKey
    {
        Client,
        Contract,
        PDL,
    }

    public interface IClientCache
    {
        T Get<T>(string clientId, ClientCacheKey cacheKey, string subKey)
            where T : class;
        T Get<T>(string clientId, ClientCacheKey cacheKey)
            where T : class;

        void Set<T>(string clientId, ClientCacheKey cacheKey, string subKey, T item)
            where T : class;
        void Set<T>(string clientId, ClientCacheKey cacheKey, T item)
            where T : class;

        void Clear(string clientId, ClientCacheKey cacheKey, string subKey);
        void Clear(string clientId, ClientCacheKey cacheKey);
        void Clear(string clientId);
        void Clear();

        bool Contains(string clientId, ClientCacheKey cacheKey, string subKey);
        bool Contains(string clientId, ClientCacheKey cacheKey);
    }
}
