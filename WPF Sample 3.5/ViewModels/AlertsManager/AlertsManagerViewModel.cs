using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;
using SampleWPF.DataContracts;

namespace SampleWPF.ViewModels.AlertsManager
{
    public class AlertsManagerViewModel : ViewModelBase, IAlertsManager
    {
        public ObservableCollection<AlertItem> Alerts { get; private set; }

        public int InfoCount
        {
            get { return Alerts.Count(x => x.Type == AlertItemTypes.Info); }
        }

        public int WarningCount
        {
            get { return Alerts.Count(x => x.Type == AlertItemTypes.Warning); }
        }

        public int ErrorCount
        {
            get { return Alerts.Count(x => x.Type == AlertItemTypes.Error); }
        }

        private ICommand _toggleCommand;
        public ICommand ToggleCommand
        {
            get
            {
                _toggleCommand = _toggleCommand ?? new RelayCommand(Toggle);
                return _toggleCommand;
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(() => IsExpanded, ref _isExpanded, value); }
        }
        
        public int Count
        {
            get { return Alerts.Count; }
        }

        public AlertsManagerViewModel()
        {
            Alerts = new ObservableCollection<AlertItem>();
        }

        public void Add(AlertItem alert)
        {
            Alerts.Add(alert);
            UpdateCounts();
        }
        
        public void Add(List<AlertItem> alerts)
        {
            foreach (AlertItem alert in alerts)
                Alerts.Add(alert);
            UpdateCounts();
        }

        public void Add(AlertData alert)
        {
            Alerts.Add(AlertItem.Map(alert));
            UpdateCounts();
        }

        public void Add(List<AlertData> alerts)
        {
            foreach (AlertData alert in alerts)
                Alerts.Add(AlertItem.Map(alert));
            UpdateCounts();
        }

        public void Clear()
        {
            Alerts.Clear();
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            RaisePropertyChanged(() => InfoCount);
            RaisePropertyChanged(() => WarningCount);
            RaisePropertyChanged(() => ErrorCount);
        }

        private void Toggle()
        {
            IsExpanded = !IsExpanded;
        }
    }

    public class AlertsManagerViewModelDesignData : AlertsManagerViewModel
    {
        public AlertsManagerViewModelDesignData()
        {
            Add(new AlertItem
                {
                    Title = "Info1",
                    Type = AlertItemTypes.Info
                });
            Add(new AlertItem
            {
                Title = "Info2",
                Type = AlertItemTypes.Info
            });
            Add(new AlertItem
            {
                Title = "Info3",
                Type = AlertItemTypes.Info
            });
            Add(new AlertItem
            {
                Title = "Warning1",
                Type = AlertItemTypes.Warning
            });
            Add(new AlertItem
            {
                Title = "Warning2",
                Type = AlertItemTypes.Warning
            });
            Add(new AlertItem
            {
                Title = "Error1",
                Type = AlertItemTypes.Error
            });
            IsExpanded = true;
        }
    }
}
