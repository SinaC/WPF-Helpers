using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace GridAutoFilter
{
    public class AutoFilterColumnHeaderViewModel<TRow, TItem> : INotifyPropertyChanged
        where TItem : IEquatable<TItem>
    {
        private bool _isAutoFilterModifiedForbidden; // avoid raising multiple AutoFilterModified events when SelectAll/UnselectAll is used

        private readonly Func<TRow, TItem> _getItemFunc; // function to get item from row
        private readonly Action<AutoFilterColumnHeaderViewModel<TRow, TItem>> _filterModifiedCallback; // callback raised when a filter is modified

        public List<AutoFilterItem<TItem>> Items { get; private set; }

        private ICommand _filterClickCommand;
        public ICommand FilterClickCommand // triggered when user click on filter button to open filter popup
        {
            get
            {
                _filterClickCommand = _filterClickCommand ?? new RelayCommand(FilterClick);
                return _filterClickCommand;
            }
        }

        private ICommand _selectAllCommand;
        public ICommand SelectAllCommand // select all filters
        {
            get
            {
                _selectAllCommand = _selectAllCommand ?? new RelayCommand(() => ChangeAllCheckedState(true));
                return _selectAllCommand;
            }
        }

        private ICommand _unselectAllCommand;
        public ICommand UnselectAllCommand // unselect all filters
        {
            get
            {
                _unselectAllCommand = _unselectAllCommand ?? new RelayCommand(() => ChangeAllCheckedState(false));
                return _unselectAllCommand;
            }
        }

        private string _header;
        public string Header // column header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged("Header");
                }
            }
        }

        private bool _isFilterOpened;
        public bool IsFilterOpened // is filter popup opened?
        {
            get { return _isFilterOpened; }
            set
            {
                if (_isFilterOpened != value)
                {
                    _isFilterOpened = value;
                    OnPropertyChanged("IsFilterOpened");
                }
            }
        }

        public bool IsAnyFilterUnselected // Is there a filter unselected
        {
            get { return Items.Any(x => !x.IsChecked); }
        }

        public AutoFilterColumnHeaderViewModel(Func<TRow, TItem> getItemFunc, Action<AutoFilterColumnHeaderViewModel<TRow, TItem>> filterModifiedCallback, IEnumerable<TItem> items)
        {
            _isAutoFilterModifiedForbidden = false;
            _getItemFunc = getItemFunc;
            _filterModifiedCallback = filterModifiedCallback;

            Items = items.Select(x => new AutoFilterItem<TItem>(OnAutoFilterItemIsChecked)
                {
                    Item = x
                }).ToList();
        }

        // Returns true if Row.Item is rejected, false if accepted
        public bool IsFiltered(TRow row)
        {
            TItem item = _getItemFunc(row);
            return Items.Any(x => x.Item.Equals(item) && !x.IsChecked);
        }

        private void ChangeAllCheckedState(bool isChecked)
        {
            _isAutoFilterModifiedForbidden = true;
            foreach (AutoFilterItem<TItem> item in Items)
                item.IsChecked = isChecked;
            _isAutoFilterModifiedForbidden = false;
            FireOnFilterModified();
        }

        private void OnAutoFilterItemIsChecked(AutoFilterItem<TItem> item, bool isChecked)
        {
            FireOnFilterModified();
        }

        private void FireOnFilterModified()
        {
            if (!_isAutoFilterModifiedForbidden && _filterModifiedCallback != null)
            {
                _filterModifiedCallback(this);
                OnPropertyChanged("IsAnyFilterUnselected");
            }
        }

        private void FilterClick()
        {
            IsFilterOpened = true;
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

    public class AutoFilterColumnHeaderViewModelDesignData : AutoFilterColumnHeaderViewModel<string, string>
    {
        public AutoFilterColumnHeaderViewModelDesignData()
            : base(
                s => s,
                model => { },
                new[]
                    {
                        "Item1", "Item2", "Item3", "Item4"
                    })
        {
            Header = "Header 1";
        }
    }
}
