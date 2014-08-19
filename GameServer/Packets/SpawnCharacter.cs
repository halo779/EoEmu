using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// Description of SpawnCharacter.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] SpawnCharacter(ClientSocket CSocket)
        {
            PacketBuilder Packet = new PacketBuilder(1014, 89 + CSocket.Client.Name.Length);
            Packet.Long(CSocket.Client.ID);
            if (!CSocket.Client.Dead)
            {
                Packet.Long(CSocket.Client.Model);
                uint Status = 0;
                if (CSocket.Client.PkPoints >= 100)
                    Status += 0x8000;
                else if (CSocket.Client.PkPoints >= 30 && CSocket.Client.PkPoints < 100)
                    Status += 0x4000;
                if (CSocket.Client.Flashing)
                    Status += 0x1;
                if (CSocket.Client.Team != null)
                {
                    if (CSocket.Client.Team.LeaderID == CSocket.Client.ID)
                        Status += 0x40;
                }
                if (CSocket.Client.Flying)
                    Status += 0x8000000;
                Packet.Long(Status);
            }
            else
            {
                Packet.Long(CSocket.Client.GhostModel);
                Packet.Long(1024);
            }
            //Packet.Long(0); //TODO: Status See above.
            /*if (CSocket.Client.isPM)
                Packet.Long(32768);
            else if (CSocket.Client.isGM && CSocket.Client.Invincible)
                Packet.Long(1);
            else*/
                Packet.Long(0);
            //Packet.Short(0); //TODO: Guilds
            //Packet.Int(0); //GuildRank
            Packet.Long(0); //Unknown
            Packet.Long(0);
            Packet.Long(0);
            if (!CSocket.Client.Dead && !CSocket.Client.Transformed)
            {
                int garment = 0;
                int weapon = 0;
                /*foreach (KeyValuePair<int, Struct.ItemInfo> Item in CSocket.Client.Equipment)
                {
                    if (Item.Value.Position == 1)
                        Head = Item.Value.ItemID;
                    else if (Item.Value.Position == 3 && Armor == 0)
                        Armor = Item.Value.ItemID;
                    else if (Item.Value.Position == 4)
                        RH = Item.Value.ItemID;
                    else if (Item.Value.Position == 5)
                        LH = Item.Value.ItemID;
                    else if (Item.Value.Position == 9)
                        Armor = Item.Value.ItemID;


                }*/
                Packet.Long(garment);
                Packet.Long(weapon);
              }
            else
            {
                Packet.Long(0); //Headgear
                Packet.Long(0); //Armor or garment
                
            }
            Packet.Long(0);
            Packet.Long(0);
            Packet.Short(CSocket.Client.X);
            Packet.Short(CSocket.Client.Y);
            Packet.Short(CSocket.Client.Hair);
            Packet.Short(0);
            Packet.Long(Timer);
            Packet.Long(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Long(0); //Unknown
            Packet.Short(0);
            Packet.Int(0);
            Packet.Int(1); //String count
            Packet.Int(CSocket.Client.Name.Length);
            Packet.Text(CSocket.Client.Name);
            return Packet.getFinal();
        }

        
    }
}
