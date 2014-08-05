namespace Aero2Reload.CaptchaDialog
{
    using System;
    using System.Deployment.Application;
    using System.IO;
    using System.IO.Pipes;
    using System.Windows.Forms;

    public partial class CaptchaForm : Form
    {
        private readonly NamedPipeClientStream pipeClient;

        private string captchaValue;

        public CaptchaForm(string arg0, NamedPipeClientStream pipeClient)
        {
            this.pipeClient = pipeClient;
            this.InitializeComponent();

            this.Load += this.CaptchaFormLoad;

            this.LoadImage(arg0);
        }

        public void LoadImage(string value)
        {
            this.captchaPictureBox.LoadAsync(value);
        }

        public string GetValue()
        {
            return this.captchaValue;
        }

        private void CaptchaFormLoad(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Text += @" (" + ApplicationDeployment.CurrentDeployment.CurrentVersion + @")";
            }
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            this.captchaValue = ((TextBox)sender).Text;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            this.pipeClient.Connect();
            using (var sw = new StreamWriter(this.pipeClient))
            {
                sw.WriteLine(this.captchaValue);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }

            this.pipeClient.Close();

            this.Close();
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 10 || e.KeyChar == 13)
            {
                this.ButtonClick(null, null);
            }
        }
    }
}
