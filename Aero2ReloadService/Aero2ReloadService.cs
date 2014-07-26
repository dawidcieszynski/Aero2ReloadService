﻿namespace Aero2ReloadService
{
    using System;
    using System.Linq;
    using System.Management;
    using System.ServiceProcess;
    using System.Windows.Forms;

    using global::Aero2ReloadService.CustomDevices;

    using RestSharp;

    using ROOT.CIMV2.Win32;

    using HtmlDocument = HtmlAgilityPack.HtmlDocument;

    public partial class Aero2ReloadService : ServiceBase
    {
        private RestClient restClient;

        private bool checking;

        public Aero2ReloadService()
        {
            this.InitializeComponent();

            this.InitializeEventLog();

            this.InitializeTimer();

            this.InitializeRestSharp();
        }

        public void Check()
        {
            if (this.checking)
            {
                return;
            }

            try
            {
                this.checking = true;

                this.LogEvent("OnTimer");

                if (!this.InternetValid())
                {
                    bool captchaResolved;
                    do
                    {
                        captchaResolved = this.ProcessCaptcha();
                    }
                    while (!captchaResolved);

                    this.RestartConnection();
                }
            }
            finally
            {
                this.checking = false;
            }
        }

        protected override void OnStart(string[] args)
        {
            this.LogEvent("OnStart");

            this.timer.Start();
        }

        protected override void OnStop()
        {
            this.LogEvent("OnStop");

            this.timer.Stop();
        }

        private void InitializeRestSharp()
        {
            this.restClient = new RestClient();
        }

        private void LogEvent(string logEntry)
        {
#if EVENTLOG
            this.eventLog.WriteEntry(logEntry);
#endif
        }

        private void InitializeTimer()
        {
            this.timer.Interval = 60000;
            this.timer.Tick += this.OnTimer;
        }

        private void OnTimer(object sender, EventArgs eventArgs)
        {
            this.Check();
        }

        private void InitializeEventLog()
        {
#if EVENTLOG
            this.eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(Consts.EventSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(Consts.EventSource, Consts.EventLog);
            }

            this.eventLog.Source = Consts.EventSource;
            this.eventLog.Log = Consts.EventLog;
#endif
        }

        private void RestartConnection()
        {
            // restart kart WWAN wbudowanych
            var devicesCount = this.RestartIntegratedDevices();

            if (devicesCount == 1)
            {
                // ok
                return;
            }

            if (devicesCount > 1)
            {
                // za dużo ale na razie ok
                return;
            }

            // restart innych kart zewnętrznych
            devicesCount = new HuaweiE355().Restart();

            if (devicesCount == 1)
            {
                // ok
                return;
            }

            if (devicesCount > 1)
            {
                // za dużo ale na razie ok
                return;
            }
        }

        private int RestartIntegratedDevices()
        {
            var query = new SelectQuery("Win32_NetworkAdapter", "NetConnectionStatus=2");
            var search = new ManagementObjectSearcher(query);
            int count = 0;
            foreach (var managementBaseObject in search.Get())
            {
                var result = (ManagementObject)managementBaseObject;
                var adapter = new NetworkAdapter(result);
                if (adapter.AdapterTypeId == NetworkAdapter.AdapterTypeIdValues.Wireless)
                {
                    adapter.Disable();
                    adapter.Enable();
                    count++;
                }
            }

            return count;
        }

        private bool ProcessCaptcha()
        {
            var aeroFormRequest = new RestRequest(Consts.AeroRootUrl, Method.POST);
            aeroFormRequest.AddParameter("viewForm", "true");

            var aeroFormResponse = this.restClient.Execute(aeroFormRequest);
            var fordoc = new HtmlDocument();
            fordoc.LoadHtml(aeroFormResponse.Content);

            var captchaImg = fordoc.DocumentNode.SelectSingleNode(".//*[@id='captcha']");
            string captchaImgUrl = Consts.AeroRootUrl + captchaImg.Attributes["src"].Value;

            string resolvedCaptchaValue;
            var dialogResult = this.ShowCaptchaFormDialogResult(captchaImgUrl, out resolvedCaptchaValue);
            if (dialogResult == DialogResult.OK)
            {
                var sendresolvedCaptchaRequest = new RestRequest(Consts.AeroRootUrl, Method.POST);
                foreach (var d in fordoc.DocumentNode.SelectNodes(".//input").Where(d => d.Attributes.Contains("name") && d.Attributes.Contains("value")))
                {
                    string name = d.Attributes["name"].Value;
                    string value = d.Attributes["value"].Value;

                    sendresolvedCaptchaRequest.AddParameter(name, value);
                }

                sendresolvedCaptchaRequest.AddParameter("captcha", resolvedCaptchaValue);
                var result = this.restClient.Execute(sendresolvedCaptchaRequest);

                return result.Content.Contains("Odpowiedź prawidłowa");
            }

            return false;
        }

        private DialogResult ShowCaptchaFormDialogResult(string captchaImgUrl, out string resolvedCaptchaValue)
        {
            var captchaForm = new CaptchaForm();
            captchaForm.LoadImage(captchaImgUrl);
            var dialogResult = captchaForm.ShowDialog();
            resolvedCaptchaValue = captchaForm.GetValue();
            return dialogResult;
        }

        private bool InternetValid()
        {
            var internetCheckResponse = this.restClient.Execute(new RestRequest(Consts.HomeUrl));
            return !internetCheckResponse.Content.Contains("Kliknij tutaj");
        }
    }
}