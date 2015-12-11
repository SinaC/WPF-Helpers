using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MVVM
{
    public class PagedCollection<T> : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<T>
    {
        private T[] _unpagedCollection;
        private IEnumerable<T> _pagedCollection;

        // Number of items in a page
        private readonly int _pageSize;
        public int PageSize
        {
            get { return _pageSize; }
        }

        // Number of pages
        private int _pageCount = 1;
        public int PageCount
        {
            get { return _pageCount; }
            private set
            {
                _pageCount = value;
                RaisePropertyChanged("PageCount");
            }
        }

        // Current page index
        private int _pageIndex = 1;
        public int PageIndex
        {
            get { return _pageIndex; }
            private set
            {
                _pageIndex = value;
                RaisePropertyChanged("PageIndex");
            }
        }

        public PagedCollection(IEnumerable<T> sourceCollection, int itemsPerPage)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");
            if (itemsPerPage <= 0)
                throw new ArgumentOutOfRangeException("itemsPerPage", "itemsPerPage must be strictly positive");
            _pageSize = itemsPerPage;

            CopyToInternal(sourceCollection);
            Refresh();
        }

        #region Move

        public void MoveToPage(int pageIndex)
        {
            if (pageIndex <= 0 || pageIndex > PageCount)
                throw new ArgumentOutOfRangeException("pageIndex");

            PageIndex = pageIndex;
            Refresh();
        }

        public void MoveToNextPage()
        {
            if (PageIndex < PageCount)
            {
                PageIndex += 1;
                Refresh();
            }
        }

        public void MoveToPreviousPage()
        {
            if (PageIndex > 1)
            {
                PageIndex -= 1;
                Refresh();
            }
        }

        #endregion

        public void Clear()
        {
            _unpagedCollection = null;
            _pagedCollection = null;
            PageCount = 0;
            PageIndex = 0;
            OnCollectionChanged();
        }

        // Copy source collection to internal collection and count pages
        private void CopyToInternal(IEnumerable<T> sourceCollection)
        {
            _unpagedCollection = sourceCollection.ToArray(); // make a copy
            PageCount = (_unpagedCollection.Length + PageSize - 1) / PageSize; // count pages
        }

        // Rebuild paged collection
        private void Refresh()
        {
            int position = (PageIndex - 1) * PageSize;
            _pagedCollection = _unpagedCollection.Skip(position).TakeWhile((n, i) => i < PageSize && position + i < _unpagedCollection.Length).ToList(); // flatten to avoid lazy evaluation
            OnCollectionChanged();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged()
        {
            //OnPropertyChanged();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, T changedItem)
        {
            //OnPropertyChanged();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, T newItem, T oldItem)
        {
            //OnPropertyChanged();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            //OnPropertyChanged();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _pagedCollection == null
                ? Enumerable.Empty<T>().GetEnumerator()
                : _pagedCollection.GetEnumerator();
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion
    }
}
