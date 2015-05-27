using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SampleWPF.Core.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action execute)
            : this(obj => execute())
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(obj => execute(), obj => canExecute())
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion // ICommand Members
    }

    public class GenericRelayCommand<T> : ICommand
    {
        #region Fields

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        #endregion // Fields

        #region Constructor

        public GenericRelayCommand(Action<T> execute, Predicate<T> canExecuteFunc = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecuteFunc;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            return parameter != null ? _canExecute((T)parameter) : _canExecute(default(T));
        }

        public void Execute(object parameter)
        {
            var val = parameter;
            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
                val = Convert.ChangeType(parameter, typeof(T), null);

            if (!CanExecute(val))
                return;

            if (val != null)
                _execute((T)val);
            else
                _execute(default(T));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion // ICommand Members
    }
}
