using SampleWPF.Core;
using SampleWPF.Core.MVVM;
using SampleWPF.Utility.Interfaces;
using SampleWPF.ViewModels.AlertsManager;

namespace SampleWPF.ViewModels
{
    public abstract class MainTabBaseViewModel : ViewModelBase
    {
        private readonly AlertsManagerViewModel _alertsManagerViewModel;

        public abstract bool IsTabClosable { get; } // Can tab be closed

        public IClientManager ClientManager { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public override void CleanUp()
        {
            base.CleanUp();

            _alertsManagerViewModel.CleanUp();
        }

        protected MainTabBaseViewModel(IClientManager clientManager)
        {
            ClientManager = clientManager;

            _alertsManagerViewModel = new AlertsManagerViewModel(); // TODO: Use UnityResolve
            AlertsManager = _alertsManagerViewModel;
        }

        public virtual void OnSelected()
        {
            // NOP
        }
    }
}
