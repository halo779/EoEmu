using System;
using System.Collections;
using System.Collections.Generic;
using GameServer;
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
        public static bool itemexist(string find, string where)
        {
            int Begin = where.IndexOf(find);
            int End = where.LastIndexOf(find);
            if (Begin >= 0 && End >= 0)
                return true;
            else
                return false;
        }
        public static void ItemBuy(byte[] Data, ClientSocket CSocket)
        {
            int ID = PacketProcessor.ReadLong(Data, 8);
            if (CSocket.Client.Inventory.Count == 40)
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your inventory is full.", Struct.ChatType.System));
                return;
            }
            string Shop = System.IO.File.ReadAllText("Shop.dat");
            if (itemexist(Convert.ToString(ID), Shop))
            {
                Struct.ItemInfo Item = new Struct.ItemInfo();
                if (MainGS.Items.ContainsKey(ID))
                {
                    Struct.ItemData NewItem = MainGS.Items[ID];
                    if (NewItem.EPCost > 0)
                    {
                        if (CSocket.Client.EPs >= NewItem.EPCost)
                        {
                            Handler.CPs(NewItem.EPCost * -1, CSocket);
                            Item.ItemID = NewItem.ID;
                            Item.UID = MainGS.Rand.Next(1, 9999999);
                            bool created = Database.Database.NewItem(Item, CSocket);
                            while (!created)
                            {
                                Item.UID = MainGS.Rand.Next(1, 9999999);
                                created = Database.Database.NewItem(Item, CSocket);
                            }
                            CSocket.Client.Inventory.Add(Item.UID, Item);
                            CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, 0, 0, 0, 0, 0, 0, 0, 0, 0));
                        }
                        else
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough EPs.", Struct.ChatType.System));
                        }
                    }
                    else if (NewItem.Cost > 0)
                    {
                        if (CSocket.Client.Money >= NewItem.Cost)
                        {
                            Handler.Money(NewItem.Cost * -1, CSocket);
                            Item.ItemID = NewItem.ID;
                            Item.UID = MainGS.Rand.Next(1, 9999999);
                            bool created = Database.Database.NewItem(Item, CSocket);
                            while (!created)
                            {
                                Item.UID = MainGS.Rand.Next(1, 9999999);
                                created = Database.Database.NewItem(Item, CSocket);
                            }
                            CSocket.Client.Inventory.Add(Item.UID, Item);
                            CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, 0, 0, 0, 0, 0, 0, 0, 0, 0));
                        }
                        else
                        {
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] You do not have enough money.", Struct.ChatType.System));
                        }
                    }
                    else
                    {
                        Item.ItemID = NewItem.ID;
                        Item.UID = MainGS.Rand.Next(1, 9999999);
                        bool created = Database.Database.NewItem(Item, CSocket);
                        while (!created)
                        {
                            Item.UID = MainGS.Rand.Next(1, 9999999);
                            created = Database.Database.NewItem(Item, CSocket);
                        }
                        CSocket.Client.Inventory.Add(Item.UID, Item);
                        CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, 0, 0, 0, 0, 0, 0, 0, 0, 0));
                    }
                }
            }
            else
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item does not exist in Shop.dat", Struct.ChatType.System));
            }
        }
    }
}
