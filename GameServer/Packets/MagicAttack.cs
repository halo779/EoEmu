using System;
using System.Collections;
using System.Collections.Generic;

namespace GameServer.Packets
{
    /// <summary>
    /// Notifies clients that a dictionary of monsters has been hit by a magic attack.
    /// </summary>
    public partial class ConquerPacket
    {
        public static byte[] MagicAttack(int Attacker, int Spell, int Level, Dictionary<int, int> Targets, int X, int Y)
        {
            PacketBuilder Packet = new PacketBuilder(1105, 32 + (Targets.Count * 12));
            Packet.Long(Attacker);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Short(Spell);
            Packet.Short(Level);
            Packet.Long(Targets.Count);
            foreach (KeyValuePair<int, int> T in Targets)
            {
                Packet.Long(T.Key);
                Packet.Long(T.Value);
                Packet.Long(0);
            }
            Packet.Long(0);
            Packet.Long(0);
            Packet.Long(0);
            return Packet.getFinal();
        }
    }
}
