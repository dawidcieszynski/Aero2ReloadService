namespace Aero2Reload.Service
{
    using System;
    using System.ServiceProcess;

    using Aero2Reload.Service.Loggers;
    using Aero2Reload.Service.Logic;

    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "demo")
            {
                var internetLogic = new InternetLogic(new ConsoleLogger());

                internetLogic.Start();

                Console.ReadLine();

                internetLogic.Stop();

                return;
            }

            ServiceBase[] servicesToRun = { new Aero2ReloadService(args) };
            ServiceBase.Run(servicesToRun);
        }
    }
}
