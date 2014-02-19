using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomControls
{
    public class MaxRowsDataGrid : DataGrid
    {
        private const double DefaultRowHeight = 20;

        public static readonly DependencyProperty MaxRowsProperty = DependencyProperty.Register(
            "MaxRows", 
            typeof(int?), 
            typeof(MaxRowsDataGrid), 
            new UIPropertyMetadata(MaxRowsPropertyChangedCallback));

        public int? MaxRows
        {
            get { return (int?)GetValue(MaxRowsProperty); }
            set { SetValue(MaxRowsProperty, value); }
        }

        public static void MaxRowsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaxRowsDataGrid @this = d as MaxRowsDataGrid;
            if (@this != null)
                @this.SetMaxHeight();
        }

        private void SetMaxHeight()
        {
            int maxRows = MaxRows.HasValue ? MaxRows.Value : -1;
            if (maxRows > 0)
            {
                // No more rows than items count
                maxRows = maxRows > Items.Count ? Items.Count : maxRows;

                // Get header height
                double headerHeight = Columns.Max(x => GetColumnHeaderFromColumn(x).ActualHeight);

                // Get row height
                DataGridRow row0 = GetRow(0);
                double rowHeight = row0 == null ? DefaultRowHeight : row0.ActualHeight;

                // Compute max grid height
                double maxHeight = rowHeight * maxRows + headerHeight;
                MaxHeight = maxHeight;
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            SetMaxHeight();
        }

        private DataGridRow GetRow(int index)
        {
            if (Items == null || Items.Count <= index)
                return null;
            DataGridRow row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                UpdateLayout();
                ScrollIntoView(Items[index]);
                row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private DataGridColumnHeader GetColumnHeaderFromColumn(DataGridColumn column)
        {
            List<DataGridColumnHeader> columnHeaders = GetVisualChildCollection<DataGridColumnHeader>(this);
            return columnHeaders.FirstOrDefault(columnHeader => Equals(columnHeader.Column, column));
        }

        public static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    visualCollection.Add(child as T);
                else if (child != null)
                    GetVisualChildCollection(child, visualCollection);
            }
        }
    }
}
