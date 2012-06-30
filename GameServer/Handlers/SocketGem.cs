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
    /// <summary>
    /// Handles items that require socketing
    /// </summary>
    public partial class Handler
    {
        public static void SocketGem(int ItemID, int GItemID, int Socket, ClientSocket CSocket)
        {
            int SocGem = 0;
            if (CSocket.Client.Inventory.ContainsKey(ItemID) && CSocket.Client.Inventory.ContainsKey(GItemID))
            {
                SocGem = Convert.ToInt32(CSocket.Client.Inventory[GItemID].ItemID.ToString().Substring(4));
                Struct.ItemInfo Item = CSocket.Client.Inventory[ItemID];
                if (Socket == 1)
                {
                    Item.Soc1 = SocGem;
                }
                else if (Socket == 2 && Item.Soc1 > 0)
                {
                    Item.Soc2 = SocGem;
                }
                else if (Socket == 2 && Item.Soc1 == 0)
                {
                    Item.Soc1 = SocGem;
                }
                CSocket.Client.Inventory.Remove(GItemID);
                Database.Database.UpdateItem(Item);
                Database.Database.DeleteItem(GItemID);
                CSocket.Send(ConquerPacket.ItemUsage(ItemID, 255, Struct.ItemUsage.RemoveItem));
                CSocket.Send(ConquerPacket.ItemUsage(GItemID, 255, Struct.ItemUsage.RemoveItem));
                CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
            }
            else
            {
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] One of your items does not exist.", Struct.ChatType.Top));
            }
        }
    }
}
