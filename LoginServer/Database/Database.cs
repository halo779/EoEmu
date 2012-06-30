using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace LoginServer.Database
{
    /// <summary>
    /// Provides connections for information retrevial from the database, which is MySQL based.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Tests the connection to the Databaase
        /// </summary>
        /// <returns>returns true/false depending on the connection status of the database</returns>
        public static bool TestConnect()
        {
            MySqlConnection Connection = DatabaseConnection.NewConnection();
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
                Connection.Dispose();
                return true;
            }
            else
            {
                Connection.Dispose();
                return false;
            }
        }
        /// <summary>
        /// Checks the password of an account
        /// </summary>
        /// <param name="Account">Account name to be checked</param>
        /// <returns>returns the password in a string or if it is wrong then the string "ERROR" will be returned</returns>
        public static string Password(string Account)
        {
            string Password = "ERROR";
            MySqlCommand Cmd = new MySqlCommand("SELECT * FROM `accounts` WHERE `AccountID` = \"" + Account + "\"", DatabaseConnection.NewConnection());
            MySqlDataReader DR = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (DR.Read())
            {
                Password = Convert.ToString(DR["Password"]);
            }
            DR.Close();
            return Password;
        }
        /// <summary>
        /// Used to set password if the password set on first login is active
        /// </summary>
        /// <param name="Account">Account name</param>
        /// <param name="Pass">given password</param>
        public static void SetPass(string Account, string Pass)
        {
            MySqlCommand Cmd = new MySqlCommand("UPDATE `accounts` SET `Password` = \"" + Pass + "\" WHERE `AccountID` = \"" + Account + "\"", DatabaseConnection.NewConnection());
            Cmd.ExecuteNonQuery();
            Cmd.Connection.Close();
            Cmd.Dispose();
        }
    }
}
