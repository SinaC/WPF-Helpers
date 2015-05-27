using System;
using System.Collections.Generic;
using SampleWPF.Core.MVVM;

namespace SampleWPF.Core.Interfaces
{
    public class ActionButton
    {
        public string Caption { get; set; }
        public Action ClickCallback { get; set; }
        public int Order { get; set; }
        public bool CloseOnClick { get; set; }

        public ActionButton()
        {
            ClickCallback = null;
            CloseOnClick = true;
        }
    }

    public interface IPopupService
    {
        // Modal popup
        IPopup DisplayModal<T>(T viewModel, string title, Func<bool> closeConfirmation = null)
            where T : ViewModelBase;

        // Messages popup (shouldn't be moved)
        IPopup DisplayMessages(List<string> messages);

        // Question popup (displayed in a Modal)
        IPopup DisplayQuestion(string title, string question, params ActionButton[] actionButtons);

        // Move
        void Move(IPopup popup, double horizontalOffset, double verticalOffset);

        // Close
        void Close(IPopup popup);

        // Close
        void Close<T>(T viewModel)
            where T : ViewModelBase;
    }
}
