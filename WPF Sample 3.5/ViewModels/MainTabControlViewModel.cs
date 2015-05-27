using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.Core.MVVM;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;
using SampleWPF.ViewModels.CreateClient;
using SampleWPF.ViewModels.DisplayClient;
using SampleWPF.ViewModels.SearchClient;

namespace SampleWPF.ViewModels
{
    public class MainTabControlViewModel : ViewModelBase
    {
        private const int MaxTabs = 6; // Search + Create + Main + 3 Secondary
        private readonly IClientManager _clientManager;

        public List<DisplayClientTabViewModel> DisplayedClients
        {
            get { return Tabs.OfType<DisplayClientTabViewModel>().ToList(); }
        }

        private SearchClientTabViewModel _searchClientTabViewModel;
        public SearchClientTabViewModel SearchClientTabViewModel
        {
            get { return _searchClientTabViewModel; }
            set { Set(() => SearchClientTabViewModel, ref _searchClientTabViewModel, value); }
        }

        private CreateClientTabViewModel _createClientTabViewModel;
        public CreateClientTabViewModel CreateClientTabViewModel
        {
            get { return _createClientTabViewModel; }
            set { Set(() => CreateClientTabViewModel, ref _createClientTabViewModel, value); }
        }

        private ObservableCollection<MainTabBaseViewModel> _tabs;
        public ObservableCollection<MainTabBaseViewModel> Tabs
        {
            get { return _tabs; }
            set { Set(() => Tabs, ref _tabs, value); }
        }

        private MainTabBaseViewModel _selectedTab;
        public MainTabBaseViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (Set(() => SelectedTab, ref _selectedTab, value))
                    if (SelectedTab != null)
                        SelectedTab.OnSelected();
            }
        }

        private ICommand _closeTabCommand;
        public ICommand CloseTabCommand
        {
            get
            {
                _closeTabCommand = _closeTabCommand ?? new GenericRelayCommand<MainTabBaseViewModel>(CloseTab);
                return _closeTabCommand;
            }
        }

        public override void CleanUp()
        {
            base.CleanUp();

            foreach(MainTabBaseViewModel tab in Tabs)
                tab.CleanUp();
        }

        public MainTabControlViewModel(IClientManager clientManager)
        {
            _clientManager = clientManager;

            SearchClientTabViewModel = new SearchClientTabViewModel(clientManager);
            CreateClientTabViewModel = new CreateClientTabViewModel(clientManager);

            Tabs = new ObservableCollection<MainTabBaseViewModel>
                {
                    SearchClientTabViewModel, // always first
                    CreateClientTabViewModel // always last
                };
        }
        
        // Only DisplayClientTab may be added or removed
        public void AddTab(DisplayClientTabViewModel tab)
        {
            if (Tabs.Count >= MaxTabs)
            {
                Logger.Log(LogTypes.Warning, "Max tabs reached, cannot open a new tab");
            }
            else
            {
                // TODO: only one main tab, main tab is inserted after search, secondary tab inserted before create tab

                // Insert before CreateClientTabViewModel
                int createIndex = Tabs.IndexOf(CreateClientTabViewModel);
                if (createIndex == -1)
                {
                    Logger.Log(LogTypes.Warning, "CreateClientTab not found while adding new tab, inserting at the end");
                    createIndex = Tabs.Count;
                }
                Tabs.Insert(createIndex, tab);
            }
        }

        // Only DisplayClientTab may be added or removed
        public void RemoveTab(DisplayClientTabViewModel tab)
        {
            Tabs.Remove(tab);
            // TODO: closing main should close every tab except search/create
        }

        private void CloseTab(MainTabBaseViewModel tab)
        {
            // Cannot close non-display tab
            DisplayClientTabViewModel displayTab = tab as DisplayClientTabViewModel;
            if (displayTab == null)
                Logger.Log(LogTypes.Warning, "Trying to close non-display tab");
            else
                // TODO: perform cleaning
                RemoveTab(displayTab);
        }
    }

    public class MainTabControlViewModelDesignData : MainTabControlViewModel
    {
        public MainTabControlViewModelDesignData() : base(null)
        {
            SearchClientTabViewModel = new SearchClientTabViewModelDesignData(); // always first
            CreateClientTabViewModel = new CreateClientTabViewModelDesignData(); // always last

            Tabs = new ObservableCollection<MainTabBaseViewModel>
                {
                    SearchClientTabViewModel,
                    CreateClientTabViewModel,
                };
            AddTab(new DisplayClientTabViewModelDesignData(true));
            AddTab(new DisplayClientTabViewModelDesignData(false));
        }
    }
}
