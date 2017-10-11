namespace Aero2ReloadService.CustomDevices.HuaweiE355Requests
{
    using RestSharp;

    public class HuaweiE355DisconnectRequest : RestRequest
    {
        public HuaweiE355DisconnectRequest()
            : base("http://192.168.1.1/api/dialup/dial", Method.POST)
        {
            this.AddBody(new HuaweiE355DisconnectRequestBody());

        }
    }
}