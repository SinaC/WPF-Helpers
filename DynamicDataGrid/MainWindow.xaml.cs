//http://stackoverflow.com/questions/882214/data-binding-dynamic-data
//http://www.reimers.dk/jacob-reimers-blog/auto-generating-datagrid-columns-from-dynamicobjects
//http://jopinblog.wordpress.com/2007/04/30/implementing-itypedlist-for-virtual-properties/
//http://jopinblog.wordpress.com/2007/05/12/dynamic-propertydescriptors-with-anonymous-methods/
//http://www.shujaat.net/2012/09/dynamicobject-wpf-binding.html
//http://www.shujaat.net/2012/09/dynamically-generated-properties-using.html
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DynamicDataGrid.DynamicGrid;

namespace DynamicDataGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<DynamicColumn> columns = new List<DynamicColumn>
            {
                new DynamicColumn
                {
                    Name = "Foo",
                    Type = typeof(string),
                    IsReadOnly = false,
                },
                new DynamicColumn
                {
                    Name = "Bar",
                    Type = typeof(bool),
                    IsReadOnly = false,
                },
                new DynamicColumn
                {
                    Name = "Order",
                    Type = typeof(int),
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

            DynamicGrid<CustomRow, DynamicColumn> collection = new DynamicGrid<CustomRow, DynamicColumn>(rows, columns);

            DynamicDataGrid.ItemsSource = collection;
            //StaticDataGrid.ItemsSource = rows;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debugger.Break();
        }

        private void DynamicDataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(int) && e.Column is DataGridBoundColumn)
            {
                // TODO: replace with DataGridTemplateColumn including a NumericUpDown control
                DataGridComboBoxColumn column = new DataGridComboBoxColumn
                    {
                        IsReadOnly = e.Column.IsReadOnly,
                        Header = e.Column.Header,
                        SelectedItemBinding = (e.Column as DataGridBoundColumn).Binding,
                        ItemsSource = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8},
                    };
                e.Column = column;
            }
        }
    }

    public class CustomRow : DynamicRow
    {
        public int Status { get; set; } // will displayed only if a column is specified in DataGrid

        public CustomRow(params Tuple<string, object>[] propertyNames)
            : base(propertyNames)
        {
        }
    }
}