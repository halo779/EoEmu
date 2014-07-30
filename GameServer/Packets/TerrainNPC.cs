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
    /// <summary>
    /// TG, GW, etc
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] TerrainNPC(Struct.TerrainNPC TNpc)
        {
            PacketBuilder Packet = new PacketBuilder(1109, 28);
            Packet.Long(TNpc.UID);
            Packet.Long(TNpc.MaximumHP);
            Packet.Long(TNpc.CurrentHP);
            Packet.Short(TNpc.X);
            Packet.Short(TNpc.Y);
            Packet.Short(TNpc.Type);
            Packet.Short(TNpc.Flag);
            Packet.Int(17);
            Packet.Short(0);
            Packet.Int(0);
            return Packet.getFinal();
        }
        public static byte[] TerrainNPC(int TNpcID, int MaxHp, int CurrentHp, int X, int Y, int TNpcType, string Name, int Facing)
        {
            PacketBuilder Packet = new PacketBuilder(1109, 28 + Name.Length);
            Packet.Long(TNpcID);
            Packet.Long(MaxHp);
            Packet.Long(CurrentHp);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Long(TNpcType);
            Packet.Short(26);//unknown, possibly subnpctype
            Packet.Int(Facing);
            Packet.Int(Name.Length);
            Packet.Text(Name);
            return Packet.getFinal();
        }
    }
}
