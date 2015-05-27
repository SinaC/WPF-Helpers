using System.Collections.Generic;
using System.Linq;
using SampleWPF.Core;
using SampleWPF.Core.MVVM;
using SampleWPF.Models;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;
using SampleWPF.ViewModels.DisplayClient;
using SampleWPF.ViewModels.Header;

namespace SampleWPF.ViewModels
{
    public class MainViewModel : ViewModelBase, IClientManager
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        private MainHeaderViewModel _headerViewModel;
        public MainHeaderViewModel HeaderViewModel
        {
            get { return _headerViewModel; }
            set { Set(() => HeaderViewModel, ref _headerViewModel, value); }
        }

        private MainTabControlViewModel _mainTabControlViewModel;
        public MainTabControlViewModel MainTabControlViewModel
        {
            get { return _mainTabControlViewModel; }
            set { Set(() => MainTabControlViewModel, ref _mainTabControlViewModel, value); }
        }

        public override void CleanUp()
        {
            base.CleanUp();

            Repository.ClientCache.Clear(); // Clear all client cache entries

            HeaderViewModel.CleanUp();
            MainTabControlViewModel.CleanUp();
        }
        
        public MainViewModel()
        {
            Title = "Portal Conseilller ENGIE - 5 build 0 - LOCAL";
            HeaderViewModel = new MainHeaderViewModel();
            MainTabControlViewModel = new MainTabControlViewModel(this);

            // Search by default
            MainTabControlViewModel.SelectedTab = MainTabControlViewModel.SearchClientTabViewModel;

            //
            Repository.Session = new SessionData();
        }

        // Create/Add Tab, DisplayClientTabViewModel is responsible for calling SM
        public void OpenClient(string clientId)
        {
            // Check if main tab is already opened
            bool mainAlreadyExists = MainTabControlViewModel.DisplayedClients.Any(x => x.IsMain);
            // Create main or secondary tab
            DisplayClientTabViewModel client = new DisplayClientTabViewModel(this, clientId, !mainAlreadyExists);
            client.Open(clientId);
            // Add tab
            MainTabControlViewModel.AddTab(client);
            // Switch to opened client
            MainTabControlViewModel.SelectedTab = client;
        }

        // Close/Remove Tab, DisplayClientTabViewModel is responsible for calling SM
        public void CloseMainClient()
        {
            List<DisplayClientTabViewModel> displayedClients = MainTabControlViewModel.DisplayedClients;
            // Get main client and close it if any
            DisplayClientTabViewModel mainClient = displayedClients.FirstOrDefault(x => x.IsMain);
            if (mainClient != null)
                mainClient.Close();
            // Remove every displayed client tab
            foreach(DisplayClientTabViewModel displayedClient in displayedClients)
                MainTabControlViewModel.RemoveTab(displayedClient);
            // Switch to search
            MainTabControlViewModel.SelectedTab = MainTabControlViewModel.SearchClientTabViewModel;
        }

        //
        public void PrefetchClient(string clientId)
        {
            // TODO: read client and insert in cache
        }
    }

    public class MainViewModelDesignData : MainViewModel
    {
        public MainViewModelDesignData()
        {
            Title = "Portal Conseilller ENGIE - 5 build 0 - LOCAL";

            HeaderViewModel = new MainHeaderViewModelDesignData();
            MainTabControlViewModel = new MainTabControlViewModelDesignData();
        }
    }
}
