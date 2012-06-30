using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GameServer.Connections;
using GameServer.Packets;

namespace GameServer.Connections
{
    /// <summary>
    /// The Master socket thread.
    /// </summary>
    public class MasterSocket
    {
        protected Socket ServerSocket;
        protected const int LOGIN_PORT = 9958;
        protected const string SERVER_IP = "127.0.0.1";
        protected const string SITE_IP = "88.88.88.88";
        protected const int AUTH_PORT = 5817;
        protected const int GAME_PORT = 5816;
        protected string ServerName;
        protected static bool Continue = true;

        public MasterSocket(string name)
        {
            ServerName = name;
            if (name == "GameServer")
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint IPE = new IPEndPoint(new IPAddress(0), GAME_PORT);
                try
                {
                    ServerSocket.Bind(IPE);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[" + name + "-Init] Failed, unable to bind server to socket.");
                    Console.WriteLine(e.StackTrace);
                    System.Environment.Exit(-1);
                }
            }
            else if (name == "AuthServer")
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint IPE = new IPEndPoint(new IPAddress(0), AUTH_PORT);
                try
                {
                    ServerSocket.Bind(IPE);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[" + name + "-Init] Failed, unable to bind server to socket.");
                    Console.WriteLine(e.StackTrace);
                    System.Environment.Exit(-1);
                }
            }
        }

        public void AcceptNewConnections()
        {
            ServerSocket.Listen(100);
            while (Continue)
            {
                if (ServerName == "GameServer")
                {
                    Socket CSocket = null;
                    try
                    {
                        CSocket = ServerSocket.Accept();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to accept a new connection, closing now.");
                        ServerSocket.Close();
                        Console.WriteLine(e.StackTrace);
                        return;
                    }
                    try
                    {
                        if (CSocket.Connected)
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
                    }
                    catch (Exception e)
                    {
                        if (CSocket != null)
                            CSocket.Close();
                        Console.WriteLine("[GameServer] Unable to login a new client, see exception print below.");
                        Console.WriteLine(e.ToString());
                    }
                }
                else if (ServerName == "AuthServer")
                {
                    Socket ASocket = null;
                    try
                    {
                        ASocket = ServerSocket.Accept();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to accept data from Auth Server.");
                        ServerSocket.Close();
                        Console.WriteLine(e.ToString());
                        return;
                    }
                    if (ASocket != null)
                    {
                        try
                        {
                            //AuthHandler.NewAuth(ASocket);
                            byte[] Buffer = new byte[8092];
                            int size = ASocket.Receive(Buffer);
                            byte[] Data = new byte[size];
                            Array.Copy(Buffer, Data, size);
                            AuthHandler.HandleAuth(Data);
                            Data = null;
                            Buffer = null;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                System.Threading.Thread.Sleep(200);
            }
        }
        public void Close()
        {
            Console.WriteLine("[" + ServerName + "-End] Closing Socket.");
            ServerSocket.Close();
            Continue = false;
            if (ServerName == "GameServer")
            {
                Nano.Continue = false;
            }
        }
    }
}
