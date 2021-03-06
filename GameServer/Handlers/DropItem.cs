﻿using System;
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
    /// Handles dropping an item into the world.
    /// </summary>
    public partial class Handler
    {
        public static void DropItem(int UID, ClientSocket CSocket)
        {
            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Item UID: " + UID, Struct.ChatType.Normal));
            if (CSocket.Client.Inventory.ContainsKey(UID))
            {
                Struct.ItemInfo Ii = CSocket.Client.Inventory[UID];
                Struct.ItemGround IG = new Struct.ItemGround();
                IG.CopyItem(Ii);
                IG.Position = (int)Struct.ItemPosition.Ground;
                IG.Map = (int)CSocket.Client.Map;
                IG.X = (CSocket.Client.X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                IG.Y = (CSocket.Client.Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                if (MainGS.Maps.ContainsKey(IG.Map))
                {
                    Struct.DmapData Mapping = MainGS.Maps[IG.Map];
                    byte tries = 0;
                    while (!Mapping.CheckLoc((ushort)IG.X, (ushort)IG.Y))
                    {
                        IG.X = (CSocket.Client.X - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                        IG.Y = (CSocket.Client.Y - MainGS.Rand.Next(4) + MainGS.Rand.Next(4));
                        tries++;
                        if (tries > 8)
                            break;
                    }
                }

                IG.OwnerID = CSocket.Client.ID;
                IG.OwnerOnly = new System.Timers.Timer();
                IG.OwnerOnly.Interval = 7000;
                IG.OwnerOnly.AutoReset = false;
                IG.Dispose = new System.Timers.Timer();
                IG.Dispose.Interval = 60000;
                IG.Dispose.AutoReset = false;
                IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                IG.OwnerOnly.Start();
                IG.Dispose.Start();
                if (!MainGS.ItemFloor.ContainsKey(UID))
                {
                    Database.Database.DeleteItem(UID);
                    CSocket.Client.Inventory.Remove(UID);
                    CSocket.Send(EudemonPacket.ItemUsage(UID, 254, Struct.ItemUsage.RemoveDropItem));
                    //EudemonPacket.ToLocal(tmp, IG.X, IG.Y, IG.Map, 0, 0);
                    EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Ground UID: " + IG.UID + " Dropped at X:  " + IG.X + " Y: " + IG.Y, Struct.ChatType.Normal));

                    //lock(MainGS.ItemFloor)
                    //{
                    try
                    {
                        Monitor.Enter(MainGS.ItemFloor);
                        MainGS.ItemFloor.Add(IG.UID, IG);
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
            }
            else
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Item does not exist.", Struct.ChatType.System));
            }
        }
    }
}
