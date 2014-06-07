using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows.Data;

namespace DynamicDataGrid.DynamicGrid
{
    public class DynamicRow : DynamicObject
    {
        private readonly Dictionary<string, object> _dynamicProperties;

        public DynamicRow(params KeyValuePair<string, object>[] propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(s => s.Key, s => s.Value);
        }

        public DynamicRow(IEnumerable<KeyValuePair<string, object>> propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(s => s.Key, s => s.Value);
        }

        public DynamicRow(params Tuple<string, object>[] propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public DynamicRow(IEnumerable<Tuple<string, object>> propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public bool AddCell(string name, object value)
        {
            if (_dynamicProperties.ContainsKey(name))
                return false;
            _dynamicProperties.Add(name, value);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // !!!! no type check, if column is supposed to be an int and value is a string, _dynamicProperties will be 'transformed' to string
            // TODO: type checking
            if (_dynamicProperties.ContainsKey(binder.Name))
            {
                if (_dynamicProperties[binder.Name].GetType() != value.GetType())
                    return false;
                _dynamicProperties[binder.Name] = value;
                return true;
            }

            return base.TrySetMember(binder, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_dynamicProperties.ContainsKey(binder.Name))
            {
                result = _dynamicProperties[binder.Name];
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dynamicProperties.Keys.ToArray();
        }
    }
}
