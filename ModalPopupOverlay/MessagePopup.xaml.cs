using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModalPopupOverlay
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// </summary>
    public partial class MessagePopup : UserControl, IPopup, ISaveNavigationAndFocusPopup
    {
        private IPopupService _popupService;

        #region IPopup
        
        public Guid Guid { get; private set; }

        public void Close()
        {
            IPopupService popupService = _popupService ?? VisualHelper.FindParent<IPopupService>(this);
            if (popupService != null)
                popupService.Close(this as IPopup);
        }

        #endregion

        #region ISaveNavigationAndFocusPopup

        public KeyboardNavigationMode SavedTabNavigationMode { get; set; }
        
        public KeyboardNavigationMode SavedDirectionalNavigationMode { get; set; }
        
        public IInputElement SavedFocusedElement { get; set; }

        #endregion

        public MessagePopup()
        {
            InitializeComponent();

            // Unique id
            Guid = Guid.NewGuid();

            //
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _popupService = VisualHelper.FindParent<IPopupService>(this);
        }
    }
}
