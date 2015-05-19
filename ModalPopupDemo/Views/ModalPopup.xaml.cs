using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModalPopupDemo.Core;

namespace ModalPopupDemo.Views
{
    /// <summary>
    /// Interaction logic for ModalPopup.xaml
    /// </summary>
    public partial class ModalPopup : UserControl, INotifyPropertyChanged, IPopup
    {
        public Guid Guid { get; private set; }

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

        public ModalPopup()
        {
            InitializeComponent();

            // Inner UserControl will be loaded when setting DataContext

            // Unique id
            Guid = Guid.NewGuid();
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Factory.PopupService.Move(this, e.HorizontalChange, e.VerticalChange);
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
