using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Various spawn effects for monsters.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] MobSpawnEffect(int ID, int X, int Y, int Dir, int UK)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 24);
            Packet.Long(Timer);
            Packet.Long(ID);
            Packet.Long(0);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Short(Dir);
            Packet.Short(UK);
            return Packet.getFinal();
        }
    }
}
