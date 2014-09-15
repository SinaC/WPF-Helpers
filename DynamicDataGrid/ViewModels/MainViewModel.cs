using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DynamicDataGrid.DynamicGrid;

namespace DynamicDataGrid.ViewModels
{
    public class CustomRow : DynamicRow
    {
        private int _status;
        public int Status
        {
            get { return _status; }
            set { Set(() => Status, ref _status, value); }
        }

        public CustomRow(params Tuple<string, object>[] propertyNames)
            : base(propertyNames)
        {
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private DynamicGrid<CustomRow, DynamicColumn> _collection;
        public DynamicGrid<CustomRow, DynamicColumn> Collection
        {
            get { return _collection; }
            set
            {
                if (_collection != value)
                {
                    _collection = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ICommand _addRowsCommand;
        public ICommand AddRowsCommand
        {
            get
            {
                _addRowsCommand = _addRowsCommand ?? new RelayCommand(AddRows);
                return _addRowsCommand;
            }
        }

        public MainViewModel()
        {
            List<DynamicColumn> columns = new List<DynamicColumn>
                {
                    new DynamicColumn
                        {
                            Name = "Foo",
                            Type = typeof (string),
                            IsReadOnly = false,
                        },
                    new DynamicColumn
                        {
                            Name = "Bar",
                            Type = typeof (bool),
                            IsReadOnly = false,
                        },
                    new DynamicColumn
                        {
                            Name = "Order",
                            Type = typeof (int),
                            IsReadOnly = false,
                        },
                };

            List<CustomRow> rows = new List<CustomRow>
                {
                    new CustomRow(
                        new Tuple<string, object>("Foo", "Value1"),
                        new Tuple<string, object>("Bar", true),
                        new Tuple<string, object>("Order", 1))
                        {
                            Status = 2,
                        },
                    new CustomRow(
                        new Tuple<string, object>("Foo", "Value2"),
                        new Tuple<string, object>("Bar", false),
                        new Tuple<string, object>("Order", 2))
                        {
                            Status = 3,
                        },
                    new CustomRow(
                        new Tuple<string, object>("Foo", "Value3"),
                        new Tuple<string, object>("Bar", true),
                        new Tuple<string, object>("Order", 3))
                        {
                            Status = 4,
                        }
                };

            Collection = new DynamicGrid<CustomRow, DynamicColumn>(rows, columns);
        }

        private void AddRows()
        {
            CustomRow newRow = new CustomRow();
            newRow.Status = 4;
            newRow.TryAddProperty("Foo", "Value4");
            newRow.TryAddProperty("Bar", false);
            newRow.TryAddProperty("Order", 4);
            Collection.AddRow(newRow);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
