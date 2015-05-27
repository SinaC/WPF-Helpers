using SampleWPF.DataContracts;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.Core.ServerCalls
{
    public abstract class ServerCallGlobalCacheBase<TRequest, TResponse, TResult> : ServerCallBase<TRequest, TResponse, TResult>
        where TResult : class
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        public abstract GlobalCacheKey CacheKey { get; }

        public sealed override TResult Result
        {
            get
            {
                if (IsInCache)
                    return GetFromCache();
                TResult result = BuildResultFromResponse();
                SetInCache(result);
                return result;
            }
        }

        public sealed override bool IsInCache
        {
            get
            {
                IGlobalCache cacheManager = Repository.GlobalCache;
                return cacheManager.Contains(CacheKey);
            }
        }

        protected sealed override TResult GetFromCache()
        {
            IGlobalCache cacheManager = Repository.GlobalCache;
            return cacheManager.Get<TResult>(CacheKey);
        }

        protected sealed override void SetInCache(TResult value)
        {
            IGlobalCache cacheManager = Repository.GlobalCache;
            cacheManager.Set(CacheKey, value);
        }
    }
}
