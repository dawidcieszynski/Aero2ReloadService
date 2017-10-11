namespace Aero2ReloadService.Tests
{
    using System.Diagnostics;

    using global::Aero2ReloadService.CustomDevices;

    using Xunit;

    public class HuaweiE355Tests
    {
        [Fact]
        public void TestRestartHuaweiE355()
        {
            var o = new HuaweiE355(new EventLog());
            o.Restart();
        }
    }
}
