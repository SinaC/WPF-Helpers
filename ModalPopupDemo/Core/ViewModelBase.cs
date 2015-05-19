using System.ComponentModel;

namespace ModalPopupDemo.Core
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public virtual void CleanUp()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
