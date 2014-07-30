using System;


namespace GameServer.Packets
{
    /// <summary>
    /// Sends various forms of NPC talking.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] NPCTalk(int LinkBack, int DT, string Text)
        {
            PacketBuilder Packet = new PacketBuilder(2032, 16 + Text.Length);
            Packet.Long(0); //Unknown
            Packet.Short(0); //Unknown
            Packet.Int(LinkBack);
            Packet.Int(DT);
            Packet.Int(1); //# of strings
            Packet.Int(Text.Length);
            Packet.Text(Text);
            Packet.Short(0); //Unknown
            return Packet.getFinal();
        }
        public static byte[] NPCTalk(int UK1, int ID, int LinkBack, int DT)
        {
            PacketBuilder Packet = new PacketBuilder(2032, 16);
            Packet.Long(UK1);
            Packet.Short(ID);
            Packet.Int(LinkBack);
            Packet.Int(DT);
            Packet.Long(0);//Uknonwn
            return Packet.getFinal();
        }
    }
}
