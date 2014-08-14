namespace Aero2Reload.Config
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;

    using Aero2Reload.Service;
    using Aero2Reload.Service.Loggers;

    using AeroReload.Common;

    public class Aero2ReloadServiceInstaller
    {
        private readonly string serviceInstallUtilPath;
        private readonly string clickOnceAssemblyPath = System.Reflection.Assembly.GetAssembly(typeof(Aero2ReloadServiceInstaller)).Location;
        private readonly string clickOnceDirectory;
        private readonly string serviceExecutableFileName = Path.GetFileName(System.Reflection.Assembly.GetAssembly(typeof(Aero2ReloadService)).Location);
        private readonly string installAppDataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aero2ReloadTools");
        private readonly string serviceExecutablePath;

        private EventLogLogger logger;

        public Aero2ReloadServiceInstaller()
        {
            this.clickOnceDirectory = Path.GetDirectoryName(this.clickOnceAssemblyPath);
            this.serviceExecutablePath = Path.Combine(this.installAppDataDirPath, this.serviceExecutableFileName);
            this.serviceInstallUtilPath = GetInstallUtilPath();
            this.logger = new EventLogLogger(Consts.EventSourceName, Consts.EventLog);
        }

        public void InstallService()
        {
            try
            {
                if (this.ServiceIsRunning())
                {
                    this.StopService(10000);
                }

                if (!Directory.Exists(this.installAppDataDirPath))
                {
                    this.logger.Debug("Tworzę " + this.installAppDataDirPath);
                    Directory.CreateDirectory(this.installAppDataDirPath);
                }

                foreach (var executableFilePath in Directory.EnumerateFiles(this.clickOnceDirectory))
                {
                    var executableFileName = Path.GetFileName(executableFilePath);
                    if (executableFileName != null)
                    {
                        var source = Path.Combine(this.clickOnceDirectory, executableFileName);
                        var destination = Path.Combine(this.installAppDataDirPath, executableFileName);
                        this.logger.Debug("Kopiuję " + destination);
                        File.Copy(source, destination);
                    }
                }

                var args = string.Format("\"{0}\"", this.serviceExecutablePath);
                var process = new Process { StartInfo = new ProcessStartInfo(this.serviceInstallUtilPath, args) };
                this.logger.Debug("Uruchamian " + process.StartInfo.FileName + " " + args);
                process.Start();
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
        }

        public void UnInstallService()
        {
            try
            {
                if (this.ServiceIsRunning())
                {
                    this.StopService(10000);
                }

                var tk = new Process { StartInfo = new ProcessStartInfo("taskkill", "/F /IM mmc.exe") };
                this.logger.Debug("Uruchamian " + tk.StartInfo.FileName);
                tk.Start();
                tk.WaitForExit();

                var args = string.Format("/u \"{0}\"", this.serviceExecutablePath);
                var iu = new Process { StartInfo = new ProcessStartInfo(this.serviceInstallUtilPath, args) };
                this.logger.Debug("Uruchamian " + iu.StartInfo.FileName + " " + args);
                iu.Start();
                iu.WaitForExit();

                if (Directory.Exists(this.installAppDataDirPath))
                {
                    Directory.Delete(this.installAppDataDirPath, true);
                }
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
        }

        public void StartService(int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(Consts.ServiceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
        }

        public void StopService(int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(Consts.ServiceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
        }

        public void RestartService(int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(Consts.ServiceName);
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
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }
        }

        public bool ServiceIsRunning()
        {
            try
            {
                var service = new ServiceController(Consts.ServiceName);
                return service.Status == ServiceControllerStatus.Running;
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
            }

            return false;
        }

        public bool ServiceIsStopped()
        {
            try
            {
                var service = new ServiceController(Consts.ServiceName);
                return service.Status == ServiceControllerStatus.Stopped;
            }
            catch (Exception exception)
            {
                this.logger.DebugException(exception);
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
