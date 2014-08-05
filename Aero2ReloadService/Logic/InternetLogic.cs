namespace Aero2Reload.Service.Logic
{
    using System;
    using System.Linq;
    using System.Management;
    using System.Threading;
    using System.Threading.Tasks;

    using Aero2Reload.Service.CustomDevices;
    using Aero2Reload.Service.Exceptions;
    using Aero2Reload.Service.Loggers;

    using AeroReload.Common;

    using HtmlAgilityPack;

    using RestSharp;

    using ROOT.CIMV2.Win32;

    public class InternetLogic
    {
        private readonly Logger logger;

        private readonly RestClient restClient;

        private bool checking;

        private Timer timer;

        private TransmissionServer server;

        private CaptchaDialogLogic captchaDialogLogic;

        public InternetLogic(Logger logger)
        {
            this.logger = logger;
            this.restClient = new RestClient();
            this.captchaDialogLogic = new CaptchaDialogLogic(logger);
        }

        public void Stop()
        {
            this.timer.Dispose();
            this.server.Dispose();

            this.logger.Debug("Logic stopped");
        }

        public void Start()
        {
            this.timer = new Timer(this.OnTimerTick, null, 1000, 60 * 1000);
            this.server = new TransmissionServer(this.logger);

            this.logger.Debug("Logic started");
        }

        public async void Check()
        {
            this.logger.Debug("Check");

            if (this.checking)
            {
                return;
            }

            try
            {
                this.checking = true;

                this.logger.Debug("Checking");

                if (!this.InternetValid())
                {
                    if (!this.NeedRestartConnection())
                    {
                        bool captchaResolved;
                        do
                        {
                            captchaResolved = await this.ProcessCaptcha();
                        }
                        while (!captchaResolved);
                    }

                    this.RestartConnection();
                }
            }
            catch (Exception exception)
            {
                this.logger.Debug("Check " + exception.Message);
            }
            finally
            {
                this.checking = false;
            }
        }

        private void OnTimerTick(object sender)
        {
            this.logger.Debug("OnTimerTick");

            try
            {
                this.Check();
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
            finally
            {
                this.logger.Debug("OnTimerTickEnd");
            }
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
            devicesCount = new HuaweiE355(this.logger).Restart();

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

            // todo: inne karty
        }

        private bool NeedRestartConnection()
        {
            var aeroFormRequest = new RestRequest(Consts.AeroRootUrl, Method.POST);
            aeroFormRequest.AddParameter("viewForm", "true");

            var aeroFormResponse = this.restClient.Execute(aeroFormRequest);
            return aeroFormResponse.Content.Contains("Rozłącz i ponownie połącz się z Internetem.");
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
                    var disableResult = adapter.Disable();
                    if (disableResult != 0)
                    {
                        throw new DeviceDisableException();
                    }

                    var enableResult = adapter.Enable();
                    if (enableResult != 0)
                    {
                        throw new DeviceDisableException();
                    }

                    count++;
                }
            }

            return count;
        }

        private async Task<bool> ProcessCaptcha()
        {
            var aeroFormRequest = new RestRequest(Consts.AeroRootUrl, Method.POST);
            aeroFormRequest.AddParameter("viewForm", "true");

            var aeroFormResponse = this.restClient.Execute(aeroFormRequest);
            var fordoc = new HtmlDocument();
            fordoc.LoadHtml(aeroFormResponse.Content);

            var captchaImg = fordoc.DocumentNode.SelectSingleNode(".//*[@id='captcha']");
            string captchaImgUrl = Consts.AeroRootUrl + captchaImg.Attributes["src"].Value;

            var resolvedCaptchaValue = await this.ShowCaptchaFormDialogResult(captchaImgUrl);

            if (!string.IsNullOrWhiteSpace(resolvedCaptchaValue))
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

        private Task<string> ShowCaptchaFormDialogResult(string captchaImgUrl)
        {
            var tcs = new TaskCompletionSource<string>();

            this.server.Done = captcha =>
            {
                this.logger.Debug("Captcha: " + captcha);
                tcs.SetResult(captcha);
            };

            this.captchaDialogLogic.ShowDialog(captchaImgUrl);

            return tcs.Task;
        }

        private bool InternetValid()
        {
            var internetCheckResponse = this.restClient.Execute(new RestRequest(Consts.HomeUrl));
            return !internetCheckResponse.Content.Contains("Kliknij tutaj");
        }
    }
}
