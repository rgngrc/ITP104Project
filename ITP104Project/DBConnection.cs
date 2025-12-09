using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ITP104Project
{
    internal class DBConnection
    {
        private static string connectionString = "Server=127.0.0.1;Database=dbproject;Uid=root;Pwd=password;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
