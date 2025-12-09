using AForge.Video;
using AForge.Video.DirectShow;
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
        FilterInfoCollection cameras;
        VideoCaptureDevice cam;

        public ScanScreen()
        {
            InitializeComponent();
        }

        private void ScanScreen_Load(object sender, EventArgs e)
        {
            cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (cameras.Count > 0)
            {
                cam = new VideoCaptureDevice(cameras[0].MonikerString);
                cam.NewFrame += Cam_NewFrame;
                cam.Start();
            }
            else
            {
                MessageBox.Show("No camera found!");
            }
        }

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox6.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void ScanScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cam != null && cam.IsRunning)
            {
                cam.SignalToStop();
                cam.WaitForStop();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            StudentsScreen studentsScreen = new StudentsScreen();
            studentsScreen.Show();
            this.Hide();
        }
    }
}
