using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Databases;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities
{
    class DatabaseProvider
    {
        static MySqlConnectionStringBuilder ConnectionString;
        static MySqlConnection Connection;

        public static void Initialize()
        {
            ConnectionString = new MySqlConnectionStringBuilder()
            {
                Server = Utilities.Config.GetString("WORLD_DATABASE_SERVER"),
                UserID = Utilities.Config.GetString("WORLD_DATABASE_USER"),
                Password = Utilities.Config.GetString("WORLD_DATABASE_PASS"),
                Database = Utilities.Config.GetString("WORLD_DATABASE_NAME"),
                IgnorePrepare = false,
                MaximumPoolSize = 10,
                MinimumPoolSize = 1
            };

            Connection = new MySqlConnection(ConnectionString.ConnectionString);
            Connection.Open();
        }

        public static MySqlConnection CreateConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Broken)
            {
                Connection.Close();
                Connection.Open();

                PreparedStatements.PrepareStatements();
            }

            return Connection;
        }
    }
}