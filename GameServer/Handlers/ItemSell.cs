using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Handlers
{
    public partial class Handler
    {
        public static void ItemSell(byte[] Data, ClientSocket CSocket)
        {
            int ItemUID = PacketProcessor.ReadLong(Data, 8);
            if (CSocket.Client.Inventory.ContainsKey(ItemUID))
            {
                Struct.ItemInfo Item = new Struct.ItemInfo();
                Item = CSocket.Client.Inventory[ItemUID];
                if (Nano.Items.ContainsKey(Item.ItemID))
                {
                    Struct.ItemData iData = Nano.Items[Item.ItemID];
                    int Money = iData.Cost / 3;
                    if (Money > 0)
                    {
                        CSocket.Client.Money += Money;
                        CSocket.Client.Inventory.Remove(Item.UID);
                        CSocket.Send(EudemonPacket.ItemUsage(Item.UID, 255, Struct.ItemUsage.RemoveDropItem));
                        Database.Database.DeleteItem(Item.UID);
                        CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                    }
                }
            }
            else
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have that item..", Struct.ChatType.System));
            }
        }
    }
}
