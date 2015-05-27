using System.Collections.Generic;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.ViewModels.RequestDetails
{
    // !!!
    // RequestDetails are displayed on MainClient/SecondaryClient/CreateClient
    // but linked to MainClient id
    // if no MainClient opened, CreateClient's RequestDetails are not editable

    // Proposal:
    // 'Cloture la fiche' is enabled only in MainClient, disable everywhere else
    // 'Commentaire' is editable from MainClient and CreateClient (not from SecondaryClient)
    public class RequestDetailViewModel : ViewModelBase
    {
        public IClientManager ClientManager { get; private set; }
        
        // No data stored in VM but stored in a static class
        public string Comment
        {
            get { return Repository.Session.RequestDetailData.Comment; }
            set
            {
                if (Repository.Session.RequestDetailData.Comment != value)
                {
                    Repository.Session.RequestDetailData.Comment = value;
                    RaisePropertyChanged(() => Comment);
                }
            }
        }

        public string SelectedTypeTheme
        {
            get { return Repository.Session.RequestDetailData.SelectedTypeTheme; }
            set
            {
                if (Repository.Session.RequestDetailData.SelectedTypeTheme != value)
                {
                    Repository.Session.RequestDetailData.SelectedTypeTheme = value;
                    RaisePropertyChanged(() => SelectedTypeTheme);
                }
            }
        }

        //
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(() => IsExpanded, ref _isExpanded, value); }
        }

        private List<string> _typeThemeList;
        public List<string> TypeThemeList
        {
            get { return _typeThemeList; }
            set { Set(() => TypeThemeList, ref _typeThemeList, value); }
        }
        
        private bool _isCloseEnabled;
        public bool IsCloseEnabled
        {
            get { return _isCloseEnabled; }
            set { Set(() => IsCloseEnabled, ref _isCloseEnabled, value); }
        }
        
        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { _closeCommand = _closeCommand ?? new RelayCommand(Close);
                return _closeCommand;
            }
        }

        public RequestDetailViewModel(IClientManager clientManager)
        {
            ClientManager = clientManager;

            TypeThemeList = new List<string>
                {
                    "Type/Theme1",
                    "Type/Theme2",
                    "Type/Theme3",
                    "Type/Theme4",
                };
        }

        public void Refresh()
        {
            // TODO: refresh buttons state
            RaisePropertyChanged(() => Comment);
            RaisePropertyChanged(() => SelectedTypeTheme);
        }

        private void Close()
        {
            //ClientManager.CloseMainClient();
            UIRepository.PopupService.DisplayQuestion(
                "Fermeture de la fiche client",
                "Etes-vous sûr ?",
                new ActionButton
                    {
                        Caption = "Oui",
                        ClickCallback = () => ClientManager.CloseMainClient(),
                        Order = 1
                    },
                new ActionButton
                    {
                        Caption = "Non",
                        Order = 2
                    });
        }
    }

    public class RequestDetailViewModelDesignData : RequestDetailViewModel
    {
        public RequestDetailViewModelDesignData() : base(null)
        {
            IsCloseEnabled = false;
            IsExpanded = true;
        }
    }
}
