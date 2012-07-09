using System;


namespace GameServer.Packets
{
    public partial class ConquerPacket
    {
        /// <summary>
        /// Drops an item into the conquer world.
        /// </summary>
        /// <param name="UID">Item UID</param>
        /// <param name="GID">GroundItemLook (itemID)</param>
        /// <param name="X">X Cords</param>
        /// <param name="Y">Y Cords</param>
        /// <returns>Packet Bytes</returns>
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
