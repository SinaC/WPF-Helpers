using System;
using System.Collections.Generic;
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
using Sample.ViewModels;

namespace Sample.Views
{
    /// <summary>
    /// Interaction logic for MediatorView.xaml
    /// </summary>
    public partial class MediatorView : UserControl
    {
        public MediatorView()
        {
            InitializeComponent();

            DataContext = new MediatorViewModel();
        }
    }
}
