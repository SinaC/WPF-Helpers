using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModalPopupOverlay.ViewModels;

namespace ModalPopupOverlay
{
    /// <summary>
    /// Interaction logic for ModalPopup.xaml
    /// </summary>
    public partial class ModalPopup : UserControl, INotifyPropertyChanged, IPopup, ISaveNavigationAndFocusPopup
    {
        private IPopupService _popupService;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

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
        
        public ModalPopup()
        {
            InitializeComponent();

            // Inner UserControl will be loaded when setting DataContext

            // Unique id
            Guid = Guid.NewGuid();

            //
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _popupService = VisualHelper.FindParent<IPopupService>(this);
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Search popup service in parent
            IPopupService popupService = _popupService ?? VisualHelper.FindParent<IPopupService>(this);
            if (popupService != null)
                popupService.Move(this, e.HorizontalChange, e.VerticalChange);
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
}
