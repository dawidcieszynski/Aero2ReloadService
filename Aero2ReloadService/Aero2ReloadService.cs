namespace Aero2Reload.Service
{
    using System;
    using System.Deployment.Application;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;

    using Aero2Reload.Service.Loggers;
    using Aero2Reload.Service.Logic;

    using AeroReload.Common;

    using BugSense;
    using BugSense.Core;
    using BugSense.Model;

    public partial class Aero2ReloadService : ServiceBase
    {
        private readonly InternetLogic internetLogic;

        private readonly EventLogLogger logger;

        public Aero2ReloadService(string[] args)
        {
            this.InitializeComponent();

            if (!BugSenseHandlerBase.IsInitialized)
            {
                var exceptionManager = new ExceptionManager();
                BugSenseHandler.Instance.InitAndStartSession(exceptionManager, Consts.BugSenseId);
            }

            string eventSourceName = Consts.EventSourceName;
            string logName = Consts.EventLog;
            if (args.Any())
            {
                eventSourceName = args[0];
            }

            if (args.Count() > 1)
            {
                logName = args[1];
            }

            this.logger = new EventLogLogger(eventSourceName, logName);

            this.internetLogic = new InternetLogic(this.logger);
        }

        protected override void OnStart(string[] args)
        {
            this.logger.Debug("OnStart");

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.logger.Debug(ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
            }

            this.SetServiceStatusStartPending();

            this.internetLogic.Start();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += this.NetworkChangeNetworkAvailabilityChanged;

            this.SetServiceStatusRunning();
        }

        protected override void OnStop()
        {
            this.logger.Debug("OnStop");

            this.SetServiceStatusStopPending();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged -= this.NetworkChangeNetworkAvailabilityChanged;

            this.internetLogic.Stop();

            this.SetServiceStatusStopped();
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private void NetworkChangeNetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            this.internetLogic.NetworkDevicesChanged(e);
        }

        private void SetServiceStatusRunning()
        {
            var serviceStatus = new ServiceStatus { dwCurrentState = ServiceState.SERVICE_RUNNING, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void SetServiceStatusStartPending()
        {
            var serviceStatus = new ServiceStatus { dwCurrentState = ServiceState.SERVICE_START_PENDING, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void SetServiceStatusStopped()
        {
            var serviceStatus = new ServiceStatus { dwCurrentState = ServiceState.SERVICE_STOPPED, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void SetServiceStatusStopPending()
        {
            var serviceStatus = new ServiceStatus { dwCurrentState = ServiceState.SERVICE_STOP_PENDING, dwWaitHint = 100000 };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
    }
}
