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
    // Simple authentication layout.
    public partial class LoginScreen : Form
    {
        // Initializes the form components.
        public LoginScreen()
        {
            InitializeComponent();
        }


        // Handles the click event for the Login button.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Call AuthService to validate login
            if (AuthService.Login(username, password, out string role))
            {
                MessageBox.Show("Login Successful! Welcome " + username + " (Role: " + role + ")", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open Dashboard
                DashboardScreen dashboard = new DashboardScreen();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                // AuthService handled connection errors, so this means invalid credentials
                MessageBox.Show("Invalid Username or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}