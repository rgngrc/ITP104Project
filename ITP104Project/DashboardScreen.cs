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

namespace ITP104Project
{
    public partial class DashboardScreen : Form
    {
        public DashboardScreen()
        {
            InitializeComponent();
        }

        // Call the data loading method when the form loads
        private void Dashboard_Load(object sender, EventArgs e)
        {

            // Box 1: Present Today
            int presentCount = DBCounters.GetPresentTodayCount();
            lblPresentToday.Text = presentCount.ToString();

            // Box 2: Total Students
            int totalCount = DBCounters.GetTotalStudentsCount();
            lblTotalStudents.Text = totalCount.ToString();
            
            // Box 3. Monthly Scans (Transactions)
            int monthlyCount = DBCounters.GetMonthlyScansCount();
            lblMonthly.Text = monthlyCount.ToString();

            // --- END DBCounters Usage ---

            LoadRecentAttendance();
        }

        private void LoadRecentAttendance()
        {
            // Query to fetch the 5 most recent attendance records
            string query = @"
        SELECT 
            A.timestamp AS 'Time',
            S.full_name AS 'Student Name',
            P.program_code AS 'Program',
            D.dept_name AS 'Department'
        FROM Attendance A
        INNER JOIN Students S ON A.student_id = S.student_id
        INNER JOIN Programs P ON S.program_id = P.program_id
        INNER JOIN Departments D ON P.dept_id = D.dept_id
        ORDER BY A.timestamp DESC
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

        // Navigation buttons
        private void btnScan_Click(object sender, EventArgs e)
        {
            ScanScreen scanScreen = new ScanScreen();
            scanScreen.Show();
            this.Hide();
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            StudentsScreen studentsScreen = new StudentsScreen();
            studentsScreen.Show();
            this.Hide();
        }

    }
}