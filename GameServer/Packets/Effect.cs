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
    /// Sends visual effects to the client
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Effect(int UID, string value)
        {
            PacketBuilder Packet = new PacketBuilder(1015, 13 + value.Length);
            Packet.Long(UID);
            Packet.Int(10);
            Packet.Int(1);
            Packet.Int(value.Length);
            Packet.Text(value);
            Packet.Short(0);
            return Packet.getFinal();
        }
    }
}
