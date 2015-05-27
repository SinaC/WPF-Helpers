using SampleWPF.DataContracts;

namespace SampleWPF.Core.ServerCalls
{
    public abstract class ServerCallNoCacheBase<TRequest, TResponse, TResult> : ServerCallBase<TRequest, TResponse, TResult>
        where TResult : class
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        public sealed override TResult Result
        {
            get { return BuildResultFromResponse(); }
        }


        public sealed override bool IsInCache
        {
            get { return false; } // NOP
        }

        protected sealed override TResult GetFromCache()
        {
            return null; // NOP
        }

        protected sealed override void SetInCache(TResult value)
        {
            // NOP
        }
    }
}
