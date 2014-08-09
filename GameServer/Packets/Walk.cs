using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Walking(3005) packet.
    /// </summary>
    public partial class EudemonPacket
    {
        /// <summary>
        /// Walk Packet without the Movement Mode Included, Assumes Walking.
        /// </summary>
        /// <param name="Direction">Direction to move in, modulated it seems. because fuck TQ</param>
        /// <param name="UID">Entity Unique ID</param>
        /// <param name="x">X Pos to Move to</param>
        /// <param name="y">Y Pos to Move to</param>
        /// <returns>Packet Byte array</returns>
        public static byte[] Walk(int Direction, int UID, int x, int y)
        {
            PacketBuilder Packet = new PacketBuilder(3005, 20);
            Packet.Long(Timer);
            Packet.Long(UID);
            Packet.Short(x);
            Packet.Short(y);
            Packet.Short(Direction);
            Packet.Short(0);
            return Packet.getFinal();
        }

        /// <summary>
        /// Walk Packet with the movement Mode Included, often refered to as WalkEX
        /// </summary>
        /// <param name="ucMode">Movement mode + direction (direction before % 8 and Movement Mode)</param>
        /// <param name="UID">Entity Unique ID</param>
        /// <param name="x">X Pos to Move to</param>
        /// <param name="y">Y Pos to Move to</param>
        /// <returns>Packet Byte array</returns>
        public static byte[] WalkRun(int ucMode, int UID, int x, int y)
        {
            PacketBuilder Packet = new PacketBuilder(3005, 20);
            Packet.Long(Timer);
            Packet.Long(UID);
            Packet.Short(x);
            Packet.Short(y);
            Packet.Short(ucMode);
            Packet.Short(0);
            return Packet.getFinal();
        }
    }
}
