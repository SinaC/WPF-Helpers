using System;

namespace MVVM
{
    public interface IMediator
    {
        void Register<T>(object recipient, Action<T> action);
        void Register<T>(object recipient, object token, Action<T> action);

        void Unregister(object recipient);
        void Unregister<T>(object recipient);
        void Unregister<T>(object recipient, Action<T> action);
        void Unregister<T>(object recipient, object token);
        void Unregister<T>(object recipient, object token, Action<T> action);

        void Send<T>(T message);
        void Send<T>(T message, object token);
    }
}
