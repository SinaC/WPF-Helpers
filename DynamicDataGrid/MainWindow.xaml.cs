using System.ComponentModel;
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
        }


        private void DynamicDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DynamicPropertyDescriptor propertyDescriptor = e.PropertyDescriptor as DynamicPropertyDescriptor;
            if (propertyDescriptor != null)
            {
                e.Column.Header = propertyDescriptor.DisplayName ?? propertyDescriptor.Name;
            }
            //double toto = DynamicDataGrid.ActualHeight;
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

        //private void DynamicDataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    var propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
        //    var dataBoundColumn = (DataGridBoundColumn)e.Column;

        //    if (propertyDescriptor.PropertyType == typeof(int))
        //    {
        //        e.Column = new CustomBoundColumn // TODO: value is not updated in DynamicRow
        //        {
        //            TemplateName = "IntTemplate",
        //            Header = propertyDescriptor.Name,
        //            Binding = dataBoundColumn.Binding,
        //            IsReadOnly = propertyDescriptor.IsReadOnly
        //        };
        //    }
        //}

        //public class CustomBoundColumn : DataGridBoundColumn
        //{
        //    public string TemplateName { get; set; }

        //    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        //    {
        //        var binding = new Binding(((Binding)Binding).Path.Path)
        //        {
        //            Source = dataItem,
        //            Mode =  BindingMode.TwoWay
        //        };

        //        var content = new ContentControl
        //        {
        //            ContentTemplate = (DataTemplate) cell.FindResource(TemplateName)
        //        };
        //        content.SetBinding(ContentControl.ContentProperty, binding);
        //        return content;
        //    }

        //    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        //    {
        //        return GenerateElement(cell, dataItem);
        //    }
        //}
    }
}