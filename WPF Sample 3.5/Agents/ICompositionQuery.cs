using SampleWPF.DataContracts;

namespace SampleWPF.Agents
{
    public interface ICompositionQuery
    {
        RequestBase RequestBase { get; }
        ResponseBase ResponseBase { set; }
    }
}
