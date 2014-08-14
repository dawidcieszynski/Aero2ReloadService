namespace Aero2Reload.Config
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security.Principal;
    using System.Windows;

    using AeroReload.Common;

    using BugSense;
    using BugSense.Model;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!this.IsRunAsAdministrator())
            {
                this.RunThisAsAdministrator();
                this.Shutdown();
                return;
            }

            var exceptionManager = new ExceptionManager(this);
            BugSenseHandler.Instance.InitAndStartSession(exceptionManager, Consts.BugSenseId);

            base.OnStartup(e);
        }

        private void RunThisAsAdministrator()
        {
            var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase)
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (Exception)
            {
                MessageBox.Show("Aplikacja musi być uruchomiona z uprawnieniami administratora.");
            }
        }

        private bool IsRunAsAdministrator()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
                return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return false;
        }
    }
}
