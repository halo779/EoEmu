using System;
using System.IO;
using System.Net.Sockets;

namespace LoginServer.Connections
{
    // <summary>
    /// Description of AuthSocket.
    /// </summary>
    public static class AuthSocket
    {
        private static Socket Auth;
        private const string SERVER_IP = "127.0.0.1";
        public const string NANO_IP = "70.190.77.101";
        private const int AUTH_PORT = 5817;

        public static bool Authorize(string user, uint key, bool nano)
        {
            try
            {
                Auth = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (!nano)
                {
                    try
                    {
                        Auth.Connect(SERVER_IP, AUTH_PORT);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to send client data to gameserver");
                    }
                }
                else
                {
                    Auth.Connect(NANO_IP, AUTH_PORT);
                }
                Auth.Send(System.Text.Encoding.ASCII.GetBytes("(A)" + user + "," + key));
                Auth.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Exception Cought while connecting to game server: " + e.ErrorCode.ToString());
                return false;
            }
            return true;
        }
    }
}
