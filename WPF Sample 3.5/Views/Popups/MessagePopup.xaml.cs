using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Interfaces;

namespace SampleWPF.Views.Popups
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// </summary>
    public partial class MessagePopup : UserControl, IPopup, ISaveNavigationAndFocusPopup
    {
        #region IPopup

        public Guid Guid { get; private set; }

        #endregion

        #region ISaveNavigationAndFocusPopup

        public KeyboardNavigationMode SavedTabNavigationMode { get; set; }

        public KeyboardNavigationMode SavedDirectionalNavigationMode { get; set; }

        public IInputElement SavedFocusedElement { get; set; }

        #endregion

        public MessagePopup()
        {
            InitializeComponent();

            Guid = Guid.NewGuid();
        }
    }
}
