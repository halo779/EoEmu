using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Drops an item into the conquer world.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] DropItem(int UID, int GID, int X, int Y)
        {
            PacketBuilder Packet = new PacketBuilder(1101, 24);
            Packet.Long(UID);
            Packet.Long(GID);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Int(1); //Drop/Remove(Bool) 1:drop - 4:remove w/ effect, other: remove
            Packet.Int(0);
            Packet.Int(0);
            Packet.Int(0);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
