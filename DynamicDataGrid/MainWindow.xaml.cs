using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            //MainViewModel.Collection.AddRow(new CustomRow(
            //            new Tuple<string, object>("Foo", "Value4"),
            //            new Tuple<string, object>("Bar", false),
            //            new Tuple<string, object>("Order", 4))
            //            {
            //                Status = 4,
            //            });
            CustomRow newRow = new CustomRow();
            newRow.Status = 4;
            newRow.TryAddProperty("Foo", "Value4");
            newRow.TryAddProperty("Bar", false);
            newRow.TryAddProperty("Order", 4);
            MainViewModel.Collection.AddRow(newRow);
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

        private void DynamicDataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            var dataBoundColumn = (DataGridBoundColumn)e.Column;

            if (propertyDescriptor.PropertyType == typeof(int))
            {
                e.Column = new CustomBoundColumn // TODO: value is not updated in DynamicRow
                {
                    TemplateName = "IntTemplate",
                    Header = propertyDescriptor.Name,
                    Binding = dataBoundColumn.Binding,
                    IsReadOnly = propertyDescriptor.IsReadOnly
                };
            }
        }

        public class CustomBoundColumn : DataGridBoundColumn
        {
            public string TemplateName { get; set; }

            protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
                var binding = new Binding(((Binding)Binding).Path.Path)
                {
                    Source = dataItem,
                    Mode =  BindingMode.TwoWay
                };

                var content = new ContentControl
                {
                    ContentTemplate = (DataTemplate) cell.FindResource(TemplateName)
                };
                content.SetBinding(ContentControl.ContentProperty, binding);
                return content;
            }

            protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
            {
                return GenerateElement(cell, dataItem);
            }
        }
    }
}