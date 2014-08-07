using System;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// 0x3f2 or 1010 packet, used for so very many things.
    /// </summary>
    public partial class EudemonPacket
    {
        /// <summary>
        /// Will Be removed soon.
        /// </summary>
        public static byte[] GeneralOld(int Identifier, int Value1, int Value2, int Value3, int Value4, int Value5, Struct.DataType PacketType)
        {
            
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
        /// General Packet Type 1
        /// </summary>
        /// <param name="PlayerID">Player ID</param>
        /// <param name="PosX">Position X</param>
        /// <param name="PosY">Position X</param>
        /// <param name="Dir">Direction</param>
        /// <param name="SubType">SubType</param>
        /// <param name="Data">Data</param>
        /// <returns>Packet in Byte Array Form</returns>
        public static byte[] General(int PlayerID, ushort PosX, ushort PosY, ushort Dir, Struct.DataType SubType, int Data)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(PlayerID);
            Packet.Short(PosX);
            Packet.Short(PosY);
            Packet.Short(Dir);
            Packet.Short(0); //Fill in the Padding
            Packet.Long(Data);//union
            Packet.Short((int)SubType + 9527);
            Packet.Short(0); //fill in the padding.

            return Packet.getFinal();
        }

        /// <summary>
        /// General Packet Type 2
        /// </summary>
        /// <param name="PlayerID">Player ID</param>
        /// <param name="PosX">Position X</param>
        /// <param name="PosY">Position Y</param>
        /// <param name="Dir">Direction</param>
        /// <param name="SubType">SubType</param>
        /// <param name="Data">Data</param>
        /// <param name="TargetX">Target Position X</param>
        /// <param name="TargetY">Target Position Y</param>
        /// <returns>Packet in Byte Array Form</returns>
        public static byte[] General(int PlayerID, ushort PosX, ushort PosY, ushort Dir, Struct.DataType SubType, ushort TargetX, ushort TargetY)
        {
            PacketBuilder Packet = new PacketBuilder(1010, 28);
            Packet.Long(Timer);
            Packet.Long(PlayerID);
            Packet.Short(PosX);
            Packet.Short(PosY);
            Packet.Short(Dir);
            Packet.Short(0); //Fill in the Padding
            Packet.Short(TargetX);//union
            Packet.Short(TargetY);//union
            Packet.Short((int)SubType + 9527);
            Packet.Short(0); //fill in the padding.

            return Packet.getFinal();
        }



    }
}
