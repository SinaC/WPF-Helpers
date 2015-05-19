using System.Collections.Generic;
using ModalPopupDemo.Core;

namespace ModalPopupDemo.ViewModels
{
    public class MessagePopupViewModel : ViewModelBase
    {
        private List<string> _messages;
        public List<string> Messages
        {
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged("Messages");
                }
            }
        }
    }

    public class MessagePopupViewModelDesignData : MessagePopupViewModel
    {
        public MessagePopupViewModelDesignData()
        {
            Messages = new List<string>
            {
                "LIGNE 1",
                "UNE LONGUE LIGNE 2",
                "UN TRES TRES TRES LONGUE LIGNE 3",
                "LIGNE 4"
            };
        }
    }
}
