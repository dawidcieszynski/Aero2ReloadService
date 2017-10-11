namespace Aero2ReloadService.Tests
{
    using Xunit;

    public class Aero2ReloadServiceTests
    {
        [Fact]
        public void TestAero2ReloadServiceCheck()
        {
            var o = new Aero2ReloadService(new string[] { });
            o.Check();
        }
    }
}
