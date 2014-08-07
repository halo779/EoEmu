using System;
using GameServer.Connections;

namespace GameServer.Packets
{
    public partial class EudemonPacket
    {
        public static byte[] String(ClientSocket Client, int SubType, params string[] Strings)
        {
            int stringLengths = 0;
            foreach (string str in Strings)
            {
                stringLengths += str.Length;
                stringLengths++;
            }
            PacketBuilder Packet = new PacketBuilder(1015, 12 + stringLengths);
            Packet.Long(Client.Client.ID);
            Packet.Short(SubType);
            Packet.Byte(Strings.Length);
            foreach (string str in Strings)
            {
                Packet.Byte(str.Length);
                Packet.Text(str);
            }
            Packet.Byte(0);
            return Packet.getFinal();
        }

        public static byte[] String(int UniqueID, int SubType, params string[] Strings)
        {
            int stringLengths = 0;
            foreach (string str in Strings)
            {
                stringLengths += str.Length;
                stringLengths++;
            }
            PacketBuilder Packet = new PacketBuilder(1015, 12 + stringLengths);
            Packet.Long(UniqueID);
            Packet.Short(SubType);
            Packet.Byte(Strings.Length);
            foreach (string str in Strings)
            {
                Packet.Byte(str.Length);
                Packet.Text(str);
            }
            Packet.Byte(0);
            return Packet.getFinal();
        }

        public static byte[] String(short X, short Y, int SubType, params string[] Strings)
        {
            int stringLengths = 0;
            foreach (string str in Strings)
            {
                stringLengths += str.Length;
                stringLengths++;
            }
            PacketBuilder Packet = new PacketBuilder(1015, 12 + stringLengths);
            Packet.Short(X);
            Packet.Short(Y);
            Packet.Short(SubType);
            Packet.Byte(Strings.Length);
            foreach (string str in Strings)
            {
                Packet.Byte(str.Length);
                Packet.Text(str);
            }
            Packet.Byte(0);
            return Packet.getFinal();
        }
    }
}
