using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace DynamicGridView
{
    //http://stackoverflow.com/questions/2643545/wpf-mvvm-how-to-bind-gridviewcolumn-to-viewmodel-collection
    public class GridViewColumnCollectionBehaviour
    {
        private object _columnsSource;
        private readonly GridView _gridView;

        public GridViewColumnCollectionBehaviour(GridView gridView)
        {
            _gridView = gridView;
        }

        public object ColumnsSource
        {
            get { return _columnsSource; }
            set
            {
                object oldValue = _columnsSource;
                _columnsSource = value;
                ColumnsSourceChanged(oldValue, _columnsSource);
            }
        }

        public string DisplayMemberFormatMember { get; set; }

        public string DisplayMemberMember { get; set; }

        public string HeaderTextMember { get; set; }

        public string WidthMember { get; set; }

        private void AddHandlers(ICollectionView collectionView)
        {
            collectionView.CollectionChanged += ColumnsSource_CollectionChanged;
        }

        private void ColumnsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ICollectionView view = sender as ICollectionView;

            if (_gridView == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        GridViewColumn column = CreateColumn(e.NewItems[i]);
                        _gridView.Columns.Insert(e.NewStartingIndex + i, column);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    List<GridViewColumn> columns = new List<GridViewColumn>();

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        GridViewColumn column = _gridView.Columns[e.OldStartingIndex + i];
                        columns.Add(column);
                    }

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        GridViewColumn column = columns[i];
                        _gridView.Columns.Insert(e.NewStartingIndex + i, column);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        _gridView.Columns.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        GridViewColumn column = CreateColumn(e.NewItems[i]);

                        _gridView.Columns[e.NewStartingIndex + i] = column;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _gridView.Columns.Clear();
                    CreateColumns(sender as ICollectionView);
                    break;
            }
        }

        private void ColumnsSourceChanged(object oldValue, object newValue)
        {
            if (_gridView != null)
            {
                _gridView.Columns.Clear();

                if (oldValue != null)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(oldValue);

                    if (view != null)
                    {
                        RemoveHandlers(view);
                    }
                }

                if (newValue != null)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(newValue);

                    if (view != null)
                    {
                        AddHandlers(view);

                        CreateColumns(view);
                    }
                }
            }
        }

        private GridViewColumn CreateColumn(object columnSource)
        {
            GridViewColumn column = new GridViewColumn();

            if (!string.IsNullOrEmpty(HeaderTextMember))
            {
                column.Header = GetPropertyValue(columnSource, HeaderTextMember);
            }

            if (!string.IsNullOrEmpty(DisplayMemberMember))
            {
                string propertyName = GetPropertyValue(columnSource, DisplayMemberMember) as string;

                string format = null;

                if (!string.IsNullOrEmpty(DisplayMemberFormatMember))
                {
                    format = GetPropertyValue(columnSource, DisplayMemberFormatMember) as string;
                }

                if (string.IsNullOrEmpty(format))
                {
                    format = "{0}";
                }

                column.DisplayMemberBinding = new Binding(propertyName)
                    {
                        StringFormat = format
                    };
            }

            if (!string.IsNullOrEmpty(WidthMember))
            {
                double width = (double) GetPropertyValue(columnSource, WidthMember);
                column.Width = width;
            }

            return column;
        }

        private void CreateColumns(ICollectionView collectionView)
        {
            foreach (object item in collectionView)
            {
                GridViewColumn column = CreateColumn(item);

                _gridView.Columns.Add(column);
            }
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            object returnVal = null;

            if (obj != null)
            {
                PropertyInfo prop = obj.GetType().GetProperty(propertyName);

                if (prop != null)
                {
                    returnVal = prop.GetValue(obj, null);
                }
            }

            return returnVal;
        }

        private void RemoveHandlers(ICollectionView collectionView)
        {
            collectionView.CollectionChanged -= ColumnsSource_CollectionChanged;
        }
    }
}
