using System;
using System.Text;

namespace SampleWPF.DataContracts
{
    public enum AlertDataTypes
    {
        Info,
        Warning,
        Error,
        Fatal,
    }

    public class AlertData
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public AlertDataTypes Type { get; set; }
        public DateTime Timestamp { get; set; }

        public AlertData()
        {
            Timestamp = DateTime.Now;
        }

        public AlertData(Exception ex)
        {
            Type = AlertDataTypes.Error;

            if (ex == null)
            {
                Title = String.Empty;
                return;
            }

            Title = ex.Message;

            StringBuilder sb = new StringBuilder();
            sb.Append("Source : " + ex.Source + Environment.NewLine);
            sb.Append("StackTrace : " + ex.StackTrace + Environment.NewLine);
            ex = ex.InnerException;

            while (ex != null)
            {
                sb.Append("Inner exception : " + ex.Message + Environment.NewLine);
                sb.Append("Source : " + ex.Source + Environment.NewLine);
                sb.Append("StackTrace : " + ex.StackTrace + Environment.NewLine);
                ex = ex.InnerException;
            }

            Detail = sb.ToString();
        }
    }
}
