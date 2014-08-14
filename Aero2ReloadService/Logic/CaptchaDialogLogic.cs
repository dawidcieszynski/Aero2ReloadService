namespace Aero2Reload.Service.Logic
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Aero2Reload.CaptchaDialog;
    using Aero2Reload.Service.Loggers;

    using AeroReload.Common;

    public class CaptchaDialogLogic
    {
        private readonly Logger logger;

        public CaptchaDialogLogic(Logger logger)
        {
            this.logger = logger;
        }

        public void ShowDialog(string captchaImgUrl)
        {
            int handle = (int)FindWindow(null, Consts.CaptchaFormTitle);
            if (handle > 0)
            {
                return;
            }

            string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(CaptchaForm)).Location;
            ApplicationLoader.PROCESS_INFORMATION procInfo;
            var result = ApplicationLoader.StartProcessAndBypassUAC(assemblyPath + " " + captchaImgUrl, out procInfo);
            this.logger.Debug("StartingCaptchaDialog (asService) result: " + result);

            if (result)
            {
                return;
            }

            var captchaDialogProcess = new Process { StartInfo = new ProcessStartInfo(assemblyPath, captchaImgUrl) };
            result = captchaDialogProcess.Start();
            this.logger.Debug("StartingCaptchaDialog (asApp) result: " + result);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string className, string windowName);
    }
}
