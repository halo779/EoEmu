using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using GameServer.Connections;
using GameServer.Structs;
using GameServer.Entities;
using OpenSSL;

namespace GameServer
{
    /// <summary>
    /// Nano, CoEmu v2's Leveling server 'nickname' is hence forth the primary location of threaded operations.
    /// This class holds all data which is to be shared amongst connected players, that being said
    /// it is here entirely to be a 'dataholder' as seen in CoEmu v2's initial release.
    /// </summary>
    public static class Nano
    {
        public const int VIEW_THRESHOLD = 18;
        public static string PoleHolder = "None";
        public static bool Continue = true;
        public static Dictionary<int, ClientSocket> ClientPool = new Dictionary<int, ClientSocket>();
        //public static Dictionary<string, ClientSocket> NewCharacters = new Dictionary<string, ClientSocket>();
        public static Dictionary<ulong, ConnectionRequest> AuthenticatedLogins = new Dictionary<ulong, ConnectionRequest>();
        public static Dictionary<int, Struct.ItemData> Items = new Dictionary<int, Struct.ItemData>();
        public static Dictionary<int, Struct.ItemPlusDB> ItemPluses = new Dictionary<int, Struct.ItemPlusDB>();
        public static Dictionary<int, MonsterInfo> BaseMonsters = new Dictionary<int, MonsterInfo>();
        public static Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();
        public static Dictionary<int, MonsterSpawn> MonsterSpawns = new Dictionary<int, MonsterSpawn>();
        public static Dictionary<int, Struct.NPC> Npcs = new Dictionary<int, Struct.NPC>();
        public static Dictionary<string, Struct.Portal> Portals = new Dictionary<string, Struct.Portal>();
        public static Dictionary<int, Struct.ItemGround> ItemFloor = new Dictionary<int, Struct.ItemGround>();
        public static Dictionary<int, Struct.DmapData> Maps = new Dictionary<int, Struct.DmapData>();
        public static Dictionary<int, Struct.ServerSkill> ServerSkills = new Dictionary<int, Struct.ServerSkill>();
        public static Dictionary<int, Struct.TerrainNPC> TerrainNpcs = new Dictionary<int, Struct.TerrainNPC>();
        public static MasterSocket GameServerNano;
        public static MasterSocket AuthServer;
        public static System.Random Rand = new System.Random();
        public static int TintR = 0;
        public static int TintG = 0;
        public const int EXP_MULTIPLER = 6;
        public const int PROF_MULTIPLER = 6;
        public const int SKILL_MULTIPLER = 4;
        public static System.Timers.Timer Shutdown;

        public static void StartServer()
        {
            int startload = System.Environment.TickCount;
            Struct.LoadItemType("itemtype.dat");
            Struct.LoadItemPlusesDatabase();
            Struct.LoadMaps();
            Struct.LoadMonsters();
            Struct.LoadNpcs();
            Struct.LoadPortals();
            Struct.LoadServerskills();
            Struct.LoadTNpcs();
            Console.WriteLine("[GameServer] Loaded all config files and data in " + (System.Environment.TickCount - startload) + "MS");
            //Database.Database.PurgeGuilds();

            Console.WriteLine("[GameServer-Init] Creating Game Thread..");
            Nano.GameServerNano = new MasterSocket("GameServer");
            new Thread(Nano.GameServerNano.AcceptNewConnections).Start();
            Console.WriteLine("[GameServer-Init](Game Thread) Success.");
            Console.WriteLine("[GameServer-Init] Creating Auth Thread..");
            Nano.AuthServer = new MasterSocket("AuthServer");
            new Thread(Nano.AuthServer.AcceptNewConnections).Start();
            Console.WriteLine("[GameServer-Init](Auth Thread) Success.");
            ConsoleCommands();
        }
        public static void ConsoleCommands()
        {
            Console.WriteLine("Ready for commands.");
            string command = Console.ReadLine();
            if (command == "#end")
            {
                /*Shutdown = new System.Timers.Timer();
                            Shutdown.Interval = 180000;
                            Shutdown.AutoReset = false;
                            Shutdown.Elapsed += delegate { Kill(); };
                            Shutdown.Start();*/
                GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 3 minutes.", GameServer.Structs.Struct.ChatType.Talk), 0);
                Kill();
            }
            else
            {
                Console.WriteLine("Unknown command.");
                ConsoleCommands();
            }
        }
        public static void Kill()
        {
            GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 5 seconds.", GameServer.Structs.Struct.ChatType.Talk), 0);
            Thread.Sleep(1000);
            GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 4 seconds.", GameServer.Structs.Struct.ChatType.Talk), 0);
            Thread.Sleep(1000);
            GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 3 seconds.", GameServer.Structs.Struct.ChatType.Talk), 0);
            Thread.Sleep(1000);
            GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 2 seconds.", GameServer.Structs.Struct.ChatType.Talk), 0);
            Thread.Sleep(1000);
            GameServer.Packets.EudemonPacket.ToServer(GameServer.Packets.EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[GameServer] Shutting down in 1 second.", GameServer.Structs.Struct.ChatType.Talk), 0);
            Nano.AuthServer.Close();
            Nano.GameServerNano.Close();
            System.Environment.Exit(1);
        }
    }
}
