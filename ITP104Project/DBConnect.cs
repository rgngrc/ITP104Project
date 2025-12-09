using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace ITP104Project
{
    public static class DBConnect
    {
        private static string connectionString =
            ConfigurationManager.ConnectionStrings["dbprojectConnection"]?.ConnectionString;

        public static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'dbprojectConnection' not found in App.config.",
                                "Configuration Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return null;
            }

            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connection Failed! Please check server status and credentials. Error: " + ex.Message,
                                "FATAL CONNECTION ERROR",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return null;
            }
        }

    }
}