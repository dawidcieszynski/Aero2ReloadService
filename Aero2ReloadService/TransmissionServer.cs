namespace Aero2Reload.Service
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.IO.Pipes;

    using Aero2Reload.Service.Loggers;

    using AeroReload.Common;

    public class TransmissionServer : IDisposable
    {
        private readonly Logger eventLog;

        private NamedPipeServerStream pipeServer;

        private readonly BackgroundWorker thread;

        private string resolvedCaptchaValue;

        public TransmissionServer(Logger eventLog)
        {
            this.eventLog = eventLog;
            this.thread = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.thread.DoWork += this.ThreadDoWork;
            this.thread.RunWorkerCompleted += this.ThreadRunWorkerCompleted;
            this.thread.RunWorkerAsync();
        }

        public Action<string> Done { get; set; }

        public void Dispose()
        {
            this.thread.CancelAsync();
        }

        private void ThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.eventLog.Debug(this.resolvedCaptchaValue);
        }

        private void ThreadDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    this.pipeServer = new NamedPipeServerStream(Consts.ServicePipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    this.pipeServer.WaitForConnection();

                    using (var sr = new StreamReader(this.pipeServer))
                    {
                        this.resolvedCaptchaValue = sr.ReadLine();
                        this.Done(this.resolvedCaptchaValue);
                    }

                    this.pipeServer.Close();
                    this.pipeServer = null;

                    if (e.Cancel)
                    {
                        break;
                    }
                }
                catch (IOException exception)
                {
                    this.eventLog.DebugException(exception);
                }
            }
        }
    }
}