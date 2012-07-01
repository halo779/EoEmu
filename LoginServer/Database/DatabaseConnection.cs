using System;
using MySql.Data.MySqlClient;

namespace LoginServer.Database
{
    /// <summary>
    /// Provides MySql resource connections, for multiple connections to a single MySql database.
    /// This is due to the fact that the server is multi-threaded, so allowing a single connection would
    /// likely result in errors, or general loss of performance.
    /// </summary>
    public static class DatabaseConnection
    {
        public static MySqlConnection NewConnection()
        {
            MySqlConnection C = null;
            try
            {
                C = new MySqlConnection("Server=" + Configs.DATABASE_SERVER_IP + ";Database='" + Configs.DATABASE_TABLE + "';Username='" + Configs.DATABASE_USER_NAME + "';Password='" + Configs.DATABASE_PASSWORD + "'");
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
