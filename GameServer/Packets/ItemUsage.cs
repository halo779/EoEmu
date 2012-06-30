using System;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// Things like db upgrade, item movement, etc
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] ItemUsage(int UID, int Pos, Struct.ItemUsage Usage)
        {
            PacketBuilder Packet = new PacketBuilder(1009, 20);
            Packet.Long(UID);
            Packet.Long(Pos);
            Packet.Long((int)Usage);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
