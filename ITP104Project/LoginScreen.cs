using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITP104Project
{
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // ===== 1. EMPTY FIELD CHECK =====
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your Username and Password.");
                ShakeForm();
                return;
            }
            else if (string.IsNullOrEmpty(username))
            {
                ShowError("Username is required.");
                txtUsername.Focus();
                ShakeForm();
                return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required.");
                txtPassword.Focus();
                ShakeForm();
                return;
            }

            try
            {
                // ===== 2. CHECK LOGIN =====
                if (AuthService.Login(username, password, out string role))
                {
                    MessageBox.Show(
                        $"Login Successful!\nWelcome {username} ({role})",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DashboardScreen dashboard = new DashboardScreen();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    ShowError("Invalid Username or Password.");
                    ShakeForm();
                }
            }
            catch (Exception ex)
            {
                // ===== 3. CRASH-PROOF ERROR HANDLING =====
                ShowError("An unexpected system error occurred.\n\nDetails:\n" + ex.Message);
            }
        }

        // Reusable Error Popup
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // Shake animation for wrong login
        private void ShakeForm()
        {
            int originalX = this.Left;

            for (int i = 0; i < 5; i++)
            {
                this.Left += 10;
                System.Threading.Thread.Sleep(20);
                this.Left -= 20;
                System.Threading.Thread.Sleep(20);
                this.Left += 10;
                System.Threading.Thread.Sleep(20);
            }

            this.Left = originalX;
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {

        }
    }
}