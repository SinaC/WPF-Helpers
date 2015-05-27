using System.Windows;
using SampleWPF.ViewModels.Login;

namespace SampleWPF.Views.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private bool _isShutdownActiveOnClose; // is closing this window triggering application shutdown or not

        public Login()
        {
            InitializeComponent();

            _isShutdownActiveOnClose = true; // by default, closing window will shutdown application

            LoginViewModel vm = new LoginViewModel();
            vm.LoginSuccessful += OnLoginSuccessful;
            DataContext = vm;
        }


        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            LoginViewModel viewModel = DataContext as LoginViewModel;
            if (viewModel != null)
            {
                viewModel.LoginSuccessful -= OnLoginSuccessful; // resharper warning explanation : http://stackoverflow.com/questions/11180068/delegate-subtraction-has-unpredictable-result-in-resharper-c
                viewModel.CleanUp();
            }

            if (_isShutdownActiveOnClose)
                Application.Current.Shutdown(0);
        }

        // Go to main page when connected
        private void OnLoginSuccessful()
        {
            _isShutdownActiveOnClose = false; // no shutdown on close
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }
    }
}
