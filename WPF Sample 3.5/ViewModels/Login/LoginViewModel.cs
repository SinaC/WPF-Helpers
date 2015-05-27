using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.Core.MVVM;

namespace SampleWPF.ViewModels.Login
{
    public class LoginViewModel : ViewModelBase
    {
        public delegate void LoginSuccessfulEventHandler();

        public LoginSuccessfulEventHandler LoginSuccessful;

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                _loginCommand = _loginCommand ?? new RelayCommand(Login);
                return _loginCommand;
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { Set(() => UserName, ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { Set(() => Password, ref _password, value); }
        }

        private void Login()
        {
            // TODO: check login/password
            if (LoginSuccessful != null)
                LoginSuccessful();
        }
    }

    public class LoginViewModelDesignData : LoginViewModel
    {
    }
}
