using AForge.Video;
using AForge.Video.DirectShow;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;


namespace ITP104Project
{
    public partial class ScanScreen : Form
    {

        private string phoneCameraURL = "http://192.168.1.6:8080/shot.jpg";
        private Timer frameTimer;
        private BarcodeReader reader;


        public ScanScreen()
        {
            InitializeComponent();
            // Initialize ZXing BarcodeReader
            reader = new BarcodeReader
            {
                AutoRotate = true,
                Options = { TryHarder = true }
            };


            // Initialize Timer to fetch frames from mobile
            frameTimer = new Timer();
            frameTimer.Interval = 300; // fetch every 300ms
            frameTimer.Tick += FrameTimer_Tick;

        }


        private void ScanScreen_Load(object sender, EventArgs e)
        {
            frameTimer.Start(); // Start fetching frames
            LoadRecentScans();  // Load recent scans on form load
        }

        private async void FrameTimer_Tick(object sender, EventArgs e)
        {
            Bitmap frame = await GetFrameFromPhoneAsync();
            if (frame != null)
            {
                // Display frame
                pictureBox6.Image?.Dispose();
                pictureBox6.Image = (Bitmap)frame.Clone();

                // Try decoding QR code
                var result = reader.Decode(frame);
                frame.Dispose();

                if (result != null)
                {
                    frameTimer.Stop(); // pause to prevent duplicate scans
                    string studentId = result.Text;

                    if (MarkAttendance(studentId))
                    {
                        MessageBox.Show($"Attendance marked for Student ID: {studentId}", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRecentScans();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to mark attendance. Student ID: {studentId} not found.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    frameTimer.Start(); // resume scanning
                }
            }
        }

        private async Task<Bitmap> GetFrameFromPhoneAsync()
        {
            try
            {
                WebRequest request = WebRequest.Create(phoneCameraURL);
                using (WebResponse response = await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                {
                    return (Bitmap)Bitmap.FromStream(stream);
                }
            }
            catch
            {
                return null; // fail silently if phone is not reachable
            }
        }

        private bool MarkAttendance(string studentId)
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn == null)
                    {
                        MessageBox.Show("Database connection failed!", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open(); // Make sure connection is open
                    }

                    // Check if student exists
                    string checkQuery = "SELECT COUNT(*) FROM Students WHERE student_id = @studentId";
                    using (MySqlCommand cmdCheck = new MySqlCommand(checkQuery, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@studentId", studentId);
                        object result = cmdCheck.ExecuteScalar();
                        int count = (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);

                        if (count == 0)
                        {
                            return false;
                        }
                    }

                    string insertQuery = @"
                    INSERT INTO Attendance(timestamp, student_id, scanner_id, result, users_id)
                    VALUES(NOW(), @studentId, @scannerId, @result, @usersId)";
                    using (MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@studentId", studentId);
                        cmdInsert.Parameters.AddWithValue("@scannerId", 1);   // replace with your scanner ID
                        cmdInsert.Parameters.AddWithValue("@result", "In"); // or "In"/"Out" depending on your app
                        cmdInsert.Parameters.AddWithValue("@usersId", 1);     // replace with the user ID who is scanning
                        cmdInsert.ExecuteNonQuery();
                    }


                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error marking attendance: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadRecentScans()
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                        SELECT A.student_id AS 'Student ID', 
                               S.full_name AS 'Student Name', 
                               A.timestamp AS 'Time In/Out'
                        FROM Attendance A
                        INNER JOIN Students S ON A.student_id = S.student_id
                        ORDER BY A.timestamp DESC
                        LIMIT 10;";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recent scans: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScanScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            frameTimer.Stop();
            pictureBox6.Image?.Dispose();
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

        //CamBox
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


        // Recent Scans
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
