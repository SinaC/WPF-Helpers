using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM
{
    public class AsyncRelayCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canExecute;

        public AsyncRelayCommand(Action execute) 
            : this(execute, null)
        {
        }

        public AsyncRelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
                await Task.Run(() => _execute());
        }

        #endregion
    }

    public class AsyncRelayCommand<T> : ICommand
    {
        readonly Action<T> _execute;
        readonly Func<T, bool> _canExecute;

        public AsyncRelayCommand(Action<T> execute) 
            : this(execute, null)
        {
        }

        public AsyncRelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public async void Execute(object parameter)
        {
            object val = parameter;

            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
                val = Convert.ChangeType(parameter, typeof(T), null);

            if (CanExecute(val))
            {
                if (val == null)
                    await Task.Run(() => _execute(default(T)));
                else
                    await Task.Run(() => _execute((T)val));
            }
        }

        #endregion
    }
}
