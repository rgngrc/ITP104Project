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
    public partial class StudentsScreen : Form
    {
        public StudentsScreen()
        {
            InitializeComponent();
            this.Load += new EventHandler(StudentsScreen_Load);
        }

        private void StudentsScreen_Load(object sender, EventArgs e)
        {
            // 1. Populate initial independent filters
            PopulateDepartments();
            PopulateYearLevels();
            // PopulatePrograms and PopulateSections will be triggered by cmbDepartment.SelectedIndex = 0.

            // 2. Load all student data initially (no filters applied)
            LoadStudentData();
        }

        // --- FILTER POPULATION LOGIC ---

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

                        // Setting SelectedIndex to 0 triggers cmbDepartment_SelectedIndexChanged
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
            // Clear and prepare Program ComboBox
            cmbProgram.DataSource = null;
            cmbProgram.Items.Clear();

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

                        // FIX: Detach the handler to prevent it from firing prematurely
                        cmbProgram.SelectedIndexChanged -= cmbProgram_SelectedIndexChanged;

                        cmbProgram.DisplayMember = "program_name";
                        cmbProgram.ValueMember = "program_id";
                        cmbProgram.DataSource = dt;

                        // Set initial selection
                        cmbProgram.SelectedIndex = 0;

                        // Re-attach the handler
                        cmbProgram.SelectedIndexChanged += cmbProgram_SelectedIndexChanged;

                        // Call PopulateSections now that cmbProgram.SelectedValue is stable
                        PopulateSections();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading Programs: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PopulateSections()
        {
            // Clear and prepare Section ComboBox
            cmbSection.DataSource = null;
            cmbSection.Items.Clear();

            // Get the current filters for Program and Year Level to constrain the sections
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();

            // SQL to find distinct sections based on current Program and Year Level filters
            string query = "SELECT DISTINCT student_section FROM Students WHERE student_section IS NOT NULL AND student_section != '' ";
            string filterWhereClause = "";

            // Check if a specific Program is selected
            if (progValue != null && progValue != DBNull.Value)
            {
                filterWhereClause += " AND program_id = @progId ";
            }
            // Check if a specific Year Level is selected
            if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
            {
                filterWhereClause += " AND year_level = @yearLevel ";
            }

            query += filterWhereClause + " ORDER BY student_section;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        if (progValue != null && progValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@progId", progValue);
                        if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
                            cmd.Parameters.AddWithValue("@yearLevel", selectedYearLevel);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Add a default "All" option
                        DataRow allRow = dt.NewRow();
                        allRow["student_section"] = "All Sections";
                        dt.Rows.InsertAt(allRow, 0);

                        cmbSection.DisplayMember = "student_section";
                        cmbSection.ValueMember = "student_section";
                        cmbSection.DataSource = dt;
                        cmbSection.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading Sections: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // --- DATA LOADING & FILTERING ---

        private void LoadStudentData()
        {
            // Get filter values
            object deptValue = cmbDepartment.SelectedValue;
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();
            string selectedSection = cmbSection.SelectedItem?.ToString();

            // Base query with joins to get Program info
            string query = @"
        SELECT
            S.student_id AS 'ID No.',
            S.full_name AS 'Full Name',
            S.year_level AS 'Year Level',
            S.student_section AS 'Section',
            P.program_code AS 'Program',
            D.dept_name AS 'Department',
            S.student_status AS 'Status'
        FROM Students S
        INNER JOIN Programs P ON S.program_id = P.program_id
        INNER JOIN Departments D ON P.dept_id = D.dept_id
        ";

            // Build dynamic WHERE clause
            string filterWhereClause = " WHERE 1=1 ";
            // WHERE 1=1 allows us to start every filter condition with " AND "

            // Add Department filter
            if (deptValue != null && deptValue != DBNull.Value)
            {
                filterWhereClause += " AND D.dept_id = @deptId ";
            }

            // Add Program filter
            if (progValue != null && progValue != DBNull.Value)
            {
                filterWhereClause += " AND P.program_id = @progId ";
            }

            // Add Year Level filter
            if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
            {
                // Use student_id != 'N/A' filter if you only want sectioned students
                filterWhereClause += " AND S.year_level = @yearLevel ";
            }

            // Add Section filter
            if (!string.IsNullOrEmpty(selectedSection) && selectedSection != "All Sections")
            {
                filterWhereClause += " AND S.student_section = @section ";
            }
            // IMPORTANT NOTE: If a student's section is 'N/A' (Irregular), 
            // they won't show up if a specific section is chosen. They *will* show up 
            // if 'All Sections' is chosen. This is correct behavior based on your logic.


            query += filterWhereClause + " ORDER BY S.student_id ASC;";

            // --- DEBUG STEP 1: Show the final query ---
            MessageBox.Show("Final SQL Query: \n" + query, "Query Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // ------------------------------------------

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        // Add parameters only if filters are applied
                        if (deptValue != null && deptValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@deptId", deptValue);
                        if (progValue != null && progValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@progId", progValue);
                        if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
                            cmd.Parameters.AddWithValue("@yearLevel", selectedYearLevel);

                        // Add Section Parameter
                        if (!string.IsNullOrEmpty(selectedSection) && selectedSection != "All Sections")
                            cmd.Parameters.AddWithValue("@section", selectedSection);


                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable studentTable = new DataTable();

                        adapter.Fill(studentTable);
                        dgvStudents.DataSource = studentTable;
                        dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // --- DEBUG STEP 2: Check for zero rows ---
                        if (studentTable.Rows.Count == 0)
                        {
                            MessageBox.Show("Filter returned 0 students. Check your selection criteria.", "No Data Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        // ------------------------------------------
                    }
                    catch (Exception ex)
                    {
                        // IMPORTANT: This will catch connection and SQL syntax errors!
                        MessageBox.Show("Failed to retrieve student data: " + ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // --- DEBUG STEP 3: Check for connection failure ---
                    MessageBox.Show("Database connection failed. Check your DBConnect class.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    // --------------------------------------------------
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

        // Program change -> Reload Sections
        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSections();
        }

        // Year Level change -> Reload Sections
        private void cmbYearLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSections();
        }

        // Section change handler (optional auto-refresh)
        private void cmbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // LoadStudentData(); // Uncomment this line for auto-refresh on section change
        }

        // Filter button handler (This should be connected to the btnFilter control)
        private void btnFilter_Click(object sender, EventArgs e)
        {
            // This is the essential call to apply all filters
            LoadStudentData();
        }

        // --- NAVIGATION HANDLERS ---

        private void button2_Click(object sender, EventArgs e)
        {
            DashboardScreen dashboardScreen = new DashboardScreen();
            dashboardScreen.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AttendanceScreen attendanceScreen = new AttendanceScreen();
            attendanceScreen.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScanScreen scanScreen = new ScanScreen();
            scanScreen.Show();
            this.Hide();
        }

        // --- UNUSED/DESIGNER HANDLERS ---
        private void button5_Click(object sender, EventArgs e) { }
        private void pictureBox5_Click(object sender, EventArgs e) { }
        private void pictureBox4_Click(object sender, EventArgs e) { }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}