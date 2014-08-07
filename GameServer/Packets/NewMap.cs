using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Notifies a client of a new map being loaded.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] NewMap(int MapID, int MapType, int MapInstance)
        {
            PacketBuilder Packet = new PacketBuilder(1110, 16);
            Packet.Long(MapID);
            Packet.Long(MapInstance);
            Packet.Long(MapType);
            return Packet.getFinal();
        }

        public static byte[] NewMap(int MapID, int MapType)
        {
            PacketBuilder Packet = new PacketBuilder(1110, 16);
            Packet.Long(MapID);
            Packet.Long(MapID);
            Packet.Long(MapType);
            return Packet.getFinal();
        }
    }
}
