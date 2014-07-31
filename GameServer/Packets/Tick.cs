using System;

namespace GameServer.Packets
{
    public partial class EudemonPacket
    {
        public static byte[] Tick(int PlayerID)
        {
            PacketBuilder Packet = new PacketBuilder(1012, 16);
            Packet.Long(PlayerID);
            Packet.Long(0);//dwdata
            Packet.Long(0);//chkData

            return Packet.getFinal();
        }
    }
}
