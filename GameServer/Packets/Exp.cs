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
    /// Sends Exp to client
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Exp(int UID, int Type, ulong value)
        {
            PacketBuilder Packet = new PacketBuilder(1017, 28);
            Packet.Long(UID);
            Packet.Long(1);
            Packet.Long(Type);
            Packet.ULong(value);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
