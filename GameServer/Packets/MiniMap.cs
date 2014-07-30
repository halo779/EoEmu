using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Tells the client if the minimap should be shown or not.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] MiniMap(bool Show)
        {
            PacketBuilder Packet = new PacketBuilder(1016, 20);
            Packet.Short(Show ? 1 : 0);
            Packet.Short(0);
            Packet.Long(0);
            Packet.Short(20);
            Packet.Short(0);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
