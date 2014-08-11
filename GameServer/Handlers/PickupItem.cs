using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Connections;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;
using GameServer.Database;

namespace GameServer.Handlers
{
    /// <summary>
    /// Handles picking up items off of MainGS.ItemFloor
    /// </summary>
    public partial class Handler
    {
        public static void PickupItem(int UID, ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Pickup: " + UID, Struct.ChatType.Normal));
            foreach (Struct.ItemGround IG in MainGS.ItemFloor.Values)
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Item On Ground: " + IG.UID, Struct.ChatType.Normal));
            }
            if (CSocket.Client.Inventory.Count < 40)
            {
                if (MainGS.ItemFloor.ContainsKey(UID))
                {
                    Struct.ItemGround IG = MainGS.ItemFloor[UID];
                    if (IG.X == CSocket.Client.X && IG.Y == CSocket.Client.Y && IG.Map == (int)CSocket.Client.Map)
                    {
                        if (IG.OwnerOnly.Enabled)
                        {
                            //Only able to give the item to its owner, until the timer expires that is, or to his/her team members.. :D
                            int TeamLeaderID = 0;
                            if (CSocket.Client.Team != null)
                            {
                                ClientSocket Leader = MainGS.ClientPool[CSocket.Client.Team.LeaderID];
                                if (IG.Money > 0)
                                {
                                    if (!Leader.Client.Team.ForbidMoney)
                                        TeamLeaderID = CSocket.Client.Team.LeaderID;
                                }
                                else
                                {
                                    if (!Leader.Client.Team.ForbidItems)
                                        TeamLeaderID = CSocket.Client.Team.LeaderID;
                                }
                            }
                            if (IG.OwnerID == CSocket.Client.ID || IG.OwnerID == TeamLeaderID)
                            {
                                IG.Stop();
                                if (MainGS.ItemFloor.ContainsKey(UID))
                                {
                                    //lock(MainGS.ItemFloor)
                                    //{
                                    try
                                    {
                                        Monitor.Enter(MainGS.ItemFloor);
                                        MainGS.ItemFloor.Remove(UID);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                    finally
                                    {
                                        Monitor.Exit(MainGS.ItemFloor);
                                    }
                                    //}
                                }
                                if (IG.Money > 0)
                                {
                                    CSocket.Client.Money += IG.Money;
                                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You picked up " + IG.Money + " silvers!", Struct.ChatType.System));
                                    EudemonPacket.ToLocal(EudemonPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                    return;
                                }
                                EudemonPacket.ToLocal(EudemonPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                Struct.ItemInfo Item = new Struct.ItemInfo();
                                Struct.ItemData iData = MainGS.Items[IG.ItemID];
                                Item.Bless = IG.Bless;
                                Item.Dura = IG.Dura;
                                Item.Enchant = IG.Enchant;
                                Item.ItemID = IG.ItemID;
                                Item.Color = IG.Color;
                                Item.MaxDura = IG.MaxDura;
                                Item.Plus = IG.Plus;
                                Item.Position = (int)Struct.ItemPosition.Invetory;
                                Item.Soc1 = IG.Soc1;
                                Item.Soc2 = IG.Soc2;
                                Item.UID = IG.UID;
                                bool created = Database.Database.NewItem(Item, CSocket);
                                while (!created)
                                {
                                    Item.UID = MainGS.Rand.Next(1000, 9999999);
                                    created = Database.Database.NewItem(Item, CSocket);
                                }
                                CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You have picked up a(n) " + iData.Name + " off the ground.", Struct.ChatType.System));
                                if (!CSocket.Client.Inventory.ContainsKey(Item.UID))
                                    CSocket.Client.Inventory.Add(Item.UID, Item);
                            }
                            else
                            {
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] That item is not yet yours to pick up.", Struct.ChatType.System));
                            }
                        }
                        else
                        {
                            IG.Stop();
                            if (MainGS.ItemFloor.ContainsKey(UID))
                            {
                                //lock(MainGS.ItemFloor)
                                //{
                                try
                                {
                                    Monitor.Enter(MainGS.ItemFloor);
                                    MainGS.ItemFloor.Remove(UID);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(MainGS.ItemFloor);
                                }
                                //}
                            }
                            if (IG.Money > 0)
                            {
                                CSocket.Client.Money += IG.Money;
                                CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You picked up " + IG.Money + " silvers!", Struct.ChatType.System));
                                EudemonPacket.ToLocal(EudemonPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                return;
                            }
                            EudemonPacket.ToLocal(EudemonPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                            Struct.ItemInfo Item = new Struct.ItemInfo();
                            Struct.ItemData iData = MainGS.Items[IG.ItemID];
                            Item.Bless = IG.Bless;
                            Item.Dura = IG.Dura;
                            Item.Enchant = IG.Enchant;
                            Item.ItemID = IG.ItemID;
                            Item.MaxDura = IG.MaxDura;
                            Item.Plus = IG.Plus;
                            Item.Position = IG.Position;
                            Item.Color = IG.Color;
                            Item.Soc1 = IG.Soc1;
                            Item.Soc2 = IG.Soc2;
                            Item.UID = IG.UID;
                            bool created = Database.Database.NewItem(Item, CSocket);
                            while (!created)
                            {
                                Item.UID = MainGS.Rand.Next(1000, 9999999);
                                created = Database.Database.NewItem(Item, CSocket);
                            }
                            Database.Database.NewItem(Item, CSocket);
                            CSocket.Send(EudemonPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You have picked up a(n) " + iData.Name + " off the ground.", Struct.ChatType.System));
                            if (!CSocket.Client.Inventory.ContainsKey(Item.UID))
                                CSocket.Client.Inventory.Add(Item.UID, Item);
                        }
                    }
                    else
                    {
                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item is not in range!", Struct.ChatType.System));
                    }
                }
                else
                {
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item does not exist in Nano floor.", Struct.ChatType.System));
                }
            }
            else
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your inventory is full.", Struct.ChatType.System));
            }
        }
    }
}
