using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM
{
    public interface IAsyncRelayCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    public interface IAsyncRelayCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
    }
}
