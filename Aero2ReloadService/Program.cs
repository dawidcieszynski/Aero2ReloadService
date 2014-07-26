namespace Aero2ReloadService
{
    using System.ServiceProcess;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun = { new Aero2ReloadService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
