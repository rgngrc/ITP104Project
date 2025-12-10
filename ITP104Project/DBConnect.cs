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
                throw new Exception("Connection string 'dbprojectConnection' not found in App.config.");
            }

            return new MySqlConnection(connectionString);
        }
    }
}
