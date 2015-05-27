using System.Collections.Generic;
using SampleWPF.Agents;
using SampleWPF.DataContracts;

namespace SampleWPF.Core.Interfaces
{
    public interface IServerCallBase : ICompositionQuery
    {
        List<AlertData> Alerts { get; }

        string WaitMessage { get; }
        bool IsInCache { get; }
    }

    public interface IServerCallBase<out TRequest, out TResponse, out TResult> : IServerCallBase
        where TRequest : RequestBase
        where TResponse : ResponseBase
        where TResult : class
    {
        TRequest Request { get; }
        TResponse Response { get; }
        TResult Result { get; }
    }
}
