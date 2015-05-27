using System.Collections.Generic;
using SampleWPF.Core;
using SampleWPF.Core.MVVM;

namespace SampleWPF.ViewModels.Popups
{
    public class MessagePopupViewModel : ViewModelBase
    {
        private List<string> _messages;
        public List<string> Messages
        {
            get { return _messages; }
            set { Set(() => Messages, ref _messages, value); }
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
