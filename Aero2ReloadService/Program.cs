namespace Aero2Reload.Service
{
    using System;
    using System.ServiceProcess;

    using Aero2Reload.Service.Loggers;
    using Aero2Reload.Service.Logic;

    using AeroReload.Common;

    using BugSense;
    using BugSense.Model;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var exceptionManager = new ExceptionManager();
            BugSenseHandler.Instance.InitAndStartSession(exceptionManager, Consts.BugSenseId);
            try
            {
                RunService(args);
            }
            catch (Exception exception)
            {
                BugSenseHandler.Instance.LogException(exception);
            }
        }

        private static void RunService(string[] args)
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
