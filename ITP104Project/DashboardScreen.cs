using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// ⚠️ MUST ADD THIS LINE: Required for MySqlCommand, MySqlConnection, etc.
using MySql.Data.MySqlClient;

namespace ITP104Project
{
    public partial class DashboardScreen : Form
    {
        public DashboardScreen()
        {
            InitializeComponent();
        }

        // 1. Call the data loading method when the form loads
        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadRecentAttendance();
        }

        // --- NEW DATA LOADING METHOD ---
        private void LoadRecentAttendance()
        {
            // Query to fetch the 5 most recent attendance records
            string query = @"
        SELECT 
            A.timestamp AS 'Time',        -- ⚠️ CORRECTED FROM scan_datetime to timestamp
            S.full_name AS 'Student Name',
            P.program_code AS 'Program',
            D.dept_name AS 'Department'
        FROM Attendance A
        INNER JOIN Students S ON A.student_id = S.student_id
        INNER JOIN Programs P ON S.program_id = P.program_id
        INNER JOIN Departments D ON P.dept_id = D.dept_id
        ORDER BY A.timestamp DESC       -- ⚠️ CORRECTED HERE TOO
        LIMIT 5;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();

                        adapter.Fill(dt);

                        // Bind the data to your DataGridView
                        dgvRecent.DataSource = dt;
                        dgvRecent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading recent attendance data: " + ex.Message,
                                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- NAVIGATION HANDLERS ---

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ScanScreen scanScreen = new ScanScreen();
            scanScreen.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StudentsScreen studentsScreen = new StudentsScreen();
            studentsScreen.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        // Ensure your DataGridView for recent scans is named dgvRecent
        private void dgvRecent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}