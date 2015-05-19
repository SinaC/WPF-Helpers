using System;
using System.Collections.Generic;
using System.Windows;
using ModalPopupDemo.Core;
using ModalPopupDemo.ViewModels;

namespace ModalPopupDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Random Random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly Stack<IPopup> _popups = new Stack<IPopup>();

        private void ButtonOpenMessages_OnClick(object sender, RoutedEventArgs e)
        {
            IPopup popup = Factory.PopupService.DisplayMessages(new List<string>
            {
                "LIGNE 1",
                "LIGNE 2",
                "LIGNE 3",
                "LIGNE 4"
            });
            _popups.Push(popup);
        }

        private void ButtonOpenModal_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelBase vm;
            string title;
            if (Random.Next()%2 == 0)
            {
                vm = new ViewModel1
                {
                    Text1 = "TEXT 1",
                    Text2 = "TEXT 2"
                };
                title = "ViewModel1";
            }
            else
            {
                vm = new ViewModel2
                    {
                        Text = "TEXT",
                        Items = new List<string>
                            {
                                "Item 1",
                                "Item 2",
                                "Item 3",
                                "Item 4",
                                "Item 5",
                                "Item 6",
                            }
                    };
                title = "ViewModel2";
            }
            IPopup popup = Factory.PopupService.DisplayModal(vm, title);
            _popups.Push(popup);
        }

        private void ButtonOpenQuestion_OnClick(object sender, RoutedEventArgs e)
        {
            IPopup popup = Factory.PopupService.DisplayQuestion(
                "Information", 
                "Are you sure ?",
                new ActionButton
                    {
                        Caption = "Yes",
                        Order = 1,
                        ClickCallback =() => ClickCallback("Yes")
                    },
                    new ActionButton
                    {
                        Caption = "No",
                        Order = 2,
                        ClickCallback = () => ClickCallback("No")
                    },
                    new ActionButton
                    {
                        Caption = "Cancel",
                        Order = 3,
                        ClickCallback = () => ClickCallback("Cancel")
                    });
            _popups.Push(popup);
        }

        private void ButtonMove_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (IPopup popup in _popups)
                Factory.PopupService.Move(popup, 50, 50);
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            if (_popups.Count > 0)
            {
                IPopup popup = _popups.Pop();
                Factory.PopupService.Close(popup);
            }
        }

        private void ClickCallback(string text)
        {
            MessageBox.Show(text);
            if (_popups.Count > 0)
            {
                IPopup popup = _popups.Pop();
                Factory.PopupService.Close(popup);
            }
        }
    }
}
