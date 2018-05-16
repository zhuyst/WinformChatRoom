using System;
using System.Windows.Forms;

namespace WinformChatRoom
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Hide();
            new MainForm(NameTextBox.Text).Show();
        }
    }
}
