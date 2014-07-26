namespace Aero2ReloadService
{
    using System.Linq;
    using System.ServiceProcess;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun = { new Aero2ReloadService() };
#if RELEASE
            ServiceBase.Run(servicesToRun);
#elif DEBUG
            ((Aero2ReloadService)servicesToRun.First()).Check();
#endif
        }
    }
}
