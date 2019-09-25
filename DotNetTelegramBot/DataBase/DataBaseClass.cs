using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.DataBase
{
    class DataBaseClass
    {
        private string host;
        private int port;
        private string db;
        private string username;
        private string password;
        private MySqlConnection conn;

        public DataBaseClass(string host, int port, string db, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.db = db;
            this.username = username;
            this.password = password;
        }

        public void StartConnection()
        {
            if(conn == null)
                conn = GetDBConnection();
            try
            {
                Console.WriteLine("[DataBase]: Connecting to MySQL...");
                conn.Open();
                conn.Close();
                Console.WriteLine("[DataBase]: Connection successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DataBase] (Error): " + ex.Message);
            }
        }

        private MySqlConnection GetDBConnection()
        {
            String connString = "Server=" + host + ";Database=" + db + ";port=" + port + ";User Id=" + username + ";password=" + password;
            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
    }
}
