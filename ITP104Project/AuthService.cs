using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Windows.Forms;

namespace ITP104Project
{
    internal class AuthService
    {
        // Attempts to log in. Returns true if successful, false otherwise.
        // Outputs the user's role if login succeeds.
        public static bool Login(string username, string password, out string role)
        {
            role = string.Empty;

            try
            {
                // Use a new connection each time to avoid "connection already open" errors
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Users WHERE username = @username AND password = @password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = reader["admin_role"].ToString();
                                return true; // login successful
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return false; // login failed
        }

        // Optional: Method to check if username exists
        public static bool UsernameExists(string username)
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM Users WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking username: " + ex.Message);
                return false;
            }
        }
    }
}
