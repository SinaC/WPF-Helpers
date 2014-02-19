using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomControls
{
    public class ManualSelectTabControl : TabControl
    {
        public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
            "SelectCommand", 
            typeof(ICommand), 
            typeof(ManualSelectTabControl), 
            new UIPropertyMetadata(null));

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public ManualSelectTabControl()
        {
            Initialized += OnInitialized;
        }

        private void OnInitialized(object sender, EventArgs eventArgs)
        {
            // Add PreviewMouseLeftButtonDown handler to TabItem
            Style style = ItemContainerStyle;
            if (style == null) // create new style if no style on control
            {
                style = new Style(typeof(TabItem));
                ItemContainerStyle = style;
            }
            style.Setters.Add(new EventSetter(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(TabItemPreviewMouseLeftButtonDown)));
        }

        private void TabItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectCommand == null) // normal behaviour if no SelectCommand
                return;
            // Get TabItem from clicked control
            TabItem newTab = FindFirstParent<TabItem>(sender as FrameworkElement);
            // If DataContext found, execute SelectCommand DataContext
            if (newTab.DataContext != null && !newTab.DataContext.Equals(SelectedItem))
            {
                e.Handled = true;
                SelectCommand.Execute(newTab.DataContext);
            }
        }

        public static T FindFirstParent<T>(FrameworkElement control) where T : FrameworkElement
        {
            if (control == null)
                return null;

            if (control is T)
                return (T)control;

            return FindFirstParent<T>(control.Parent as FrameworkElement);
        }
    }
}
