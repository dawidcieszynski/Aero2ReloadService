namespace Aero2ReloadService
{
    using System.Linq;
    using System.ServiceProcess;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            ServiceBase[] servicesToRun = { new Aero2ReloadService(args) };
#if DEBUG
            ((Aero2ReloadService)servicesToRun.First()).Check();
#else
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
