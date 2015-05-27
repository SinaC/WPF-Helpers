using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SampleWPF.Core.MVVM
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Public

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var type = GetType();
            if (string.IsNullOrEmpty(propertyName) || type.GetProperty(propertyName) != null)
                return;
            var customTypeDescriptor = (ICustomTypeDescriptor)this;
            if (customTypeDescriptor == null ||
                customTypeDescriptor.GetProperties()
                    .Cast<PropertyDescriptor>()
                    .All(property => property.Name != propertyName))
                throw new ArgumentException(@"Property not found", propertyName);
        }

        #endregion

        #region Protected

        protected void RaisePropertyChangedOnAll()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanging == null)
                return;
            var propertyName = GetPropertyName(propertyExpression);
            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanged == null)
                return;
            var propertyName = GetPropertyName(propertyExpression);
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///   Change the field value and raise property if changed
        ///  </summary>
        ///  <typeparam name="T">Type</typeparam>
        ///  <param name="propertyExpression">use it to get Property Name</param>
        ///  <param name="field">field Name</param>
        ///  <param name="newValue">value</param>
        /// <param name="forceRaisePropertyChanged"></param>
        /// <returns></returns>
        protected virtual bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, bool forceRaisePropertyChanged = false)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (!forceRaisePropertyChanged && EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));

            field = newValue;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(@"Invalid argument", "propertyExpression");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(@"Argument is not a property", "propertyExpression");
            return propertyInfo.Name;
        }

        #endregion
    }
}
