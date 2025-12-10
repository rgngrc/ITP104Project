using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            // Populate filters
            PopulateDepartments();
            PopulateYearLevels();

            // Load data grid
            LoadStudentData();
        }

        // FILTER POPULATION LOGIC

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

                        DataRow allRow = dt.NewRow();
                        allRow["dept_name"] = "All Departments";
                        allRow["dept_id"] = DBNull.Value;
                        dt.Rows.InsertAt(allRow, 0);

                        cmbDepartment.DisplayMember = "dept_name";
                        cmbDepartment.ValueMember = "dept_id";
                        cmbDepartment.DataSource = dt;

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

                        if (deptId != null && deptId != DBNull.Value)
                        {
                            cmd.Parameters.AddWithValue("@deptId", deptId);
                        }

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        DataRow allRow = dt.NewRow();
                        allRow["program_name"] = "All Programs";
                        allRow["program_id"] = DBNull.Value;
                        dt.Rows.InsertAt(allRow, 0);

                        cmbProgram.DisplayMember = "program_name";
                        cmbProgram.ValueMember = "program_id";
                        cmbProgram.DataSource = dt;

                        // Set default selection
                        if (cmbProgram.Items.Count > 0)
                        {
                            cmbProgram.SelectedIndex = 0;
                        }
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
            // List for Year Levels 
            string[] yearLevels = new string[] { "All Year Levels", "First Year", "Second Year", "Third Year", "Fourth Year" };
            cmbYearLevel.DataSource = yearLevels;
            cmbYearLevel.SelectedIndex = 0; // Set default to "All Year Levels"
        }

        // Populate Section ComboBox based on Program AND Year Level
        private void PopulateSectionsByFilters()
        {
            object selectedProgramId = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();

            cmbSection.DataSource = null;
            cmbSection.Items.Clear();

            // Define the DataTable structure for consistency
            DataTable sectionDt = new DataTable();
            sectionDt.Columns.Add("section_display_name", typeof(string));
            sectionDt.Columns.Add("section_filter_value", typeof(object));

            // Does not proceed if Program or Year Level filters are not specific
            if (selectedProgramId == null || selectedProgramId == DBNull.Value || selectedYearLevel == "All Year Levels")
            {
                DataRow allRow = sectionDt.NewRow();
                allRow["section_display_name"] = "All Sections";
                allRow["section_filter_value"] = DBNull.Value;
                sectionDt.Rows.Add(allRow);
            }
            else
            {
                // Query to pull distinct sections ONLY for the selected Program ID and Year Level
                string query = @"
                    SELECT DISTINCT student_section
                    FROM Students 
                    WHERE program_id = @progId 
                      AND year_level = @yearLevel 
                      AND student_section IS NOT NULL 
                      AND student_section <> '' 
                      AND student_section <> 'N/A'
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
                            DataTable tempDt = new DataTable();
                            adapter.Fill(tempDt);

                            DataRow allRow = sectionDt.NewRow();
                            allRow["section_display_name"] = "All Sections";
                            allRow["section_filter_value"] = DBNull.Value;
                            sectionDt.Rows.Add(allRow);

                            // Transfer the loaded data into the final DataTable, populating the display name and value
                            foreach (DataRow row in tempDt.Rows)
                            {
                                DataRow newRow = sectionDt.NewRow();
                                string sectionName = row["student_section"].ToString();
                                newRow["section_display_name"] = sectionName;
                                newRow["section_filter_value"] = sectionName;
                                sectionDt.Rows.Add(newRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading Sections: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            DataRow allRow = sectionDt.NewRow();
                            allRow["section_display_name"] = "All Sections";
                            allRow["section_filter_value"] = DBNull.Value;
                            sectionDt.Rows.Add(allRow);
                        }
                    }
                }
            }

            // Bind the ComboBox using the standardized columns
            cmbSection.DisplayMember = "section_display_name";
            cmbSection.ValueMember = "section_filter_value";
            cmbSection.DataSource = sectionDt;
            cmbSection.SelectedIndex = 0;
        }


        // DATA LOADING & FILTERING

        private void LoadStudentData()
        {
            // Get filter values
            object deptValue = cmbDepartment.SelectedValue;
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();
            object selectedSectionValue = cmbSection.SelectedValue;

            // Get the text from your search input box.
            string searchTerm = txtSearchName.Text.Trim();

            // Base query with joins to get Program and Department info
            string query = @"
                SELECT
                    S.student_id AS 'Student ID',
                    S.full_name AS 'Full Name',
                    S.year_level AS 'Year Level',
                    S.student_section AS 'Section',
                    P.program_code AS 'Program',
                    D.dept_name AS 'Department',
                    S.student_status AS 'Status',
                    S.date_of_birth AS 'Date of Birth',
                    S.nationality AS 'Nationality',
                    S.religion AS 'Religion',
                    S.sex AS 'Sex',
                    S.contact_number AS 'Contact Number',
                    S.email_address AS 'Email Address'
                FROM
                    Students S
                INNER JOIN
                    Programs P ON S.program_id = P.program_id
                INNER JOIN
                    Departments D ON P.dept_id = D.dept_id
                ";

            // Build dynamic WHERE clause
            string filterWhereClause = " WHERE 1=1 ";

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
                filterWhereClause += " AND S.year_level = @yearLevel ";
            }

            // Add Section filter
            if (selectedSectionValue != null && selectedSectionValue != DBNull.Value)
            {
                filterWhereClause += " AND S.student_section = @section ";
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // This search allows filtering by ID or Name
                filterWhereClause += " AND (S.student_id LIKE @searchTerm OR LCASE(S.full_name) LIKE LCASE(@searchTerm)) ";
            }

            query += filterWhereClause + " ORDER BY S.student_id ASC;";

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

                        // Add section parameter only if value is not DBNull.Value
                        if (selectedSectionValue != null && selectedSectionValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@section", selectedSectionValue.ToString());

                        if (!string.IsNullOrEmpty(searchTerm))
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");


                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable studentTable = new DataTable();

                        adapter.Fill(studentTable);
                        dgvStudents.DataSource = studentTable;
                        dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to retrieve student data: " + ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Database connection failed. Check your DBConnect class.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        // UI EVENT HANDLERS

        // Cascading Logic: Department changes -> Reload Programs
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbDepartment.DataSource != null)
            {
                object selectedDeptId = cmbDepartment.SelectedValue;
                PopulatePrograms(selectedDeptId);
            }
        }

        // Program changes -> Reload Sections
        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbProgram.DataSource != null)
            {
                PopulateSectionsByFilters();
            }
        }

        // Year Level changes -> Reload Sections
        private void cmbYearLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbYearLevel.DataSource != null)
            {
                PopulateSectionsByFilters();
            }
        }

        // Filter button handler
        private void btnFilter_Click(object sender, EventArgs e)
        {
            // This is the essential call to apply all filters
            LoadStudentData();
        }

        // Navigation buttons

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

        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchName.Text))
            {
                LoadStudentData();
            }
        }

        // Search btn
        private void button4_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchName.Text.Trim();

            // Load current filter values
            object deptValue = cmbDepartment.SelectedValue;
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();
            object selectedSectionValue = cmbSection.SelectedValue;

            string query = @"
        SELECT
            S.student_id AS 'Student ID',
            S.full_name AS 'Full Name',
            S.year_level AS 'Year Level',
            S.student_section AS 'Section',
            P.program_code AS 'Program',
            D.dept_name AS 'Department',
            S.student_status AS 'Status'
        FROM Students S
        INNER JOIN Programs P ON S.program_id = P.program_id
        INNER JOIN Departments D ON P.dept_id = D.dept_id
        WHERE 1=1
    ";

            // Apply filters like LoadStudentData() does
            if (deptValue != null && deptValue != DBNull.Value)
                query += " AND D.dept_id = @deptId";

            if (progValue != null && progValue != DBNull.Value)
                query += " AND P.program_id = @progId";

            if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
                query += " AND S.year_level = @yearLevel";

            if (selectedSectionValue != null && selectedSectionValue != DBNull.Value)
                query += " AND S.student_section = @section";

            // Search: ID or Name
            if (!string.IsNullOrEmpty(searchText))
                query += " AND (S.student_id LIKE @search OR LCASE(S.full_name) LIKE LCASE(@search))";

            query += " ORDER BY S.student_id ASC;";

            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        // Parameters for filters
                        if (deptValue != null && deptValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@deptId", deptValue);

                        if (progValue != null && progValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@progId", progValue);

                        if (!string.IsNullOrEmpty(selectedYearLevel) && selectedYearLevel != "All Year Levels")
                            cmd.Parameters.AddWithValue("@yearLevel", selectedYearLevel);

                        if (selectedSectionValue != null && selectedSectionValue != DBNull.Value)
                            cmd.Parameters.AddWithValue("@section", selectedSectionValue.ToString());

                        // Search parameter
                        if (!string.IsNullOrEmpty(searchText))
                            cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvStudents.DataSource = dt;
                        dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error searching student: " + ex.Message,
                            "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //Logout

        private void HandleLogout()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                                                 "Confirm Logout",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Clear the session data using the centralized service
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
    }
}