using System;

namespace ModalPopupOverlay.ViewModels
{
    public class QuestionPopupAnswerItem : ViewModelBase // should be ObservableObject
    {
        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    OnPropertyChanged("Caption");
                }
            }
        }

        public Action ClickCallback { get; set; }
    }
}
