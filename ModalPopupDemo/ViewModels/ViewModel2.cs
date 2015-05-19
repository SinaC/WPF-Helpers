using System.Collections.Generic;
using ModalPopupDemo.Core;

namespace ModalPopupDemo.ViewModels
{
    public class ViewModel2 : ViewModelBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        private List<string> _items;
        public List<string> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged("Items");
                }
            }
        }
    }

    public class ViewModel2DesignData : ViewModel2
    {
        public ViewModel2DesignData()
        {
            Text = "Text";
            Items = new List<string>
                {
                    "Item 1",
                    "Item 2",
                    "Item 3",
                    "Item 4",
                    "Item 5",
                    "Item 6",
                };
        }
    }
}
