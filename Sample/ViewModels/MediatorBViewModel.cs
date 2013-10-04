using MVVM;

namespace Sample.ViewModels
{
    public class MediatorBViewModel : ViewModelBase
    {
        private string _myPropertyB;
        public string MyPropertyB
        {
            get { return _myPropertyB; }
            set
            {
                if (_myPropertyB != value)
                {
                    _myPropertyB = value;
                    OnPropertyChanged();
                    Mediator.Send(value, "ViewA");
                }
            }
        }

        public MediatorBViewModel()
        {
            Mediator.Register<string>(this, "ViewB", DoAction);
        }

        private void DoAction(string param)
        {
            _myPropertyB = param as string;
            OnPropertyChanged("MyPropertyB");
        }
    }
}