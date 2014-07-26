namespace Aero2ReloadService
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class Aero2ReloadServiceInstaller
    {
        private string installUtilPath;

        public Aero2ReloadServiceInstaller()
        {
            this.installUtilPath = GetInstallUtilPath();
        }

        private static string GetInstallUtilPath()
        {
            var installUtils = new[]
                                   {
                                      @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe",
                                       @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"
                                   };
            return installUtils.FirstOrDefault(File.Exists);
        }

        public void InstallService()
        {
            var path = System.Reflection.Assembly.GetAssembly(typeof(Aero2ReloadService)).Location;

            var args = string.Format("\"{0}\"", path);
            var process = new Process { StartInfo = new ProcessStartInfo(this.installUtilPath, args) };
            process.Start();
        }

        public void UnInstallService()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;

            var args = string.Format("/u \"{0}\"", path);
            new Process { StartInfo = new ProcessStartInfo(this.installUtilPath, args) }.Start();
        }
    }
}
