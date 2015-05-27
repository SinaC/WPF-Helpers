using System;
using System.Collections.Generic;

namespace SampleWPF.DataContracts
{
    public class ResponseBase
    {
        public Guid CorrelationId { get; set; }
        public List<AlertData> Messages { get; set; }

        public ResponseBase()
        {
            Messages = new List<AlertData>();
        }
    }
}
