namespace Aero2ReloadServiceConfig
{
    using System.Windows;

    using Aero2ReloadService;

    public partial class MainWindow : Window
    {
        private readonly Aero2ReloadServiceInstaller serviceInstaller;

        public MainWindow()
        {
            this.InitializeComponent();

            this.serviceInstaller = new Aero2ReloadServiceInstaller();
        }

        private void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            this.serviceInstaller.InstallService();
            this.serviceInstaller.StartService(0);
        }

        private void UninstallButtonClick(object sender, RoutedEventArgs e)
        {
            this.serviceInstaller.StopService(0);
            this.serviceInstaller.UnInstallService();
        }
    }
}
