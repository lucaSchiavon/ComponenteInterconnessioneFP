using System;
using System.Windows.Forms;
using COMPINT_UI.Config;

namespace COMPINT_UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ConfigManager.Initialize();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var pwd = txtPassword.Text;

            var cfgUser = ConfigManager.Config.Login?.Username ?? "Admin";
            var cfgPwd = ConfigManager.Config.Login?.Password ?? "revihcra";

            if (user == cfgUser && pwd == cfgPwd)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenziali non valide.", "Accesso negato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // optionally prefill username from config
            txtUsername.Text = ConfigManager.Config.Login?.Username ?? "";
        }
    }
}