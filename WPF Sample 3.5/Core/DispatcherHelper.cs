using System;
using System.Windows.Threading;

namespace SampleWPF.Core
{
    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher { get; private set; }

        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (UIDispatcher.CheckAccess())
                action();
            else
                UIDispatcher.BeginInvoke(action);
        }

        public static DispatcherOperation RunAsync(Action action)
        {
            return UIDispatcher.BeginInvoke(action);
        }

        public static void Initialize()
        {
            if (UIDispatcher != null && UIDispatcher.Thread.IsAlive)
                return;

            UIDispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
