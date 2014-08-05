namespace Aero2Reload.Service.CustomDevices
{
    using RestSharp;

    public class HuaweiE355ConnectRequest : RestRequest
    {
        public HuaweiE355ConnectRequest()
            : base("http://192.168.1.1/api/dialup/dial", Method.POST)
        {
            this.AddBody(new HuaweiE355ConnectRequestBody());
        }
    }
}