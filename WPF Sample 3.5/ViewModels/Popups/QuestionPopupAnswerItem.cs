using System;
using SampleWPF.Core;
using SampleWPF.Core.MVVM;

namespace SampleWPF.ViewModels.Popups
{
    public class QuestionPopupAnswerItem : ViewModelBase // should be ObservableObject
    {
        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set { Set(() => Caption, ref _caption, value); }
        }

        public Action ClickCallback { get; set; }
        public bool CloseOnClick { get; set; }
    }
}
