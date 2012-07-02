using System;
using System.Threading;
using LoginServer.Database;

namespace LoginServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Eudemons C# Login Server © Hio77";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("-  Eudemons C# LoginServer - Starting up!  -");
            Console.WriteLine("--------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("\nAccount Server Created by Hio77");
            Console.WriteLine("This Account Server has been based off the COEMUv2 source");
            Console.WriteLine("COEMUv2 was created by andyd123");
            Console.WriteLine("The CoEMUv2 source can be found at: http://tinyurl.com/coemuv2");
            Console.WriteLine("\nThis is made to work with the game server created by hio77 not any eo binary!");

            Console.WriteLine("[LoginServer] Starting MasterSocket Thread.");
            Connections.MasterSocket LoginSocket = new Connections.MasterSocket("LoginServer");
            new Thread(LoginSocket.AcceptNewConnections).Start();
            Console.WriteLine("[LoginServer] Init OK.");
            Console.WriteLine("[LoginServer] Connecting to Mysql.");
            if (Database.Database.TestConnect())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Database] Connection OK.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Database] Connection FAIL.");
                LoginSocket.Close();
                Console.WriteLine("[LoginServer-End-ERROR] Mastersocket closed, thread killed.");
                Console.WriteLine("[LoginServer-End-ERROR] Server Shutdown/n");
                Console.ResetColor();
            }
            Console.WriteLine("[LoginServer] Searching for the Game Server....");
            Console.WriteLine("[LoginServer] Press enter to kill the server.");
            Console.Read();
            LoginSocket.Close();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[LoginServer-End] Mastersocket closed, thread killed.");
            Console.WriteLine("[LoginServer-End] Server Shutdown");

        }
    }
}
