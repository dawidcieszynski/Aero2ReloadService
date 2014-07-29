namespace Aero2ReloadService
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;

    public class Aero2ReloadServiceInstaller
    {
        private readonly string installUtilPath;
        private readonly string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(Aero2ReloadService)).Location;

        public Aero2ReloadServiceInstaller()
        {
            this.installUtilPath = GetInstallUtilPath();
        }

        public void InstallService()
        {
            var args = string.Format("\"{0}\"", this.assemblyPath);
            var process = new Process { StartInfo = new ProcessStartInfo(this.installUtilPath, args) };
            process.Start();
        }

        public void UnInstallService()
        {
            new Process { StartInfo = new ProcessStartInfo("taskkill", "/F /IM mmc.exe") }.Start();

            var args = string.Format("/u \"{0}\"", this.assemblyPath);
            new Process { StartInfo = new ProcessStartInfo(this.installUtilPath, args) }.Start();
        }

        public void StartService(int timeoutMilliseconds)
        {
            var service = new ServiceController(Consts.ServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public void StopService(int timeoutMilliseconds)
        {
            var service = new ServiceController(Consts.ServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }

        public void RestartService(int timeoutMilliseconds)
        {
            var service = new ServiceController(Consts.ServiceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public bool ServiceIsRunning()
        {
            var service = new ServiceController(Consts.ServiceName);
            try
            {
                return service.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                // ...
            }

            return false;
        }


        public bool ServiceIsStopped()
        {
            var service = new ServiceController(Consts.ServiceName);
            try
            {
                return service.Status == ServiceControllerStatus.Stopped;
            }
            catch
            {
                // ...
            }

            return false;
        }

        public bool ServiceIsInstalled()
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Consts.ServiceName);
            return ctl != null;
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
    }
}
