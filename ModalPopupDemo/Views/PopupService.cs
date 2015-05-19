using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModalPopupDemo.Core;
using ModalPopupDemo.ViewModels;

namespace ModalPopupDemo.Views
{
    public class PopupService : Canvas, INotifyPropertyChanged, IPopupService
    {
        // When a popup is displayed, disable every previously displayed popup
        private readonly Stack<FrameworkElement> _popups = new Stack<FrameworkElement>();

        public bool NoPopupDisplayed
        {
            get { return _popups.Count == 0; }
        }

        public PopupService()
        {
            Factory.PopupService = this;

            SetZIndex(this, int.MaxValue);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
        }

        public IPopup DisplayModal<T>(T viewModel, string title)
            where T : ViewModelBase
        {
            // Create popup
            ModalPopup popup = new ModalPopup
            {
                DataContext = viewModel,
                Title = title,
            };

            // Display popup
            DisplayPopup(popup);

            //
            return popup;
        }

        // Messages popup (shouldn't be moved)
        public IPopup DisplayMessages(List<string> messages)
        {
            // Create popup
            MessagePopup popup = new MessagePopup
            {
                DataContext = new MessagePopupViewModel
                {
                    Messages = messages
                }
            };

            // Display popup
            DisplayPopup(popup);

            //
            return popup;
        }

        // Question popup (can be moved)
        public IPopup DisplayQuestion(string title, string question, params ActionButton[] actionButtons)
        {
            // Create QuestionPopupViewModel
            QuestionPopupViewModel vm = new QuestionPopupViewModel();
            vm.Initialize(question, actionButtons);

            // Display as modal
            return DisplayModal(vm, title);
        }

        // Move
        public void Move(IPopup popup, double horizontalOffset, double verticalOffset)
        {
            // Search popup
            FrameworkElement frameworkElement = Children.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
            if (frameworkElement != null)
            {
                double top = GetTop(frameworkElement) + verticalOffset;
                double left = GetLeft(frameworkElement) + horizontalOffset;

                // Cannot be moved outside canvas
                top = Math.Min(Math.Max(top, 0), ActualHeight - frameworkElement.ActualHeight);
                left = Math.Min(Math.Max(left, 0), ActualWidth - frameworkElement.ActualWidth);

                SetTop(frameworkElement, top);
                SetLeft(frameworkElement, left);
            }
        }

        // Close
        public void Close(IPopup popup)
        {
            // Search popup
            FrameworkElement frameworkElement = Children.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
            // Close popup
            Close(frameworkElement);
        }

        // Close
        public void Close<T>(T viewModel)
            where T : ViewModelBase
        {
            // Search popup
            FrameworkElement frameworkElement = Children.OfType<ModalPopup>().FirstOrDefault(x => x.DataContext == viewModel);
            // Close popup
            Close(frameworkElement);
        }

        //
        private void Close(FrameworkElement frameworkElement)
        {
            // Close found popup if any
            if (frameworkElement != null)
            {
                // If view model associated to popup, clean it
                ViewModelBase viewModel = frameworkElement.DataContext as ViewModelBase;
                if (viewModel != null)
                    viewModel.CleanUp();
                // Remove popup from canvas
                Children.Remove(frameworkElement);

                // Remove from stack
                _popups.Pop();

                // Enable first popup in stack
                FrameworkElement previous = _popups.Count > 0 ? _popups.Peek() : null;
                if (previous != null)
                    previous.IsEnabled = true;

                //
                OnPropertyChanged("NoPopupDisplayed");
            }
        }

        //
        private void DisplayPopup(FrameworkElement frameworkElement)
        {
            // Add popup to canvas
            Children.Add(frameworkElement);

            // Centered by default
            frameworkElement.Loaded += (sender, args) =>
            {
                SetTop(frameworkElement, (ActualHeight - frameworkElement.ActualHeight) / 2);
                SetLeft(frameworkElement, (ActualWidth - frameworkElement.ActualWidth) / 2);
            };

            // Disable first popup in stack
            FrameworkElement previous = _popups.Count > 0 ? _popups.Peek() : null;
            if (previous != null)
                previous.IsEnabled = false;

            // Add to stack
            _popups.Push(frameworkElement);

            // 
            OnPropertyChanged("NoPopupDisplayed");
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
