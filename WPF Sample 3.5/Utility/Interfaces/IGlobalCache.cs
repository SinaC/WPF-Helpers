namespace SampleWPF.Utility.Interfaces
{
    public enum GlobalCacheKey
    {
    }

    public interface IGlobalCache
    {
        T Get<T>(GlobalCacheKey cacheKey)
            where T : class;

        void Set<T>(GlobalCacheKey cacheKey, T item)
            where T : class;

        void Clear(GlobalCacheKey cacheKey);
        void Clear();

        bool Contains(GlobalCacheKey cacheKey);
    }
}
