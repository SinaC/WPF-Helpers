using System.Collections.Generic;
using System.Linq;
using SampleWPF.Core.Interfaces;
using SampleWPF.DataContracts;

namespace SampleWPF.Core.ServerCalls
{
    public class ServerCallCollection : List<IServerCallBase>
    {
        public TServerCall GetServerCallOfType<TServerCall>()
            where TServerCall : class, IServerCallBase
        {
            return this.OfType<TServerCall>().FirstOrDefault(); // we don't have to create a new instance of ServerCall if it's inexistant ?? Activator.CreateInstance<T>();
        }

        // TODO
        //public TResult GetResultOfType<TServerCall, TResult>()
        //    where TServerCall : class, IServerCallBase
        //    where TResult : class
        //{
        //    TServerCall serverCall = GetServerCallOfType<TServerCall>();
        //    return serverCall == null ? default(TResult) : serverCall.Result as TResult;
        //}

        public bool IsInCache
        {
            get { return this.All(x => x.IsInCache); }
        }

        public List<string> WaitMessage
        {
            get { return this.Where(x => !x.IsInCache).Select(x => x.WaitMessage).ToList(); }
        }

        public List<AlertData> Alerts
        {
            get
            {
                return this.SelectMany(x => x.Alerts).ToList();
            }
        }
    }
}
