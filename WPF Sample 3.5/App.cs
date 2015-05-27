using System;
using System.Windows;
using SampleWPF.Cache;
using SampleWPF.DataContracts;
using SampleWPF.Models;
using SampleWPF.Utility;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF
{
    //splash screen
    //http://www.codeproject.com/Articles/38291/Implement-Splash-Screen-with-WPF

    //switch between windows
    //http://stackoverflow.com/questions/5708992/how-to-switch-wpf-windows

    class App : Application
    {
        private static App _applicationInstance;

        //  splash screen
        //  run Application
        //      login screen (display main screen when credentials are validated)
        //      main screen (display login screen when closed)

        [STAThread]
        static void Main()
        {
            Repository.GlobalCache = new GlobalCache(); // TODO: use UnityResolve
            Repository.ClientCache = new ClientCache(); // TODO: use UnityResolve
            Repository.Session = new SessionData();

            // Display splash screen
            if (Settings.DisplaySplashScreen)
            {
                Splasher.Splash = new Views.SplashScreen.SplashScreen();
                Splasher.ShowSplash();
                // TODO: load datas
                System.Threading.Thread.Sleep(2000);
                //
                Splasher.CloseSplash();
            }

            //
            _applicationInstance = new App();
        }

        public App()
        {
            if (Settings.DisplayLogin)
                StartupUri = new Uri(@"Views\Login\Login.xaml", UriKind.Relative); // starts on Login
            else
                StartupUri = new Uri(@"Views\MainWindow.xaml", UriKind.Relative);

            Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Settings.DisplayLogin)
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown; // Don't shutdown application when a window is closed
            else
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            Repository.ClientCache.Set("1234", ClientCacheKey.Client, new ClientData
            {
                ClientId = "1234",
                Name = "HEYMBEECK"
            });
            Repository.ClientCache.Set("2345", ClientCacheKey.Client, new ClientData
            {
                ClientId = "2345",
                Name = "VAN DER FRAENEN"
            });
            Repository.ClientCache.Set("3456", ClientCacheKey.Client, new ClientData
            {
                ClientId = "3456",
                Name = "DE BOURNONVILLE"
            });
        }
    }
}
