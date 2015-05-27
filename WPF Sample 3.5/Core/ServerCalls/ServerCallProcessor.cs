using System.Collections.Generic;
using System.Linq;
using SampleWPF.Agents;
using SampleWPF.Core.Interfaces;

namespace SampleWPF.Core.ServerCalls
{
    public class ServerCallProcessor
    {
        public void Execute(IServerCallBase serverCall)
        {
            ServerCallCollection serverCalls = new ServerCallCollection
                {
                    serverCall
                };
            Execute(serverCalls);
        }

        public void Execute(List<IServerCallBase> serverCalls, bool executeInParallel = true)
        {
            // Get server call not found in cache
            List<ICompositionQuery> notInCacheServerCalls = serverCalls.Where(x => !x.IsInCache).Cast<ICompositionQuery>().ToList();

            // Call backend
            CompositeAgent agent = new CompositeAgent();
            agent.Invoke(notInCacheServerCalls, executeInParallel);
        }
    }
}
