using System;
using System.Windows.Controls;
using ModalPopupDemo.Core;

namespace ModalPopupDemo.Views
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// </summary>
    public partial class MessagePopup : UserControl, IPopup
    {
        public Guid Guid { get; private set; }

        public MessagePopup()
        {
            InitializeComponent();

            Guid = Guid.NewGuid();
        }
    }
}
