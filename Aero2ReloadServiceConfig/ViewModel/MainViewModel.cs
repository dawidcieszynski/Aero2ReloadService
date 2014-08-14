namespace Aero2Reload.Config.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;

    using AeroReload.Common;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    public class MainViewModel : ViewModelBase
    {
        private readonly Aero2ReloadServiceInstaller serviceInstaller;

        private readonly EventLog eventLog;

        private readonly BackgroundWorker logEventWorker;

        private int logEntriesIndex;

        private DateTime? lastTimeGenerated;

        public MainViewModel()
        {
            this.serviceInstaller = new Aero2ReloadServiceInstaller();

            this.InstallServiceCommand = new RelayCommand(this.InstallService, this.CanInstallService);
            this.UninstallServiceCommand = new RelayCommand(this.UninstallService, this.CanUninstallService);
            this.StartServiceCommand = new RelayCommand(this.StartService, this.CanStartService);
            this.StopServiceCommand = new RelayCommand(this.StopService, this.CanStopService);

            this.eventLog = new EventLog();
            if (!EventLog.SourceExists(Consts.EventSourceName))
            {
                EventLog.CreateEventSource(Consts.EventSourceName, Consts.EventLog);
            }

            this.eventLog.Source = Consts.EventSourceName;
            this.eventLog.Log = Consts.EventLog;

            this.LogEntries = new ObservableCollection<LogEntry>();
            this.logEventWorker = new BackgroundWorker { WorkerReportsProgress = true };
            this.logEventWorker.DoWork += delegate
                {
                    Thread.Sleep(1000);

                    foreach (EventLogEntry entry in this.eventLog.Entries)
                    {
                        if (this.lastTimeGenerated != null && entry.TimeGenerated <= this.lastTimeGenerated)
                        {
                            continue;
                        }

                        this.logEventWorker.ReportProgress(
                            this.logEntriesIndex,
                            new LogEntry
                                {
                                    Index = this.logEntriesIndex++,
                                    DateTime = entry.TimeGenerated,
                                    Message = entry.Message,
                                });
                        this.lastTimeGenerated = entry.TimeGenerated;
                    }
                };

            this.logEventWorker.ProgressChanged += (x, z) => this.LogEntries.Insert(0, z.UserState as LogEntry);
            this.logEventWorker.RunWorkerCompleted += delegate
                {
                    this.logEventWorker.RunWorkerAsync();
                };
            this.logEventWorker.RunWorkerAsync();
        }

        public RelayCommand InstallServiceCommand { get; private set; }

        public RelayCommand UninstallServiceCommand { get; private set; }

        public RelayCommand StartServiceCommand { get; private set; }

        public RelayCommand StopServiceCommand { get; private set; }

        public ObservableCollection<LogEntry> LogEntries { get; set; }

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
