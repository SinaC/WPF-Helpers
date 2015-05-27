using System;
using System.Windows;

namespace SampleWPF
{
    public static class Splasher
    {
        private static Window _splash;

        // Get or set the splash screen window
        public static Window Splash
        {
            get
            {
                return _splash;
            }
            set
            {
                _splash = value;
            }
        }

        // Show splash screen
        public static void ShowSplash()
        {
            if (_splash != null)
                _splash.Show();
        }

        // Close splash screen
        public static void CloseSplash()
        {
            if (_splash != null)
            {
                _splash.Close();

                IDisposable disposable = _splash as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}
