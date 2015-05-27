using System;
using System.ComponentModel;
using System.Windows.Input;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;
using SampleWPF.Core.Processors;
using SampleWPF.Core.ServerCalls;

namespace SampleWPF.Core.Commands
{
    public class AsyncProcessorCommand : ICommand
    {
        protected class ReturnValue
        {
            public ProcessorStatus Status;
            public IPopup MessagePopup;
        }

        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Func<ProcessorBase> _getProcessorFunc;
        private readonly Predicate<object> _canExecute;
        private readonly Action _completed;
        private readonly bool _continueWithError;
        private readonly bool _silentCall;
        private readonly Action<bool> _loading;

        public AsyncProcessorCommand(Func<ProcessorBase> getProcessorFunc, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
            : this(getProcessorFunc, () => true, completed, continueWithError, silentCall, loading)
        {
        }

        public AsyncProcessorCommand(Func<ProcessorBase> getProcessorFunc, Func<bool> canExecute, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
            : this(getProcessorFunc, obj => canExecute(), completed, continueWithError, silentCall, loading)
        {
        }

        public AsyncProcessorCommand(Func<ProcessorBase> getProcessorFunc, Predicate<object> canExecute = null, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
        {
            if (getProcessorFunc == null)
                throw new ArgumentNullException("getProcessorFunc");

            if (getProcessorFunc == null)
                throw new ArgumentNullException("getProcessorFunc");

            _getProcessorFunc = getProcessorFunc;
            _canExecute = canExecute;
            _completed = completed;
            _continueWithError = continueWithError;
            _silentCall = silentCall;
            _loading = loading;

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
            ProcessorBase processor = _getProcessorFunc();
            processor.Initialize();

            IPopup messagePopup = null;
            if (processor.Status == ProcessorStatus.Initialized)
            {
                if (!_silentCall)
                    messagePopup = UIRepository.PopupService.DisplayMessages(processor.WaitMessage);

                if (_loading != null)
                    _loading(true);

                processor.Execute();
            }

            CommandManager.InvalidateRequerySuggested();

            doWorkEventArgs.Result = new ReturnValue
            {
                Status = processor.Status,
                MessagePopup = messagePopup
            };
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            ReturnValue returnValue = runWorkerCompletedEventArgs.Result as ReturnValue;
            if (returnValue != null)
            {
                if (returnValue.Status == ProcessorStatus.NotValidated)
                    return;

                if (returnValue.MessagePopup != null)
                    UIRepository.PopupService.Close(returnValue.MessagePopup);
                if (_completed != null && runWorkerCompletedEventArgs.Error == null)
                {
                    if (returnValue.Status == ProcessorStatus.Terminated || (_continueWithError && returnValue.Status == ProcessorStatus.TerminatedWithErrors))
                        _completed();
                }
            }
            CommandManager.InvalidateRequerySuggested();

            if (_loading != null)
                _loading(false);
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
                       : !_worker.IsBusy && _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }

    public class AsyncProcessorCommand<T> : ICommand
        where T : ViewModelBase
    {
        protected class ReturnValue
        {
            public ProcessorStatus Status;
            public IPopup MessagePopup;
        }

        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Func<ProcessorBase<T>> _getProcessorFunc;
        private readonly Predicate<object> _canExecute;
        private readonly Action _completed;
        private readonly bool _continueWithError;
        private readonly bool _silentCall;
        private readonly Action<bool> _loading;

        public AsyncProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
            : this(getProcessorFunc, () => true, completed, continueWithError, silentCall, loading)
        {
        }

        public AsyncProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc, Func<bool> canExecute, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
            : this(getProcessorFunc, obj => canExecute(), completed, continueWithError, silentCall, loading)
        {
        }

        public AsyncProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc, Predicate<object> canExecute = null, Action completed = null, bool continueWithError = true, bool silentCall = false, Action<bool> loading = null)
        {
            if (getProcessorFunc == null)
                throw new ArgumentNullException("getProcessorFunc");

            _getProcessorFunc = getProcessorFunc;
            _canExecute = canExecute;
            _completed = completed;
            _continueWithError = continueWithError;
            _silentCall = silentCall;
            _loading = loading;

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
            ProcessorBase<T> processor = _getProcessorFunc();
            processor.Initialize();

            IPopup messagePopup = null;
            if (processor.Status == ProcessorStatus.Initialized)
            {
                if (!_silentCall)
                    messagePopup = UIRepository.PopupService.DisplayMessages(processor.WaitMessage);

                if (_loading != null)
                    _loading(true);

                processor.Execute();
            }

            CommandManager.InvalidateRequerySuggested();

            doWorkEventArgs.Result = new ReturnValue
            {
                Status = processor.Status,
                MessagePopup = messagePopup
            };
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            ReturnValue returnValue = runWorkerCompletedEventArgs.Result as ReturnValue;
            if (returnValue != null)
            {
                if (returnValue.Status == ProcessorStatus.NotValidated)
                    return;

                if (returnValue.MessagePopup != null)
                    UIRepository.PopupService.Close(returnValue.MessagePopup);

                if (_completed != null && runWorkerCompletedEventArgs.Error == null)
                {
                    if (returnValue.Status == ProcessorStatus.Terminated || (_continueWithError && returnValue.Status == ProcessorStatus.TerminatedWithErrors))
                        _completed();
                }
            }

            CommandManager.InvalidateRequerySuggested();

            if (_loading != null)
                _loading(false);
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
                       : !_worker.IsBusy && _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }
}
