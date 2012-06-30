using System;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// 0x3f2 or 1010 packet, used for so very many things.
    /// </summary>
    public partial class ConquerPacket
    {
        /*public static byte[] General(int Identifier, int Value1, int Value2, int Value3, Struct.DataType PacketType)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(Identifier);
            Packet.Long(Value1);
            Packet.Short(Value2);
            Packet.Short(Value3);
            Packet.Short(0);
            Packet.Short((int)PacketType);
            Packet.Long(0);
            return Packet.getFinal();
        }*/
        //public static byte[] General(int Identifier, int Value1, int Value2, int Value3, int Value4, int Value5, Struct.DataType PacketType)
        //{
        //    PacketBuilder Packet = new PacketBuilder(1010, 28);
        //    Packet.Long(Timer);
        //    Packet.Long(Identifier);
        //    Packet.Short(Value1);
        //    Packet.Short(Value2);
        //    Packet.Short(Value3);
        //    Packet.Long(Value4);
        //    Packet.Short(Value5);
        //    Packet.Short((int)PacketType);
        //    Packet.Short(0);
        //    return Packet.getFinal();
        //}
        public static byte[] General(int Identifier, int Value1, int Value2, int Value3, int Value4, int Value5, Struct.DataType PacketType)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(Value1);//val1
            Packet.Short(Value2);//val2
            Packet.Short(Value3);//val3
            Packet.Long(Value4);//val4
            Packet.Long(Value5);//val5  
            Packet.Short((int)PacketType);
            Packet.Short(0);
            return Packet.getFinal();
        }
    }
}
