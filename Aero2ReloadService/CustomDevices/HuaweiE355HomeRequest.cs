namespace Aero2ReloadService.CustomDevices.HuaweiE355Requests
{
    using RestSharp;

    public class HuaweiE355HomeRequest : RestRequest
    {
        public HuaweiE355HomeRequest()
            : base("http://192.168.1.1/html/home.html", Method.GET)
        {
        }
    }
}