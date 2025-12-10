using System;
using MySql.Data.MySqlClient;
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
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    conn.Open(); // OPEN HERE (Correct place)

                    string query = "SELECT admin_role FROM Users WHERE username=@username AND password=@password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = reader["admin_role"].ToString();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return false;
        }


        // The UsernameExists method is optional but included for completeness.
        public static bool UsernameExists(string username)
        {
            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn == null) return false;

                    string query = "SELECT COUNT(*) FROM Users WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        // ExecuteScalar returns the first column of the first row (the count)
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // This is less critical, but good practice to catch
                MessageBox.Show("Error checking username: " + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}