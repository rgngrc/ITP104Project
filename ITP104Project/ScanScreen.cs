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
    public partial class ScanScreen : Form
    {
        public ScanScreen()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ScanScreen_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DashboardScreen dashboardScreen = new DashboardScreen();
            dashboardScreen.Show();
            this.Hide();
        }
    }
}
