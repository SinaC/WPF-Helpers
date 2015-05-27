using System;
using System.ComponentModel;
using System.Windows.Input;

namespace SampleWPF.Core.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        private readonly Action _completed;
        private readonly Action<Exception> _error;

        public AsyncRelayCommand(Action execute, Action completed = null, Action<Exception> error = null)
            : this(obj => execute(), null, completed, error)
        {
        }

        public AsyncRelayCommand(Action execute, Func<bool> canExecute, Action completed = null, Action<Exception> error = null)
            : this(obj => execute(), obj => canExecute(), completed, error)
        {
        }

        public AsyncRelayCommand(Action<object> execute, Predicate<object> canExecute = null, Action completed = null, Action<Exception> error = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
            _completed = completed;
            _error = error;

            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public void Cancel()
        {
            if (_worker.IsBusy)
                _worker.CancelAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            CommandManager.InvalidateRequerySuggested();
            _execute(doWorkEventArgs.Argument);
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (_completed != null && runWorkerCompletedEventArgs.Error == null)
                _completed();
            if (_error != null && runWorkerCompletedEventArgs.Error != null)
                _error(runWorkerCompletedEventArgs.Error);
            CommandManager.InvalidateRequerySuggested();
        }

        #region ICommand

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _worker.RunWorkerAsync(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null
                       ? !_worker.IsBusy
                       : !_worker.IsBusy && _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }

    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        private readonly Action _completed;
        private readonly Action<Exception> _error;

        public AsyncRelayCommand(Action<T> execute, Action completed = null, Action<Exception> error = null)
            : this(execute, null, completed, error)
        {
        }

        public AsyncRelayCommand(Action<T> execute, Predicate<T> canExecute = null, Action completed = null, Action<Exception> error = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
            _completed = completed;
            _error = error;

            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public void Cancel()
        {
            if (_worker.IsBusy)
                _worker.CancelAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            CommandManager.InvalidateRequerySuggested();

            var val = doWorkEventArgs.Argument;
            if (doWorkEventArgs.Argument != null && doWorkEventArgs.Argument.GetType() != typeof(T) && doWorkEventArgs.Argument is IConvertible)
                val = Convert.ChangeType(doWorkEventArgs.Argument, typeof(T), null);

            if (!CanExecute(val))
                return;

            if (val != null)
                _execute((T)val);
            else
                _execute(default(T));
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (_completed != null && runWorkerCompletedEventArgs.Error == null)
                _completed();
            if (_error != null && runWorkerCompletedEventArgs.Error != null)
                _error(runWorkerCompletedEventArgs.Error);
            CommandManager.InvalidateRequerySuggested();
        }

        #region ICommand

        public void Execute(object parameter)
        {
            _worker.RunWorkerAsync(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null
                       ? !_worker.IsBusy
                       : !_worker.IsBusy && (parameter != null
                                                 ? _canExecute((T)parameter)
                                                 : _canExecute(default(T)));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }
}
