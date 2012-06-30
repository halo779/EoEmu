using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Tells clients an item is no longer there.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] RemoveItemDropEffect(int UID, int GID, int X, int Y)
        {
            PacketBuilder Packet = new PacketBuilder(1101, 20);
            Packet.Long(UID);
            Packet.Long(GID);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Short(4);//Color
            Packet.Int(4); //Drop/Remove(Bool) 1:drop - 4:remove w/ effect, other: remove
            Packet.Int(0);
            return Packet.getFinal();
        }
        public static byte[] RemoveItemDrop(int UID)
        {
            PacketBuilder Packet = new PacketBuilder(1101, 20);
            Packet.Long(UID);
            Packet.Long(0);
            Packet.Long(0);
            Packet.Short(3);
            Packet.Short(2);
            return Packet.getFinal();
        }
    }
}
