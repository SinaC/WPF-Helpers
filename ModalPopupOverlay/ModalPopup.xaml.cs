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
        #region IPopup

        public Guid Guid { get; private set; }

        #endregion

        #region IOverlayedPopup

        public KeyboardNavigationMode SavedTabNavigationMode { get; set; }

        public KeyboardNavigationMode SavedDirectionalNavigationMode { get; set; }

        public IInputElement SavedFocusedElement { get; set; }

        #endregion

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

        private IPopupService _popupService;

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
            _popupService = FindParent<IPopupService>(this);
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Search popup service in parent
            IPopupService popupService = _popupService ?? FindParent<IPopupService>(this);
            if (popupService != null)
                popupService.Move(this, e.HorizontalChange, e.VerticalChange);
        }

        private static T FindParent<T>(DependencyObject child)
            where T:class
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) 
                return default(T);

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
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
