using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;
namespace GameServer.Handlers
{
    public partial class Handler
    {
        public static void NewCharacter(byte[] Packet, ClientSocket CSocket)
        {
            int Mesh = PacketProcessor.ReadLong(Packet, 100);
            int Class = Packet[104];
            string Name = "";
            int X = 20;
            while (X < 36 && Packet[X] != 0x00)
            {
                Name += Convert.ToChar(Packet[X]);
                X++;
            }
            if (!ValidName(Name))
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "Error: Invalid character name.", Struct.ChatType.Dialog));
            else if (Database.Database.CharExists(Name))
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "Error: Character already exists.", Struct.ChatType.Dialog));
            else
            {
                Console.WriteLine("NewCharacter");
                int CharID = Database.Database.NewCharacter(Name, Mesh, Class, CSocket.AccountName);
                if (CharID > -1)
                {
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "Characted created! Please hit back twice and then re-log in to the game!", Struct.ChatType.Dialog));
                    CSocket.Disconnect();
                }
                else
                {
                    Console.WriteLine("CharID: " + CharID);
                    CSocket.Disconnect();
                }
            }
        }
        public static bool ValidName(string Name)
        {
            if (Name.ToLower().Contains("gm") || Name.ToLower().Contains("pm"))
                return false;
            else if (Name.IndexOfAny(new char[8] { ' ', '[', ']', '#', '*', '{', '(', ')' }) > -1)
                return false;
            else
                return true;
        }
    }
}
