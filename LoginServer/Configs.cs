using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer
{
    /// <summary>
    /// Contains all global configurations, at a later date ini parsing will be directed to here
    /// </summary>
    public class Configs
    {
        public static string SERVER_IP = "127.0.0.1";
        public static string SITE_IP = "23.46.92.184";//ignore IP, denies requests from web server after letting it connect for the port check.
        public static int AUTH_PORT = 5817;
        public static int GAME_PORT = 5818;
        public static int LOGIN_PORT = 9958;
        public static string DATABASE_USER_NAME = "root";
        public static string DATABASE_PASSWORD = "";
        public static string DATABASE_SERVER_IP = "127.0.0.1";
        public static string DATABASE_TABLE = "conqueremu";
    }
}
