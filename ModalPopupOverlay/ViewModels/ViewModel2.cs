namespace ModalPopupOverlay.ViewModels
{
    public class ViewModel2 : ViewModelBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
    }

    public class ViewModel2DesignData : ViewModel2
    {
        public ViewModel2DesignData()
        {
            Text = "Text";
        }
    }
}
