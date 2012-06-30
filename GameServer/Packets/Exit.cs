using System;

namespace GameServer.Packets
{
    public partial class ConquerPacket
    {
        public static byte[] ExitPacket()
        {
            PacketBuilder Packet = new PacketBuilder(1032, 28);
            Packet.Long(Timer);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Short(0);
            Packet.Short(29);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
