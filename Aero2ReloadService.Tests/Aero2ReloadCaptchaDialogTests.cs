namespace Aero2Reload.Tests
{
    using Aero2Reload.CaptchaDialog;
    using Aero2Reload.Service;

    using Xunit;

    public class Aero2ReloadCaptchaDialogTests
    {
        [Fact]
        public void TestAero2ReloadCaptchaDialogStart()
        {
            string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(CaptchaForm)).Location;

            ApplicationLoader.PROCESS_INFORMATION procInfo;
            var result = ApplicationLoader.StartProcessAndBypassUAC(assemblyPath, out procInfo);

            Assert.True(result);
        }
    }
}
