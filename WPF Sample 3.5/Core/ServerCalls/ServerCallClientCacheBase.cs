using SampleWPF.DataContracts;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.Core.ServerCalls
{
    public abstract class ServerCallClientCacheBase<TRequest, TResponse, TResult> : ServerCallBase<TRequest, TResponse, TResult>
        where TResult : class
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        public string ClientId { get; private set; }
        public ClientCacheKey ClientCacheKey { get; private set; }
        public string SubKey { get; private set; }

        protected ServerCallClientCacheBase(string clientId, ClientCacheKey clientCacheKey)
        {
            ClientCacheKey = clientCacheKey;
            ClientId = clientId;
            SubKey = null;
        }

        protected ServerCallClientCacheBase(string clientId, ClientCacheKey clientCacheKey, string subKey)
        {
            ClientCacheKey = clientCacheKey;
            ClientId = clientId;
            SubKey = subKey;
        }

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
                IClientCache cacheManager = Repository.ClientCache;
                return cacheManager.Contains(ClientId, ClientCacheKey, SubKey);
            }
        }

        protected sealed override TResult GetFromCache()
        {
            IClientCache cacheManager = Repository.ClientCache;
            return cacheManager.Get<TResult>(ClientId, ClientCacheKey, SubKey);
        }

        protected sealed override void SetInCache(TResult value)
        {
            IClientCache cacheManager = Repository.ClientCache;
            cacheManager.Set(ClientId, ClientCacheKey, SubKey, value);
        }
    }
}
