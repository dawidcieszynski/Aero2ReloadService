namespace Aero2Reload.Service.CustomDevices
{
    using System;
    using System.Net;
    using System.Threading;

    using Aero2Reload.Service.Loggers;

    using RestSharp;

    public class HuaweiE355
    {
        private readonly Logger eventLog;

        public HuaweiE355(Logger eventLog)
        {
            this.eventLog = eventLog;
        }

        public int Restart()
        {
            var restClient = new RestClient { CookieContainer = new CookieContainer() };

            var homeResponse = restClient.Execute(new HuaweiE355HomeRequest());
            if (homeResponse.ResponseStatus == ResponseStatus.Error)
            {
                // brak odpowiedzi od routera
                return 0;
            }

            var loginResponse = restClient.Execute(new HuaweiE355LoginRequest());
            if (!loginResponse.Content.Contains("<response>OK</response>"))
            {
                this.eventLog.Error("B³¹d logowania do HuaweiE355 (admin, admin)");
                return 0;
            }

            var statusResponse = restClient.Execute<HuaweiE355StatusResponseBody>(new HuaweiE355StatusRequest());
            if (statusResponse.ResponseStatus == ResponseStatus.Error)
            {
                // brak odpowiedzi od routera
                return 0;
            }

            int result = 0;
            if (statusResponse.Data.ConnectionStatus == HuaweiE355ConnectionStatus.Connected)
            {
                var disconnectResponse = restClient.Execute(new HuaweiE355DisconnectRequest());
                if (!disconnectResponse.Content.Contains("<response>OK</response>"))
                {
                    this.eventLog.Error("B³¹d roz³¹czania HuaweiE355");
                    return 0;
                }

                const int WaitingForDisconnectionTimeout = 1000 * 5;
                var timeoutTickCount = Environment.TickCount + WaitingForDisconnectionTimeout;
                do
                {
                    statusResponse = restClient.Execute<HuaweiE355StatusResponseBody>(new HuaweiE355StatusRequest());
                }
                while (statusResponse.Data.ConnectionStatus != HuaweiE355ConnectionStatus.Disconnected && Environment.TickCount < timeoutTickCount);

                result = 1;
            }

            if (statusResponse.Data.ConnectionStatus == HuaweiE355ConnectionStatus.Disconnected)
            {
                var connectResponse = restClient.Execute(new HuaweiE355ConnectRequest());
                if (!connectResponse.Content.Contains("<response>OK</response>"))
                {
                    this.eventLog.Error("B³¹d ³¹czenia HuaweiE355");
                    return 0;
                }

                do
                {
                    Thread.Sleep(1000);
                    statusResponse = restClient.Execute<HuaweiE355StatusResponseBody>(new HuaweiE355StatusRequest());
                }
                while (statusResponse.Data.ConnectionStatus != HuaweiE355ConnectionStatus.Connected);

                result = 1;
            }

            return result;
        }
    }
}