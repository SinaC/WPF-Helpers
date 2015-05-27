using System;
using System.Collections.Generic;
using System.Linq;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;
using SampleWPF.Core.ServerCalls;
using SampleWPF.DataContracts;
using SampleWPF.Utility;

namespace SampleWPF.Core.Processors
{
    public abstract class ProcessorBase
    {
        public ProcessorStatus Status { get; protected set; }

        protected IAlertsManager AlertsManager { get; private set; }
        protected ServerCallCollection ServerCalls { get; private set; }

        protected ProcessorBase(IAlertsManager alertsManager)
        {
            Status = ProcessorStatus.Created;
            AlertsManager = alertsManager;
            ServerCalls = new ServerCallCollection();
        }

        public virtual void Initialize()
        {
            Status = ProcessorStatus.Initializing;

            try
            {
                bool isValid = ValidateModel();

                if (isValid)
                {
                    InitializeServerCalls();

                    Status = ProcessorStatus.Initialized;
                }
                else
                    Status = ProcessorStatus.NotValidated;

            }
            catch (Exception ex)
            {
                Status = ProcessorStatus.UnhandledException;
                OnUnhandledException(ex);
                Logger.Log(ex);
            }
        }

        public virtual void Execute()
        {
            if (Status != ProcessorStatus.Initialized)
                return;

            try
            {
                Status = ProcessorStatus.Executing;

                ServerCallProcessor serverCallProcessor = new ServerCallProcessor();
                serverCallProcessor.Execute(ServerCalls);

                Status = ProcessorStatus.Executed;

                bool hasErrors = ManageServerCallsErrors();

                MapResult();

                OnAfterExecute();

                Status = hasErrors ? ProcessorStatus.TerminatedWithErrors : ProcessorStatus.Terminated;
            }
            catch (Exception ex)
            {
                Status = ProcessorStatus.UnhandledException;
                OnUnhandledException(ex);
                Logger.Log(ex);
            }
        }

        public List<string> WaitMessage
        {
            get { return ServerCalls.WaitMessage; }
        }

        #region Abstract

        public abstract void InitializeServerCalls();

        public abstract void MapResult();

        #endregion

        #region Virtual

        public virtual bool ValidateModel()
        {
            return true;
        }

        public virtual void OnBeforeExecute()
        {
        }

        public virtual void OnAfterExecute()
        {
        }

        public virtual void OnUnhandledException(Exception ex)
        {
            AlertData alert = new AlertData(ex);
            if (AlertsManager != null)
                AlertsManager.Add(alert);
            else
                LogInvalidAlertsManager(alert);
        }

        #endregion

        protected void LogInvalidAlertsManager(AlertData alert)
        {
            //Logger.Log(LogTypes.Error, "Cannot send alerts: AlertsManager not initialized in {0}", GetType().FullName);
            LogInvalidAlertsManager(new List<AlertData>
                {
                    alert
                });
        }

        protected void LogInvalidAlertsManager(List<AlertData> alerts)
        {
            Logger.Log(LogTypes.Error, "Cannot send alerts: AlertsManager not initialized in {0}", GetType().FullName);
            if (alerts != null)
                foreach (AlertData alert in alerts)
                {
                    string alertMessage = String.Format("{0} | {1} | {2} | {3}", alert.Type, alert.Timestamp, alert.Title, alert.Detail);
                    Logger.Log(LogTypes.Error, alertMessage);
                }
        }

        // return true if error occured in ServerCall
        private bool ManageServerCallsErrors()
        {
            List<AlertData> alerts = ServerCalls.SelectMany(x => x.Alerts).ToList();
            if (alerts.Any())
            {
                if (AlertsManager != null)
                    AlertsManager.Add(alerts);
                else
                    LogInvalidAlertsManager(alerts);
            }

            return alerts.Any(x => x.Type == AlertDataTypes.Error || x.Type == AlertDataTypes.Fatal);
        }
    }

    public abstract class ProcessorBase<TViewModel> : ProcessorBase
        where TViewModel : ViewModelBase
    {
        public TViewModel ViewModel { get; private set; }

        protected ProcessorBase(TViewModel viewModel)
            : base(viewModel.AlertsManager)
        {
            ViewModel = viewModel;
        }

    }
}
