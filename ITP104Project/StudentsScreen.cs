using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;

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

            // Note: LoadStudentData is called here, and it will initially load ALL students 
            // because all combo boxes default to "All..." and pass DBNull.Value.
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

                        // Setting SelectedIndex to 0 triggers cmbDepartment_SelectedIndexChanged, 
                        // which then calls PopulatePrograms(), setting up the cascade.
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

                        // Set default selection. This should trigger cmbProgram_SelectedIndexChanged 
                        // which then calls PopulateSectionsByFilters().
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
            // Static list for Year Levels (No DB connection needed)
            string[] yearLevels = new string[] { "All Year Levels", "First Year", "Second Year", "Third Year", "Fourth Year" };
            cmbYearLevel.DataSource = yearLevels;
            cmbYearLevel.SelectedIndex = 0; // Set default to "All Year Levels"
        }

        // --- CORRECTED METHOD for Section ComboBox (Populate based on Program AND Year Level) ---
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

            // Check 1: Do not proceed if Program or Year Level filters are not specific
            if (selectedProgramId == null || selectedProgramId == DBNull.Value || selectedYearLevel == "All Year Levels")
            {
                // Populate with just "All Sections"
                DataRow allRow = sectionDt.NewRow();
                allRow["section_display_name"] = "All Sections";
                allRow["section_filter_value"] = DBNull.Value; // Use DBNull.Value to signify "All"
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

                            // Add the "All Sections" option at the top
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
                                newRow["section_filter_value"] = sectionName; // The actual section string is the filter value
                                sectionDt.Rows.Add(newRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading Sections: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // Add "All Sections" as fallback on error
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


        // --- CORRECTED METHOD for Data Loading & Filtering ---

        private void LoadStudentData()
        {
            // Get filter values
            object deptValue = cmbDepartment.SelectedValue;
            object progValue = cmbProgram.SelectedValue;
            string selectedYearLevel = cmbYearLevel.SelectedItem?.ToString();
            object selectedSectionValue = cmbSection.SelectedValue;

            // 🆕 GET SEARCH TERM: Get the text from your search input box.
            // Assuming your search TextBox is named 'txtSearchName'.
            string searchTerm = txtSearchName.Text.Trim();

            // Base query with joins to get Program and Department info
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

            // Add Section filter (check if value is NOT DBNull.Value)
            if (selectedSectionValue != null && selectedSectionValue != DBNull.Value)
            {
                filterWhereClause += " AND S.student_section = @section ";
            }

            // 🆕 ADD NAME SEARCH FILTER: Uses LIKE and wildcard % for partial matching, case-insensitive.
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // We use LCASE(S.full_name) and LCASE(@searchTerm) for a case-insensitive search.
                filterWhereClause += " AND LCASE(S.full_name) LIKE LCASE(@searchTerm) ";
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

                        // 🆕 ADD SEARCH PARAMETER: Wrap search term in '%' for LIKE
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

        // --- UI EVENT HANDLERS (for cascading filters) ---

        // Cascading Logic: Department changes -> Reload Programs
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbDepartment.DataSource != null)
            {
                object selectedDeptId = cmbDepartment.SelectedValue;
                PopulatePrograms(selectedDeptId);
                // No need to call LoadStudentData here, as changing the Department automatically 
                // resets the Program which resets the Section, then LoadStudentData should be 
                // called on btnFilter_Click.
            }
        }

        // ⚠️ NEW/CORRECTED: Program changes -> Reload Sections
        private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbProgram.DataSource != null)
            {
                PopulateSectionsByFilters();
            }
        }

        // ⚠️ NEW/CORRECTED: Year Level changes -> Reload Sections
        private void cmbYearLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only run if the DataSource is not null
            if (cmbYearLevel.DataSource != null)
            {
                PopulateSectionsByFilters();
            }
        }

        // Section change handler
        private void cmbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If you want the grid to auto-refresh whenever the section changes, uncomment this:
            // LoadStudentData(); 
        }

        // 🆕 NEW EVENT HANDLER: Live search when typing
        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            // Reloads the student grid every time the user types a character
            LoadStudentData();
        }

        // Filter button handler
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

    }
}