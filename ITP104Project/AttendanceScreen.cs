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
    public partial class AttendanceScreen : Form
    {
        public AttendanceScreen()
        {
            InitializeComponent();
        }

        private void AttendanceScreen_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DashboardScreen dashboardScreen = new DashboardScreen();
            dashboardScreen.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScanScreen scanScreen = new ScanScreen();
            scanScreen.Show();
            this.Hide();
        }
    }
}
