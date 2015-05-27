using System;
using SampleWPF.DataContracts;

namespace SampleWPF.ViewModels.AlertsManager
{
    public enum AlertItemTypes
    {
        Info,
        Warning,
        Error
    }

    public class AlertItem
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public AlertItemTypes Type { get; set; }
        public DateTime Timestamp { get; set; }

        public AlertItem()
        {
            Timestamp = DateTime.Now;
        }

        public static AlertItem Map(AlertData data)
        {
            AlertItem item = new AlertItem
                {
                    Title = data.Title,
                    Detail = data.Detail,
                    Timestamp = data.Timestamp
                };
            switch(data.Type)
            {
                case AlertDataTypes.Fatal:
                    item.Type = AlertItemTypes.Error;
                    break;
                case AlertDataTypes.Error:
                    item.Type = AlertItemTypes.Error;
                    break;
                case AlertDataTypes.Warning:
                    item.Type = AlertItemTypes.Warning;
                    break;
                case AlertDataTypes.Info:
                    item.Type = AlertItemTypes.Info;
                    break;
                default:
                    item.Type = AlertItemTypes.Info;
                    break;
            }
            return item;
        }
    }
}
