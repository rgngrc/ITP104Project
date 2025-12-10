using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;

namespace ITP104Project
{

    public static class CurrentSession
    {
        // Store the role of the logged-in user for permission checks later
        public static string UserRole { get; set; } = string.Empty;
    }

    public static class AuthService
    {
        // Attempts to log in. Returns true if successful, false otherwise.
        public static bool Login(string username, string password, out string role)
        {
            role = string.Empty;

            try
            {
                using (MySqlConnection conn = DBConnect.GetConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

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

                                // SESSION START
                                CurrentSession.UserRole = role;

                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error during login: " + ex.Message, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        // Logs out the current user by clearing session data
        public static void Logout()
        {
            // Clear the static session variables that track the logged-in user
            CurrentSession.UserRole = string.Empty;
        }
    }
}