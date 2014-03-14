using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;

namespace SunDofus.Auth.Entities
{
    class DatabaseProvider
    {
        public static MySqlConnection Connection { get; set; }
        public static object Locker { get; set; }

        private static Timer timer;
        private static bool isConnected;
        private static int lastAction;

        private static int getLastActionTime
        {
            get
            {
                return (Environment.TickCount - lastAction);
            }
        }

        public static void InitializeConnection()
        {
            isConnected = false;

            Connection = new MySqlConnection();
            Locker = new object();

            Connection.ConnectionString = string.Format("server={0};uid={1};pwd='{2}';database={3}",
                    Utilities.Config.GetString("REALM_DATABASE_SERVER"),
                    Utilities.Config.GetString("REALM_DATABASE_USER"),
                    Utilities.Config.GetString("REALM_DATABASE_PASS"),
                    Utilities.Config.GetString("REALM_DATABASE_NAME"));

            lock (Locker)
                Connection.Open();

            isConnected = true;
            lastAction = Environment.TickCount;

            Utilities.Logger.Write(Utilities.Logger.LoggerType.Infos, "Connected to the Realms' Database !");

            Requests.AccountsRequests.ResetConnectedValue();
            Requests.ServersRequests.LoadCache();
        }

        public static void CheckConnection()
        {
            if (!isConnected)
                ReConnect();

            lastAction = Environment.TickCount;
        }

        private static void ReConnect()
        {
        }

        private static void UpdateConnection(object sender, EventArgs e)
        {
        }
    }
}
