using System;
using MySql.Data.MySqlClient;

namespace GameServer.Database
{
    /// <summary>
    /// Provides MySql resource connections, for multiple connections to a single MySql database.
    /// This is due to the fact that the server is multi-threaded, so allowing a single connection would
    /// likely result in errors, or general loss of performance.
    /// </summary>
    public static class DatabaseConnection
    {
        public const string USER_NAME = "root";
        public const string PASSWORD = "";
        public const string SERVER = "127.0.0.1";
        public const string DATA_BASE = "conqueremu";
        public static MySqlConnection DBConnection = null;
        public static MySqlConnection NewConnection()
        {
            MySqlConnection C = null;
            try
            {
                C = new MySqlConnection("Server=" + SERVER + ";Database='" + DATA_BASE + "';Username='" + USER_NAME + "';Password='" + PASSWORD + "'");
                C.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            return C;
        }
    }
}
