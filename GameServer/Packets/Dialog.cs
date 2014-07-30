using System;

namespace GameServer.Packets
{
    /// <summary>
    /// sending friendshare packet (2036)
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Dialog(int ActionID)
        {
            PacketBuilder Packet = new PacketBuilder(2036, 12);
            Packet.Short(ActionID);
            Packet.Short(0);
            Packet.Long(0);

            return Packet.getFinal();
        }
    }
}
