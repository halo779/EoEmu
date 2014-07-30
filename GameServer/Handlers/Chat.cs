using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Connections;
using GameServer.Database;
using GameServer.Entities;
using GameServer.Structs;
using GameServer.Packets;
using GameServer.Calculations;

namespace GameServer.Handlers
{
    /// <summary>
    /// Handles chatting in the CoEmu world.
    /// </summary>
    public partial class Handler
    {
        public static void Chat(byte[] Data, ClientSocket CSocket)
        {
            Struct.ChatType Type = (Struct.ChatType)((Data[9] << 8) + Data[8]);
            int Position = 26;
            int Len = 0;
            string From = "";
            string To = "";
            string Message = "";

            for (int C = 0; C < Data[25]; C++)
            {
                From += Convert.ToChar(Data[Position]);
                Position++;
            }
            Len = Data[Position];
            Position++;
            for (int C = 0; C < Len; C++)
            {
                To += Convert.ToChar(Data[Position]);
                Position++;
            }
            Position++;
            Len = Data[Position];
            Position++;
            for (int C = 0; C < Len; C++)
            {
                Message += Convert.ToChar(Data[Position]);
                Position++;
            }
            #region Commands
            try
            {
                if (Message.StartsWith("/"))
                {
                    string[] Command = Message.Substring(1).Split(' ');
                    switch (Command[0].ToLower())
                    {
                        case "heal":
                            {
                                CSocket.Send(EudemonPacket.Status(CSocket, Struct.StatusTypes.Hp, CSocket.Client.MaxHP));
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[GameServer] Healed " + CSocket.Client.Name, Struct.ChatType.System));
                                break;
                            }
                        case "dc":
                            {
                                CSocket.Disconnect();
                                break;
                            }
                        case "dl":
                            {
                                int type = Convert.ToInt16(Command[1]);
                                CSocket.Send(EudemonPacket.General(0, CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 1, type, Struct.DataType.Dialog));
                                break;
                            }
                        case "pos":
                            {
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[GameServer] Current Pos " + CSocket.Client.X + ", " + CSocket.Client.Y, Struct.ChatType.System));
                                break;
                            }
                        case "unstuck":
                            {
                                Handler.Teleport((int)Struct.Maps.Cronus, 290, 424, 0, CSocket);
                                break;
                            }
                        case "stattest":
                            {

                                CSocket.Send(EudemonPacket.Status(CSocket, Struct.StatusTypes.Hp, 50));

                                break;
                            }
                        case "kick":
                            {
                                if (CSocket.Client.isGM || CSocket.Client.isPM)
                                {
                                    if (Command.Length < 2)
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Command example: /kick player", Struct.ChatType.Talk));
                                        break;
                                    }
                                    bool kicked = false;
                                    try
                                    {
                                        Monitor.Enter(Nano.ClientPool);
                                        foreach (KeyValuePair<int, ClientSocket> Player in Nano.ClientPool)
                                        {
                                            if (Player.Value.Client.Name.ToLower() == Command[1].ToLower())
                                            {
                                                if (Player.Value.Client.isPM && !CSocket.Client.isPM)
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "GMs cannot kick PMs, sorry!", Struct.ChatType.System));
                                                else
                                                {
                                                    EudemonPacket.ToServer(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", Player.Value.Client.Name + " has been kicked from the server", Struct.ChatType.CenterGm), 0);
                                                    Player.Value.Disconnect();
                                                    kicked = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                    finally
                                    {
                                        Monitor.Exit(Nano.ClientPool);
                                    }
                                    if (!kicked)
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Cannot kick player.", Struct.ChatType.Talk));
                                    }
                                }
                                break;
                            }
                        case "ban":
                            {
                                if (CSocket.Client.isPM)
                                {
                                    if (Command.Length < 2)
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Command example: /kick player", Struct.ChatType.Talk));
                                        break;
                                    }
                                    bool kicked = false;
                                    Database.Database.BanPlayer(Command[1]);
                                    try
                                    {
                                        Monitor.Enter(Nano.ClientPool);
                                        foreach (KeyValuePair<int, ClientSocket> Player in Nano.ClientPool)
                                        {
                                            if (Player.Value.Client.Name.ToLower() == Command[1].ToLower())
                                            {
                                                EudemonPacket.ToServer(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", Player.Value.Client.Name + " has been kicked & banned from the server.", Struct.ChatType.CenterGm), 0);
                                                Player.Value.Disconnect();
                                                kicked = true;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                    finally
                                    {
                                        Monitor.Exit(Nano.ClientPool);
                                    }
                                    if (!kicked)
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Cannot ban player.", Struct.ChatType.Talk));
                                    }
                                }
                                break;
                            }
                        case "save":
                            {
                                Database.Database.SaveCharacter(CSocket.Client);
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[GameServer] Saved " + CSocket.Client.Name, Struct.ChatType.System));
                                break;
                            }
                        case "reload":
                            {
                                if (Command.Length == 2 && CSocket.Client.isGM || CSocket.Client.isPM)
                                {
                                    switch (Command[1].ToLower())
                                    {
                                        case "portals":
                                            {
                                                Nano.Portals.Clear();
                                                Struct.LoadPortals();
                                                break;
                                            }
                                        case "monsters":
                                            {
                                                Nano.BaseMonsters.Clear();
                                                Nano.MonsterSpawns.Clear();
                                                Nano.Monsters.Clear();
                                                Struct.LoadMonsters();
                                                break;
                                            }
                                        case "npcs":
                                            {
                                                Nano.Npcs.Clear();
                                                Struct.LoadNpcs();
                                                break;
                                            }
                                        case "tnpcs":
                                            {
                                                Nano.TerrainNpcs.Clear();
                                                Struct.LoadTNpcs();
                                                break;
                                            }
                                    }
                                }
                                break;
                            }
                        case "scroll":
                            {
                                if (Command.Length == 2)
                                {
                                    switch (Command[1].ToLower())
                                    {
                                        case "tc":
                                            {
                                                Handler.Teleport(1002, 438, 377, 0, CSocket);
                                                break;
                                            }
                                        default:
                                            {
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Unknown scroll location. Example: /scroll tc", Struct.ChatType.Talk));
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] /scroll tc", Struct.ChatType.Talk));
                                }
                                break;
                            }
                        case "i":
                            {
                                if (CSocket.Client.isGM || CSocket.Client.isPM)
                                {
                                    if (!CSocket.Client.Invincible)
                                    {
                                        CSocket.Client.Invincible = true;
                                        EudemonPacket.ToLocal(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", CSocket.Client.Name + " has just become even more godly..", Struct.ChatType.CenterGm), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                    }
                                    else
                                    {
                                        CSocket.Client.Invincible = false;
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You are no longer flagged as invincible.", Struct.ChatType.System));
                                    }
                                    EudemonPacket.ToLocal(EudemonPacket.General(CSocket.Client.ID, CSocket.Client.X, CSocket.Client.Y, 0, 0, 0, Struct.DataType.EntityRemove), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                    EudemonPacket.ToLocal(EudemonPacket.SpawnCharacter(CSocket), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);

                                }
                                break;
                            }
                        case "money":
                            {
                                if (Command.Length == 2 && CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    int Money = Convert.ToInt32(Command[1]);
                                    CSocket.Client.Money += Money;
                                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Money, Struct.StatusTypes.InvMoney));
                                }
                                break;
                            }
                        case "eps":
                            {
                                if (Command.Length == 2 && CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    int Ep = Convert.ToInt32(Command[1]);
                                    CSocket.Client.EPs += Ep;
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "You now have " + CSocket.Client.EPs + " EPs.", Struct.ChatType.System));
                                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.EPs, Struct.StatusTypes.InvEPoints));
                                }
                                break;
                            }
                        case "hair":
                            {
                                if (Command.Length == 2)
                                {
                                    int Hair = Convert.ToInt32(Command[1]);
                                    CSocket.Client.Hair = Hair;
                                    CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Hair, Struct.StatusTypes.HairStyle));
                                }
                                break;
                            }
                        case "dropitem":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Calculation.PercentSuccess(30))
                                    {
                                        int Level = 137;
                                        int Times = 1;
                                        if (Calculation.PercentSuccess(15))
                                        {
                                            Times = Nano.Rand.Next(1, 6);
                                        }
                                        for (int i = 0; i < Times; i++)
                                        {
                                            int Money = Nano.Rand.Next(1, 10);
                                            if (Calculation.PercentSuccess(90))
                                                Money = Nano.Rand.Next(2, 240);
                                            if (Calculation.PercentSuccess(70))
                                                Money = Nano.Rand.Next(60, 3000);
                                            if (Calculation.PercentSuccess(50))
                                                Money = Nano.Rand.Next(200, 4000);
                                            if (Calculation.PercentSuccess(30))
                                                Money = Nano.Rand.Next(1000, 30000);
                                            if (Calculation.PercentSuccess(100))
                                                Money = Nano.Rand.Next(2000, 50000);
                                            Money = Money / ((138 - Level) * 10);
                                            if (Money < 1)
                                                Money = 1;
                                            Struct.ItemGround IG = new Struct.ItemGround();
                                            IG.Money = Money;
                                            if (Money < 10)
                                                IG.ItemID = 1090000;
                                            else if (Money < 100)
                                                IG.ItemID = 1090010;
                                            else if (Money < 1000)
                                                IG.ItemID = 1090020;
                                            else if (Money < 3000)
                                                IG.ItemID = 1091000;
                                            else if (Money < 10000)
                                                IG.ItemID = 1091010;
                                            else
                                                IG.ItemID = 1091020;
                                            IG.UID = Nano.Rand.Next(1, 1000);
                                            while (Nano.ItemFloor.ContainsKey(IG.UID))
                                            {
                                                IG.UID = Nano.Rand.Next(1, 1000);
                                            }
                                            IG.X = CSocket.Client.X;
                                            IG.Y = CSocket.Client.Y;
                                            IG.OwnerOnly = new System.Timers.Timer();
                                            IG.Map = (int)CSocket.Client.Map;
                                            IG.Dispose = new System.Timers.Timer();
                                            IG.Dispose.Interval = 60000;
                                            IG.Dispose.AutoReset = false;
                                            IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                                            IG.Dispose.Start();
                                            Nano.ItemFloor.Add(IG.UID, IG);
                                            EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                                        }
                                    }
                                    else
                                    {
                                        if (Calculation.PercentSuccess(5))
                                        {
                                            Struct.ItemGround IG = new Struct.ItemGround();
                                            IG.ItemID = 1088001;
                                            IG.X = CSocket.Client.X;
                                            IG.Y = CSocket.Client.Y;
                                            IG.Map = (int)CSocket.Client.Map;
                                            IG.OwnerOnly = new System.Timers.Timer();
                                            IG.UID = Nano.Rand.Next(1000, 9999999);
                                            while (Nano.ItemFloor.ContainsKey(IG.UID))
                                            {
                                                IG.UID = Nano.Rand.Next(1000, 9999999);
                                            }
                                            //TODO: UID generation that is better.
                                            IG.Dispose.Interval = 10000;
                                            IG.Dispose.AutoReset = false;
                                            IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                                            IG.Dispose.Start();
                                            Nano.ItemFloor.Add(IG.UID, IG);
                                            EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                                        }
                                        else if (Calculation.PercentSuccess(3))
                                        {
                                            Struct.ItemGround IG = new Struct.ItemGround();
                                            IG.ItemID = 1088000;
                                            IG.X = CSocket.Client.X;
                                            IG.Y = CSocket.Client.Y;
                                            IG.Map = (int)CSocket.Client.Map;
                                            IG.OwnerOnly = new System.Timers.Timer();
                                            IG.UID = Nano.Rand.Next(1000, 9999999);
                                            while (Nano.ItemFloor.ContainsKey(IG.UID))
                                            {
                                                IG.UID = Nano.Rand.Next(1000, 9999999);
                                            }
                                            //TODO: UID generation that is better.
                                            IG.Dispose.Interval = 10000;
                                            IG.Dispose.AutoReset = false;
                                            IG.Dispose.Elapsed += delegate { IG.Disappear(); };
                                            IG.Dispose.Start();
                                            Nano.ItemFloor.Add(IG.UID, IG);
                                            EudemonPacket.ToLocal(EudemonPacket.DropItem(IG.UID, IG.ItemID, IG.X, IG.Y), IG.X, IG.Y, IG.Map, 0, 0);
                                        }
                                    }
                                }
                                break;
                            }
                        case "bringtome":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length == 2)
                                    {
                                        ClientSocket Target = null;
                                        try
                                        {
                                            Monitor.Enter(Nano.ClientPool);
                                            foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                                            {
                                                if (Clients.Value.Client.Name == Command[1])
                                                {
                                                    Target = Clients.Value;
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.ToString());
                                        }
                                        finally
                                        {
                                            Monitor.Exit(Nano.ClientPool);
                                        }
                                        if (Target != null)
                                        {
                                            Handler.Teleport((int)CSocket.Client.Map, CSocket.Client.X, CSocket.Client.Y, 0, Target);
                                            Target.Send(EudemonPacket.Chat(0, "SYSTEM", Target.Client.Name, "You have been summoned by " + CSocket.Client.Name, Struct.ChatType.System));
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Player not found!", Struct.ChatType.System));
                                        }
                                    }
                                }
                                break;
                            }
                        case "bringmeto":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length == 2)
                                    {
                                        ClientSocket Target = null;
                                        try
                                        {
                                            Monitor.Enter(Nano.ClientPool);
                                            foreach (KeyValuePair<int, ClientSocket> Clients in Nano.ClientPool)
                                            {
                                                if (Clients.Value.Client.Name == Command[1])
                                                {
                                                    Target = Clients.Value;
                                                    break;
                                                }
                                            }
                                            //}
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.ToString());
                                        }
                                        finally
                                        {
                                            Monitor.Exit(Nano.ClientPool);
                                        }
                                        if (Target != null)
                                        {
                                            Handler.Teleport((int)Target.Client.Map, Target.Client.X, Target.Client.Y, 0, CSocket);
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Teleported to " + Target.Client.Name, Struct.ChatType.System));
                                        }
                                        else
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Player not found!", Struct.ChatType.System));
                                        }
                                    }
                                }
                                break;
                            }
                        case "bc":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    EudemonPacket.ToServer(EudemonPacket.Chat(0, CSocket.Client.Name, "ALLUSERS", Message.Substring(2 + Command[0].Length), Struct.ChatType.NewBroadcast), 0);
                                }
                                break;
                            }
                        case "gm":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    EudemonPacket.ToServer(EudemonPacket.Chat(0, CSocket.Client.Name, "ALLUSERS", Message.Substring(2 + Command[0].Length), Struct.ChatType.CenterGm), 0);
                                }
                                break;
                            }
                        case "itemtest":
                            {
                                if (Command.Length == 2)
                                {
                                    Struct.ItemInfo NewItem = new Struct.ItemInfo();
                                    NewItem.Bless = 0;
                                    NewItem.Dura = 1;
                                    NewItem.Enchant = 0;
                                    NewItem.ItemID = Convert.ToInt32(Command[1]);
                                    NewItem.MaxDura = 1;
                                    NewItem.Plus = 0;
                                    NewItem.Position = 50;
                                    NewItem.Soc1 = 0;
                                    NewItem.Soc2 = 0;
                                    NewItem.Color = 0;
                                    NewItem.UID = Nano.Rand.Next(1, 9999999);
                                    bool created = Database.Database.NewItem(NewItem, CSocket);
                                    while (!created)
                                    {
                                        NewItem.UID = Nano.Rand.Next(1, 9999999);
                                        created = Database.Database.NewItem(NewItem, CSocket);
                                    }
                                    CSocket.Client.Inventory.Add(NewItem.UID, NewItem);
                                    //CSocket.Send(EudemonPacket.ItemInfo(0, Item.Value.ItemID, Item.Value.Plus, Item.Value.Soc1, Item.Value.Soc2, Item.Value.Dura, Item.Value.MaxDura, Item.Value.Position, 0, 0, 6));


                                    CSocket.Send(EudemonPacket.ItemInfo(NewItem.UID, NewItem.ItemID, NewItem.Plus, NewItem.Soc1, NewItem.Soc2, NewItem.Dura, NewItem.MaxDura, NewItem.Position, 0, 0, 0));


                                    // CSocket.Send(EudemonPacket.ItemInfo(NewItem.UID, NewItem.ItemID, NewItem.Plus, NewItem.Bless, NewItem.Enchant, NewItem.Soc1, NewItem.Soc2, NewItem.Dura, NewItem.MaxDura, NewItem.Position, NewItem.Color));
                                    //CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Created a(n) " + Item.Name + " Plus: " + Plus + ", Bless: " + Bless + ", Enchant: " + Enchant + ", Version: " + version, Struct.ChatType.Top));
                                }
                                else
                                {
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Error in command", Struct.ChatType.System));

                                }

                                break;
                            }
                        case "item":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length == 8)
                                    {
                                        if (CSocket.Client.Inventory.Count == 40)
                                        {
                                            CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Your inventory is full.", Struct.ChatType.System));
                                            break;
                                        }
                                        foreach (KeyValuePair<int, Struct.ItemData> Items in Nano.Items)
                                        {
                                            Struct.ItemData Item = Items.Value;
                                            if (Item.Name.ToLower() == Command[1].ToLower())
                                            {
                                                int version = Convert.ToInt32(Command[2]);
                                                int NewItemID = Item.ID;
                                                int Plus = Convert.ToInt32(Command[4]);
                                                int Bless = Convert.ToInt32(Command[5]);
                                                int Enchant = Convert.ToInt32(Command[6]);
                                                int Soc1 = 0;
                                                int Soc2 = 0;
                                                int Color = Convert.ToInt32(Command[7]);
                                                if ((Convert.ToInt32(Command[3])) == 1)
                                                {
                                                    Soc1 = 255;
                                                }
                                                else if ((Convert.ToInt32(Command[3])) == 2)
                                                {
                                                    Soc1 = 255;
                                                    Soc2 = 255;
                                                }
                                                else if ((Convert.ToInt32(Command[3])) == 0)
                                                {
                                                    Soc1 = 0;
                                                    Soc2 = 0;
                                                }
                                                if (Color < 2 || Color > 9)
                                                    if (Bless != 1 && Bless != 3 && Bless != 5 && Bless != 7 && Bless != 0)
                                                    {
                                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + Bless + " is an invalid bless amount. Accepted amounts are 0/1/3/5/7.", Struct.ChatType.System));
                                                        break;
                                                    }
                                                if (Enchant > 255 || Enchant < 0)
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + Enchant + " is an invalid enchant amount. Accepted amounts are 255>x>0.", Struct.ChatType.System));
                                                    break;
                                                }
                                                if (Plus > 12 || Plus < 0)
                                                {
                                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] " + Plus + " is an invalid plus amount. Accepted amounts are 12>x>0.", Struct.ChatType.System));
                                                    break;
                                                }
                                                string ItemID = Item.ID.ToString();
                                                if (ItemID.EndsWith("0"))
                                                    NewItemID += 3;
                                                #region Version Switch
                                                switch (version)
                                                {
                                                    case 3:
                                                        {
                                                            //the default item
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            NewItemID += 1;
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            NewItemID += 2;
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            NewItemID += 3;
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            NewItemID += 4;
                                                            break;
                                                        }
                                                    case 8:
                                                        {
                                                            NewItemID += 5;
                                                            break;
                                                        }
                                                    case 9:
                                                        {
                                                            NewItemID += 6;
                                                            break;
                                                        }
                                                    case 13:
                                                        {
                                                            NewItemID += 10;
                                                            break;
                                                        }
                                                    case 14:
                                                        {
                                                            NewItemID += 11;
                                                            break;
                                                        }
                                                    case 15:
                                                        {
                                                            NewItemID += 12;
                                                            break;
                                                        }
                                                    case 16:
                                                        {
                                                            NewItemID += 13;
                                                            break;
                                                        }
                                                    case 17:
                                                        {
                                                            NewItemID += 14;
                                                            break;
                                                        }
                                                    case 18:
                                                        {
                                                            NewItemID += 15;
                                                            break;
                                                        }
                                                    case 19:
                                                        {
                                                            NewItemID += 16;
                                                            break;
                                                        }
                                                    case 23:
                                                        {
                                                            NewItemID += 20;
                                                            break;
                                                        }
                                                    case 24:
                                                        {
                                                            NewItemID += 21;
                                                            break;
                                                        }
                                                    case 25:
                                                        {
                                                            NewItemID += 22;
                                                            break;
                                                        }
                                                    case 26:
                                                        {
                                                            NewItemID += 23;
                                                            break;
                                                        }
                                                    case 27:
                                                        {
                                                            NewItemID += 24;
                                                            break;
                                                        }
                                                    case 28:
                                                        {
                                                            NewItemID += 25;
                                                            break;
                                                        }
                                                    case 29:
                                                        {
                                                            NewItemID += 26;
                                                            break;
                                                        }
                                                    case 33:
                                                        {
                                                            NewItemID += 30;
                                                            break;
                                                        }
                                                    case 34:
                                                        {
                                                            NewItemID += 31;
                                                            break;
                                                        }
                                                    case 35:
                                                        {
                                                            NewItemID += 32;
                                                            break;
                                                        }
                                                    case 36:
                                                        {
                                                            NewItemID += 33;
                                                            break;
                                                        }
                                                    case 37:
                                                        {
                                                            NewItemID += 34;
                                                            break;
                                                        }
                                                    case 38:
                                                        {
                                                            NewItemID += 35;
                                                            break;
                                                        }
                                                    case 39:
                                                        {
                                                            NewItemID += 36;
                                                            break;
                                                        }

                                                    case 43:
                                                        {
                                                            NewItemID += 40;
                                                            break;
                                                        }
                                                    case 44:
                                                        {
                                                            NewItemID += 41;
                                                            break;
                                                        }
                                                    case 45:
                                                        {
                                                            NewItemID += 42;
                                                            break;
                                                        }
                                                    case 46:
                                                        {
                                                            NewItemID += 43;
                                                            break;
                                                        }
                                                    case 47:
                                                        {
                                                            NewItemID += 44;
                                                            break;
                                                        }
                                                    case 48:
                                                        {
                                                            NewItemID += 45;
                                                            break;
                                                        }
                                                    case 49:
                                                        {
                                                            NewItemID += 46;
                                                            break;
                                                        }
                                                    case 53:
                                                        {
                                                            NewItemID += 50;
                                                            break;
                                                        }
                                                    case 54:
                                                        {
                                                            NewItemID += 51;
                                                            break;
                                                        }
                                                    case 55:
                                                        {
                                                            NewItemID += 52;
                                                            break;
                                                        }
                                                    case 56:
                                                        {
                                                            NewItemID += 53;
                                                            break;
                                                        }
                                                    case 57:
                                                        {
                                                            NewItemID += 54;
                                                            break;
                                                        }
                                                    case 58:
                                                        {
                                                            NewItemID += 55;
                                                            break;
                                                        }
                                                    case 59:
                                                        {
                                                            NewItemID += 56;
                                                            break;
                                                        }
                                                    case 63:
                                                        {
                                                            NewItemID += 60;
                                                            break;
                                                        }
                                                    case 64:
                                                        {
                                                            NewItemID += 61;
                                                            break;
                                                        }
                                                    case 65:
                                                        {
                                                            NewItemID += 62;
                                                            break;
                                                        }
                                                    case 66:
                                                        {
                                                            NewItemID += 63;
                                                            break;
                                                        }
                                                    case 67:
                                                        {
                                                            NewItemID += 64;
                                                            break;
                                                        }
                                                    case 68:
                                                        {
                                                            NewItemID += 65;
                                                            break;
                                                        }
                                                    case 69:
                                                        {
                                                            NewItemID += 66;
                                                            break;
                                                        }
                                                    case 73:
                                                        {
                                                            NewItemID += 70;
                                                            break;
                                                        }
                                                    case 74:
                                                        {
                                                            NewItemID += 71;
                                                            break;
                                                        }
                                                    case 75:
                                                        {
                                                            NewItemID += 72;
                                                            break;
                                                        }
                                                    case 76:
                                                        {
                                                            NewItemID += 73;
                                                            break;
                                                        }
                                                    case 77:
                                                        {
                                                            NewItemID += 74;
                                                            break;
                                                        }
                                                    case 78:
                                                        {
                                                            NewItemID += 75;
                                                            break;
                                                        }
                                                    case 79:
                                                        {
                                                            NewItemID += 76;
                                                            break;
                                                        }
                                                    case 83:
                                                        {
                                                            NewItemID += 80;
                                                            break;
                                                        }
                                                    case 84:
                                                        {
                                                            NewItemID += 81;
                                                            break;
                                                        }
                                                    case 85:
                                                        {
                                                            NewItemID += 82;
                                                            break;
                                                        }
                                                    case 86:
                                                        {
                                                            NewItemID += 83;
                                                            break;
                                                        }
                                                    case 87:
                                                        {
                                                            NewItemID += 84;
                                                            break;
                                                        }
                                                    case 88:
                                                        {
                                                            NewItemID += 85;
                                                            break;
                                                        }
                                                    case 89:
                                                        {
                                                            NewItemID += 86;
                                                            break;
                                                        }
                                                    case 93:
                                                        {
                                                            NewItemID += 90;
                                                            break;
                                                        }
                                                    case 94:
                                                        {
                                                            NewItemID += 91;
                                                            break;
                                                        }
                                                    case 95:
                                                        {
                                                            NewItemID += 92;
                                                            break;
                                                        }
                                                    case 96:
                                                        {
                                                            NewItemID += 93;
                                                            break;
                                                        }
                                                    case 97:
                                                        {
                                                            NewItemID += 94;
                                                            break;
                                                        }
                                                    case 98:
                                                        {
                                                            NewItemID += 95;
                                                            break;
                                                        }
                                                    case 99:
                                                        {
                                                            NewItemID += 96;
                                                            break;
                                                        }
                                                    case 103:
                                                        {
                                                            NewItemID += 100;
                                                            break;
                                                        }
                                                    case 104:
                                                        {
                                                            NewItemID += 101;
                                                            break;
                                                        }
                                                    case 105:
                                                        {
                                                            NewItemID += 102;
                                                            break;
                                                        }
                                                    case 106:
                                                        {
                                                            NewItemID += 103;
                                                            break;
                                                        }
                                                    case 107:
                                                        {
                                                            NewItemID += 104;
                                                            break;
                                                        }
                                                    case 108:
                                                        {
                                                            NewItemID += 105;
                                                            break;
                                                        }
                                                    case 109:
                                                        {
                                                            NewItemID += 106;
                                                            break;
                                                        }
                                                    case 113:
                                                        {
                                                            NewItemID += 110;
                                                            break;
                                                        }
                                                    case 114:
                                                        {
                                                            NewItemID += 111;
                                                            break;
                                                        }
                                                    case 115:
                                                        {
                                                            NewItemID += 112;
                                                            break;
                                                        }
                                                    case 116:
                                                        {
                                                            NewItemID += 113;
                                                            break;
                                                        }
                                                    case 117:
                                                        {
                                                            NewItemID += 114;
                                                            break;
                                                        }
                                                    case 118:
                                                        {
                                                            NewItemID += 115;
                                                            break;
                                                        }
                                                    case 119:
                                                        {
                                                            NewItemID += 116;
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            //Do nothing
                                                            break;
                                                        }
                                                }
                                                #endregion
                                                Struct.ItemInfo NewItem = new Struct.ItemInfo();
                                                NewItem.Bless = Bless;
                                                NewItem.Dura = Item.MaxDura;
                                                NewItem.Enchant = Enchant;
                                                NewItem.ItemID = NewItemID;
                                                NewItem.MaxDura = Item.MaxDura;
                                                NewItem.Plus = Plus;
                                                NewItem.Position = 0;
                                                NewItem.Soc1 = Soc1;
                                                NewItem.Soc2 = Soc2;
                                                NewItem.Color = Color;
                                                NewItem.UID = Nano.Rand.Next(1, 9999999);
                                                bool created = Database.Database.NewItem(NewItem, CSocket);
                                                while (!created)
                                                {
                                                    NewItem.UID = Nano.Rand.Next(1, 9999999);
                                                    created = Database.Database.NewItem(NewItem, CSocket);
                                                }
                                                CSocket.Client.Inventory.Add(NewItem.UID, NewItem);
                                                CSocket.Send(EudemonPacket.ItemInfo(NewItem.UID, NewItem.ItemID, NewItem.Plus, NewItem.Bless, NewItem.Enchant, NewItem.Soc1, NewItem.Soc2, NewItem.Dura, NewItem.MaxDura, NewItem.Position, NewItem.Color));
                                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Created a(n) " + Item.Name + " Plus: " + Plus + ", Bless: " + Bless + ", Enchant: " + Enchant + ", Version: " + version, Struct.ChatType.System));
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case "effect":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length == 2)
                                    {
                                        EudemonPacket.ToLocal(EudemonPacket.Effect(CSocket.Client.ID, Command[1]), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                    }
                                }
                                break;
                            }
                        case "modify":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length > 2)
                                    {
                                        switch (Command[1].ToLower())
                                        {
                                            case "name":
                                                {
                                                    //TODO:Modify name and other's name's.
                                                    break;
                                                }
                                            case "level":
                                                {
                                                    if (Command.Length == 3)
                                                    {
                                                        int NewLevel = Convert.ToInt32(Command[2]);
                                                        if (NewLevel <= 135)
                                                        {
                                                            CSocket.Client.Level = NewLevel;
                                                            CSocket.Client.Exp = 0;
                                                            CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Level, Struct.StatusTypes.Level));
                                                            EudemonPacket.ToLocal(EudemonPacket.Effect(CSocket.Client.ID, "LevelUp"), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, 0);
                                                            if (CSocket.Client.Level == 135)
                                                                EudemonPacket.ToServer(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "CONGRATULATIONS! " + CSocket.Client.Name + " has just achieved level 135! Great job!", Struct.ChatType.NewBroadcast), 0);

                                                        }
                                                    }
                                                    break;
                                                }
                                            case "dex":
                                                {
                                                    if (Command.Length == 3)
                                                    {
                                                        int Dex = Convert.ToInt32(Command[2]);
                                                        CSocket.Client.Dexterity = Dex;
                                                        CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Dexterity, Struct.StatusTypes.DexterityStatPoints));
                                                        Calculation.Vitals(CSocket, false);
                                                    }
                                                    break;
                                                }
                                            case "vit":
                                                {
                                                    if (Command.Length == 3)
                                                    {
                                                        int Vit = Convert.ToInt32(Command[2]);
                                                        CSocket.Client.Vitality = Vit;
                                                        CSocket.Send(EudemonPacket.Status(CSocket, 2, CSocket.Client.Vitality, Struct.StatusTypes.VitalityStatPoints));
                                                        Calculation.Vitals(CSocket, false);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                }
                                break;
                            }
                        case "skill":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    if (Command.Length == 3)
                                    {
                                        int ID = Convert.ToInt32(Command[1]);
                                        int Level = Convert.ToInt32(Command[2]);
                                        if (CSocket.Client.Skills.ContainsKey(ID))
                                        {
                                            Struct.CharSkill Skill = CSocket.Client.Skills[ID];
                                            Skill.Level = Level;
                                            Skill.Exp = 0;
                                            Database.Database.SetSkill(Skill.ID, Skill.Level, Skill.Exp, CSocket.Client.ID, true);
                                        }
                                        else
                                        {
                                            Struct.CharSkill Skill = new Struct.CharSkill();
                                            Skill.Level = Level;
                                            Skill.ID = ID;
                                            CSocket.Client.Skills.Add(Skill.ID, Skill);
                                            Database.Database.SetSkill(ID, Level, 0, CSocket.Client.ID, false);
                                        }
                                        CSocket.Send(EudemonPacket.Skill(ID, Level, 0));
                                    }
                                    else
                                    {
                                        CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] /skill ID Level.", Struct.ChatType.Talk));
                                    }

                                }
                                break;
                            }
                        case "status":
                            {
                                if (CSocket.Client.isPM || CSocket.Client.isGM)
                                {
                                    CSocket.Send(EudemonPacket.Chat(0, "", "", "", Struct.ChatType.ClearTopRight));
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Players online: " + Nano.ClientPool.Count, Struct.ChatType.TopRight));
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Monsters spawned: " + Nano.Monsters.Count, Struct.ChatType.TopRight));
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "Exp/Prof/Skill Rates: " + Nano.EXP_MULTIPLER + "/" + Nano.PROF_MULTIPLER + "/" + Nano.SKILL_MULTIPLER, Struct.ChatType.TopRight));
                                }
                                break;
                            }
                        /*case "tnpc":
                            {
                                if(CSocket.Client.isPM)
                                {
                                    if(Command.Length == 4)
                                    {
                                        int ID = Convert.ToInt32(Command[1]);
                                        int TType = Convert.ToInt32(Command[2]);
                                        int Flag = Convert.ToInt32(Command[3]);
                                        CSocket.Send(EudemonPacket.TerrainNPC(ID, 100, 100, CSocket.Client.X, CSocket.Client.Y, TType, Flag));
                                    }
                                }
                                break;
                            }*/
                        /*case "killallmobs":
                            {
                                if(CSocket.Client.isPM)
                                {
                                    Dictionary<int, Monster> Mobs = new Dictionary<int, Monster>();
                                    foreach(KeyValuePair<int, Monster> Mon in Nano.Monsters)
                                    {
                                        Mobs.Add(Mon.Key, Mon.Value);
                                    }
                                    Nano.Monsters.Clear();
                                    foreach(KeyValuePair<int, Monster> Mon in Mobs)
                                    {
                                        Handler.doMonster(Mon.Value, Mon.Value.CurrentHP, 2, CSocket);
                                    }
                                }
                                break;
                            }*/
                        case "gmove":
                            {
                                if (Command.Length == 4)
                                {
                                    Handler.Teleport(Convert.ToInt32(Command[1]), Convert.ToUInt16(Command[2]), Convert.ToUInt16(Command[3]), 0, CSocket);
                                }
                                break;
                            }
                    }
                }
            #endregion
                #region Chats
                else
                {
                    switch (Type)
                    {
                        case Struct.ChatType.Talk:
                            {
                                EudemonPacket.ToLocal(EudemonPacket.Chat(0, CSocket.Client.Name, To, Message, Type), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                break;
                            }
                        case Struct.ChatType.Team:
                            {
                                if (CSocket.Client.Team != null)
                                {
                                    if (Nano.ClientPool.ContainsKey(CSocket.Client.Team.LeaderID))
                                    {
                                        ClientSocket Leader = Nano.ClientPool[CSocket.Client.Team.LeaderID];
                                        foreach (KeyValuePair<int, ClientSocket> Member in Leader.Client.Team.Members)
                                        {
                                            if (Member.Value.Client.ID != CSocket.Client.ID)
                                                Member.Value.Send(EudemonPacket.Chat(0, CSocket.Client.Name, To, Message, Type));
                                        }
                                    }
                                }
                                break;
                            }
                        case Struct.ChatType.Ghost:
                            {
                                EudemonPacket.ToLocal(EudemonPacket.Chat(0, CSocket.Client.Name, To, Message, Type), CSocket.Client.X, CSocket.Client.Y, (int)CSocket.Client.Map, 0, CSocket.Client.ID);
                                break;
                            }
                        case Struct.ChatType.Whisper:
                            {
                                bool online = false;
                                //lock(Nano.ClientPool)
                                //{
                                try
                                {
                                    Monitor.Enter(Nano.ClientPool);
                                    foreach (KeyValuePair<int, ClientSocket> Player in Nano.ClientPool)
                                    {
                                        if (Player.Value.Client.Name == To)
                                        {
                                            Player.Value.Send(EudemonPacket.Chat(0, CSocket.Client.Name, To, Message, Type));
                                            online = true;
                                            break;
                                        }
                                    }
                                    //}
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                finally
                                {
                                    Monitor.Exit(Nano.ClientPool);
                                }
                                //}
                                if (!online)
                                    CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", "ALLUSERS", "[ERROR] Player not found.", Struct.ChatType.Talk));
                                break;
                            }
                        case Struct.ChatType.Friend:
                            {
                                EudemonPacket.ToServer(EudemonPacket.Chat(0, From, To, Message, Struct.ChatType.Friend), CSocket.Client.ID);
                                break;
                            }
                        default:
                            {
                                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Chat type not coded.", Struct.ChatType.Talk));
                                break;
                            }
                    }
                }
                #endregion
            }
            catch (Exception Except)
            {
                CSocket.Send(EudemonPacket.Chat(0, "SYSTEM", CSocket.Client.Name, "[ERROR] Please report: Exception thrown during command parsing/message handling.", Struct.ChatType.System));
                Console.WriteLine(Except.ToString());
            }
        }
    }
}
