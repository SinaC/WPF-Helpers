using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sample.Views
{
    public class LocalViewModelItem
    {
        public string Data1 { get; set; }
        public string Data2 { get; set; }
    }

    public class LocalViewModel : INotifyPropertyChanged
    {
        public List<LocalViewModelItem> Items { get; set; }

        private int _maxRows;
        public int MaxRows
        {
            get
            {
                return _maxRows;
            }
            set
            {
                if (_maxRows != value)
                {
                    _maxRows = value;
                    OnPropertyChanged("MaxRows");
                }
            }
        }

        public LocalViewModel()
        {
            MaxRows = 2;
            Items = new List<LocalViewModelItem>
                {
                    new LocalViewModelItem
                        {
                            Data1 = "11",
                            Data2 = "12"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "21",
                            Data2 = "22"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "31",
                            Data2 = "32"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "41",
                            Data2 = "42"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "51",
                            Data2 = "52"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "61",
                            Data2 = "62"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "71",
                            Data2 = "72"
                        },
                    new LocalViewModelItem
                        {
                            Data1 = "81",
                            Data2 = "82"
                        },
                };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for CustomGridView.xaml
    /// </summary>
    public partial class CustomGridView : UserControl
    {
        public CustomGridView()
        {
            InitializeComponent();

            DataContext = new LocalViewModel();
        }
    }
}
