namespace Aero2Reload.Config
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security.Principal;
    using System.Windows;

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
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
