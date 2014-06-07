using System;

namespace DynamicDataGrid.DynamicGrid
{
    public class DynamicColumn : IDynamicColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
