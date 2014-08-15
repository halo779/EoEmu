using System;
using EO_Proxy.Connections.Asynchronous;
using System.Collections.Generic;
using IniParser;

namespace EO_Proxy
{
    public class Program
    {
        public static string AuthAddress = "5.93.22.136";         // This is TQ's authentication ip address.
        public static string GameAddress = "";                  // Leave this blank. It will be filled automatically.
        public static string ProxyAddress = "127.0.0.1";        // This is your hosting ip address
        public static ushort WorldPort = 4446;
        public static bool ConsoleLogging = true, DebugMode = true, WriteToBytes = true;


        public static string PacketSniffingPath = @"D:\Packet Sniffing\";   // packet sniff results will be stored (by time and date).
                                                                            

        protected static void Main(string[] args)
        {
            DateTime buildDate = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
            string buildDateStr = buildDate.ToString("G");
            
            Console.Title = "EO Proxy";
            Console.WriteLine("EO Proxy =-= Build : " + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion + " (" + buildDateStr + ")\n");
            Console.WriteLine();

            Console.WriteLine("Loading Configs\n\n");
            IniParser.FileIniDataParser parser = new FileIniDataParser();
            parser.CommentDelimiter = '#';
            IniData settings = parser.LoadFile("config.ini");
            AuthAddress = settings["IP"]["AuthAddress"];
            ProxyAddress = settings["IP"]["ProxyAddress"];
            Console.WriteLine("Connecting to the Authentication server on: " + AuthAddress);
            Console.WriteLine("Proxy is redirecting game server connects to: " + ProxyAddress);

            ConsoleLogging = Convert.ToBoolean(settings["Options"]["ConsoleLogging"]);
            DebugMode = Convert.ToBoolean(settings["Options"]["DebugMode"]);
            WriteToBytes = Convert.ToBoolean(settings["Options"]["WriteToBytes"]);

            PacketSniffingPath = settings["Options"]["PacketLocation"];
            Console.WriteLine("\nPackets will be saved to: " + PacketSniffingPath);

            Console.WriteLine("\nAll Settings Loaded.\n\n\n");

            Console.WriteLine("Preparing Connections...\n");
            KeyValuePair<ushort, ushort>[] bindings = new KeyValuePair<ushort,ushort>[2];
            bindings[0] = new KeyValuePair<ushort, ushort>(4444, 9958);                     // Again, here are the ports that the client
            bindings[1] = new KeyValuePair<ushort, ushort>(4445, 9959);                     // can use to connect. Make sure that yours is
            
            foreach (KeyValuePair<ushort, ushort> bindpair in bindings)
            {
                Console.WriteLine("  Launching AuthServer [" + bindpair.Value + "] on " + bindpair.Key + "...");
                var server = new AsyncSocket(bindpair.Key, bindpair.Value);
                server.ClientConnect += new Action<AsyncWrapper>(accountServer_ClientConnect);                      // This is initializing the socket
                server.ClientReceive += new Action<AsyncWrapper, byte[]>(accountServer_ClientReceive);              // system... basic Async. This is 
                server.ClientDisconnect += new Action<object>(accountServer_ClientDisconnect);                      // for each auth port.
                server.Listen();
            }
            Console.WriteLine("  Launching GameServer [5816] on 4446...");
            var gameServer = new AsyncSocket(WorldPort, 5816);
            gameServer.ClientConnect += new Action<AsyncWrapper>(gameServer_ClientConnect);                         // This is the game server's socket
            gameServer.ClientReceive += new Action<AsyncWrapper, byte[]>(gameServer_ClientReceive);                 // system. Notice the actions... right
            gameServer.ClientDisconnect += new Action<object>(gameServer_ClientDisconnect);                         // click them and say "Go to definition".
            gameServer.Listen();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nProxy is online.\n");
            Console.ForegroundColor = ConsoleColor.White;
            while (true)
                Console.ReadLine();
        }

        private static void accountServer_ClientConnect(AsyncWrapper obj) 
        {
            Console.WriteLine("Client Connected");
            obj.StartSending(); 
        }
        private static void accountServer_ClientReceive(AsyncWrapper obj, byte[] data) 
        {
            if (Functions.PacketSniffing.Sniffing)
                Functions.PacketSniffing.Sniff(data, false);
            Connections.PacketHandler.HandleAuthentication(obj, data);
            obj.OutgoingSocket.Send(data); 
        }
        private static void accountServer_ClientDisconnect(object obj) { }

        private static void gameServer_ClientConnect(AsyncWrapper obj) 
        {
            obj.Client = new World.Client(obj.IncomingSocket, obj.Port); 
        }
        private static void gameServer_ClientReceive(AsyncWrapper obj, byte[] buffer)
        {
            World.Client client = obj.Client as World.Client;
            client.Continue = true;
        Again:
                if (client.Continue)
                {
                    Connections.PacketHandler.HandleBuffer(client, buffer, true);
                }
                else
                    goto Again;
            
        }
        private static void gameServer_ClientDisconnect(object obj)
        {
            Console.WriteLine("Disconnecting...");
            AsyncWrapper client = obj as AsyncWrapper;
            if (client != null)
            {
                if (client.IncomingSocket != null)
                    if (client.IncomingSocket.Connected)
                        client.IncomingSocket.Disconnect(false);
                if (client.OutgoingSocket != null)
                    if (client.OutgoingSocket.Connected)
                        client.OutgoingSocket.Disconnect();
                client = null;
            }
        }
    }
}
