using System;
using System.Collections.Generic;
using System.Windows;
using ModalPopupOverlay.ViewModels;

namespace ModalPopupOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel
                {
                    PopupService = ModalPopupPresenter // !!!!! IMPORTANT
                };
        }

        private static readonly Random Random = new Random();

        private readonly Stack<IPopup> _popups = new Stack<IPopup>();

        private void ShowMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            IPopup popup = ModalPopupPresenter.DisplayMessages(new List<string>
                {
                    "Message 1",
                    "Message 2",
                    "Message 3",
                    "Message 4",
                    "Message 5"
                });
            _popups.Push(popup);
        }

        private void ShowModalButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModelBase vm;
            string title;
            if (Random.Next() % 2 == 0)
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
                };
                title = "ViewModel2";
            }
            IPopup popup = ModalPopupPresenter.DisplayModal(vm, title);
            _popups.Push(popup);
        }

        private void ShowQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            IPopup popup = ModalPopupPresenter.DisplayQuestion(
                "Information",
                "Are you sure ?",
                new ActionButton
                {
                    Caption = "Yes",
                    Order = 1,
                    ClickCallback = () => ClickCallback("Yes")
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_popups.Count > 0)
            {
                IPopup popup = _popups.Pop();
                ModalPopupPresenter.Close(popup);
            }
        }

        private void ClickCallback(string text)
        {
            MessageBox.Show(text);
            if (_popups.Count > 0)
            {
                IPopup popup = _popups.Pop();
                ModalPopupPresenter.Close(popup);
            }
        }
    }
}
