using System;
using System.Collections.Generic;
using System.Linq;
using SampleWPF.DataContracts;
using SampleWPF.Utility;

namespace SampleWPF.Agents
{
    public class CompositeAgent
    {
        public void Invoke(List<ICompositionQuery> serverCalls, bool executeInParallel = true)
        {
            Logger.Log(LogTypes.Info, "Composite call");

            // Create correlation id
            foreach (ICompositionQuery serverCall in serverCalls)
            {
                Logger.Log(LogTypes.Info, "Send request {0}", serverCall.RequestBase.GetType());

                serverCall.RequestBase.CorrelationId = Guid.NewGuid();
            }

            // TODO: call backend and fill responses list
            List<ResponseBase> responses = new List<ResponseBase>();
            System.Threading.Thread.Sleep(1000); // Simulate a slow call

            // Correlate response and serverCalls
            foreach (ICompositionQuery serverCall in serverCalls)
            {
                Logger.Log(LogTypes.Info, "Receive response {0}", serverCall.RequestBase.GetType());

                Guid requestId = serverCall.RequestBase.CorrelationId;
                ResponseBase relatedResponse = responses.FirstOrDefault(x => x.CorrelationId == requestId);
                if (relatedResponse == null)
                    Logger.Log(LogTypes.Error, "No response for request {0}", serverCall.RequestBase.GetType());
                serverCall.ResponseBase = relatedResponse;
            }
        }
    }
}
