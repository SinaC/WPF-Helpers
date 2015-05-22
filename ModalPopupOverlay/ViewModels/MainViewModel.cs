using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ModalPopupOverlay.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public IPopupService PopupService { protected get; set; }

        private static readonly Random Random = new Random();
        private readonly Stack<IPopup> _popups = new Stack<IPopup>();

        public ICommand ShowMessagesCommand
        {
            get
            {
                return new RelayCommand(_ =>
                    {
                        IPopup popup = PopupService.DisplayMessages(new List<string>
                            {
                                "LIGNE 1",
                                "LIGNE 2",
                                "LIGNE 3",
                                "LIGNE 4"
                            });
                        _popups.Push(popup);
                    });
            }
        }

        public ICommand ShowModalCommand
        {
            get
            {
                return new RelayCommand(_ =>
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
                        IPopup popup = PopupService.DisplayModal(vm, title);
                        _popups.Push(popup);
                    });
            }
        }

        public ICommand ShowQuestionCommand
        {
            get
            {
                return new RelayCommand(_ =>
                    {
                        IPopup popup = PopupService.DisplayQuestion(
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
                    });
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(_ =>
                    {
                        if (_popups.Count > 0)
                        {
                            IPopup popup = _popups.Pop();
                            PopupService.Close(popup);
                        }
                    });
            }
        }

        private void ClickCallback(string text)
        {
            MessageBox.Show(text);
            if (_popups.Count > 0)
            {
                IPopup popup = _popups.Pop();
                PopupService.Close(popup);
            }
        }
    }

    public class MainViewModelDesignData : MainViewModel
    {
    }
}
