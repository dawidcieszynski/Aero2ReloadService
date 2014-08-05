namespace Aero2Reload.Tests
{
    using Aero2Reload.Service.CustomDevices;
    using Aero2Reload.Service.Loggers;

    using Xunit;

    public class HuaweiE355Tests
    {
        [Fact]
        public void TestRestartHuaweiE355()
        {
            var o = new HuaweiE355(new ConsoleLogger());
            o.Restart();
        }
    }
}
