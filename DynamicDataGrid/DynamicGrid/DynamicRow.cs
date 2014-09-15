using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DynamicDataGrid.DynamicGrid
{
    public class DynamicRow : DynamicObject, INotifyPropertyChanged//, IDataErrorInfo
    {
        private readonly Dictionary<string, object> _dynamicProperties;
        //private readonly Dictionary<string, bool> _dynamicValidities;

        public DynamicRow()
        {
            _dynamicProperties = new Dictionary<string, object>();
            //_dynamicValidities = new Dictionary<string, bool>();
        }

        public DynamicRow(params KeyValuePair<string, object>[] propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(s => s.Key, s => s.Value);
            //_dynamicValidities = propertyNames.ToDictionary(s => s.Key, s => true);
        }

        public DynamicRow(IEnumerable<KeyValuePair<string, object>> propertyNames)
            : this(propertyNames.ToArray())
        {
        }

        public DynamicRow(params Tuple<string, object>[] propertyNames)
        {
            _dynamicProperties = propertyNames.ToDictionary(x => x.Item1, x => x.Item2);
            //_dynamicValidities = propertyNames.ToDictionary(s => s.Item1, s => true);
        }

        public DynamicRow(IEnumerable<Tuple<string, object>> propertyNames)
            : this(propertyNames.ToArray())
        {
        }

        public bool TryAddProperty(string propertyName, object propertyValue)
        {
            if (_dynamicProperties.ContainsKey(propertyName))
                return false;
            _dynamicProperties.Add(propertyName, propertyValue);
            //_dynamicValidities.Add(propertyName, true);
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool TryGetProperty(string propertyName, out object propertyValue)
        {
            propertyValue = null;
            if (_dynamicProperties.ContainsKey(propertyName))
            {
                propertyValue = _dynamicProperties[propertyName];
                return true;
            }
            return false;
        }

        public bool TrySetProperty(string propertyName, object value)
        {
            // TODO: type checking
            if (!_dynamicProperties.ContainsKey(propertyName))
                return false;
            if (_dynamicProperties[propertyName].GetType() != value.GetType())
            {
                TypeConverter converter = TypeDescriptor.GetConverter(_dynamicProperties[propertyName].GetType());
                try
                {
                    object converted = converter.ConvertFrom(value);
                    _dynamicProperties[propertyName] = converted;
                    //_dynamicValidities[propertyName] = true;
                    OnPropertyChanged(propertyName);
                }
                    // TODO: should notify row error provider
                catch (FormatException ex)
                {
                    //_dynamicValidities[propertyName] = false;
                    OnPropertyChanged(propertyName);
                    return false;
                }
                catch (Exception ex)
                {
                    //_dynamicValidities[propertyName] = false;
                    OnPropertyChanged(propertyName);
                    return false;
                }
            }
            else
            {
                _dynamicProperties[propertyName] = value;
                //_dynamicValidities[propertyName] = true;
                OnPropertyChanged(propertyName);
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (TryGetProperty(binder.Name, out result))
                return true;
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (TrySetProperty(binder.Name, value))
                return true;
            return base.TrySetMember(binder, value);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dynamicProperties.Keys.ToArray();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression body = propertyExpression.Body as MemberExpression;

            if (body == null)
                throw new ArgumentException(@"Invalid argument", "propertyExpression");

            PropertyInfo property = body.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException(@"Argument is not a property", "propertyExpression");

            return property.Name;
        }

        protected bool Set<T>(Expression<Func<T>> selectorExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            string propertyName = GetPropertyName(selectorExpression);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        //#region IDataErrorInfo

        //public string this[string columnName]
        //{
        //    get { return _dynamicValidities[columnName] ? null : "Error"; }
        //}

        //public string Error { get { return String.Empty; } }

        //#endregion
    }
}
