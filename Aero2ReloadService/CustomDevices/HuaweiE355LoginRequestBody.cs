namespace Aero2Reload.Service.CustomDevices
{
    using RestSharp.Serializers;

    [SerializeAs(Name = "request")]
    public class HuaweiE355LoginRequestBody
    {
        public HuaweiE355LoginRequestBody(string admin, string password)
        {
            this.Username = admin;
            this.Password = password;
        }

        public string Password { get; set; }

        public string Username { get; set; }
    }
}