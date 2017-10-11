namespace Aero2ReloadService
{
    using System.Collections;
    using System.ComponentModel;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            const string Parameter = Consts.EventSource + "\" \"" + Consts.EventLog;
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" \"" + Parameter + "\"";
            base.OnBeforeInstall(savedState);
        }
    }
}
