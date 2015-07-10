/*
    Jarloo
    http://www.jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/ 

    http://www.codeproject.com/Articles/657657/WPF-DataGrid-With-Excel-Style-Column-Filter
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GridAutoFilter
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        private ObservableCollection<CheckedListItem<Customer, string>> countryFilters = new ObservableCollection<CheckedListItem<Customer, string>>();
        private ObservableCollection<CheckedListItem<Customer, string>> otherFilters = new ObservableCollection<CheckedListItem<Customer, string>>();
        private List<ObservableCollection<CheckedListItem<Customer, string>>> filters = new List<ObservableCollection<CheckedListItem<Customer, string>>>();
        private CollectionViewSource viewSource = new CollectionViewSource();

        public MainWindow()
        {
            InitializeComponent();

            //Make sample data
            customers.Add(new Customer
                {
                    Name = "John Smith",
                    Country = "CA",
                    Other = "A"
                });
            customers.Add(new Customer
                {
                    Name = "Jake Shields",
                    Country = "US",
                    Other = "B"
                });
            customers.Add(new Customer
                {
                    Name = "Shelly Brown",
                    Country = "AU",
                    Other = "C"
                });
            customers.Add(new Customer
                {
                    Name = "Sidney Wright",
                    Country = "CA",
                    Other = "D"
                });
            customers.Add(new Customer
                {
                    Name = "Bart Simpson",
                    Country = "US",
                    Other = "E"
                });

            //Create a list of filters
            foreach (string country in customers.Select(w => w.Country).Distinct().OrderBy(w => w))
            {
                //countryFilters.Add(new CheckedListItem<string>(country,true));
                countryFilters.Add(new CheckedListItem<Customer, string>(
                    country,
                    c => c.Country,
                    true));
            }

            //Create a list of filters
            foreach (string other in customers.Select(w => w.Other).Distinct().OrderBy(w => w))
            {
                //otherFilters.Add(new CheckedListItem<string>(other, true));
                otherFilters.Add(new CheckedListItem<Customer, string>(
                    other,
                    c => c.Other,
                    true));
            }

            filters.Add(countryFilters);
            filters.Add(otherFilters);

            lstCountries.ItemsSource = countryFilters;

            viewSource.Filter += viewSource_Filter;
            viewSource.Source = customers;

            grdData.ItemsSource = viewSource.View;

            DataContext = this;
        }

        private void viewSource_Filter(object sender, FilterEventArgs e)
        {
            Customer cust = (Customer) e.Item;

            foreach (ObservableCollection<CheckedListItem<Customer, string>> filter in filters)
            {
                //int count = filter.Where(w => w.IsChecked).Count(w => w.Item == cust.Country);
                int count = filter.Where(w => w.IsChecked).Count(w => w.IsValid(cust));

                if (count == 0)
                {
                    e.Accepted = false;
                    return;
                }
            }
            e.Accepted = true;
        }

        private void btnCountryFilter_Click(object sender, RoutedEventArgs e)
        {
            popCountry.PlacementTarget = btnCountryFilter;
            popCountry.IsOpen = true;
            lstCountries.ItemsSource = countryFilters;
        }

        private void btnOtherFilter_Click(object sender, RoutedEventArgs e)
        {
            popCountry.PlacementTarget = btnOtherFilter;
            popCountry.IsOpen = true;
            lstCountries.ItemsSource = otherFilters;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<CheckedListItem<Customer, string>> filter = lstCountries.ItemsSource as ObservableCollection<CheckedListItem<Customer, string>>;
            foreach (CheckedListItem<Customer, string> item in filter)
            {
                item.IsChecked = true;
            }
        }

        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<CheckedListItem<Customer, string>> filter = lstCountries.ItemsSource as ObservableCollection<CheckedListItem<Customer, string>>;
            foreach (CheckedListItem<Customer, string> item in filter)
            {
                item.IsChecked = false;
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            viewSource.View.Refresh();
        }
    }
}