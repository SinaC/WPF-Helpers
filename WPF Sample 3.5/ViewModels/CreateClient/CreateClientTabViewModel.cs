using System;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.DataContracts;
using SampleWPF.Utility.Interfaces;
using SampleWPF.ViewModels.RequestDetails;

namespace SampleWPF.ViewModels.CreateClient
{
    public class CreateClientTabViewModel : MainTabBaseViewModel
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
        
        private RequestDetailViewModel _requestDetailViewModel;
        public RequestDetailViewModel RequestDetailViewModel
        {
            get { return _requestDetailViewModel; }
            set { Set(() => RequestDetailViewModel, ref _requestDetailViewModel, value); }
        }
        
        private ICommand _createClientCommand;
        public ICommand CreateClientCommand
        {
            get { _createClientCommand = _createClientCommand ?? new RelayCommand(CreateClient);
                return _createClientCommand;
            }
        }

        public override void CleanUp()
        {
            base.CleanUp();

            RequestDetailViewModel.CleanUp();
        }

        public CreateClientTabViewModel(IClientManager clientManager) : base(clientManager)
        {
            RequestDetailViewModel = new RequestDetailViewModel(clientManager);

            Title = "Créer un client";
        }

        public override void OnSelected()
        {
            RequestDetailViewModel.Refresh();
            RequestDetailViewModel.IsCloseEnabled = false;

            // TODO: remove this
            DisplayAlert(new AlertData
                {
                    Type = AlertDataTypes.Info,
                    Title = "Create Client Selected"
                });
        }

        private void CreateClient()
        {
            // TODO: call SM create client and get BP
            if (!String.IsNullOrEmpty(ClientId))
            {
                ClientManager.OpenClient(ClientId);
            }
            else
                DisplayAlert(new AlertData
                {
                    Title = "Veuillez renseigner un nom de client",
                    Type = AlertDataTypes.Error
                });
        }
    }

    public class CreateClientTabViewModelDesignData : CreateClientTabViewModel
    {
        public CreateClientTabViewModelDesignData() : base(null)
        {
        }
    }
}
