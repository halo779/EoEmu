using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Tells a client it knows a skill.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] Skill(int ID, int Level, uint Exp)
        {
            PacketBuilder Packet = new PacketBuilder(1103, 12);
            Packet.Long(Exp);
            Packet.Short(ID);
            Packet.Short(Level);
            return Packet.getFinal();
        }
    }
}
