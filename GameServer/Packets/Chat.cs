using System;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// The packet used for all forms of chatting on Conquer.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Chat(int MessageID, string From, string To, string Message, Struct.ChatType CType)
        {
            PacketBuilder Packet = new PacketBuilder(1004, 32 + From.Length + To.Length + Message.Length);
            Packet.Int(255); //Color - Red
            Packet.Int(255); //Color - Blue
            Packet.Short(255); //Color - Red
            Packet.Short((int)CType); //Chat Type
            Packet.Short(0); //Unknown
            Packet.Long(MessageID); //Message ID
            Packet.Int(255);//unknown
            Packet.Int(255);//unknown
            Packet.Int(255);//unknown
            Packet.Int(255);//unknown
            Packet.Long(0); //Unknown
            Packet.Int(4); //Strings+1
            Packet.Int(From.Length);
            Packet.Text(From);
            Packet.Int(To.Length);
            Packet.Text(To);
            Packet.Int(0); //Unknown
            Packet.Int(Message.Length);
            Packet.Text(Message);
            Packet.Int(0);
            Packet.Int(0);
            Packet.Int(0);
            return Packet.getFinal();
        }
    }
}
