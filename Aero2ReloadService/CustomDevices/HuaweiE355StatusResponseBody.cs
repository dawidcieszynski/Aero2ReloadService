namespace Aero2Reload.Service.CustomDevices
{
    public class HuaweiE355StatusResponseBody
    {
        public HuaweiE355ConnectionStatus ConnectionStatus { get; set; }

        public int SignalStrength { get; set; }

        public int SignalIcon { get; set; }

        public int CurrentNetworkType { get; set; }

        public int CurrentServiceDomain { get; set; }

        public bool RoamingStatus { get; set; }

        public int? BatteryStatus { get; set; }

        public int? BatteryLevel { get; set; }

        public bool simlockStatus { get; set; }

        public string WanIPAddress { get; set; }

        public string PrimaryDns { get; set; }

        public string SecondaryDns { get; set; }

        public int CurrentWifiUser { get; set; }

        public int TotalWifiUser { get; set; }

        public int ServiceStatus { get; set; }

        public bool SimStatus { get; set; }

        public bool WifiStatus { get; set; }
    }
}
