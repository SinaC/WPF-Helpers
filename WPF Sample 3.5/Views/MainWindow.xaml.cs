using System.Windows;
using SampleWPF.Core;
using SampleWPF.DataContracts;
using SampleWPF.Utility;
using SampleWPF.ViewModels;
using SampleWPF.ViewModels.DisplayClient;

namespace SampleWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel vm = new MainViewModel();
            DataContext = vm;

            UIRepository.PopupService = ModalPopupPresenter; // !!! without this line PopupService would be unaccessible

            // Dummy datas
            DisplayClientTabViewModel mainClient = new DisplayClientTabViewModel(vm, "200000125", true);
            mainClient.AlertsManager.Add(new AlertData
                {
                    Title = "Error1",
                    Type = AlertDataTypes.Error
                });
            mainClient.AlertsManager.Add(new AlertData
            {
                Title = "Error2",
                Type = AlertDataTypes.Error
            });
            DisplayClientTabViewModel secondaryClient = new DisplayClientTabViewModel(vm, "200000126", false);
            secondaryClient.AlertsManager.Add(new AlertData
            {
                Title = "Warning1",
                Type = AlertDataTypes.Warning
            });
            secondaryClient.AlertsManager.Add(new AlertData
            {
                Title = "Warning2",
                Type = AlertDataTypes.Warning
            });
            secondaryClient.AlertsManager.Add(new AlertData
            {
                Title = "Warning3",
                Type = AlertDataTypes.Warning
            });
            vm.MainTabControlViewModel.AddTab(mainClient);
            vm.MainTabControlViewModel.AddTab(secondaryClient);
        }

        // Go to login page when closed
        protected override void OnClosed(System.EventArgs e)
        {
            if (Settings.DisplayLogin)
            {
                Login.Login loginWindow = new Login.Login();
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();
                this.Close();
            }

            MainViewModel viewModel = DataContext as MainViewModel;
            if (viewModel != null)
                viewModel.CleanUp();
        }
    }
}
