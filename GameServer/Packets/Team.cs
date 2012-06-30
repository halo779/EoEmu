using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Packets
{
    public partial class ConquerPacket
    {
        public static byte[] Team(int UID, Struct.TeamOption T)
        {
            PacketBuilder Packet = new PacketBuilder(1023, 12);
            Packet.Long((int)T);
            Packet.Long(UID);
            return Packet.getFinal();
        }
        public unsafe static byte[] TeamMember(Character Player)
        {
            ushort PacketType = 0x402;
            uint Model = (uint)Player.Model;
            string tqs = "TQServer";
            byte[] Packet = new byte[36 + tqs.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)(Packet.Length - tqs.Length);
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 5) = 1;

                for (int i = 0; i < Player.Name.Length; i++)
                {
                    *(p + 8 + i) = Convert.ToByte(Player.Name[i]);
                }
                *((uint*)(p + 24)) = (uint)Player.ID;
                *((uint*)(p + 28)) = (uint)Model;
                *((ushort*)(p + 32)) = (ushort)Player.MaxHP;
                *((ushort*)(p + 34)) = (ushort)Player.CurrentHP;
                for (int i = 0; i < tqs.Length; i++)
                {
                    *(p + 36 + i) = Convert.ToByte(tqs[i]);
                }
            }
            return Packet;
        }
    }
}
