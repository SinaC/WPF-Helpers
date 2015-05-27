using SampleWPF.Cache;
using SampleWPF.Core;
using SampleWPF.DataContracts;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;
using SampleWPF.ViewModels.RequestDetails;

namespace SampleWPF.ViewModels.DisplayClient
{
    public enum DisplayClientModes
    {
        Summary, // info client, locaux (points de livraison), boutons de choix de fonctions (vente, projets, ...)
        Edition, // modifier client
        Sales, // vente
        Projects, // projet (intranet dolce vita)
        Invoice, // facturation
        Taf, // TAF
        Termination, // résiliation
    }

    public class DisplayClientTabViewModel : MainTabBaseViewModel
    {
        public bool IsMain { get; private set; }
        public string ClientId { get; private set; }

        public override bool IsTabClosable
        {
            get { return !IsMain; }
        }

        private DisplayClientModes _mode;
        public DisplayClientModes Mode
        {
            get { return _mode; }
            set { Set(() => Mode, ref _mode, value); }
        }

        private RequestDetailViewModel _requestDetailViewModel;
        public RequestDetailViewModel RequestDetailViewModel
        {
            get { return _requestDetailViewModel; }
            set { Set(() => RequestDetailViewModel, ref _requestDetailViewModel, value); }
        }

        public override void CleanUp()
        {
            base.CleanUp();
            
            RequestDetailViewModel.CleanUp();
        }

        public DisplayClientTabViewModel(IClientManager clientManager, string clientId, bool isMain) : base(clientManager)
        {
            RequestDetailViewModel = new RequestDetailViewModel(clientManager);

            IsMain = isMain;
            ClientId = clientId;
            Title = clientId; // TODO: update it with client name
        }

        public void Open(string clientId)
        {
            Title = "Fiche client";

            // TODO: call SM
            // 2 async command: load from CIT and load from Octopus
            ClientData clientData = Repository.ClientCache.Get<ClientData>(clientId, ClientCacheKey.Client);
            if (clientData != null)
                Initialize(clientData);
            else
            {
                // TODO: call SM and call Initialize
                Title = clientId;
            }
        }

        public void Close()
        {
            // TODO: save request detail ?
            Repository.ClientCache.Clear(ClientId); // clear every entries related to this client
        }

        public void Initialize(ClientData clientData)
        {
            Title = clientData.Name;
            // TODO
        }

        public override void OnSelected()
        {
            // Refresh RequestDetail info when switching tab
            RequestDetailViewModel.Refresh();
            RequestDetailViewModel.IsCloseEnabled = IsMain;
        }
    }

    public class DisplayClientTabViewModelDesignData : DisplayClientTabViewModel
    {
        public DisplayClientTabViewModelDesignData() : this(true)
        {
        }

        public DisplayClientTabViewModelDesignData(bool isMain) : base(null, "444666888", isMain)
        {
            Title = "BERNARD LEON";
            AlertsManager.IsExpanded = true;
        }
    }
}
