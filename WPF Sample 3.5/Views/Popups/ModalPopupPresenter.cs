using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using SampleWPF.Core;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;
using SampleWPF.ViewModels.Popups;

namespace SampleWPF.Views.Popups
{
    // https://programmingwithpassion.wordpress.com/2012/07/01/displaying-modal-content-in-wpf/

    // ModalPopupPresenter
    //  ModalPopupPresenterPanel (layoutRoot)
    //      ContentPresenter (primaryContentPresenter)
    //      Canvas (inactivePopupsContainer)
    //          UserControl (inactive popups)
    //      Canvas (overlayContainer)
    //          UserControl (active popup)

    [ContentProperty("Content")]
    public class ModalPopupPresenter : FrameworkElement, IPopupService
    {
        private readonly ModalPopupPresenterPanel _layoutRoot; // host to layout the content and modal contents
        private readonly ContentPresenter _primaryContentPresenter; // host primary content

        private readonly Canvas _inactivePopupsContainer; // host inactive popups

        private readonly Canvas _overlayContainer; // covers primary content and inactive popups container, and host active popup

        private readonly List<object> _logicalChildren; // stores primary content + popups (active and inactive)

        private KeyboardNavigationMode _primaryContentPresenterTabNavigationMode; // store primary content TabNavigationMode when overlayed
        private KeyboardNavigationMode _primaryContentPresenterDirectionalNavigationMode; // store primary content DirectionalNavigationMode when overlayed
        private IInputElement _primaryContentPresenterFocusedElement; // store primary content FocusedElement when overlayed

        private static readonly TraversalRequest TraversalDirection;

        #region Content

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(ModalPopupPresenter),
                new UIPropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModalPopupPresenter control = (ModalPopupPresenter)d;

            //If the ModalContentPresenter already contains primary content then
            //the existing content will need to be removed from the logical tree.
            if (e.OldValue != null)
            {
                control.RemoveLogicalChild(e.OldValue);
                control._logicalChildren.Remove(e.OldValue);
            }

            //Add the new content to the logical tree of the ModalContentPresenter
            //and update the logicalChildren array so that the correct element is returned
            //when it is requested by WPF.

            control._primaryContentPresenter.Content = e.NewValue;
            control.AddLogicalChild(e.NewValue);
            control._logicalChildren.Add(e.NewValue);
        }

