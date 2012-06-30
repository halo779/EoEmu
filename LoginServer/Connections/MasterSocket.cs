using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LoginServer.Connections
{
    /// <summary>
    /// The Master socket thread.
    /// </summary>
    public class MasterSocket
    {
        protected Socket LoginSocket;
        protected const int LOGIN_PORT = 9958;
        protected const string SERVER_IP = "10.1.1.6";
        protected const string SITE_IP = "25.86.1.1";
        protected const int AUTH_PORT = 5817;
        protected const int GAME_PORT = 5816;
        protected string ServerName;
        protected static bool Continue = true;

        public MasterSocket(string name)
        {
            LoginSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint IPE = new IPEndPoint(new IPAddress(0), LOGIN_PORT);
            try
            {
                LoginSocket.Bind(IPE);
                Console.WriteLine("[LoginServer-Int] listening on port " + LOGIN_PORT);
            }
            catch (Exception e)
            {
                Console.WriteLine("[LoginServer-Init] Failed, unable to bind server to socket.");
                Console.WriteLine(e.StackTrace);
                System.Environment.Exit(-1);
            }
        }

        public void AcceptNewConnections()
        {
            LoginSocket.Listen(100);
            while (Continue)
            {
                Socket CSocket = null;
                try
                {
                    CSocket = LoginSocket.Accept();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to accept a new connection, closing now.");
                    LoginSocket.Close();
                    Console.WriteLine(e.StackTrace);
                }
                if (CSocket != null)
                {
                    if (!CSocket.RemoteEndPoint.ToString().Contains(SITE_IP))
                    {
                        ClientSocket CS = new ClientSocket(CSocket);
                        new Thread(CS.Run).Start();
                    }
                    else
                    {
                        CSocket.Close();
                    }
                }
                System.Threading.Thread.Sleep(200);
            }
        }
        public void Close()
        {
            Console.WriteLine("[LoginServer-End] Closing LoginSocket.");
            LoginSocket.Close();
            
            Continue = false;
        }
    }
}
