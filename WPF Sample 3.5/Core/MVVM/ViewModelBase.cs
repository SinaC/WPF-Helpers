using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SampleWPF.Core.Interfaces;
using SampleWPF.DataContracts;
using SampleWPF.Utility;

namespace SampleWPF.Core.MVVM
{
    public abstract class ViewModelBase : ObservableObject, IDataErrorInfo
    {
        public bool IsValidationActive { get; set; } // default: true
        public bool DisplayAlertsOnValidation { get; set; } // default: false

        public IAlertsManager AlertsManager { get; set; }

        protected ViewModelBase()
        {
            IsValidationActive = true;
            DisplayAlertsOnValidation = false;
        }

        public virtual void CleanUp()
        {
        }

        protected void DisplayAlert(AlertData alert)
        {
            if (AlertsManager == null)
                LogInvalidAlertsManager();
            else
                AlertsManager.Add(alert);
        }

        protected void DisplayAlerts(List<AlertData> alerts)
        {
            if (AlertsManager == null)
                LogInvalidAlertsManager();
            else if (alerts != null && alerts.Any())
                AlertsManager.Add(alerts);
        }

        protected void ClearAlerts()
        {
            if (AlertsManager == null)
                LogInvalidAlertsManager();
            else
                AlertsManager.Clear();
        }

        protected virtual string ValidateData(string propertyName)
        {
            return String.Empty;
        }

        #region IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                if (!IsValidationActive)
                    return String.Empty;
                string message = ValidateData(propertyName);
                if (DisplayAlertsOnValidation && !String.IsNullOrEmpty(message))
                {
                    if (AlertsManager == null)
                        LogInvalidAlertsManager();
                    else
                        AlertsManager.Add(new AlertData
                            {
                                Type = AlertDataTypes.Error,
                                Title = message,
                                Detail = message,
                            });
                }
                return message;
            }
        }

        public string Error { get { return "Your Model Is Invalid"; } }

        #endregion

        protected void LogInvalidAlertsManager()
        {
            Logger.Log(LogTypes.Error, "Cannot send alerts: AlertsManager not initialized in {0}", GetType().FullName);
        }
    }
}
