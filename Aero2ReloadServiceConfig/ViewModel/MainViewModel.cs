namespace Aero2ReloadServiceConfig.ViewModel
{
    using System.Diagnostics;
    using System.Text;

    using Aero2ReloadService;

    using Aero2ReloadServiceConfig.Helpers;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    public class MainViewModel : ViewModelBase
    {
        private readonly Aero2ReloadServiceInstaller serviceInstaller;

        private EventLog eventLog;

        private string eventLogText;

        public MainViewModel()
        {
            this.serviceInstaller = new Aero2ReloadServiceInstaller();

            this.InstallServiceCommand = new RelayCommand(this.InstallService, this.CanInstallService);
            this.UninstallServiceCommand = new RelayCommand(this.UninstallService, this.CanUninstallService);
            this.StartServiceCommand = new RelayCommand(this.StartService, this.CanStartService);
            this.StopServiceCommand = new RelayCommand(this.StopService, this.CanStopService);

            string eventSourceName = Consts.EventSource;
            string logName = Consts.EventLog;
            this.eventLog = new System.Diagnostics.EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            this.eventLog.Source = eventSourceName;
            this.eventLog.Log = logName;

            var s = new StringBuilder();
            foreach (EventLogEntry entry in this.eventLog.Entries)
            {
                s.Append(entry.Format());
            }

            this.EventLogText = s.ToString();
            this.eventLog.EntryWritten += this.EventLogEntryWritten;
        }

        public RelayCommand InstallServiceCommand { get; private set; }

        public RelayCommand UninstallServiceCommand { get; private set; }

        public RelayCommand StartServiceCommand { get; private set; }

        public RelayCommand StopServiceCommand { get; private set; }

        public string EventLogText
        {
            get
            {
                return this.eventLogText;
            }

            set
            {
                if (this.eventLogText == value)
                {
                    return;
                }

                this.eventLogText = value;
                this.RaisePropertyChanged(this.EventLogText);
            }
        }

        private void EventLogEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            this.EventLogText += e.Entry.Format();
        }

        private bool CanStopService()
        {
            return this.serviceInstaller.ServiceIsRunning();
        }

        private void StopService()
        {
            this.serviceInstaller.StopService(60);
        }

        private bool CanStartService()
        {
            return this.serviceInstaller.ServiceIsStopped();
        }

        private void StartService()
        {
            this.serviceInstaller.StartService(60);
        }

        private bool CanUninstallService()
        {
            return this.serviceInstaller.ServiceIsInstalled() && this.serviceInstaller.ServiceIsStopped();
        }

        private void UninstallService()
        {
            this.serviceInstaller.UnInstallService();
        }

        private bool CanInstallService()
        {
            return !this.serviceInstaller.ServiceIsInstalled();
        }

        private void InstallService()
        {
            this.serviceInstaller.InstallService();
        }
    }
}