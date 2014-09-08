using MVVM;

namespace Sample.ViewModels
{
    public class MediatorAViewModel : ViewModelBase
    {
        private string _myPropertyA;
        public string MyPropertyA
        {
            get { return _myPropertyA; }
            set
            {
                if (_myPropertyA != value)
                {
                    _myPropertyA = value;
                    OnPropertyChanged();
                    Mediator.Default.Send(value, "ViewB");
                }
            }
        }

        private bool _isMyPropertyAFocused;
        public bool IsMyPropertyAFocused
        {
            get { return _isMyPropertyAFocused; }
            set
            {
                if (_isMyPropertyAFocused != value)
                {
                    _isMyPropertyAFocused = value;
                    OnPropertyChanged();
                }
            }
        }

        public MediatorAViewModel()
        {
            Mediator.Default.Register<string>(this, "ViewA", DoAction);
        }

        private void DoAction(string param)
        {
            _myPropertyA = param as string;
            OnPropertyChanged("MyPropertyA");
        }
    }
}
