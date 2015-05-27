using System.Collections.Generic;
using SampleWPF.DataContracts;

namespace SampleWPF.Core.Interfaces
{
    public interface IAlertsManager
    {
        int Count { get; }
        bool IsExpanded { get; set; }
        
        void Add(AlertData alert);
        void Add(List<AlertData> alerts);
        void Clear();
    }
}
