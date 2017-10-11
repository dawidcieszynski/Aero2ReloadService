namespace Aero2ReloadService
{
    using System.ServiceProcess;

    public static class Program
    {
        public static void Main(string[] args)
        {
            ServiceBase[] servicesToRun = { new Aero2ReloadService(args) };
            ServiceBase.Run(servicesToRun);
        }
    }
}
