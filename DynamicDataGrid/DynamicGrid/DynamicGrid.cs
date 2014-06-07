using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace DynamicDataGrid.DynamicGrid
{
    //public class DynamicGrid<TRow, TColumn> : ObservableCollection<TRow>, ITypedList
    //    where TRow : DynamicObject
    //    where TColumn : IDynamicColumn
    //{
    //    public List<TColumn> Columns { get; private set; }

    //    public DynamicGrid(IEnumerable<TColumn> columns)
    //    {
    //        Columns = columns.ToList();
    //    }

    //    public DynamicGrid(params TColumn[] columns)
    //    {
    //        Columns = columns.ToList();
    //    }
        
    //    public DynamicGrid(IEnumerable<TRow> rows, IEnumerable<TColumn> columns)
    //    {
    //        foreach(TRow row in rows)
    //            Add(row);
    //        Columns = columns.ToList();
    //    }

    //    public DynamicGrid(IEnumerable<TRow> rows, params TColumn[] columns)
    //    {
    //        foreach (TRow row in rows)
    //            Add(row);
    //        Columns = columns.ToList();
    //    }

    //    public string GetListName(PropertyDescriptor[] listAccessors)
    //    {
    //        return null;
    //    }

    //    public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
    //    {
    //        PropertyDescriptor[] dynamicDescriptors;

    //        if (Columns.Any())
    //            dynamicDescriptors = Columns.Select(p => new DynamicPropertyDescriptor(p.Name, p.Type, p.IsReadOnly)).Cast<PropertyDescriptor>().ToArray();
    //        else
    //            dynamicDescriptors = new PropertyDescriptor[0];

    //        return new PropertyDescriptorCollection(dynamicDescriptors);
    //    }
    //}

    public class DynamicGrid<TRow, TColumn> : IList, ITypedList
        where TRow : DynamicObject
        where TColumn : IDynamicColumn
    {
        public IList<TRow> Rows { get; private set; }
        public List<TColumn> Columns { get; private set; }

        public DynamicGrid(IList<TRow> rows, IEnumerable<TColumn> columns)
        {
            Rows = rows;
            Columns = columns.ToList();
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptor[] dynamicDescriptors;

            if (Columns.Any())
                dynamicDescriptors = Columns.Select(p => new DynamicPropertyDescriptor(p.Name, p.Type, p.IsReadOnly)).Cast<PropertyDescriptor>().ToArray();
            else
                dynamicDescriptors = new PropertyDescriptor[0];

            return new PropertyDescriptorCollection(dynamicDescriptors);
        }

        private IList UnspecializedRows {get { return (IList) Rows; }}

        #region IList

        public IEnumerator GetEnumerator()
        {
            return UnspecializedRows.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            UnspecializedRows.CopyTo(array, index);
        }

        public int Count { get { return UnspecializedRows.Count; } }
        public object SyncRoot { get { return UnspecializedRows.SyncRoot; } }
        public bool IsSynchronized { get { return UnspecializedRows.IsSynchronized; } }
        public int Add(object value)
        {
            return UnspecializedRows.Add(value);
        }

        public bool Contains(object value)
        {
            return UnspecializedRows.Contains(value);
        }

        public void Clear()
        {
            UnspecializedRows.Clear();
        }

        public int IndexOf(object value)
        {
            return UnspecializedRows.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            UnspecializedRows.Insert(index, value);
        }

        public void Remove(object value)
        {
            UnspecializedRows.Remove(value);
        }

        public void RemoveAt(int index)
        {
            UnspecializedRows.RemoveAt(index);
        }

        public object this[int index]
        {
            get { return UnspecializedRows[index]; }
            set { UnspecializedRows[index] = value; }
        }

        public bool IsReadOnly { get { return UnspecializedRows.IsReadOnly; } }
        public bool IsFixedSize { get { return UnspecializedRows.IsFixedSize; } }

        #endregion
    }
}
