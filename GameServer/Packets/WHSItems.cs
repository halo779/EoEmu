using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Packets
{
    public partial class EudemonPacket
    {
        public static byte[] WarehouseItems(ClientSocket CSocket, int ID)
        {
            if (ID == 8) //TC ... elseif next
            {
                PacketBuilder Packet = new PacketBuilder(1102, 16 + (CSocket.Client.TCWhs.Count * 24));
                Packet.Long(ID);
                Packet.Long(0);
                Packet.Long(CSocket.Client.TCWhs.Count);
                foreach (KeyValuePair<int, Struct.ItemInfo> WHItem in CSocket.Client.TCWhs)
                {
                    Struct.ItemInfo Item = WHItem.Value;
                    Packet.Long(Item.UID);
                    Packet.Long(Item.ItemID);
                    Packet.Int(0);
                    Packet.Int(Item.Soc1);
                    Packet.Int(Item.Soc2);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(Item.Plus);
                    Packet.Int(Item.Bless);
                    Packet.Int(0);
                    Packet.Int(Item.Enchant);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(0);
                    Packet.Int(Item.Color);
                }
                return Packet.getFinal();
            }
            return null;
        }
    }
}
