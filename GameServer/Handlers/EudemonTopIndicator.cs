using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Creates the eud at the top of the screen - seems a 1010 packet must send the eud data after this packet
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] EudemonTopIndicator(int EudUid, int EudType)
        {
            PacketBuilder Packet = new PacketBuilder(1008, 66);
            Packet.Long(0);
            Packet.Long(EudUid);
            Packet.Long(EudType);
            Packet.Long(0);
            Packet.Short(1);
            Packet.Short(53);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Short(0);
            Packet.Int(0);
            Packet.Int(1);
            Packet.Long(1734429961);
            Packet.Long(1852396645);
            Packet.Short(24942);
            return Packet.getFinal();
        }
    }
}
