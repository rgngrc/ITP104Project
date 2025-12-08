namespace FinalProject
{
    partial class LoginScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginScreen));
            this.bgpic = new System.Windows.Forms.Label();
            this.lblUniName = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblStudentNum = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLogin = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panelUsername = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.panelPassword = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelUsername.SuspendLayout();
            this.panelPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // bgpic
            // 
            this.bgpic.CausesValidation = false;
            this.bgpic.Dock = System.Windows.Forms.DockStyle.Right;
            this.bgpic.Image = ((System.Drawing.Image)(resources.GetObject("bgpic.Image")));
            this.bgpic.Location = new System.Drawing.Point(1071, 0);
            this.bgpic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.bgpic.Name = "bgpic";
            this.bgpic.Size = new System.Drawing.Size(853, 961);
            this.bgpic.TabIndex = 0;
            // 
            // lblUniName
            // 
            this.lblUniName.AutoSize = true;
            this.lblUniName.Font = new System.Drawing.Font("Cambria", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUniName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(183)))), ((int)(((byte)(196)))));
            this.lblUniName.Location = new System.Drawing.Point(177, 69);
            this.lblUniName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUniName.Name = "lblUniName";
            this.lblUniName.Size = new System.Drawing.Size(344, 52);
            this.lblUniName.TabIndex = 1;
            this.lblUniName.Text = "Roan University";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Font = new System.Drawing.Font("Cambria", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogin.ForeColor = System.Drawing.Color.White;
            this.lblLogin.Location = new System.Drawing.Point(492, 274);
            this.lblLogin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(185, 63);
            this.lblLogin.TabIndex = 2;
            this.lblLogin.Text = "LOGIN";
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsername.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.ForeColor = System.Drawing.Color.White;
            this.txtUsername.Location = new System.Drawing.Point(8, 22);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(499, 33);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            // 
            // lblStudentNum
            // 
            this.lblStudentNum.AutoSize = true;
            this.lblStudentNum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.lblStudentNum.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStudentNum.ForeColor = System.Drawing.Color.DarkGray;
            this.lblStudentNum.Location = new System.Drawing.Point(4, 2);
            this.lblStudentNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStudentNum.Name = "lblStudentNum";
            this.lblStudentNum.Size = new System.Drawing.Size(70, 17);
            this.lblStudentNum.TabIndex = 4;
            this.lblStudentNum.Text = "Username:";
            this.lblStudentNum.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.lblPassword.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.ForeColor = System.Drawing.Color.DarkGray;
            this.lblPassword.Location = new System.Drawing.Point(4, 2);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(65, 17);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password:";
            this.lblPassword.Click += new System.EventHandler(this.lblPassword_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(177)))), ((int)(((byte)(122)))));
            this.panel1.Location = new System.Drawing.Point(332, 463);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 6);
            this.panel1.TabIndex = 7;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(177)))), ((int)(((byte)(122)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(80)))));
            this.btnLogin.Location = new System.Drawing.Point(471, 688);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(247, 65);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.InitialImage")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(72, 53);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(84, 84);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 9;
            this.pictureBoxLogo.TabStop = false;
            // 
            // panelUsername
            // 
            this.panelUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.panelUsername.Controls.Add(this.lblStudentNum);
            this.panelUsername.Controls.Add(this.txtUsername);
            this.panelUsername.Location = new System.Drawing.Point(333, 409);
            this.panelUsername.Margin = new System.Windows.Forms.Padding(4);
            this.panelUsername.Name = "panelUsername";
            this.panelUsername.Size = new System.Drawing.Size(511, 55);
            this.panelUsername.TabIndex = 10;
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(8, 22);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(499, 33);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // panelPassword
            // 
            this.panelPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(71)))), ((int)(((byte)(105)))));
            this.panelPassword.Controls.Add(this.txtPassword);
            this.panelPassword.Controls.Add(this.lblPassword);
            this.panelPassword.Location = new System.Drawing.Point(333, 546);
            this.panelPassword.Margin = new System.Windows.Forms.Padding(4);
            this.panelPassword.Name = "panelPassword";
            this.panelPassword.Size = new System.Drawing.Size(512, 55);
            this.panelPassword.TabIndex = 11;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Font = new System.Drawing.Font("Cambria", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(183)))), ((int)(((byte)(196)))));
            this.lblWelcome.Location = new System.Drawing.Point(1507, 860);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(469, 70);
            this.lblWelcome.TabIndex = 12;
            this.lblWelcome.Text = "Roan University";
            this.lblWelcome.Click += new System.EventHandler(this.lblWelcome_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Cambria", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(183)))), ((int)(((byte)(196)))));
            this.label1.Location = new System.Drawing.Point(1675, 798);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(317, 63);
            this.label1.TabIndex = 13;
            this.label1.Text = "Welcome to";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // LoginScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(1924, 961);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.panelUsername);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.lblUniName);
            this.Controls.Add(this.bgpic);
            this.Controls.Add(this.panelPassword);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LoginScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login - Roan University";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.LoginScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelUsername.ResumeLayout(false);
            this.panelUsername.PerformLayout();
            this.panelPassword.ResumeLayout(false);
            this.panelPassword.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label bgpic;
        private System.Windows.Forms.Label lblUniName;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblStudentNum;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Panel panelUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Panel panelPassword;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label label1;
    }
}

