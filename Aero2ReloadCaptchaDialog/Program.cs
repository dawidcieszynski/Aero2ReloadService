namespace Aero2Reload.CaptchaDialog
{
    using System;
    using System.IO.Pipes;
    using System.Windows.Forms;

    using AeroReload.Common;

    using BugSense;
    using BugSense.Model;

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var exceptionManager = new ExceptionManager();
            BugSenseHandler.Instance.InitAndStartSession(exceptionManager, Consts.BugSenseId);

            if (args.Length <= 0)
            {
                args = new[] { "http://www.komputerswiat.pl/media/2011/199/1959828/rys2-darmowy-internet.gif" };
            }

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", Consts.ServicePipeName, PipeDirection.Out, PipeOptions.Asynchronous))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new CaptchaForm(args[0], pipeClient));
                }
            }
            catch (Exception exception)
            {
                BugSenseHandler.Instance.LogException(exception);
            }
        }
    }
}
