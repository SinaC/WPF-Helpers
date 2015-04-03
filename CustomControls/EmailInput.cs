using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NAllo.Arche.UI.Views.Common
{
    // Editable combobox with preselected domain list displayed when dropdown is opened
    public class EmailInput : ComboBox
    {
        private static readonly string[] DefaultDomainList =
            {
                "gmail.com",
                "yahoo.fr",
                "hotmail.fr"
            };

        //
        public static readonly DependencyProperty AvailableItemsProperty =
            DependencyProperty.Register("DomainList",
                                        typeof(List<string>),
                                        typeof(EmailInput),
                                        new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                                        {
                                            BindsTwoWayByDefault = false
                                        });

        public List<string> DomainList
        {
            get { return (List<string>)GetValue(AvailableItemsProperty); }
            set { SetValue(AvailableItemsProperty, value); }
        }

        //
        public static readonly DependencyProperty EmailValueProperty =
            DependencyProperty.Register("EmailValue",
                                        typeof(string),
                                        typeof(EmailInput),
                                        new PropertyMetadata(default(string)));

        public string EmailValue
        {
            get { return (string)GetValue(EmailValueProperty); }
            set { SetValue(EmailValueProperty, value); }
        }

        //
        private TextBox _editableTextBox;
        private ToggleButton _dropDownButton;

        public EmailInput()
        {
            IsEditable = true;
            DomainList = DefaultDomainList.ToList();
        }

        public static void OnAvailableItemsChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            // Refresh domain list
            EmailInput ctrl = sender as EmailInput;
            if (ctrl != null)
                ctrl.AppendPreDomainToDomainList();
        }

        public override void OnApplyTemplate()
        {
            _dropDownButton = GetFirstChildOfType<ToggleButton>(this);
            if (_dropDownButton == null)
                throw new Exception("ToggleButton not found in EmailInput");
            _dropDownButton.Visibility = Visibility.Hidden; // hidden by default

            _editableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (_editableTextBox == null)
                throw new Exception("PART_EditableTextBox not found in EmailInput template");
            _editableTextBox.PreviewKeyDown += EditableTextBoxOnPreviewKeyDown;
            _editableTextBox.TextChanged += EditableTextBoxOnTextChanged;

            SelectionChanged += OnSelectionChanged;
            DropDownOpened += OnDropDownOpened;
        }

        // when opening dropdown, add predomain to domain list
        private void OnDropDownOpened(object sender, EventArgs eventArgs)
        {
            AppendPreDomainToDomainList();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (_editableTextBox == null)
                return;
            // when an item is selected, copy it to editable text box
            if (selectionChangedEventArgs.AddedItems != null && selectionChangedEventArgs.AddedItems.Count > 0)
            {
                // save caret index because it's reset when modifying text
                int previousCaretIndex = _editableTextBox.CaretIndex;
                _editableTextBox.Text = selectionChangedEventArgs.AddedItems[0].ToString();
                // reposition caret
                _editableTextBox.CaretIndex = previousCaretIndex;
            }
        }

        private void EditableTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (_editableTextBox == null)
                return;
            bool containsAt = _editableTextBox.Text.Contains("@");
            // if dropdown is opened and @, add predomain to domain list
            if (IsDropDownOpen && containsAt)
            {
                // save selected index because it's reset when adding/removing item
                int previousSelectedIndex = SelectedIndex;
                //
                AppendPreDomainToDomainList();
                // reselect same item
                SelectedIndex = previousSelectedIndex;
            }
            // if @ is removed, hide dropdown
            if (!containsAt)
            {
                IsDropDownOpen = false;
                _dropDownButton.Visibility = Visibility.Collapsed;
            }
            EmailValue = _editableTextBox.Text;
        }

        private void EditableTextBoxOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            int key = KeyInterop.VirtualKeyFromKey(keyEventArgs.Key);
            switch (key)
            {
                case 50: // @: display and open dropdown
                    _dropDownButton.Visibility = Visibility.Visible;
                    IsDropDownOpen = true;
                    break;
                case 27: // escape: close dropdown if opened
                    if (IsDropDownOpen)
                    {
                        _dropDownButton.Visibility = Visibility.Collapsed;
                        IsDropDownOpen = false;
                    }
                    break;
                case 38: // up: select next item if dropdown is opened (text is changed when selected index is changed [see OnSelectionChanged])
                    if (IsDropDownOpen)
                    {
                        SelectedIndex = SelectedIndex == -1 ? Items.Count - 1 : (Items.Count + SelectedIndex - 1) % Items.Count;
                        keyEventArgs.Handled = true;
                    }
                    break;
                case 40: // down: select previous item if dropdown is opened (text is changed when selected index is changed [see OnSelectionChanged])
                    if (IsDropDownOpen)
                    {
                        SelectedIndex = SelectedIndex == -1 ? 0 : (SelectedIndex + 1) % Items.Count;
                        keyEventArgs.Handled = true;
                    }
                    break;
            }
        }

        private void AppendPreDomainToDomainList()
        {
            if (_editableTextBox == null)
                return;
            // keep email before @
            string beforeAt = _editableTextBox.Text.Substring(0, Math.Max(_editableTextBox.Text.IndexOf('@') + 1, 0));
            // and add this string before every domain in domain list
            Items.Clear();
            foreach (string domain in DomainList)
                Items.Add(beforeAt + domain);
        }

        public static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null)
                return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var result = (child as T) ?? GetFirstChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
