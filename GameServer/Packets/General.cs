using System;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// 0x3f2 or 1010 packet, used for so very many things.
    /// </summary>
    public partial class EudemonPacket
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
            /// <summary>
            /// Will Be removed soon.
            /// </summary>
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(Value1);//val1
            Packet.Short(Value2);//val2
            Packet.Short(Value3);//val3
            Packet.Long(Value4);//val4
            Packet.Long(Value5);//val5  
            Packet.Short((int)PacketType + 9527);
            Packet.Short(0);
            return Packet.getFinal();
        }
        /// <summary>
        /// Will Be removed soon.
        /// </summary>
        public static byte[] General(int Identifier, int Value1, int Value2, int Value3, int Value4, int Value5, int Value6, Struct.DataType PacketType)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(Value1);//val1
            Packet.Short(Value2);//val2
            Packet.Short(Value3);//val3
            Packet.Long(Value4);//val4
            Packet.Short(Value5);//val5  
            Packet.Short(Value6);//val6 
            Packet.Short((int)PacketType + 9527);
            Packet.Short(0);
            return Packet.getFinal();
        }


        /// <summary>
        /// General Packet Type 1
        /// </summary>
        /// <param name="PlayerID">Player ID</param>
        /// <param name="PosX">Position X</param>
        /// <param name="PosY">Position X</param>
        /// <param name="Dir">Direction</param>
        /// <param name="SubType">SubType</param>
        /// <param name="Data">Data</param>
        public static byte[] General(int PlayerID, ushort PosX, ushort PosY, ushort Dir, Struct.DataType SubType, int Data)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(PlayerID);
            Packet.Short(PosX);
            Packet.Short(PosY);
            Packet.Short(Dir);
            Packet.Short(0); //Fill in the Padding

            Packet.Long(Data);

            Packet.Short((int)SubType + 9527);
            Packet.Short(0); //fill in the padding.

            return Packet.getFinal();
        }

        /// <summary>
        /// General Packet Type 2
        /// </summary>
        /// <param name="PlayerID">Player ID</param>
        /// <param name="PosX">Position X</param>
        /// <param name="PosY">Position X</param>
        /// <param name="Dir">Direction</param>
        /// <param name="SubType">SubType</param>
        /// <param name="Data">Data</param>
        public static byte[] General(int PlayerID, ushort PosX, ushort PosY, ushort Dir, Struct.DataType SubType, ushort TargetX, ushort TargetY)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(PlayerID);
            Packet.Short(PosX);
            Packet.Short(PosY);
            Packet.Short(Dir);
            Packet.Short(0); //Fill in the Padding

            Packet.Short(TargetX);
            Packet.Short(TargetY);

            Packet.Short((int)SubType + 9527);
            Packet.Short(0); //fill in the padding.

            return Packet.getFinal();
        }



    }
}
