using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Walking(1005 or 3ED) conquer packet.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] Walk(int Direction, int UID, int x, int y)
        {
            /*PacketBuilder Packet = new PacketBuilder(1005, 16);
            Packet.Long(UID);
            Packet.Int(Direction);
            Packet.Int(1);
            Packet.Short(0);
            Packet.Long(Timer);
            return Packet.getFinal();*/

            PacketBuilder Packet = new PacketBuilder(3005, 20);
            Packet.Long(Timer);
            Packet.Long(UID);
            Packet.Short(x);
            Packet.Short(y);
            Packet.Long(Direction);
            //Packet.Long(Timer);
            return Packet.getFinal();
        }
    }
}
