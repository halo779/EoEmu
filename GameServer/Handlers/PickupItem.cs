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
    /// Handles picking up items off of Nano.ItemFloor
    /// </summary>
    public partial class Handler
    {
        public static void PickupItem(int UID, ClientSocket CSocket)
        {
            if (CSocket.Client.Inventory.Count < 40)
            {
                if (Nano.ItemFloor.ContainsKey(UID))
                {
                    Struct.ItemGround IG = Nano.ItemFloor[UID];
                    if (IG.X == CSocket.Client.X && IG.Y == CSocket.Client.Y && IG.Map == (int)CSocket.Client.Map)
                    {
                        if (IG.OwnerOnly.Enabled)
                        {
                            //Only able to give the item to its owner, until the timer expires that is, or to his/her team members.. :D
                            int TeamLeaderID = 0;
                            if (CSocket.Client.Team != null)
                            {
                                ClientSocket Leader = Nano.ClientPool[CSocket.Client.Team.LeaderID];
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
                                if (Nano.ItemFloor.ContainsKey(UID))
                                {
                                    //lock(Nano.ItemFloor)
                                    //{
                                    try
                                    {
                                        Monitor.Enter(Nano.ItemFloor);
                                        Nano.ItemFloor.Remove(UID);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                    finally
                                    {
                                        Monitor.Exit(Nano.ItemFloor);
                                    }
                                    //}
                                }
                                if (IG.Money > 0)
                                {
                                    CSocket.Client.Money += IG.Money;
                                    CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You picked up " + IG.Money + " silvers!", Struct.ChatType.Top));
                                    ConquerPacket.ToLocal(ConquerPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                    return;
                                }
                                ConquerPacket.ToLocal(ConquerPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                Struct.ItemInfo Item = new Struct.ItemInfo();
                                Struct.ItemData iData = Nano.Items[IG.ItemID];
                                Item.Bless = IG.Bless;
                                Item.Dura = IG.Dura;
                                Item.Enchant = IG.Enchant;
                                Item.ItemID = IG.ItemID;
                                Item.Color = IG.Color;
                                Item.MaxDura = IG.MaxDura;
                                Item.Plus = IG.Plus;
                                Item.Position = IG.Position;
                                Item.Soc1 = IG.Soc1;
                                Item.Soc2 = IG.Soc2;
                                Item.UID = IG.UID;
                                bool created = Database.Database.NewItem(Item, CSocket);
                                while (!created)
                                {
                                    Item.UID = Nano.Rand.Next(1000, 9999999);
                                    created = Database.Database.NewItem(Item, CSocket);
                                }
                                CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You have picked up a(n) " + iData.Name + " off the ground.", Struct.ChatType.Top));
                                if (!CSocket.Client.Inventory.ContainsKey(Item.UID))
                                    CSocket.Client.Inventory.Add(Item.UID, Item);
                            }
                            else
                            {
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] That item is not yet yours to pick up.", Struct.ChatType.Top));
                            }
                        }
                        else
                        {
                            IG.Stop();
                            if (Nano.ItemFloor.ContainsKey(UID))
                            {
                                //lock(Nano.ItemFloor)
                                //{
                                try
                                {
                                    Monitor.Enter(Nano.ItemFloor);
                                    Nano.ItemFloor.Remove(UID);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(Nano.ItemFloor);
                                }
                                //}
                            }
                            if (IG.Money > 0)
                            {
                                CSocket.Client.Money += IG.Money;
                                CSocket.Send(ConquerPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You picked up " + IG.Money + " silvers!", Struct.ChatType.Top));
                                ConquerPacket.ToLocal(ConquerPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                                return;
                            }
                            ConquerPacket.ToLocal(ConquerPacket.RemoveItemDrop(UID), IG.X, IG.Y, IG.Map, 0, 0);
                            Struct.ItemInfo Item = new Struct.ItemInfo();
                            Struct.ItemData iData = Nano.Items[IG.ItemID];
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
                                Item.UID = Nano.Rand.Next(1000, 9999999);
                                created = Database.Database.NewItem(Item, CSocket);
                            }
                            Database.Database.NewItem(Item, CSocket);
                            CSocket.Send(ConquerPacket.ItemInfo(Item.UID, Item.ItemID, Item.Plus, Item.Bless, Item.Enchant, Item.Soc1, Item.Soc2, Item.Dura, Item.MaxDura, Item.Position, Item.Color));
                            CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You have picked up a(n) " + iData.Name + " off the ground.", Struct.ChatType.Top));
                            if (!CSocket.Client.Inventory.ContainsKey(Item.UID))
                                CSocket.Client.Inventory.Add(Item.UID, Item);
                        }
                    }
                    else
                    {
                        CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item is not in range!", Struct.ChatType.Top));
                    }
                }
                else
                {
                    CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item does not exist in Nano floor.", Struct.ChatType.Top));
                }
            }
            else
            {
                CSocket.Send(ConquerPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your inventory is full.", Struct.ChatType.Top));
            }
        }
    }
}
