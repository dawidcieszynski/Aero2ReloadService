namespace Aero2ReloadService.CustomDevices.HuaweiE355Requests
{
    using RestSharp;

    public class HuaweiE355LoginRequest : RestRequest
    {
        public HuaweiE355LoginRequest()
            : base("http://192.168.1.1/api/user/login", Method.POST)
        {
            this.AddBody(new HuaweiE355LoginRequestBody("admin", "admin"));
        }
    }
}