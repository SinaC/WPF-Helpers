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
using DynamicDataGrid.ViewModels;

namespace DynamicDataGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            MainViewModel = new MainViewModel();

            DataContext = MainViewModel;

            //DynamicDataGrid.ItemsSource = MainViewModel.Collection;
            //StaticDataGrid.ItemsSource = rows;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debugger.Break();

            MainViewModel.Collection.AddRow(new CustomRow(
                        new Tuple<string, object>("Foo", "Value4"),
                        new Tuple<string, object>("Bar", false),
                        new Tuple<string, object>("Order", 4))
                        {
                            Status = 4,
                        });
        }

        private void DynamicDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            double toto = DynamicDataGrid.ActualHeight;
            //if (e.PropertyType == typeof(int) && e.Column is DataGridBoundColumn)
            //{
            //    // TODO: replace with DataGridTemplateColumn including a NumericUpDown control
            //    DataGridComboBoxColumn column = new DataGridComboBoxColumn
            //        {
            //            IsReadOnly = e.Column.IsReadOnly,
            //            Header = e.Column.Header,
            //            SelectedItemBinding = (e.Column as DataGridBoundColumn).Binding,
            //            ItemsSource = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8},
            //        };
            //    e.Column = column;
            //}
            //if (e.Column is DataGridBoundColumn)
            //{
            //    DataGridBoundColumn boundColumn = e.Column as DataGridBoundColumn;
            //    if (boundColumn.Binding is Binding)
            //    {
            //        Binding binding = boundColumn.Binding as Binding;
            //        //binding.ValidatesOnDataErrors = true;
            //        binding.ValidationRules.Clear();
            //        binding.ValidationRules.Add(new ExceptionValidationRule());
            //    }
            //}
        }
    }
}