namespace Aero2ReloadService
{
    using System.Windows.Forms;

    public partial class CaptchaForm : Form
    {
        private string captchaValue;

        public CaptchaForm()
        {
            this.InitializeComponent();
        }

        public void LoadImage(string value)
        {
            this.captchaPictureBox.LoadAsync(value);
        }

        public string GetValue()
        {
            return this.captchaValue;
        }

        private void TextBoxTextChanged(object sender, System.EventArgs e)
        {
            this.captchaValue = ((TextBox)sender).Text;
        }
    }
}
