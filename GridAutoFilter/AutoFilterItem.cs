using System;
using System.ComponentModel;

namespace GridAutoFilter
{
    public class AutoFilterItem<TItem> : INotifyPropertyChanged
    {
        private readonly Action<AutoFilterItem<TItem>, bool> _checkStateChangedCallback;

        private bool _isChecked;
        public bool IsChecked // is filter checked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                    FireAutoFilterItemIsChecked();
                }
            }
        }

        private TItem _item;
        public TItem Item // data to display  TODO: specific format ???
        {
            get { return _item; }
            set
            {
                _item = value;
                OnPropertyChanged("Item");
            }
        }

        public AutoFilterItem(Action<AutoFilterItem<TItem>, bool> checkStateChangedCallback)
        {
            _isChecked = true; // Checked by default
            _checkStateChangedCallback = checkStateChangedCallback;
        }

        private void FireAutoFilterItemIsChecked()
        {
            if (_checkStateChangedCallback != null)
                _checkStateChangedCallback(this, _isChecked);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
