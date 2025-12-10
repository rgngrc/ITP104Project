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
        //Change IP address according to your mobile IP Webcam app's IP
        private string phoneCameraURL = "http://192.168.100.125:8080/shot.jpg";
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
            LoadScansToday();  // Load scans today
        }

        private async void FrameTimer_Tick(object sender, EventArgs e)
        {
            Bitmap frame = await GetFrameFromPhoneAsync();
            if (frame != null)
            {
                // Display frame
                pictureBox6.Image?.Dispose();
                pictureBox6.Image = (Bitmap)frame.Clone();

                // Try decoding QR/Barcode
                var result = reader.Decode(frame);
                frame.Dispose();

                if (result != null)
                {
                    string studentId = result.Text;
                    if (MarkAttendance(studentId))
                    {
                        LoadScansToday();
                    }
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

        private Dictionary<string, DateTime> lastScanTime = new Dictionary<string, DateTime>();

        private bool MarkAttendance(string studentId)
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();

                    // 1. Check if student exists
                    string checkStudent = "SELECT COUNT(*) FROM Students WHERE student_id = @studentId";
                    using (MySqlCommand cmd = new MySqlCommand(checkStudent, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        {
                            ShowStatusMessage($"Student ID {studentId} not found");
                            return false;
                        }
                    }

                    // 2. Check last scan time (1-minute cooldown)
                    if (lastScanTime.ContainsKey(studentId))
                    {
                        DateTime lastTime = lastScanTime[studentId];
                        if ((DateTime.Now - lastTime).TotalSeconds < 60)
                        {
                            return false; // ignore scan if within 1 minute
                        }
                    }

                    // 3. Check existing attendance today
                    string checkAttendance = @"
                        SELECT attendance_id, time_out 
                        FROM Attendance
                        WHERE student_id = @studentId
                        AND DATE(time_in) = CURDATE()
                        ORDER BY time_in DESC
                        LIMIT 1";

                    int existingId = 0;
                    DateTime? timeOut = null;

                    using (MySqlCommand cmd = new MySqlCommand(checkAttendance, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingId = Convert.ToInt32(reader["attendance_id"]);
                                if (reader["time_out"] != DBNull.Value)
                                    timeOut = Convert.ToDateTime(reader["time_out"]);
                            }
                        }
                    }

                    // 4. If TIME OUT exists → ignore
                    if (timeOut != null)
                    {
                        lastScanTime[studentId] = DateTime.Now;
                        return false;
                    }

                    // 5. If no attendance → TIME IN
                    if (existingId == 0)
                    {
                        string insertQuery = @"
                            INSERT INTO Attendance (time_in, time_out, student_id, scanner_id, result, users_id)
                            VALUES (NOW(), NULL, @studentId, @scannerId, 'IN', @usersId)";
                        using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@studentId", studentId);
                            cmd.Parameters.AddWithValue("@scannerId", 1);
                            cmd.Parameters.AddWithValue("@usersId", 1);
                            cmd.ExecuteNonQuery();
                        }
                        ShowStatusMessage($"TIME IN recorded for {studentId}");
                    }
                    else
                    {
                        // 6. Existing attendance without TIME OUT → TIME OUT
                        string updateQuery = @"
                            UPDATE Attendance 
                            SET time_out = NOW(), result = 'OUT'
                            WHERE attendance_id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", existingId);
                            cmd.ExecuteNonQuery();
                        }
                        ShowStatusMessage($"TIME OUT recorded for {studentId}");
                    }

                    lastScanTime[studentId] = DateTime.Now;
                    return true;
                }
            }
            catch
            {
                return false; // silently fail
            }
        }
        private async void ShowStatusMessage(string message)
        {
            txtShowMessage.Text = message;
            await Task.Delay(2000); // show for 2 seconds
            txtShowMessage.Text = "";
        }



        private void LoadScansToday()
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                SELECT 
                    A.student_id AS 'Student ID',
                    S.full_name AS 'Student Name',
                    A.time_in AS 'Time In',
                    A.time_out AS 'Time Out',
                    A.result AS 'Status'
                FROM Attendance A
                INNER JOIN Students S ON A.student_id = S.student_id
                WHERE DATE(A.time_in) = CURDATE()
                ORDER BY A.time_in DESC";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvScansToday.DataSource = dt;
                        dgvScansToday.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading today's scans: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnAttendance_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            DashboardScreen dashboardScreen = new DashboardScreen();
            dashboardScreen.Show();
            this.Hide();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            StudentsScreen studentsScreen = new StudentsScreen();
            studentsScreen.Show();
            this.Hide();
        }

        private void HandleLogout()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                                                  "Confirm Logout",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Clear the server/session data using the centralized service
                AuthService.Logout();

                // Open the Login screen and close the current form
                LoginScreen loginScreen = new LoginScreen();
                loginScreen.Show();
                this.Close();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            HandleLogout();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        // Show message when scanned
        private void txtShowMessage_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
