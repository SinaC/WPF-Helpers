namespace ModalPopupOverlay.ViewModels
{
    public class ViewModel1 : ViewModelBase
    {
        private string _text1;
        public string Text1
        {
            get { return _text1; }
            set
            {
                if (_text1 != value)
                {
                    _text1 = value;
                    OnPropertyChanged("Text1");
                }
            }
        }

        private string _text2;
        public string Text2
        {
            get { return _text2; }
            set
            {
                if (_text2 != value)
                {
                    _text2 = value;
                    OnPropertyChanged("Text2");
                }
            }
        }
    }

    public class ViewModel1DesignData : ViewModel1
    {
        public ViewModel1DesignData()
        {
            Text1 = "Text1";
            Text2 = "Text2";
        }
    }
}
