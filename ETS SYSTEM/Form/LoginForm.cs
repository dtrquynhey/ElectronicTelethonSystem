using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ETS_SYSTEM
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            labelAttempt.Text = "Attempts remaining: " + attempt;
        }

        //count-down if failed login
        private int attempt = 3;

        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit the application?", "ETS Electronic Telethon System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Environment.Exit(0);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            bool valid = false;
            using (StreamReader sr = new StreamReader(@"..\Debug\admin.txt", true))
            {
                while (sr.Peek() >= 0)
                {
                    string[] field = sr.ReadLine().Split(',');
                    if (textBoxUsername.Text == field[0] && textBoxPassword.Text == field[1])
                    {
                        valid = true;
                        break;
                    }
                }
            }
            if (valid)
            {
                string username = textBoxUsername.Text;
                MessageBox.Show($"Welcome to ETS Electronic Telethon System, {username}!", "ETS Electronic Telethon System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SystemForm systemForm = new SystemForm(username);
                this.Hide();
                systemForm.Show();

            }
            else if (textBoxUsername.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("Required fields missing!", "ETS Electronic Telethon System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //failed attempt only when either one of them incorrect (not when one of them is empty string)
                attempt--;
                labelAttempt.Text = "Attempts remaining: " + attempt.ToString();
                if (attempt == 0)
                {
                    MessageBox.Show("3 failed login attempts.\nPlease restart the application and try again!", "ETS Electronic Telethon System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("Invalid username or password! You have " + attempt + " attempts left!", "ETS Electronic Telethon System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxUsername.Clear();
                    textBoxPassword.Clear();
                    textBoxUsername.Focus();
                    return;
                }
            }
        }

        //show password when checkbox.checked
        private void checkBoxShowPW_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPW.Checked)
                textBoxPassword.UseSystemPasswordChar = false;
            else
                textBoxPassword.UseSystemPasswordChar = true;
        }
    }
}
