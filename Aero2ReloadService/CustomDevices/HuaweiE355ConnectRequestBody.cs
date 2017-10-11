namespace Aero2Reload.Service.CustomDevices
{
    using RestSharp.Serializers;

    [SerializeAs(Name = "request")]
    public class HuaweiE355ConnectRequestBody
    {
        public HuaweiE355ConnectRequestBody()
        {
            this.Action = 1;
        }

        public int Action { get; set; }
    }
}