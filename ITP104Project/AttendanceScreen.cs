using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;

namespace ITP104Project
{
    public partial class AttendanceScreen : Form
    {
        // Constructor and Load Event
        public AttendanceScreen()
        {
            InitializeComponent();
            this.Load += new EventHandler(AttendanceScreen_Load);
        }

        private void AttendanceScreen_Load(object sender, EventArgs e)
        {
            // 1. Populate static/independent filters
            PopulateDepartments();
            PopulateYearLevels();

            // 2. Load attendance data for today (or selected date) by default
            // Assumes dtpFilterDate is named correctly.
            LoadAttendanceData(dateTimePicker1.Value.Date);
        }

        // --- DATABASE CONNECTION AND DATA LOGIC ---

        private void PopulateDepartments()
        {
            string query = "SELECT dept_name, dept_id FROM Departments ORDER BY dept_name;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Add a default "All" option
                        DataRow allRow = dt.NewRow();
                        allRow["dept_name"] = "All Departments";
                        allRow["dept_id"] = DBNull.Value;
                        dt.Rows.InsertAt(allRow, 0);

                        cmbDepartment.DisplayMember = "dept_name";
                        cmbDepartment.ValueMember = "dept_id";
                        cmbDepartment.DataSource = dt;

                        // Set the default selection to trigger the Program load immediately
                        cmbDepartment.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading Departments: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PopulatePrograms(object deptId = null)
        {
            // Clear and prepare
            cmbProgram.DataSource = null;
            cmbProgram.Items.Clear();

            // Build dynamic query
            string query = "SELECT program_name, program_id FROM Programs ";
            string whereClause = "";

            if (deptId != null && deptId != DBNull.Value)
            {
                whereClause = "WHERE dept_id = @deptId ";
            }

            query += whereClause + "ORDER BY program_name;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        // Add parameter only if necessary
                        if (deptId != null && deptId != DBNull.Value)
                        {
                            cmd.Parameters.AddWithValue("@deptId", deptId);
                        }

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Add a default "All" option
                        DataRow allRow = dt.NewRow();
                        allRow["program_name"] = "All Programs";
                        allRow["program_id"] = DBNull.Value;
                        dt.Rows.InsertAt(allRow, 0);

                        cmbProgram.DisplayMember = "program_name";
                        cmbProgram.ValueMember = "program_id";
                        cmbProgram.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading Programs: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PopulateYearLevels()
        {
            // Static list for Year Levels
            string[] yearLevels = new string[] { "All Year Levels", "First Year", "Second Year", "Third Year", "Fourth Year" };
            cmbYearLevel.DataSource = yearLevels;
        }

        private void PopulateSectionsByFilters()
        {
            object selectedProgramId = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();

            cmbSection.DataSource = null;
            cmbSection.Items.Clear();

            // Check 1: Do not proceed if Program or Year Level filters are not specific
            if (selectedProgramId == null || selectedProgramId == DBNull.Value || selectedYearLevel == "All Year Levels")
            {
                cmbSection.Items.Add("Select Program/Year");
                cmbSection.SelectedIndex = 0;
                return;
            }

            // Query to pull sections ONLY for the selected Program ID and Year Level
            string query = @"
                SELECT DISTINCT student_section 
                FROM Students 
                WHERE program_id = @progId AND year_level = @yearLevel AND student_section <> 'N/A'
                ORDER BY student_section;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@progId", selectedProgramId);
                        cmd.Parameters.AddWithValue("@yearLevel", selectedYearLevel);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Add an "All Sections" option
                        DataRow allRow = dt.NewRow();
                        allRow["student_section"] = "All Sections";
                        dt.Rows.InsertAt(allRow, 0);

                        cmbSection.DisplayMember = "student_section";
                        cmbSection.ValueMember = "student_section";
                        cmbSection.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading Sections: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadAttendanceData(DateTime filterDate)
        {
            // Get filter values
            object deptValue = cmbDepartment.SelectedValue;
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();
            string selectedSection = cmbSection.SelectedItem?.ToString();

            // Start of the dynamic WHERE clause (filtering by date is mandatory)
            string filterWhereClause = " WHERE DATE(A.timestamp) = @filterDate ";

            // Add Department filter
            if (deptValue != null && deptValue != DBNull.Value)
            {
                filterWhereClause += " AND P.dept_id = @deptId ";
            }

            // Add Program filter
            if (progValue != null && progValue != DBNull.Value)
            {
                filterWhereClause += " AND P.program_id = @progId ";
            }

            // Add Year Level filter
            if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
            {
                filterWhereClause += " AND S.year_level = @yearLevel ";
            }

            // Add Section filter
            if (!string.IsNullOrEmpty(selectedSection) && selectedSection != "All Sections" && selectedSection != "Select Program/Year")
            {
                filterWhereClause += " AND S.student_section = @section ";
            }

            // Main query
            string query = $@"
                SELECT
                    S.student_id AS 'Student ID',
                    S.full_name AS 'Student Name',
                    S.year_level AS 'Year Level',
                    P.program_code AS Program,
                    DATE_FORMAT(A.timestamp, '%Y-%m-%d') AS Date,
                    TIME(A.timestamp) AS 'Time In/Out'
                FROM Students S
                INNER JOIN Programs P ON S.program_id = P.program_id
                INNER JOIN Attendance A ON S.student_id = A.student_id
                {filterWhereClause}
                ORDER BY A.timestamp DESC;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        // Add parameters based on the filters included in the WHERE clause
                        cmd.Parameters.AddWithValue("@filterDate", filterDate.ToString("yyyy-MM-dd"));

                        if (deptValue != null && deptValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@deptId", deptValue);
                        if (progValue != null && progValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@progId", progValue);
                        if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
                            cmd.Parameters.AddWithValue("@yearLevel", selectedYearLevel);
                        if (!string.IsNullOrEmpty(selectedSection) && selectedSection != "All Sections" && selectedSection != "Select Program/Year")
                            cmd.Parameters.AddWithValue("@section", selectedSection);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable attendanceTable = new DataTable();
                        adapter.Fill(attendanceTable);

                        // Display the data
                        dgvAttendanceRecords.DataSource = attendanceTable;
                        dgvAttendanceRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading attendance data: " + ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        // --- UI EVENT HANDLERS ---

        // Cascading Logic: Department changes -> Reload Programs
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedDeptId = cmbDepartment.SelectedValue;
            PopulatePrograms(selectedDeptId);
        }

        // Cascading Logic: Program changes -> Reload Sections
        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSectionsByFilters();
        }

        // Cascading Logic: Year Level changes -> Reload Sections
        private void cmbYearLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSectionsByFilters();
        }

        // Main Filter Button Click: Load data based on all selections including the DatePicker
        private void btnFilter_Click(object sender, EventArgs e)
        {
            // Get the date directly from the DateTimePicker control
            DateTime selectedDate = dateTimePicker1.Value.Date;

            LoadAttendanceData(selectedDate);
        }

        // --- Navigation and Unused Handlers ---

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

        private void button5_Click(object sender, EventArgs e)
        {
            StudentsScreen studentsScreen = new StudentsScreen();
            studentsScreen.Show();
            this.Hide();
        }

        // Unused handlers (Keep these to avoid designer errors)
        private void label5_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void cmbSection_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}