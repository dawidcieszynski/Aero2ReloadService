namespace Aero2Reload.Service.CustomDevices
{
    using RestSharp;

    public class HuaweiE355StatusRequest : RestRequest
    {
        public HuaweiE355StatusRequest()
            : base("http://192.168.1.1/api/monitoring/status", Method.GET)
        {
        }
    }
}