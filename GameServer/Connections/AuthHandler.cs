using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Connections
{
    /// <summary>
    /// Handles Auth connections from the login server and stores data into MainGS.AuthenticatedLogins.
    /// </summary>
    public static class AuthHandler
    {
        public static void NewAuth(Socket ASocket)
        {
            byte[] Data = null;
            byte[] Buffer = new byte[1024];
            try
            {
                int size = ASocket.Receive(Buffer);
                if (size > 3)
                {
                    Data = new byte[size];
                    Array.Copy(Buffer, Data, size);
                    HandleAuth(Data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void HandleAuth(byte[] Data)
        {
            string Auth = "";
            for (int i = 0; i < Data.Length; i++)
            {
                Auth += Convert.ToChar(Data[i]);
            }
            if (Auth.StartsWith("(A)"))
            {
                string[] AuthString = Auth.Remove(0, 3).Split(',');
                if (AuthString.Length == 2)
                {
                    ConnectionRequest CR = new ConnectionRequest(10000, Convert.ToUInt64(AuthString[1]), AuthString[0]);
                }
            }
        }
    }
}
