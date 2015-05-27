using System;

namespace SampleWPF.Utility
{
    public enum LogTypes
    {
        Debug,
        Info,
        Warning,
        Error,
    }

    // TODO: stub-class
    public static class Logger
    {
        public static void Log(LogTypes type, string format, params object[] parameters)
        {
            // TODO: use log4net
            string message = String.Format(format, parameters);
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void Log(Exception ex)
        {
            Log(LogTypes.Error, ex.ToString());
        }
    }
}
