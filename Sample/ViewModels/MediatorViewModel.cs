using System.Windows;
using System.Windows.Input;
using MVVM;

namespace Sample.ViewModels
{
    public class MediatorViewModel
    {
        public MediatorAViewModel ViewModelA { get; set; }
        public MediatorBViewModel ViewModelB { get; set; }

        public RelayCommand<string> Command1 { get; set; }
        public RelayCommand<MouseButtonEventArgs> Command2 { get; set; }

        public MediatorViewModel()
        {
            ViewModelA = new MediatorAViewModel();
            ViewModelB = new MediatorBViewModel();

            ViewModelA.MyPropertyA = "type here";

            Command1 = new RelayCommand<string>(DoStuff1);
            Command2 = new RelayCommand<MouseButtonEventArgs>(DoStuff2);
        }

        void DoStuff1(string param)
        {
            MessageBox.Show("Command executed. Text is: " + param);
        }

        void DoStuff2(MouseButtonEventArgs param)
        {
            MessageBox.Show("Command executed. ButtonState: " + param.ButtonState);
        }
    }
}
