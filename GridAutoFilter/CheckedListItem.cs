/*
    Jarloo
    http://www.jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/ 

*/

using System;
using System.ComponentModel;

namespace GridAutoFilter
{
    public class CheckedListItem<T, U> : INotifyPropertyChanged
        where U : IEquatable<U>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isChecked;
        private U item;
        private Func<T, U> getItemFunc;

        public CheckedListItem(U item, Func<T, U> getItemFunc, bool isChecked = false)
        {
            this.item = item;
            this.getItemFunc = getItemFunc;
            this.isChecked = isChecked;
        }

        //public CheckedListItem(T item, bool isChecked=false)
        //{
        //    this.item = item;
        //    this.isChecked = isChecked;
        //}

        public U Item
        {
            get { return item; }
            set
            {
                item = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
            }
        }

        //public T Item
        //{
        //    get { return item; }
        //    set
        //    {
        //        item = value;
        //        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
        //    }
        //}

        public bool IsValid(T t)
        {
            return getItemFunc(t).Equals(item);
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }
}