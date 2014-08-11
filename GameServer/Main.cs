using System;
using System.Threading;
using GameServer.Connections;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Eudemons C# Game Server © Hio77";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("-  Eudemons C# GameServer - Starting up!   -");
            Console.WriteLine("--------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("\nGame Server Created by Hio77");
            Console.WriteLine("This Game Server has been based off the COEMUv2 source");
            Console.WriteLine("The CoEMUv2 source can be found at: http://tinyurl.com/coemuv2");
            Console.WriteLine("\n[GameServer] Starting Server");
            Console.ResetColor();
            MainGS.StartServer();
            Console.ReadKey();
            MainGS.GameServerNano.Close();
            MainGS.AuthServer.Close();
            //TODO: Get rid of connected clients...
            //Done
            Console.WriteLine("[GameServer] Server over. Goodbye.");
        }
    }
}
