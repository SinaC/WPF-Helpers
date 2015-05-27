using System.Collections.Generic;
using System.Linq;
using SampleWPF.Core.Interfaces;
using SampleWPF.DataContracts;

namespace SampleWPF.Core.ServerCalls
{
    public abstract class ServerCallBase<TRequest, TResponse, TResult> : IServerCallBase<TRequest, TResponse, TResult>
        where TRequest : RequestBase
        where TResponse : ResponseBase
        where TResult : class
    {
        #region IServerCallBase

        public abstract TRequest Request { get; } // this will avoid developer forgetting Request = new  in ctor
        public TResponse Response { get; protected set; }

        public abstract TResult Result { get; }

        public RequestBase RequestBase
        {
            get { return Request; }
        }

        public ResponseBase ResponseBase
        {
            set { Response = value as TResponse; }
        }

        public List<AlertData> Alerts
        {
            get
            {
                return Response == null
                    ? Enumerable.Empty<AlertData>().ToList()
                    : Response.Messages ?? Enumerable.Empty<AlertData>().ToList();
            }
        }

        public abstract string WaitMessage { get; }
        public abstract bool IsInCache { get; }

        #endregion

        protected abstract TResult GetFromCache();
        protected abstract void SetInCache(TResult value);
        protected abstract TResult BuildResultFromResponse();
    }
}
