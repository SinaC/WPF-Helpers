using System.Collections.Generic;
using System.Windows;

namespace GridAutoFilter
{
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        private readonly MainViewModel _mainViewModel;
        
        public MainWindow2()
        {
            InitializeComponent();

            //Make sample data
            List<Customer> customers = new List<Customer>
                {
                    new Customer
                        {
                            Name = "John Smith",
                            Country = "CA",
                            Other = "A",
                            Number = 5,
                        },
                    new Customer
                        {
                            Name = "Jake Shields",
                            Country = "US",
                            Other = "B",
                            Number = 5,
                        },
                    new Customer
                        {
                            Name = "Shelly Brown",
                            Country = "AU",
                            Other = "C",
                            Number = 3,
                        },
                    new Customer
                        {
                            Name = "Sidney Wright",
                            Country = "CA",
                            Other = "D",
                            Number = 3,
                        },
                    new Customer
                        {
                            Name = "Bart Simpson",
                            Country = "US",
                            Other = "E",
                            Number = 1,
                        }
                };
            //
            _mainViewModel = new MainViewModel();
            _mainViewModel.Initialize(customers);

            //
            DataContext = _mainViewModel;
        }
    }
}
