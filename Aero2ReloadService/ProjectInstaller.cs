namespace Aero2Reload.Service
{
    using System.Collections;
    using System.ComponentModel;

    using AeroReload.Common;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            const string Parameter = Consts.EventSourceName + "\" \"" + Consts.EventLog;
            this.Context.Parameters["assemblypath"] = "\"" + this.Context.Parameters["assemblypath"] + "\" \"" + Parameter + "\"";
            base.OnBeforeInstall(savedState);
        }
    }
}
