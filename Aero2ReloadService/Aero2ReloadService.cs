namespace Aero2ReloadService
{
    using System;
    using System.ServiceProcess;

    public partial class Aero2ReloadService : ServiceBase
    {
        public Aero2ReloadService()
        {
            this.InitializeComponent();

            this.InitializeEventLog();

            this.InitializeTimer();
        }

        protected override void OnStart(string[] args)
        {
            this.eventLog.WriteEntry("OnStart");

            this.timer.Start();
        }

        protected override void OnStop()
        {
            this.eventLog.WriteEntry("OnStop");

            this.timer.Stop();
        }

        private void InitializeTimer()
        {
            this.timer.Interval = 60000;
            this.timer.Tick += this.OnTimer;
        }

        private void OnTimer(object sender, EventArgs eventArgs)
        {
            this.eventLog.WriteEntry("OnTimer");

            throw new NotImplementedException();
        }

        private void InitializeEventLog()
        {
            this.eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(Consts.EventSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(Consts.EventSource, Consts.EventLog);
            }

            this.eventLog.Source = Consts.EventSource;
            this.eventLog.Log = Consts.EventLog;
        }
    }
}
