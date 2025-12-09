using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace ITP104Project
{
    public partial class StudentsScreen : Form
    {
        private string connectionString;

        public StudentsScreen()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["dbprojectConnection"].ConnectionString;

            LoadStudentData();
        }

        private void LoadStudentData()
        {
            // Check if connection string was successfully retrieved
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string not found in App.config.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT student_id, full_name, program_id, year_level, student_section FROM Students;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable studentTable = new DataTable();

                    adapter.Fill(studentTable);
                    dataGridView1.DataSource = studentTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load student data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DashboardScreen dashboardScreen = new DashboardScreen();
            dashboardScreen.Show();
            this.Hide();

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScanScreen scanScreen = new ScanScreen();
            scanScreen.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
