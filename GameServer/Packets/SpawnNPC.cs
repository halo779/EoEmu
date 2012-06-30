using System;


namespace GameServer.Packets
{
    /// <summary>
    /// spawns NPCs of type Struct.NPC
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] SpawnNPC(int Type, int X, int Y, int SubType, int Dir, int Flag)
        {
            PacketBuilder Packet = new PacketBuilder(2030, 32);
            Packet.Long(Type);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Long(SubType);
            Packet.Short(Dir);
            Packet.Long(Flag);
            Packet.Long(0);
            Packet.Short(0);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
