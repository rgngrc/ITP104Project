using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace ITP104Project
{
    public static class DBCounters
    {
        // Fetches the count of unique students present today.
        public static int GetPresentTodayCount()
        {
            // Only count unique student_ids for the current date using time_in
            string query = @"
                SELECT COUNT(DISTINCT student_id)
                FROM Attendance
                WHERE DATE(time_in) = CURDATE();";

            return ExecuteScalarQuery(query);
        }

        // Fetches the total number of students registered in the database
        public static int GetTotalStudentsCount()
        {
            string query = "SELECT COUNT(*) FROM Students;";
            return ExecuteScalarQuery(query);
        }

        // Fetches the total number of scans (log entries/transactions) for the current month.
        public static int GetMonthlyScansCount()
        {
            // Count ALL attendance records (transactions) for the current year and month using time_in
            string query = @"
                SELECT COUNT(*)
                FROM Attendance
                WHERE YEAR(time_in) = YEAR(CURDATE()) AND MONTH(time_in) = MONTH(CURDATE());";

            return ExecuteScalarQuery(query);
        }

        // Helper method to execute a scalar query (returns a single value like COUNT)
        private static int ExecuteScalarQuery(string query)
        {
            using (MySqlConnection connection = DBConnect.GetConnection())
            {
                if (connection != null)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        // Open the connection if it's not already open
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        object result = cmd.ExecuteScalar();
                        // Handle possible DBNull or null return before conversion
                        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                    catch (Exception ex)
                    {
                        // Display error if database operation fails
                        MessageBox.Show("Database Error fetching counter: " + ex.Message,
                                        "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }
                }
                return 0; // Return 0 if connection fails
            }
        }
    }
}
