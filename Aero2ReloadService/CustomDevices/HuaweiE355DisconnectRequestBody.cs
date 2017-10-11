namespace Aero2ReloadService.CustomDevices.HuaweiE355Requests
{
    using RestSharp.Serializers;

    [SerializeAs(Name = "request")]
    public class HuaweiE355DisconnectRequestBody
    {
        public HuaweiE355DisconnectRequestBody()
        {
            this.Action = 0;
        }

        public int Action { get; set; }
    }
}