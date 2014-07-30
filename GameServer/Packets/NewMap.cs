using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Notifies a client of a new map being loaded.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] NewMap(int MapID)
        {
            PacketBuilder Packet = new PacketBuilder(1110, 16);
            Packet.Long(MapID);
            Packet.Long(MapID);
            Packet.Long(65666);
            return Packet.getFinal();
        }
        //public static byte[] NewMap(int MapID)
        //{
        //    PacketBuilder Packet = new PacketBuilder(1110, 28);
        //    Packet.Long(13021937);
        //    Packet.Long(MapID);
        //    Packet.Short(294);
        //    Packet.Short(412);
        //    Packet.Long(0);
        //    Packet.Long(MapID);
        //    Packet.Long(9541);
        //    return Packet.getFinal();
        //}
    }
}
