using System;
using System.Windows.Input;
using SampleWPF.Core.MVVM;
using SampleWPF.Core.Processors;

namespace SampleWPF.Core.Commands
{
    // TODO: messages
    public class ProcessorCommand : ICommand
    {
        private readonly Func<ProcessorBase> _getProcessorFunc;
        private readonly Predicate<object> _canExecute;

        public ProcessorCommand(Func<ProcessorBase> getProcessorFunc)
        {
            if (getProcessorFunc == null)
                throw new ArgumentNullException("getProcessorFunc");
            _getProcessorFunc = getProcessorFunc;
        }

        public ProcessorCommand(Func<ProcessorBase> getProcessorFunc, Predicate<object> canExecute)
            : this(getProcessorFunc)
        {
            _canExecute = canExecute;
        }

        public ProcessorCommand(Func<ProcessorBase> getProcessorFunc, Func<bool> canExecute)
            : this(getProcessorFunc, obj => canExecute())
        {
        }

        #region ICommand

        public void Execute(object parameter)
        {
            ProcessorBase processor = _getProcessorFunc();

            CommandManager.InvalidateRequerySuggested();

            processor.Initialize();
            processor.Execute();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }

    public class ProcessorCommand<T> : ICommand
        where T : ViewModelBase
    {
        private readonly Func<ProcessorBase<T>> _getProcessorFunc;
        private readonly Predicate<object> _canExecute;

        public ProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc)
        {
            if (getProcessorFunc == null)
                throw new ArgumentNullException("getProcessorFunc");
            _getProcessorFunc = getProcessorFunc;
        }

        public ProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc, Predicate<object> canExecute)
            : this(getProcessorFunc)
        {
            _canExecute = canExecute;
        }

        public ProcessorCommand(Func<ProcessorBase<T>> getProcessorFunc, Func<bool> canExecute)
            : this(getProcessorFunc, obj => canExecute())
        {
        }

        #region ICommand

        public void Execute(object parameter)
        {
            ProcessorBase<T> processor = _getProcessorFunc();

            processor.Initialize();
            processor.Execute();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }
}
