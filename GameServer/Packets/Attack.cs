using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Sends attacking(1022) packets
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Attack(int Attacker, int Attacked, int X, int Y, int Damage, int AType)
        {
            PacketBuilder Packet = new PacketBuilder(1022, 28);
            Packet.Long(Timer);
            Packet.Long(Attacker);
            Packet.Long(Attacked);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Long(AType);
            Packet.Long(Damage);
            return Packet.getFinal();
        }
    }
}
