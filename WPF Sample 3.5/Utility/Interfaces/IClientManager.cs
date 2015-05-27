namespace SampleWPF.Utility.Interfaces
{
    public interface IClientManager
    {
        void OpenClient(string clientId);
        void CloseMainClient();
        void PrefetchClient(string clientId);
    }
}
