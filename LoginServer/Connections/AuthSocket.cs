using System;
using System.IO;
using System.Net.Sockets;

namespace LoginServer.Connections
{
    /// <summary>
    /// Description of AuthSocket.
    /// </summary>
    public static class AuthSocket
    {
        private static Socket Auth;

        public static bool Authorize(string user, uint key)
        {
            try
            {
                Auth = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    Auth.Connect(Configs.SERVER_IP, Configs.AUTH_PORT);
                }
                catch
                {
                    Console.WriteLine("Failed to send client data to game server");
                }

                Auth.Send(System.Text.Encoding.ASCII.GetBytes("(A)" + user + "," + key));
                Auth.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Exception Caught while connecting to game server: " + e.ErrorCode.ToString());
                return false;
            }
            return true;
        }
    }
}