        // Gets or sets the primary content of the ModalContentPresenter. 
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region OverlayBrush

        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
                "OverlayBrush",
                typeof(Brush),
                typeof(ModalPopupPresenter),
                new UIPropertyMetadata(
                    new SolidColorBrush(Color.FromArgb(204, 169, 169, 169)),
                    OnOverlayBrushChanged));

        private static void OnOverlayBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModalPopupPresenter control = (ModalPopupPresenter)d;
            control._overlayContainer.Background = (Brush)e.NewValue;
        }

        // Gets or sets a brush that describes the overlay that is displayed when the modal content is being shown.
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        #endregion

        #region Ctor

        static ModalPopupPresenter()
        {
            TraversalDirection = new TraversalRequest(FocusNavigationDirection.First);
        }

        public ModalPopupPresenter()
        {
            //
            _layoutRoot = new ModalPopupPresenterPanel();
            _primaryContentPresenter = new ContentPresenter();
            _inactivePopupsContainer = new Canvas();
            _overlayContainer = new Canvas();

            //
            AddVisualChild(_layoutRoot);

            //
            _logicalChildren = new List<object>();

            //
            _inactivePopupsContainer.Background = Brushes.Transparent;
            // no children yet
            _inactivePopupsContainer.Visibility = Visibility.Hidden; // will be visible when there is inactive popup

            _overlayContainer.Background = OverlayBrush;
            // no children yet
            _overlayContainer.Visibility = Visibility.Hidden; // will be visible when there is an active popup

            // 
            _layoutRoot.Children.Add(_primaryContentPresenter);
            _layoutRoot.Children.Add(_inactivePopupsContainer);
            _layoutRoot.Children.Add(_overlayContainer);
        }

        #endregion

        #region IPopupService

        public IPopup DisplayModal<T>(T viewModel, string title, Func<bool> closeConfirmation = null)
            where T : ViewModelBase
        {
            // Create popup
            ModalPopup popup = new ModalPopup
            {
                DataContext = viewModel,
                Title = title,
                CloseConfirmation = closeConfirmation
            };

            // Display popup
            DisplayPopup(popup);

            //
            return popup;
        }
        
        public IPopup DisplayMessages(List<string> messages)
        {
            // Create popup + view model
            MessagePopup popup = new MessagePopup
            {
                DataContext = new MessagePopupViewModel
                {
                    Messages = messages,
                }
            };
            // Display popup
            DisplayPopup(popup);

            //
            return popup;
        }

        public IPopup DisplayQuestion(string title, string question, params ActionButton[] actionButtons)
        {
            // Create QuestionPopupViewModel
            QuestionPopupViewModel vm = new QuestionPopupViewModel(this);
            vm.Initialize(question, actionButtons);

            // Display as modal
            return DisplayModal(vm, title);
        }

        public void Move(IPopup popup, double horizontalOffset, double verticalOffset)
        {
            // Search popup
            FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
            if (frameworkElement != null)
            {
                double top = Canvas.GetTop(frameworkElement) + verticalOffset;
                double left = Canvas.GetLeft(frameworkElement) + horizontalOffset;

                // Cannot be moved outside canvas
                top = Math.Min(Math.Max(top, 0), ActualHeight - frameworkElement.ActualHeight);
                left = Math.Min(Math.Max(left, 0), ActualWidth - frameworkElement.ActualWidth);

                Canvas.SetTop(frameworkElement, top);
                Canvas.SetLeft(frameworkElement, left);
            }
        }

        public void Close(IPopup popup)
        {
            // Search popup
            FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
            // Close popup
            if (frameworkElement != null)
                ClosePopup(frameworkElement);
        }

        public void Close<T>(T viewModel)
            where T : ViewModelBase
        {
            // Search popup
            //FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().Cast<FrameworkElement>().FirstOrDefault(x => x.DataContext == viewModel);
            FrameworkElement frameworkElement = _logicalChildren.OfType<ModalPopup>().FirstOrDefault(x => x.DataContext == viewModel);
            // Close popup
            ClosePopup(frameworkElement);
        }

        #endregion


        private void DisplayPopup(FrameworkElement popup)
        {
            // if a popup is already active
            //      save and disable navigation/focus
            //      save active popup position
            //      remove active popup from overlay
            //      add active popup to inactive popups container
            //      set saved position
            // else
            //      save/disable primary content navigation/focus
            // add popup to overlay
            // add popup to logical children
            // show overlay
            // move focus
            // center popup

            // Check if a popup is active
            if (_overlayContainer.Children.Count > 0) // a popup is already active
            {
                FrameworkElement displayedPopup = _overlayContainer.Children.OfType<FrameworkElement>().Single();
                if (displayedPopup != null) // should never be null
                {
                    // Save/Disable navigation and focus is possible
                    ISaveNavigationAndFocusPopup overlayedPopup = displayedPopup as ISaveNavigationAndFocusPopup;
                    if (overlayedPopup != null)
                    {
                        // Save navigation and focus
                        overlayedPopup.SavedTabNavigationMode = KeyboardNavigation.GetTabNavigation(displayedPopup);
                        overlayedPopup.SavedDirectionalNavigationMode = KeyboardNavigation.GetDirectionalNavigation(displayedPopup);
                        overlayedPopup.SavedFocusedElement = Keyboard.FocusedElement;

                        // Disable navigation
                        KeyboardNavigation.SetTabNavigation(displayedPopup, KeyboardNavigationMode.None);
                        KeyboardNavigation.SetDirectionalNavigation(displayedPopup, KeyboardNavigationMode.None);
                    }

                    // Get popup position
                    double top = Canvas.GetTop(displayedPopup);
                    double left = Canvas.GetLeft(displayedPopup);

                    // Remove active poup from overlay
                    _overlayContainer.Children.Remove(displayedPopup);

                    // Add active popup to inactive popups container
                    _inactivePopupsContainer.Children.Add(displayedPopup);

                    // Show inactive popups container
                    _inactivePopupsContainer.Visibility = Visibility.Visible;

                    // Set popup position
                    Canvas.SetTop(displayedPopup, top);
                    Canvas.SetLeft(displayedPopup, left);
                }
            }
            else // no popup displayed
            {
                // Save primary content navigation and focus
                _primaryContentPresenterTabNavigationMode = KeyboardNavigation.GetTabNavigation(_primaryContentPresenter);
                _primaryContentPresenterDirectionalNavigationMode = KeyboardNavigation.GetDirectionalNavigation(_primaryContentPresenter);
                _primaryContentPresenterFocusedElement = Keyboard.FocusedElement;

                // Disable navigation
                KeyboardNavigation.SetTabNavigation(_primaryContentPresenter, KeyboardNavigationMode.None);
                KeyboardNavigation.SetDirectionalNavigation(_primaryContentPresenter, KeyboardNavigationMode.None);
            }

            // Add popup to overlay
            _overlayContainer.Children.Add(popup);

            // Add to logical children
            _logicalChildren.Add(popup);

            // Show overlay
            _overlayContainer.Visibility = Visibility.Visible;

            // Move focus
            _overlayContainer.MoveFocus(TraversalDirection);

            // Set default position
            popup.Loaded += PopupOnLoaded;
        }

        private void PopupOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // Center popup on first load
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                Canvas.SetTop(frameworkElement, (ActualHeight - frameworkElement.ActualHeight) / 2);
                Canvas.SetLeft(frameworkElement, (ActualWidth - frameworkElement.ActualWidth) / 2);

                //http://stackoverflow.com/questions/3421303/loaded-event-of-a-wpf-user-control-fire-two-times
                //http://stackoverflow.com/questions/2460704/wpf-user-control-loading-twice
                frameworkElement.Loaded -= PopupOnLoaded; // Loaded is called each time a control is added in a canvas
            }
        }

        private void ClosePopup(FrameworkElement popup)
        {
            //  if popup to close is currently displayed
            //      remove it from _overlay and from logical children
            //      if no popup in inactive popups container (there was only one popup)
            //          hide overlay
            //          restore primary content navigation and focus
            //      else
            //          get last popup from inactive popups container
            //          remove it from inactive popups container
            //          move it to overlay
            //          restore navigation and focus
            //          hide inactive popups container if not more inactive popup
            //  else
            //      remove it from inactive popups container and from logical children
            //      hide inactive popups container if not more inactive popup
            //  clean viewmodel if any

            // Check if popup is active
            if (_overlayContainer.Children.Contains(popup)) // popup is active
            {
                // Remove from overlay
                _overlayContainer.Children.Remove(popup);

                // Remove from logical children
                _logicalChildren.Remove(popup);

                // Check if there is an inactive popup
                FrameworkElement inactivePopup = _inactivePopupsContainer.Children.OfType<FrameworkElement>().LastOrDefault();
                if (inactivePopup != null) // inactive popup found
                {
                    // Remove from inactive popups container
                    _inactivePopupsContainer.Children.Remove(inactivePopup);

                    // Add popup to overlay
                    _overlayContainer.Children.Add(inactivePopup);

                    // Restore navigation and focus is possible
                    ISaveNavigationAndFocusPopup overlayedInactive = inactivePopup as ISaveNavigationAndFocusPopup;
                    if (overlayedInactive != null)
                    {
                        // Restore navigation
                        KeyboardNavigation.SetTabNavigation(inactivePopup, overlayedInactive.SavedTabNavigationMode);
                        KeyboardNavigation.SetDirectionalNavigation(inactivePopup, overlayedInactive.SavedDirectionalNavigationMode);

                        // Restore focus
                        Keyboard.Focus(overlayedInactive.SavedFocusedElement);
                    }

                    // Move focus
                    inactivePopup.MoveFocus(TraversalDirection);

                    // Hide inactive popups container if no more inactive popups
                    if (_inactivePopupsContainer.Children.Count == 0)
                        _inactivePopupsContainer.Visibility = Visibility.Hidden;
                }
                else // removed popup was the only one
                {
                    // Hide overlay
                    _overlayContainer.Visibility = Visibility.Hidden;

                    // Restore primary content navigation
                    KeyboardNavigation.SetTabNavigation(_primaryContentPresenter, _primaryContentPresenterTabNavigationMode);
                    KeyboardNavigation.SetDirectionalNavigation(_primaryContentPresenter, _primaryContentPresenterDirectionalNavigationMode);

                    // Restore primary content focus
                    Keyboard.Focus(_primaryContentPresenterFocusedElement);

                    // Move focus
                    _primaryContentPresenter.MoveFocus(TraversalDirection);
                }
            }
            else // popup is inactive
            {
                // Remove from inactive popups container
                _inactivePopupsContainer.Children.Remove(popup);

                // Remove from logical children
                _logicalChildren.Remove(popup);

                // Hide inactive popups container if no more inactive popups
                if (_inactivePopupsContainer.Children.Count == 0)
                    _inactivePopupsContainer.Visibility = Visibility.Hidden;
            }

            // If view model associated to popup, clean it
            ViewModelBase viewModel = popup.DataContext as ViewModelBase;
            if (viewModel != null)
                viewModel.CleanUp();
        }

        #region FrameworkElement overrides

        //These methods are required as a bare minimum for making a custom
        //FrameworkElement render correctly. These methods get the objects which
        //make up the visual and logical tree (the AddVisualChild and AddLogicalChild
        //methods only setup the relationship between the parent/child objects).

        //The Arrange and Measure methods simply delegate to the layoutRoot panel which
        //calculates where any content should be placed.

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index");

            return _layoutRoot;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _layoutRoot.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _layoutRoot.Measure(availableSize);
            return _layoutRoot.DesiredSize;
        }

        #endregion

        #region Layout panel

        // Defines a basic, lightweight layout panel for the ModalPopupPresenter. 
        class ModalPopupPresenterPanel : Panel
        {
            protected override Size MeasureOverride(Size availableSize)
            {
                Size resultSize = new Size(0, 0);

                foreach (UIElement child in Children)
                {
                    child.Measure(availableSize);
                    resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
                    resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
                }

                return resultSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                foreach (UIElement child in InternalChildren)
                {
                    child.Arrange(new Rect(finalSize));
                }

                return finalSize;
            }
        }

        #endregion
    }

    //// ModalPopupPresenter
    ////  ModalPopupPresenterPanel (layoutRoot)
    ////      Canvas (overlayableContainer)
    ////          ContentPresenter (primaryContentPresenter)
    ////          UserControl (inactive popups)
    ////      Canvas (overlay)
    ////          UserControl (active popup)

    //[ContentProperty("Content")]
    //public class ModalPopupPresenter : FrameworkElement, IPopupService
    //{
    //    private readonly ModalPopupPresenterPanel _layoutRoot; // host to layout the content and modal contents
    //    private readonly ContentPresenter _primaryContentPresenter; // host primary content

    //    private readonly Canvas _overlayedContainer; // contains primary content (stored at 0,0) + inactive popups

    //    private readonly Canvas _overlayContainer; // covers primaryContainer

    //    private readonly List<object> _logicalChildren; // stores primary content + popups (active and inactive)

    //    private KeyboardNavigationMode _primaryContentPresenterTabNavigationMode; // store original TabNavigationMode when overlayed
    //    private KeyboardNavigationMode _primaryContentPresenterDirectionalNavigationMode; // store original DirectionalNavigationMode when overlayed
    //    private IInputElement _primaryContentPresenterFocusedElement; // store original FocusedElement when overlayed

    //    private static readonly TraversalRequest TraversalDirection;

    //    #region Content

    //    public static readonly DependencyProperty ContentProperty =
    //        DependencyProperty.Register(
    //            "Content",
    //            typeof (object),
    //            typeof (ModalPopupPresenter),
    //            new UIPropertyMetadata(null, OnContentChanged));

    //    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        ModalPopupPresenter control = (ModalPopupPresenter)d;

    //        /*
    //         * If the ModalContentPresenter already contains primary content then
    //         * the existing content will need to be removed from the logical tree.
    //         */
    //        if (e.OldValue != null)
    //        {
    //            control.RemoveLogicalChild(e.OldValue);
    //            control._logicalChildren.Remove(e.OldValue);
    //        }

    //        /*
    //         * Add the new content to the logical tree of the ModalContentPresenter
    //         * and update the logicalChildren array so that the correct element is returned
    //         * when it is requested by WPF.
    //         */
    //        control._primaryContentPresenter.Content = e.NewValue;
    //        control.AddLogicalChild(e.NewValue);
    //        control._logicalChildren.Add( e.NewValue);
    //    }

    //    // Gets or sets the primary content of the ModalContentPresenter. 
    //    public object Content
    //    {
    //        get { return GetValue(ContentProperty); }
    //        set { SetValue(ContentProperty, value); }
    //    }

    //    #endregion

    //    #region OverlayBrush

    //    public static readonly DependencyProperty OverlayBrushProperty =
    //        DependencyProperty.Register(
    //            "OverlayBrush",
    //            typeof (Brush),
    //            typeof(ModalPopupPresenter),
    //            new UIPropertyMetadata(
    //                new SolidColorBrush(Color.FromArgb(204, 169, 169, 169)),
    //                OnOverlayBrushChanged));

    //    private static void OnOverlayBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        ModalPopupPresenter control = (ModalPopupPresenter)d;
    //        control._overlayContainer.Background = (Brush)e.NewValue;
    //    }

    //    // Gets or sets a brush that describes the overlay that is displayed when the modal content is being shown.
    //    public Brush OverlayBrush
    //    {
    //        get { return (Brush)GetValue(OverlayBrushProperty); }
    //        set { SetValue(OverlayBrushProperty, value); }
    //    }

    //    #endregion


    //    #region Ctor

    //    static ModalPopupPresenter()
    //    {
    //        TraversalDirection = new TraversalRequest(FocusNavigationDirection.First);
    //    }

    //    // NOT WORKING VERSION
    //    public ModalPopupPresenter()
    //    {
    //        _layoutRoot = new ModalPopupPresenterPanel();
    //        _primaryContentPresenter = new ContentPresenter();
    //        _overlayedContainer = new Canvas();
    //        _overlayContainer = new Canvas();

    //        AddVisualChild(_layoutRoot);

    //        _logicalChildren = new List<object>();

    //        _overlayedContainer.Background = Brushes.Transparent;
    //        _overlayedContainer.Children.Add(_primaryContentPresenter);
    //        _overlayedContainer.Visibility = Visibility.Visible;

    //        _overlayContainer.Background = OverlayBrush;
    //        // no children yet
    //        _overlayContainer.Visibility = Visibility.Hidden;

    //        _layoutRoot.Children.Add(_overlayedContainer);
    //        _layoutRoot.Children.Add(_overlayContainer);
    //    }

    //    // ALMOST WORKING VERSION
    //    //public ModalPopupPresenter()
    //    //{
    //    //    _layoutRoot = new ModalPopupPresenterPanel();
    //    //    _primaryContentPresenter = new ContentPresenter();
    //    //    _overlayedContainer = new Canvas();
    //    //    _overlayContainer = new Canvas();

    //    //    AddVisualChild(_layoutRoot);

    //    //    _logicalChildren = new List<object>();

    //    //    _overlayedContainer.Background = Brushes.Transparent;
    //    //    //_overlayedContainer.Children.Add(_primaryContentPresenter);
    //    //    _overlayedContainer.Visibility = Visibility.Hidden;

    //    //    _overlayContainer.Background = OverlayBrush;
    //    //    // no children yet
    //    //    _overlayContainer.Visibility = Visibility.Hidden;

    //    //    _layoutRoot.Children.Add(_primaryContentPresenter);
    //    //    _layoutRoot.Children.Add(_overlayedContainer);
    //    //    _layoutRoot.Children.Add(_overlayContainer);
    //    //}

    //    #endregion

    //    #region IPopupService

    //    public IPopup DisplayModal<T>(T viewModel, string title, Func<bool> closeConfirmation = null)
    //        where T : ViewModelBase
    //    {
    //        // Create popup
    //        ModalPopup popup = new ModalPopup
    //        {
    //            DataContext = viewModel,
    //            Title = title,
    //            CloseConfirmation = closeConfirmation
    //        };

    //        // Display popup
    //        DisplayPopup(popup);

    //        //
    //        return popup;
    //    }

    //    public IPopup DisplayMessages(List<string> messages)
    //    {
    //        // Create popup + view model
    //        MessagePopup popup = new MessagePopup
    //            {
    //                DataContext = new MessagePopupViewModel
    //                    {
    //                        Messages = messages,
    //                    }
    //            };
    //        // Display popup
    //        DisplayPopup(popup);

    //        //
    //        return popup;
    //    }

    //    public IPopup DisplayQuestion(string title, string question, params ActionButton[] actionButtons)
    //    {
    //        // Create QuestionPopupViewModel
    //        QuestionPopupViewModel vm = new QuestionPopupViewModel
    //            {
    //                PopupService = this
    //            };
    //        vm.Initialize(question, actionButtons);

    //        // Display as modal
    //        return DisplayModal(vm, title);
    //    }

    //    public void Move(IPopup popup, double horizontalOffset, double verticalOffset)
    //    {
    //        // Search popup
    //        FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
    //        if (frameworkElement != null)
    //        {
    //            double top = Canvas.GetTop(frameworkElement) + verticalOffset;
    //            double left = Canvas.GetLeft(frameworkElement) + horizontalOffset;

    //            // Cannot be moved outside canvas
    //            top = Math.Min(Math.Max(top, 0), ActualHeight - frameworkElement.ActualHeight);
    //            left = Math.Min(Math.Max(left, 0), ActualWidth - frameworkElement.ActualWidth);

    //            Canvas.SetTop(frameworkElement, top);
    //            Canvas.SetLeft(frameworkElement, left);
    //        }
    //    }

    //    public void Close(IPopup popup)
    //    {
    //        // Search popup
    //        FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().FirstOrDefault(x => x.Guid == popup.Guid) as FrameworkElement;
    //        // Close popup
    //        if (frameworkElement != null)
    //            ClosePopup(frameworkElement);
    //    }

    //    public void Close<T>(T viewModel) 
    //        where T : ViewModelBase
    //    {
    //        // Search popup
    //        //FrameworkElement frameworkElement = _logicalChildren.OfType<IPopup>().Cast<FrameworkElement>().FirstOrDefault(x => x.DataContext == viewModel);
    //        FrameworkElement frameworkElement = _logicalChildren.OfType<ModalPopup>().FirstOrDefault(x => x.DataContext == viewModel);
    //        // Close popup
    //        ClosePopup(frameworkElement);
    //    }

    //    #endregion


    //    private void DisplayPopup(FrameworkElement popup)
    //    {
    //        // if a popup is active
    //        //      save and disable navigation/focus
    //        //      save active popup position
    //        //      remove active popup from overlay
    //        //      add active popup to primary container
    //        //      set saved position
    //        // else
    //        //      save/disable primary content presenter navigation/focus
    //        // add popup to overlay
    //        // add popup to logical children
    //        // show overlay
    //        // move focus
    //        // center popup

    //        // Check if a popup is active
    //        if (_overlayContainer.Children.Count > 0) // a popup is already active
    //        {
    //            FrameworkElement displayedPopup = _overlayContainer.Children.OfType<FrameworkElement>().Single();
    //            if (displayedPopup != null) // should never be null
    //            {
    //                // Save/Disable navigation and focus is possible
    //                ISaveNavigationAndFocusPopup overlayedPopup = displayedPopup as ISaveNavigationAndFocusPopup;
    //                if (overlayedPopup != null)
    //                {
    //                    // Save navigation and focus
    //                    overlayedPopup.SavedTabNavigationMode = KeyboardNavigation.GetTabNavigation(displayedPopup);
    //                    overlayedPopup.SavedDirectionalNavigationMode = KeyboardNavigation.GetDirectionalNavigation(displayedPopup);
    //                    overlayedPopup.SavedFocusedElement = Keyboard.FocusedElement;

    //                    // Disable navigation
    //                    KeyboardNavigation.SetTabNavigation(displayedPopup, KeyboardNavigationMode.None);
    //                    KeyboardNavigation.SetDirectionalNavigation(displayedPopup, KeyboardNavigationMode.None);
    //                }

    //                // Get popup position
    //                double top = Canvas.GetTop(displayedPopup);
    //                double left = Canvas.GetLeft(displayedPopup);

    //                // Remove active poup from overlay
    //                _overlayContainer.Children.Remove(displayedPopup);

    //                // Add active popup to primary container
    //                _overlayedContainer.Children.Add(displayedPopup);

    //                // Set popup position
    //                Canvas.SetTop(displayedPopup, top);
    //                Canvas.SetLeft(displayedPopup, left);
    //            }
    //        }
    //        else // no popup displayed
    //        {
    //            // Save primary content navigation and focus
    //            _primaryContentPresenterTabNavigationMode = KeyboardNavigation.GetTabNavigation(_primaryContentPresenter);
    //            _primaryContentPresenterDirectionalNavigationMode = KeyboardNavigation.GetDirectionalNavigation(_primaryContentPresenter);
    //            _primaryContentPresenterFocusedElement = Keyboard.FocusedElement;

    //            // Disable navigation
    //            KeyboardNavigation.SetTabNavigation(_primaryContentPresenter, KeyboardNavigationMode.None);
    //            KeyboardNavigation.SetDirectionalNavigation(_primaryContentPresenter, KeyboardNavigationMode.None);
    //        }

    //        // Add popup to overlay
    //        _overlayContainer.Children.Add(popup);

    //        // Add to logical children
    //        _logicalChildren.Add(popup);

    //        // Show overlay
    //        _overlayContainer.Visibility = Visibility.Visible;

    //        // Move focus
    //        _overlayContainer.MoveFocus(TraversalDirection);

    //        // Set default position
    //        popup.Loaded += PopupOnLoaded;
    //    }
        
    //    private void PopupOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    //    {
    //        // Center popup on first load
    //        FrameworkElement frameworkElement = sender as FrameworkElement;
    //        if (frameworkElement != null)
    //        {
    //            Canvas.SetTop(frameworkElement, (ActualHeight - frameworkElement.ActualHeight) / 2);
    //            Canvas.SetLeft(frameworkElement, (ActualWidth - frameworkElement.ActualWidth) / 2);

    //            //http://stackoverflow.com/questions/3421303/loaded-event-of-a-wpf-user-control-fire-two-times
    //            //http://stackoverflow.com/questions/2460704/wpf-user-control-loading-twice
    //            frameworkElement.Loaded -= PopupOnLoaded; // Loaded is called each time a control is added in a canvas
    //        }
    //    }

    //    private void ClosePopup(FrameworkElement popup)
    //    {
    //        //  if popup to close is currently displayed
    //        //      remove it from _overlay and from logical children
    //        //      if no popup in _primaryContainer (there was only one popup)
    //        //          hide overlay
    //        //          restore _primaryContentPresenter navigation and focus
    //        //      else
    //        //          get last popup from _primaryContainer
    //        //          remove it from _primaryContainer
    //        //          move it to overlay
    //        //          restore navigation and focus
    //        //  else
    //        //      remove it from _primaryContainer and from logical children
    //        //  clean viewmodel if any

    //        // Check if popup is active
    //        if (_overlayContainer.Children.Contains(popup)) // popup is active
    //        {
    //            // Remove from overlay
    //            _overlayContainer.Children.Remove(popup);
    //            // Remove from logical children
    //            _logicalChildren.Remove(popup);

    //            // Check if there is an inactive popup
    //            FrameworkElement inactivePopup = _overlayedContainer.Children.OfType<FrameworkElement>().LastOrDefault(x => !x.Equals(_primaryContentPresenter));
    //            if (inactivePopup != null) // inactive popup found
    //            {
    //                // Remove from inactive popups
    //                _overlayedContainer.Children.Remove(inactivePopup);

    //                // Add popup to overlay
    //                _overlayContainer.Children.Add(inactivePopup);

    //                // Restore navigation and focus is possible
    //                ISaveNavigationAndFocusPopup overlayedInactive = inactivePopup as ISaveNavigationAndFocusPopup;
    //                if (overlayedInactive != null)
    //                {
    //                    // Restore navigation
    //                    KeyboardNavigation.SetTabNavigation(inactivePopup, overlayedInactive.SavedTabNavigationMode);
    //                    KeyboardNavigation.SetDirectionalNavigation(inactivePopup, overlayedInactive.SavedDirectionalNavigationMode);

    //                    // Restore focus
    //                    Keyboard.Focus(overlayedInactive.SavedFocusedElement);
    //                }

    //                // Move focus
    //                inactivePopup.MoveFocus(TraversalDirection);
    //            }
    //            else // removed popup was the only one
    //            {
    //                // Hide overlay
    //                _overlayContainer.Visibility = Visibility.Hidden;

    //                // Restore navigation
    //                KeyboardNavigation.SetTabNavigation(_primaryContentPresenter, _primaryContentPresenterTabNavigationMode);
    //                KeyboardNavigation.SetDirectionalNavigation(_primaryContentPresenter, _primaryContentPresenterDirectionalNavigationMode);

    //                // Restore focus
    //                Keyboard.Focus(_primaryContentPresenterFocusedElement);

    //                // Move focus
    //                _primaryContentPresenter.MoveFocus(TraversalDirection);
    //            }
    //        }
    //        else // popup is inactive
    //        {
    //            // Remove from primary container
    //            _overlayedContainer.Children.Remove(popup);
    //            // Remove from logical children
    //            _logicalChildren.Remove(popup);
    //        }

    //        // If view model associated to popup, clean it
    //        ViewModelBase viewModel = popup.DataContext as ViewModelBase;
    //        if (viewModel != null)
    //            viewModel.CleanUp();
    //    }

    //    #region FrameworkElement overrides

    //    /*
    //     * These methods are required as a bare minimum for making a custom
    //     * FrameworkElement render correctly. These methods get the objects which
    //     * make up the visual and logical tree (the AddVisualChild and AddLogicalChild
    //     * methods only setup the relationship between the parent/child objects).
    //     * 
    //     * The Arrange and Measure methods simply delegate to the layoutRoot panel which
    //     * calculates where any content should be placed.
    //     */

    //    protected override Visual GetVisualChild(int index)
    //    {
    //        if (index < 0 || index > 1)
    //            throw new ArgumentOutOfRangeException("index");

    //        return _layoutRoot;
    //    }

    //    protected override int VisualChildrenCount
    //    {
    //        get { return 1; }
    //    }

    //    protected override IEnumerator LogicalChildren
    //    {
    //        get { return _logicalChildren.GetEnumerator(); }
    //    }

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        _layoutRoot.Arrange(new Rect(finalSize));
    //        return finalSize;
    //    }

    //    protected override Size MeasureOverride(Size availableSize)
    //    {
    //        _layoutRoot.Measure(availableSize);
    //        return _layoutRoot.DesiredSize;
    //    }

    //    #endregion

    //    #region Layout panel

    //    // Defines a basic, lightweight layout panel for the ModalPopupPresenter. 
    //    class ModalPopupPresenterPanel : Panel
    //    {
    //        protected override Size MeasureOverride(Size availableSize)
    //        {
    //            Size resultSize = new Size(0, 0);

    //            foreach (UIElement child in Children)
    //            {
    //                child.Measure(availableSize);
    //                resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
    //                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
    //            }

    //            return resultSize;
    //        }

    //        protected override Size ArrangeOverride(Size finalSize)
    //        {
    //            foreach (UIElement child in InternalChildren)
    //            {
    //                child.Arrange(new Rect(finalSize));
    //            }

    //            return finalSize;
    //        }
    //    }

    //    #endregion
    //}
}
