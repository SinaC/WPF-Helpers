using System;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.DataContracts;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.ViewModels.SearchClient
{
    public class SearchClientTabViewModel : MainTabBaseViewModel
    {
        public override bool IsTabClosable
        {
            get { return false; }
        }

        private string _clientId;
        public string ClientId
        {
            get { return _clientId; }
            set { Set(() => ClientId, ref _clientId, value); }
        }
        
        private ICommand _searchClientCommand;
        public ICommand SearchClientCommand
        {
            get
            {
                _searchClientCommand = _searchClientCommand ?? new RelayCommand(SearchClient);
                return _searchClientCommand;
            }
        }

        public SearchClientTabViewModel(IClientManager clientManager) : base(clientManager)
        {
            Title = "Recherche";
        }

        private void SearchClient()
        {
            if (!String.IsNullOrEmpty(ClientId))
            {
                // TODO: Search client
                ClientManager.OpenClient(ClientId);
            }
            else
                DisplayAlert(new AlertData
                    {
                        Title = "Veuillez renseigner un numéro de client",
                        Type = AlertDataTypes.Error
                    });
        }
    }

    public class SearchClientTabViewModelDesignData : SearchClientTabViewModel
    {
        public SearchClientTabViewModelDesignData() : base(null)
        {
            AlertsManager.Add(new AlertData
                {
                    Title = "ALERT 1",
                    Type = AlertDataTypes.Warning
                });
            AlertsManager.Add(new AlertData
            {
                Title = "ALERT 2",
                Type = AlertDataTypes.Warning
            });
            AlertsManager.Add(new AlertData
            {
                Title = "ALERT 3",
                Type = AlertDataTypes.Warning
            });
            AlertsManager.IsExpanded = true;
        }
    }
}
