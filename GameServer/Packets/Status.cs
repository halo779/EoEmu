using System;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;

namespace GameServer.Packets
{
    /// <summary>
    /// All forms of statuses in the conquer world.
    /// </summary>
    public partial class EudemonPacket
    {
        public static byte[] Status(ClientSocket CSocket, Struct.StatusTypes Type, int Value)
        {
            PacketBuilder Packet = new PacketBuilder(1017, 20);
            Packet.Long(CSocket.Client.ID);
            Packet.Long(1);//statuse count
            Packet.Long((int)Type);
            Packet.Long(Value);
            return Packet.getFinal();
        }
        public static byte[] Status(ClientSocket CSocket, int Switch, int Value, Struct.StatusTypes Type)
        {
            PacketBuilder Packet = null;
            if (Switch == 1 || Switch == 3)
                Packet = new PacketBuilder(1017, 36);
            else if (Switch == 2 || Switch == 4)
                Packet = new PacketBuilder(1017, 48);
            Packet.Long(CSocket.Client.ID);
            if (Switch == 1)
            {
                Packet.Long(Switch);
                Packet.Long(Value);
                Packet.Long(0);
                Packet.Long((int)Type);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 2)
            {
                Packet.Long(Switch);//Unknown Var
                Packet.Short(65535);
                Packet.Short(65535);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long((int)(Type));
                if (Type == Struct.StatusTypes.StatusEffect)
                {
                    uint Status = 0;
                    if (Value != 1024 && !CSocket.Client.Dead)//Dead
                    {
                        if (CSocket.Client.PkPoints >= 100)
                            Status += 0x8000;
                        else if (CSocket.Client.PkPoints >= 30 && CSocket.Client.PkPoints < 100)
                            Status += 0x4000;
                        if (CSocket.Client.Flashing)
                            Status += 0x1;
                        if (CSocket.Client.Team != null)
                        {
                            if (CSocket.Client.Team.LeaderID == CSocket.Client.ID)
                                Status += 0x40;
                        }
                        if (CSocket.Client.Flying)
                            Status += 0x8000000;
                    }
                    else
                        Status = 1024;
                    Packet.Long(Status);
                }
                else
                    Packet.Long(Value);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 3)
            {
                Packet.Long(Value);//Unknown Var
                Packet.Long(26);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 4)
            {
                Packet.Long(2);//Unknown Var
                Packet.Long(4294967295); //(FF FF FF FF)
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(9);
                Packet.Long(3);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            return Packet.getFinal();
        }
        public static byte[] Status(ClientSocket CSocket, int Switch, ulong Value, Struct.StatusTypes Type)
        {
            PacketBuilder Packet = null;
            if (Switch == 1 || Switch == 3)
                Packet = new PacketBuilder(1017, 36);
            else if (Switch == 2 || Switch == 4)
                Packet = new PacketBuilder(1017, 48);
            Packet.Long(CSocket.Client.ID);
            if (Switch == 1)
            {
                Packet.Long(Switch);
                //Packet.Long(Value);
                //Packet.Long(0);
                Packet.ULong(Value);
                Packet.Long((int)Type);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 2)
            {
                Packet.Long(Switch);//Unknown Var
                Packet.Short(65535);
                Packet.Short(65535);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long((int)(Type));
                Packet.Long(Value);
                Packet.Long(0);
                // Packet.ULong(Value);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 3)
            {
                //Packet.Long(Value);//Unknown Var
                //Packet.Long(26);
                Packet.ULong(Value);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            else if (Switch == 4)
            {
                Packet.Long(2);//Unknown Var
                Packet.Long(4294967295); //(FF FF FF FF)
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(9);
                Packet.Long(3);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
                Packet.Long(0);
            }
            return Packet.getFinal();
        }
    }
}
