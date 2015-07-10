using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace GridAutoFilter
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CollectionViewSource _viewSource;

        private List<Customer> _customers;

        public AutoFilterColumnHeaderViewModel<Customer, string> CountryAutoFilter { get; private set; }
        public AutoFilterColumnHeaderViewModel<Customer, string> OtherAutoFilter { get; private set; }
        public AutoFilterColumnHeaderViewModel<Customer, int> NumberAutoFilter { get; private set; }

        public ICollectionView Items
        {
            get { return _viewSource.View; }
        }

        public MainViewModel()
        {
            _viewSource = new CollectionViewSource();
            _viewSource.Filter += ViewSourceFilter;
        }

        public void Initialize(List<Customer> customers)
        {
            // Country auto filter
            CountryAutoFilter = new AutoFilterColumnHeaderViewModel<Customer, string>(
                customer => customer.Country,
                CountryAutoFilterOnAutoFilterModified,
                customers
                    .Select(x => x.Country)
                    .Distinct())
                {
                    Header = "Country",
                };

            // Other auto filter
            OtherAutoFilter = new AutoFilterColumnHeaderViewModel<Customer, string>(
                customer => customer.Other,
                OtherAutoFilterOnAutoFilterModified,
                customers
                    .Select(x => x.Other)
                    .Distinct())
                {
                    Header = "Other",
                };

            // Number auto filter
            NumberAutoFilter = new AutoFilterColumnHeaderViewModel<Customer, int>(
                customer => customer.Number,
                NumberAutoFilterOnAutoFilterModified,
                customers
                    .Select(x => x.Number)
                    .Distinct())
                {
                    Header = "Number",
                };

            _customers = customers;
            _viewSource.Source = _customers;
        }

        private void CountryAutoFilterOnAutoFilterModified(AutoFilterColumnHeaderViewModel<Customer, string> column)
        {
            ApplyFilters();
        }

        private void OtherAutoFilterOnAutoFilterModified(AutoFilterColumnHeaderViewModel<Customer, string> column)
        {
            ApplyFilters();
        }

        private void NumberAutoFilterOnAutoFilterModified(AutoFilterColumnHeaderViewModel<Customer, int> column)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            _viewSource.View.Refresh();
        }

        private void ViewSourceFilter(object sender, FilterEventArgs e)
        {
            Customer cust = (Customer) e.Item;

            bool isFiltered = CountryAutoFilter.IsFiltered(cust) || OtherAutoFilter.IsFiltered(cust) || NumberAutoFilter.IsFiltered(cust); // TODO: loop or reflection

            if (isFiltered)
            {
                e.Accepted = false;
                return;
            }

            e.Accepted = true;
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

    public class MainViewModelDesignData : MainViewModel
    {
        public MainViewModelDesignData()
        {
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
            Initialize(customers);
        }
    }
}
