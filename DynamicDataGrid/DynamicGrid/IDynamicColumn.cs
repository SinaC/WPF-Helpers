using System;

namespace DynamicDataGrid.DynamicGrid
{
    public interface IDynamicColumn
    {
        string Name { get; }
        Type Type { get; }
        bool IsReadOnly { get; }
    }
}
