namespace Aero2Reload.Config
{
    using System.Deployment.Application;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Title += " (" + ApplicationDeployment.CurrentDeployment.CurrentVersion + ")";
            }
        }
    }
}
